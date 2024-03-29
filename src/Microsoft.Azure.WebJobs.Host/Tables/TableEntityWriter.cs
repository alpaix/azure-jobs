﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Storage;
using Microsoft.Azure.WebJobs.Host.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Microsoft.Azure.WebJobs.Host.Tables
{
    internal class TableEntityWriter<T> : ICollector<T>, IAsyncCollector<T>, IWatcher
        where T : ITableEntity
    {
        private readonly IStorageTable _table;

        /// <summary>
        /// Max batch size is an azure limitation on how many entries can be in each batch.
        /// </summary>
        public const int MaxBatchSize = 100;

        /// <summary>
        /// How many different partition keys to cache offline before flushing.
        /// This means the max offline cache size is (MaxPartitionWidth * (MaxBatchSize-1)) entries.
        /// </summary>
        public const int MaxPartitionWidth = 1000;

        private readonly Dictionary<string, Dictionary<string, IStorageTableOperation>> _map =
            new Dictionary<string, Dictionary<string, IStorageTableOperation>>();

        private readonly TableParameterLog _log;

        private readonly Stopwatch _watch = new Stopwatch();

        public TableEntityWriter(IStorageTable table, TableParameterLog log)
        {
            _table = table;
            _log = log;
        }

        public TableEntityWriter(IStorageTable table)
            : this(table, new TableParameterLog())
        {
        }

        public void Add(T item)
        {
            AddAsync(item, CancellationToken.None).GetAwaiter().GetResult();
        }

        public async Task AddAsync(T item, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Careful: 
            // 1. even with upsert, all rowkeys within a batch must be unique. If they aren't, the previous items
            // will be flushed.
            // 2. Capture at time of Add, in case item is mutated after add. 
            // 3. Validate rowkey on the client so we get a nice error instead of the cryptic 400 from auzre.

            string partitionKey = item.PartitionKey;
            string rowKey = item.RowKey;

            TableClient.ValidateAzureTableKeyValue(partitionKey);
            TableClient.ValidateAzureTableKeyValue(rowKey);

            Dictionary<string, IStorageTableOperation> partition;
            if (!_map.TryGetValue(partitionKey, out partition))
            {
                if (_map.Count >= MaxPartitionWidth)
                {
                    // Offline cache is too large. Clear some room
                    await FlushAsync(cancellationToken);
                }

                partition = new Dictionary<string, IStorageTableOperation>();
                _map[partitionKey] = partition;
            }

            var itemCopy = Copy(item);

            if (partition.ContainsKey(rowKey))
            {
                // Replacing item forces a flush to ensure correct eTag behaviour.
                await FlushPartitionAsync(partition, cancellationToken);

                // Reinitialize partition
                partition = new Dictionary<string, IStorageTableOperation>();
                _map[partitionKey] = partition;
            }

            _log.EntitiesWritten++;

            if (String.IsNullOrEmpty(itemCopy.ETag))
            {
                partition.Add(rowKey, _table.CreateInsertOperation(itemCopy));
            }
            else if (itemCopy.ETag.Equals("*"))
            {
                partition.Add(rowKey, _table.CreateInsertOrReplaceOperation(itemCopy));
            }
            else 
            {
                partition.Add(rowKey, _table.CreateReplaceOperation(itemCopy));
            }

            if (partition.Count >= MaxBatchSize)
            {
                await FlushPartitionAsync(partition, cancellationToken);
                _map.Remove(partitionKey);
            }
        }

        private static ITableEntity Copy(ITableEntity item)
        {
            var props = TableEntityValueBinder.DeepClone(item.WriteEntity(null));
            DynamicTableEntity copy = new DynamicTableEntity(item.PartitionKey, item.RowKey, item.ETag, props);
            return copy;
        }

        public virtual async Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var kv in _map)
            {
                await FlushPartitionAsync(kv.Value, cancellationToken);
            }
            _map.Clear();
        }

        internal virtual async Task FlushPartitionAsync(Dictionary<string, IStorageTableOperation> partition,
            CancellationToken cancellationToken)
        {
            if (partition.Count > 0)
            {
                try
                {
                    _watch.Start();
                    await ExecuteBatchAndCreateTableIfNotExistsAsync(partition, cancellationToken);
                }
                finally
                {
                    _watch.Stop();
                    _log.ElapsedWriteTime = _watch.Elapsed;
                }
            }
        }

        internal virtual async Task ExecuteBatchAndCreateTableIfNotExistsAsync(
            Dictionary<string, IStorageTableOperation> partition, CancellationToken cancellationToken)
        {
            IStorageTableBatchOperation batch = _table.CreateBatch();

            foreach (var operation in partition.Values)
            {
                batch.Add(operation);
            }

            if (batch.Count > 0)
            {
                StorageException exception = null;

                try
                {
                    // Commit the batch
                    await _table.ExecuteBatchAsync(batch, cancellationToken);
                }
                catch (StorageException e)
                {
                    if (!e.IsNotFoundTableNotFound())
                    {
                        throw;
                    }

                    exception = e;
                }

                if (exception != null)
                {
                    // Make sure the table exists
                    await _table.CreateIfNotExistsAsync(cancellationToken);

                    // Commit the batch
                    await _table.ExecuteBatchAsync(batch, cancellationToken);
                }
            }
        }

        public ParameterLog GetStatus()
        {
            if (_log.EntitiesWritten > 0)
            {
                return _log;
            }
            else
            {
                return null;
            }
        }
    }
}

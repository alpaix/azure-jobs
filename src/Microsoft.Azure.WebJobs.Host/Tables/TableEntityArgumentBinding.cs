﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table;

namespace Microsoft.Azure.WebJobs.Host.Tables
{
    internal class TableEntityArgumentBinding<TElement> : IArgumentBinding<TableEntityContext>
        where TElement : ITableEntity, new()
    {
        public Type ValueType
        {
            get { return typeof(TElement); }
        }

        public async Task<IValueProvider> BindAsync(TableEntityContext value, ValueBindingContext context)
        {
            IStorageTable table = value.Table;
            IStorageTableOperation retrieve = table.CreateRetrieveOperation<TElement>(value.PartitionKey, value.RowKey);
            TableResult result = await table.ExecuteAsync(retrieve, context.CancellationToken);
            TElement entity = (TElement)result.Result;

            if (entity == null)
            {
                return new NullEntityValueProvider<TElement>(value);
            }

            return new TableEntityValueBinder(value, entity, typeof(TElement));
        }
    }
}

﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Storage.Blob;
using Microsoft.Azure.WebJobs.Host.Timers;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Microsoft.Azure.WebJobs.Host.Blobs.Listeners
{
    internal sealed class SharedBlobQueueListener : ISharedListener
    {
        private readonly IListener _listener;
        private readonly BlobQueueTriggerExecutor _executor;

        private bool _started;
        private bool _disposed;

        public SharedBlobQueueListener(IListener listener, BlobQueueTriggerExecutor executor)
        {
            _listener = listener;
            _executor = executor;
        }

        public void Register(string functionId, ITriggeredFunctionInstanceFactory<IStorageBlob> instanceFactory)
        {
            if (_started)
            {
                throw new InvalidOperationException(
                    "Registrations may not be added while the shared listener is running.");
            }

            _executor.Register(functionId, instanceFactory);
        }

        public async Task EnsureAllStartedAsync(CancellationToken cancellationToken)
        {
            if (!_started)
            {
                await _listener.StartAsync(cancellationToken);
                _started = true;
            }
        }

        public async Task EnsureAllStoppedAsync(CancellationToken cancellationToken)
        {
            if (_started)
            {
                await _listener.StopAsync(cancellationToken);
                _started = false;
            }
        }

        public void EnsureAllCanceled()
        {
            _listener.Cancel();
        }

        public void EnsureAllDisposed()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _listener.Dispose();
                _disposed = true;
            }
        }
    }
}

﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.WindowsAzure.Storage;

namespace Microsoft.Azure.WebJobs.Host.UnitTests.Blobs.Bindings
{
    internal sealed class CompletedCancellableAsyncResult : ICancellableAsyncResult, IDisposable
    {
        private readonly object _state;

        private bool _canceled;
        private bool _disposed;
        private WaitHandle _waitHandle;

        public CompletedCancellableAsyncResult(object state)
        {
            _state = state;
        }

        public object AsyncState
        {
            get
            {
                ThrowIfDisposed();
                return _state;
            }
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                ThrowIfDisposed();

                // Lazily allocate
                if (_waitHandle == null)
                {
                    _waitHandle = new ManualResetEvent(initialState: true);
                }

                return _waitHandle;
            }
        }

        public bool Canceled
        {
            get
            {
                ThrowIfDisposed();
                return _canceled;
            }
        }

        public bool CompletedSynchronously
        {
            get
            {
                ThrowIfDisposed();
                return true;
            }
        }

        public bool IsCompleted
        {
            get
            {
                ThrowIfDisposed();
                return true;
            }
        }

        public void Cancel()
        {
            ThrowIfDisposed();
            _canceled = true;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_waitHandle != null)
                {
                    _waitHandle.Dispose();
                }

                _disposed = true;
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(null);
            }
        }
    }
}

﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Blobs.Bindings;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Microsoft.Azure.WebJobs.Host.UnitTests.Blobs.Bindings
{
    internal class FakeCloudBlobStream : CloudBlobStream
    {
        private readonly Stream _inner;

        private bool _committed;

        public FakeCloudBlobStream(Stream inner)
        {
            _inner = inner;
        }

        public override bool CanRead
        {
            get { return _inner.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _inner.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return !_committed && _inner.CanWrite; }
        }

        public override bool CanTimeout
        {
            get { return _inner.CanTimeout; }
        }

        public override long Length
        {
            get { return _inner.Length; }
        }

        public override long Position
        {
            get { return _inner.Position; }
            set
            {
                ThrowIfCommitted();
                _inner.Position = value;
            }
        }

        public override int ReadTimeout
        {
            get { return _inner.ReadTimeout; }
            set { _inner.ReadTimeout = value; }
        }

        public override int WriteTimeout
        {
            get { return _inner.WriteTimeout; }
            set { _inner.WriteTimeout = value; }
        }

        public override ICancellableAsyncResult BeginCommit(AsyncCallback callback, object state)
        {
            ThrowIfCommitted();
            return CompleteSynchronously(callback, state);
        }

        public override void EndCommit(IAsyncResult asyncResult)
        {
        }

        public override ICancellableAsyncResult BeginFlush(AsyncCallback callback, object state)
        {
            ThrowIfCommitted();
            CancellationTokenSource cancellationSource = new CancellationTokenSource();
            Task task = FlushAsync(cancellationSource.Token);
            return new TaskCancellableAsyncResult(task, cancellationSource, callback, state);
        }

        public override void EndFlush(IAsyncResult asyncResult)
        {
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback,
            object state)
        {
            return _inner.BeginRead(buffer, offset, count, callback, state);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return _inner.EndRead(asyncResult);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback,
            object state)
        {
            ThrowIfCommitted();
            return _inner.BeginWrite(buffer, offset, count, callback, state);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            _inner.EndWrite(asyncResult);
        }

        public override void Close()
        {
            _inner.Close();
        }

        public override void Commit()
        {
            ThrowIfCommitted();
            _committed = true;
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return _inner.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public override void Flush()
        {
            ThrowIfCommitted();
            _inner.Flush();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            ThrowIfCommitted();
            return _inner.FlushAsync(cancellationToken);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _inner.Read(buffer, offset, count);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return _inner.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override int ReadByte()
        {
            return _inner.ReadByte();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            ThrowIfCommitted();
            return _inner.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            ThrowIfCommitted();
            _inner.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ThrowIfCommitted();
            _inner.Write(buffer, offset, count);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            ThrowIfCommitted();
            return _inner.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override void WriteByte(byte value)
        {
            ThrowIfCommitted();
            _inner.WriteByte(value);
        }

        private static ICancellableAsyncResult CompleteSynchronously(AsyncCallback callback, object state)
        {
            ICancellableAsyncResult result = new CompletedCancellableAsyncResult(state);
            InvokeCallback(callback, result);
            return result;
        }

        private static void InvokeCallback(AsyncCallback callback, IAsyncResult result)
        {
            if (callback != null)
            {
                callback.Invoke(result);
            }
        }

        private void ThrowIfCommitted()
        {
            if (_committed)
            {
                throw new InvalidOperationException("The stream has already been committed.");
            }
        }
    }
}

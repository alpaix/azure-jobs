﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Microsoft.Azure.WebJobs.Host.Blobs.Bindings
{
    internal class DelegatingCloudBlobStream : CloudBlobStream
    {
        private readonly CloudBlobStream _inner;

        public DelegatingCloudBlobStream(CloudBlobStream inner)
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

        public override bool CanTimeout
        {
            get { return _inner.CanTimeout; }
        }

        public override bool CanWrite
        {
            get { return _inner.CanWrite; }
        }

        public override long Length
        {
            get { return _inner.Length; }
        }

        public override long Position
        {
            get { return _inner.Position; }
            set { _inner.Position = value; }
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
            return _inner.BeginCommit(callback, state);
        }

        public override void EndCommit(IAsyncResult asyncResult)
        {
            _inner.EndCommit(asyncResult);
        }

        public override ICancellableAsyncResult BeginFlush(AsyncCallback callback, object state)
        {
            return _inner.BeginFlush(callback, state);
        }

        public override void EndFlush(IAsyncResult asyncResult)
        {
            _inner.EndFlush(asyncResult);
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
            _inner.Commit();
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return _inner.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public override void Flush()
        {
            _inner.Flush();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            // The Storage NuGet package for .NET 4.0 can't implement FlushAsync, since that is a .NET 4.5-only method.
            // Rely on BeginFlush/EndFlush instead of FlushAsync.
            return CancellableTaskFactory.FromAsync(_inner.BeginFlush, _inner.EndFlush, cancellationToken);
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
            return _inner.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _inner.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _inner.Write(buffer, offset, count);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return _inner.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override void WriteByte(byte value)
        {
            _inner.WriteByte(value);
        }
    }
}

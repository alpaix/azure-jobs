﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Microsoft.Azure.WebJobs.Host.Blobs.Bindings
{
    internal class BlobCommittedAction : IBlobCommitedAction
    {
        private readonly IStorageBlob _blob;
        private readonly IBlobWrittenWatcher _blobWrittenWatcher;

        public BlobCommittedAction(IStorageBlob blob, IBlobWrittenWatcher blobWrittenWatcher)
        {
            _blob = blob;
            _blobWrittenWatcher = blobWrittenWatcher;
        }

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (_blobWrittenWatcher != null)
            {
                _blobWrittenWatcher.Notify(_blob.SdkObject);
            }

            return Task.FromResult(0);
        }
    }
}

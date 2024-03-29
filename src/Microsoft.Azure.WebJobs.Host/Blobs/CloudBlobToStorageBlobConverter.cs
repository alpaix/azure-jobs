﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.Azure.WebJobs.Host.Converters;
using Microsoft.Azure.WebJobs.Host.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Microsoft.Azure.WebJobs.Host.Blobs
{
    internal class CloudBlobToStorageBlobConverter : IConverter<ICloudBlob, IStorageBlob>
    {
        public IStorageBlob Convert(ICloudBlob input)
        {
            if (input == null)
            {
                return null;
            }

            CloudBlobContainer sdkContainer = input.Container;
            Debug.Assert(sdkContainer != null);
            IStorageBlobContainer container = new StorageBlobContainer(sdkContainer);

            CloudBlockBlob blockBlob = input as CloudBlockBlob;

            if (blockBlob != null)
            {
                return new StorageBlockBlob(container, blockBlob);
            }
            else
            {
                return new StoragePageBlob(container, (CloudPageBlob)input);
            }
        }
    }
}

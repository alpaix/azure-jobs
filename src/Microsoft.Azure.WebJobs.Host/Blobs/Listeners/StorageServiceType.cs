﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Azure.WebJobs.Host.Blobs.Listeners
{
    /// <summary>
    /// Enumerates possible values of the requested storage service field declared by
    /// Storage Analytics Log format.
    /// </summary>
    internal enum StorageServiceType
    {
        Blob,
        Table,
        Queue
    }
}

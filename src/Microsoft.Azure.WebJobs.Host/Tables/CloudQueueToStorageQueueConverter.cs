﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Host.Converters;
using Microsoft.Azure.WebJobs.Host.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table;

namespace Microsoft.Azure.WebJobs.Host.Tables
{
    internal class CloudTableToStorageTableConverter : IConverter<CloudTable, IStorageTable>
    {
        public IStorageTable Convert(CloudTable input)
        {
            if (input == null)
            {
                return null;
            }

            return new StorageTable(input);
        }
    }
}

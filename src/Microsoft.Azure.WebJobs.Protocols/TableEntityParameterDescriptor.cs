﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if PUBLICPROTOCOL
namespace Microsoft.Azure.WebJobs.Protocols
#else
namespace Microsoft.Azure.WebJobs.Host.Protocols
#endif
{
    /// <summary>Represents a parameter bound to a table entity in Azure Storage.</summary>
    [JsonTypeName("TableEntity")]
#if PUBLICPROTOCOL
    public class TableEntityParameterDescriptor : ParameterDescriptor
#else
    internal class TableEntityParameterDescriptor : ParameterDescriptor
#endif
    {
        /// <summary>Gets or sets the name of the storage account.</summary>
        public string AccountName { get; set; }

        /// <summary>Gets or sets the name of the table.</summary>
        public string TableName { get; set; }

        /// <summary>Gets or sets the partition key of the entity.</summary>
        public string PartitionKey { get; set; }

        /// <summary>Gets or sets the row key of the entity.</summary>
        public string RowKey { get; set; }
    }
}

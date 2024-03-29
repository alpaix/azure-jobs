﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

#if PUBLICPROTOCOL
namespace Microsoft.Azure.WebJobs.Protocols
#else
namespace Microsoft.Azure.WebJobs.Host.Protocols
#endif
{
    /// <summary>Represents a parameter bound to a blob in Azure Storage.</summary>
    [JsonTypeName("Blob")]
#if PUBLICPROTOCOL
    public class BlobParameterDescriptor : ParameterDescriptor
#else
    internal class BlobParameterDescriptor : ParameterDescriptor
#endif
    {
        /// <summary>Gets or sets the name of the storage account.</summary>
        public string AccountName { get; set; }

        /// <summary>Gets or sets the name of the container.</summary>
        public string ContainerName { get; set; }

        /// <summary>Gets or sets the name of the blob.</summary>
        public string BlobName { get; set; }

        /// <summary>Gets or sets the kind of access the parameter has to the blob.</summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public FileAccess Access { get; set; }
    }
}

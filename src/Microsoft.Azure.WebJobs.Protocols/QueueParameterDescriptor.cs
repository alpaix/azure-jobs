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
    /// <summary>Represents a parameter bound to a queue in Azure Storage.</summary>
    [JsonTypeName("Queue")]
#if PUBLICPROTOCOL
    public class QueueParameterDescriptor : ParameterDescriptor
#else
    internal class QueueParameterDescriptor : ParameterDescriptor
#endif
    {
        /// <summary>Gets or sets the name of the storage account.</summary>
        public string AccountName { get; set; }

        /// <summary>Gets or sets the name of the queue.</summary>
        public string QueueName { get; set; }

        /// <summary>Gets or sets the kind of access the parameter has to the queue.</summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public FileAccess Access { get; set; }
    }
}

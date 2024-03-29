﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Host.Converters;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Microsoft.Azure.WebJobs.Host.Tables.Converters
{
    internal class PocoToEntityPropertyConverter<TProperty> : IConverter<TProperty, EntityProperty>
    {
        public EntityProperty Convert(TProperty input)
        {
            string json = JsonConvert.SerializeObject(input, JsonSerialization.Settings);
            return new EntityProperty(json);
        }
    }
}

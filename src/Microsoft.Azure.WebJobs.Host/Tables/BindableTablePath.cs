﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Bindings.Path;

namespace Microsoft.Azure.WebJobs.Host.Tables
{
    internal static class BindableTablePath
    {
        public static IBindableTablePath Create(string tableNamePattern)
        {
            BindingTemplate template = BindingTemplate.FromString(tableNamePattern);

            if (template.ParameterNames.Count() > 0)
            {
                return new ParameterizedTablePath(template);
            }

            TableClient.ValidateAzureTableName(tableNamePattern);
            return new BoundTablePath(tableNamePattern);
        }
    }
}

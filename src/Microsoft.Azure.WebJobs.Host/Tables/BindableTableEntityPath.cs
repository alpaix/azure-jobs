﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Bindings.Path;

namespace Microsoft.Azure.WebJobs.Host.Tables
{
    internal static class BindableTableEntityPath
    {
        public static IBindableTableEntityPath Create(string tableNamePattern, string partitionKeyPattern,
            string rowKeyPattern)
        {
            BindingTemplate tableNameTemplate = BindingTemplate.FromString(tableNamePattern);
            BindingTemplate partitionKeyTemplate = BindingTemplate.FromString(partitionKeyPattern);
            BindingTemplate rowKeyTemplate = BindingTemplate.FromString(rowKeyPattern);

            if (tableNameTemplate.ParameterNames.Count() > 0 ||
                partitionKeyTemplate.ParameterNames.Count() > 0 ||
                rowKeyTemplate.ParameterNames.Count() > 0)
            {
                return new ParameterizedTableEntityPath(tableNameTemplate, partitionKeyTemplate, rowKeyTemplate);
            }

            TableClient.ValidateAzureTableName(tableNamePattern);
            TableClient.ValidateAzureTableKeyValue(partitionKeyPattern);
            TableClient.ValidateAzureTableKeyValue(rowKeyPattern);
            TableEntityPath innerPath = new TableEntityPath(tableNamePattern, partitionKeyPattern, rowKeyPattern);
            return new BoundTableEntityPath(innerPath);
        }
    }
}

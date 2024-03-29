﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.WebJobs.Host.Blobs
{
    internal static class BlobPathSourceExtensions
    {
        public static IReadOnlyDictionary<string, Type> CreateBindingDataContract(this IBlobPathSource path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            Dictionary<string, Type> contract = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

            foreach (string parameterName in path.ParameterNames)
            {
                contract.Add(parameterName, typeof(string));
            }

            return contract;
        }
    }
}

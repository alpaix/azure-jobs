﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using Microsoft.Azure.WebJobs.Protocols;
using Newtonsoft.Json;

namespace Dashboard.Data
{
    [JsonTypeName("Blob")]
    internal class BlobParameterSnapshot : ParameterSnapshot
    {
        public string ContainerName { get; set; }

        public string BlobName { get; set; }

        public bool IsInput { get; set; }

        [JsonIgnore]
        private string Path
        {
            get
            {
                return ContainerName + "/" + BlobName;
            }
        }

        public override string Description
        {
            get
            {
                if (IsInput)
                {
                    return string.Format("Read from blob: {0}", Path);
                }
                else
                {
                    return string.Format("Write to blob: {0}", Path);
                }
            }
        }

        public override string AttributeText
        {
            get { return String.Format(CultureInfo.CurrentCulture, "[Blob(\"{0}\")]", Path); }
        }

        public override string Prompt
        {
            get
            {
                if (IsInput)
                {
                    return "Enter the input blob path";
                }
                else
                {
                    return "Enter the output blob path";
                }
            }
        }

        public override string DefaultValue
        {
            get
            {
                if (HasRouteParameter(ContainerName) || HasRouteParameter(BlobName))
                {
                    return null;
                }
                else
                {
                    return Path;
                }
            }
        }

        internal static bool HasRouteParameter(string value)
        {
            if (value == null)
            {
                return false;
            }

            return value.IndexOf('{') != -1;
        }
    }
}

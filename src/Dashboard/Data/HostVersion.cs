﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Dashboard.Data
{
    /// <summary>Represents a host version compatibility warning.</summary>
    public class HostVersion
    {
        /// <summary>Gets or sets a label describing the required feature.</summary>
        public string Label { get; set; }

        /// <summary>Gets or sets a link with more compatibility information.</summary>
        public string Link { get; set; }
    }
}

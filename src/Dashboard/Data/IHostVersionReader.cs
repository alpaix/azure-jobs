﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Dashboard.Data
{
    /// <summary>Defines a reader that provides host version information.</summary>
    public interface IHostVersionReader
    {
        /// <summary>Reads all hosts and their versions.</summary>
        /// <returns>All hosts and their versions.</returns>
        HostVersion[] ReadAll();
    }
}

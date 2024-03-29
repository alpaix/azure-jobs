﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Dashboard.Data;

namespace Dashboard
{
    internal static class HostVersionConfig
    {
        private static HostVersion[] _warnings;

        public static bool HasWarning
        {
            get { return (_warnings != null) && (_warnings.Length > 0); }
        }

        public static HostVersion[] Warnings
        {
            get { return _warnings; }
        }

        public static void RegisterWarnings(IHostVersionReader versionReader)
        {
            // Currently, no host versions are supported (there's no need to create an entry for v1).
            // Any version in this table is unsupported and should result in a warning being displayed.
            _warnings = versionReader.ReadAll();
        }
    }
}

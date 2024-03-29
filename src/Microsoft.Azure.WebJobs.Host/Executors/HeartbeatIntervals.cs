﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Azure.WebJobs.Host.Executors
{
    internal static class HeartbeatIntervals
    {
        public static readonly TimeSpan NormalSignalInterval = new TimeSpan(0, 0, 30);
        public static readonly TimeSpan MinimumSignalInterval = new TimeSpan(0, 0, 10);
        public static readonly TimeSpan ExpirationInterval = new TimeSpan(0, 0, 45);
    }
}

﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Host.Executors;

namespace Microsoft.Azure.WebJobs.Host.TestCommon
{
    public class NullServiceBusAccountProvider : IServiceBusAccountProvider
    {
        public string ConnectionString
        {
            get { return null; }
        }
    }
}

﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Protocols;

namespace Dashboard.Indexers
{
    public interface IHostIndexer
    {
        void ProcessHostStarted(HostStartedMessage message);
    }
}

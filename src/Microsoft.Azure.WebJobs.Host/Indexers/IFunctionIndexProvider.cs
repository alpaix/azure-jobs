﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Host.Indexers
{
    internal interface IFunctionIndexProvider
    {
        Task<IFunctionIndex> GetAsync(CancellationToken cancellationToken);
    }
}

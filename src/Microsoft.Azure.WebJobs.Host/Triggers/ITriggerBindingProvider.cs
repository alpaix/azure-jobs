﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Host.Triggers
{
    internal interface ITriggerBindingProvider
    {
        Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context);
    }
}

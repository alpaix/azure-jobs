﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Host.Protocols;

namespace Microsoft.Azure.WebJobs.Host.Executors
{
    internal interface IFunctionInstanceFactory
    {
        IFunctionInstance Create(Guid id, Guid? parentId, ExecutionReason reason,
            IDictionary<string, object> parameters);
    }
}

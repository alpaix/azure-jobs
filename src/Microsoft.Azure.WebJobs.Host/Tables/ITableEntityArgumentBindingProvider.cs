﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Microsoft.Azure.WebJobs.Host.Tables
{
    internal interface ITableEntityArgumentBindingProvider
    {
        IArgumentBinding<TableEntityContext> TryCreate(Type parameterType);
    }
}

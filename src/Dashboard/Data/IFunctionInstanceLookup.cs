﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Dashboard.Data
{
    internal interface IFunctionInstanceLookup
    {
        FunctionInstanceSnapshot Lookup(Guid id);
    }
}

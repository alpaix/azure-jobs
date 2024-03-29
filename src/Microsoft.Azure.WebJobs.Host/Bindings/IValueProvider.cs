﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Azure.WebJobs.Host.Bindings
{
    internal interface IValueProvider
    {
        Type Type { get; }

        object GetValue();

        string ToInvokeString();
    }
}

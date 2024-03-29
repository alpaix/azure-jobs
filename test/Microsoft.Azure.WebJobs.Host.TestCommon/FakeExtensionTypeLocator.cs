﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Host.Indexers;

namespace Microsoft.Azure.WebJobs.Host.TestCommon
{
    public class FakeExtensionTypeLocator : IExtensionTypeLocator
    {
        private readonly Type[] _cloudBlobStreamBinderTypes;

        public FakeExtensionTypeLocator(params Type[] cloudBlobStreamBinderTypes)
        {
            _cloudBlobStreamBinderTypes = cloudBlobStreamBinderTypes;
        }

        public IReadOnlyList<Type> GetCloudBlobStreamBinderTypes()
        {
            return _cloudBlobStreamBinderTypes;
        }
    }
}

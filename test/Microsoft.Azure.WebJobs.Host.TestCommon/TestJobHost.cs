﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Host.TestCommon
{
    public class TestJobHost<TProgram> : JobHost
    {
        internal TestJobHost(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public void Call(string methodName)
        {
            base.Call(typeof(TProgram).GetMethod(methodName));
        }

        public void Call(string methodName, object arguments)
        {
            base.Call(typeof(TProgram).GetMethod(methodName), arguments);
        }

        public void Call(string methodName, IDictionary<string, object> arguments)
        {
            base.Call(typeof(TProgram).GetMethod(methodName), arguments);
        }

        public Task CallAsync(string methodName, IDictionary<string, object> arguments,
            CancellationToken cancellationToken)
        {
            return base.CallAsync(typeof(TProgram).GetMethod(methodName), arguments, cancellationToken);
        }
    }
}

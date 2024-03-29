﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Loggers;
using Microsoft.Azure.WebJobs.Host.Protocols;

namespace Microsoft.Azure.WebJobs.Host.FunctionalTests.TestDoubles
{
    internal class ExpectInstanceSuccessTaskFunctionInstanceLogger : IFunctionInstanceLogger
    {
        private readonly TaskCompletionSource<object> _taskSource;

        public ExpectInstanceSuccessTaskFunctionInstanceLogger(TaskCompletionSource<object> taskSource)
        {
            _taskSource = taskSource;
        }

        public Task<string> LogFunctionStartedAsync(FunctionStartedMessage message, CancellationToken cancellationToken)
        {
            return Task.FromResult(String.Empty);
        }

        public Task LogFunctionCompletedAsync(FunctionCompletedMessage message, CancellationToken cancellationToken)
        {
            if (message != null && message.Failure != null)
            {
                _taskSource.SetException(message.Failure.Exception);
            }
            else
            {
                _taskSource.SetResult(null);
            }

            return Task.FromResult(0);
        }

        public Task DeleteLogFunctionStartedAsync(string startedMessageId, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}

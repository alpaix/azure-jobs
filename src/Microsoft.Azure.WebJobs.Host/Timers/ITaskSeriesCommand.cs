﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Host.Timers
{
    internal interface ITaskSeriesCommand
    {
        Task<TaskSeriesCommandResult> ExecuteAsync(CancellationToken cancellationToken);
    }
}

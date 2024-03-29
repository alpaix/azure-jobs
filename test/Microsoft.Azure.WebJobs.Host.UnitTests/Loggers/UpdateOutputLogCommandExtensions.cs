﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.Azure.WebJobs.Host.Loggers;

namespace Microsoft.Azure.WebJobs.Host.UnitTests.Loggers
{
    internal static class UpdateOutputLogCommandExtensions
    {
        public static bool TryExecute(this UpdateOutputLogCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            return command.TryExecuteAsync(CancellationToken.None).GetAwaiter().GetResult();
        }

        public static void SaveAndClose(this UpdateOutputLogCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            command.SaveAndCloseAsync(CancellationToken.None).GetAwaiter().GetResult();
        }
    }
}

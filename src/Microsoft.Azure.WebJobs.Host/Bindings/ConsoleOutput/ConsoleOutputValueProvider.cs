﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;

namespace Microsoft.Azure.WebJobs.Host.Bindings.ConsoleOutput
{
    internal sealed class ConsoleOutputValueProvider : IValueProvider
    {
        private readonly TextWriter _consoleOutput;

        public ConsoleOutputValueProvider(TextWriter consoleOutput)
        {
            _consoleOutput = consoleOutput;
        }

        public Type Type
        {
            get { return typeof(TextWriter); }
        }

        public object GetValue()
        {
            return _consoleOutput;
        }

        public string ToInvokeString()
        {
            return null;
        }
    }
}

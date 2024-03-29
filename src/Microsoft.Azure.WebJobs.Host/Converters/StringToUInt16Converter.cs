﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;

namespace Microsoft.Azure.WebJobs.Host.Converters
{
    internal class StringToUInt16Converter : IConverter<string, ushort>
    {
        public ushort Convert(string input)
        {
            return UInt16.Parse(input, NumberStyles.None, CultureInfo.InvariantCulture);
        }
    }
}

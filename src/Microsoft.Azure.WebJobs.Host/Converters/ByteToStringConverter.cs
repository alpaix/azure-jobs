﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Globalization;

namespace Microsoft.Azure.WebJobs.Host.Converters
{
    internal class ByteToStringConverter : IConverter<byte, string>
    {
        public string Convert(byte input)
        {
            return input.ToString(CultureInfo.InvariantCulture);
        }
    }
}

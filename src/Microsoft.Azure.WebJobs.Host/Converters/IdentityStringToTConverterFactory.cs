﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Azure.WebJobs.Host.Converters
{
    internal class IdentityStringToTConverterFactory : IStringToTConverterFactory
    {
        public IConverter<string, TOutput> TryCreate<TOutput>()
        {
            if (typeof(TOutput) != typeof(string))
            {
                return null;
            }

            return (IConverter<string, TOutput>)new IdentityConverter<string>();
        }
    }
}

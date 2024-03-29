﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Host.Converters;

namespace Microsoft.Azure.WebJobs.Host.Bindings
{
    internal static class ObjectToTypeConverterFactory
    {
        public static IObjectToTypeConverter<TOutput> CreateForClass<TOutput>() where TOutput : class
        {
            IObjectToTypeConverter<TOutput> identityConverter =
                new ClassOutputConverter<TOutput, TOutput>(new IdentityConverter<TOutput>());
            return Create(identityConverter);
        }

        public static IObjectToTypeConverter<TOutput> CreateForStruct<TOutput>() where TOutput : struct
        {
            IObjectToTypeConverter<TOutput> identityConverter =
                new StructOutputConverter<TOutput, TOutput>(new IdentityConverter<TOutput>());
            return Create(identityConverter);
        }

        private static IObjectToTypeConverter<TOutput> Create<TOutput>(
            IObjectToTypeConverter<TOutput> identityConverter)
        {
            IConverter<string, TOutput> stringConverter =
                StringToTConverterFactory.Instance.TryCreate<TOutput>();

            if (stringConverter == null)
            {
                return identityConverter;
            }

            return new CompositeObjectToTypeConverter<TOutput>(
                identityConverter,
                new StringConverterObjectToTypeConverter<TOutput>(stringConverter));
        }
    }
}

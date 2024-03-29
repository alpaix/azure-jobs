﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Host.Converters;
using Microsoft.WindowsAzure.Storage.Table;

namespace Microsoft.Azure.WebJobs.Host.Tables.Converters
{
    internal class DateTimeToEntityPropertyConverter : IConverter<DateTime, EntityProperty>
    {
        private static readonly DateTime minimumValidValue = new DateTime(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public EntityProperty Convert(DateTime input)
        {
            ThrowIfUnsupportedValue(input);
            return new EntityProperty(input);
        }

        internal static void ThrowIfUnsupportedValue(DateTime input)
        {
            if (input < minimumValidValue)
            {
                throw new ArgumentOutOfRangeException("input", "Azure Tables cannot store DateTime values before the " +
                    "year 1601. Did you mean to use a nullable DateTime?");
            }
        }
    }
}

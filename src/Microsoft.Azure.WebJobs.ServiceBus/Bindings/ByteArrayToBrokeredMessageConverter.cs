﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.Azure.WebJobs.Host.Converters;
using Microsoft.ServiceBus.Messaging;

namespace Microsoft.Azure.WebJobs.ServiceBus.Bindings
{
    internal class ByteArrayToBrokeredMessageConverter : IConverter<byte[], BrokeredMessage>
    {
        public BrokeredMessage Convert(byte[] input)
        {
            if (input == null)
            {
                throw new InvalidOperationException("A brokered message cannot contain a null byte array instance.");
            }

            MemoryStream stream = new MemoryStream(input, writable: false);

            return new BrokeredMessage(stream)
            {
                ContentType = ContentTypes.ApplicationOctetStream
            };
        }
    }
}

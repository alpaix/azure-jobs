﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Newtonsoft.Json;
using Xunit;

namespace Microsoft.Azure.WebJobs.Host.UnitTests.Protocols
{
    public class HostStartedMessageTests
    {
        [Fact]
        public void JsonConvertRepeatedIdenticalChildConverterType_Roundtrips()
        {
            // Arrange
            HostStartedMessage expectedMessage = new HostStartedMessage
            {
                Functions = new FunctionDescriptor[]
                {
                    new FunctionDescriptor
                    {
                        Parameters = new ParameterDescriptor[]
                        {
                            new CallerSuppliedParameterDescriptor { Name = "A" },
                            new CallerSuppliedParameterDescriptor { Name = "B" }
                        }
                    }
                }
            };

            // Act
            PersistentQueueMessage message = JsonConvert.DeserializeObject<PersistentQueueMessage>(
                JsonConvert.SerializeObject(expectedMessage));

            // Assert
            Assert.NotNull(message);
            Assert.IsType<HostStartedMessage>(message);
            HostStartedMessage typedMessage = (HostStartedMessage)message;
            ParameterDescriptor secondChildItem = typedMessage.Functions.Single().Parameters.FirstOrDefault(p => p.Name == "B");
            Assert.IsType<CallerSuppliedParameterDescriptor>(secondChildItem);
        }
    }
}

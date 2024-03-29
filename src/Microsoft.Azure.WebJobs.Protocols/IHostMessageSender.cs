﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Azure.WebJobs.Protocols
{
    /// <summary>Defines a host message sender.</summary>
    public interface IHostMessageSender
    {
        /// <summary>Enqueues a message to the host.</summary>
        /// <param name="queueName">The name of the queue to which the host is listening.</param>
        /// <param name="message">The message to the host.</param>
        void Enqueue(string queueName, HostMessage message);
    }
}

﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;

#if PUBLICPROTOCOL
namespace Microsoft.Azure.WebJobs.Protocols
#else
namespace Microsoft.Azure.WebJobs.Host.Protocols
#endif
{
    /// <summary>Represents a null object implementation of <see cref="IHeartbeatCommand"/>.</summary>
#if PUBLICPROTOCOL
    public class NullHeartbeatCommand : IHeartbeatCommand
#else
    internal class NullHeartbeatCommand : IHeartbeatCommand
#endif
    {
        /// <inheritdoc />
        public Task BeatAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}

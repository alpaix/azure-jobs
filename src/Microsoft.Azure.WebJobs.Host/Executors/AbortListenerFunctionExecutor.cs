﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;

namespace Microsoft.Azure.WebJobs.Host.Executors
{
    internal class AbortListenerFunctionExecutor : IFunctionExecutor
    {
        private readonly IListenerFactory _abortListenerFactory;
        private readonly IFunctionExecutor _abortExecutor;
        private readonly HostBindingContext _abortContext;
        private readonly string _hostId;
        private readonly IFunctionExecutor _innerExecutor;

        public AbortListenerFunctionExecutor(IListenerFactory abortListenerFactory, IFunctionExecutor abortExecutor,
            HostBindingContext abortContext, string hostId, IFunctionExecutor innerExecutor)
        {
            _abortListenerFactory = abortListenerFactory;
            _abortExecutor = abortExecutor;
            _abortContext = abortContext;
            _hostId = hostId;
            _innerExecutor = innerExecutor;
        }

        public async Task<IDelayedException> TryExecuteAsync(IFunctionInstance instance,
            CancellationToken cancellationToken)
        {
            IDelayedException result;

            using (IListener listener = await CreateListenerAsync(cancellationToken))
            {
                await listener.StartAsync(cancellationToken);

                result = await _innerExecutor.TryExecuteAsync(instance, cancellationToken);

                await listener.StopAsync(cancellationToken);
            }

            return result;
        }

        private Task<IListener> CreateListenerAsync(CancellationToken cancellationToken)
        {
            ListenerFactoryContext listenerContext = new ListenerFactoryContext(_abortContext, _hostId,
                new SharedListenerContainer(), cancellationToken);
            return _abortListenerFactory.CreateAsync(_abortExecutor, listenerContext);
        }
    }
}

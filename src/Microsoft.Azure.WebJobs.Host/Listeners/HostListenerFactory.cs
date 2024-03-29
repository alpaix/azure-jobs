﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Indexers;

namespace Microsoft.Azure.WebJobs.Host.Listeners
{
    internal class HostListenerFactory : IListenerFactory
    {
        private readonly IEnumerable<IFunctionDefinition> _functionDefinitions;
        private readonly IListenerFactory _sharedQueueListenerFactory;
        private readonly IListenerFactory _instanceQueueListenerFactory;

        public HostListenerFactory(IEnumerable<IFunctionDefinition> functionDefinitions,
            IListenerFactory sharedQueueListenerFactory,
            IListenerFactory instanceQueueListenerFactory)
        {
            _functionDefinitions = functionDefinitions;
            _sharedQueueListenerFactory = sharedQueueListenerFactory;
            _instanceQueueListenerFactory = instanceQueueListenerFactory;
        }

        public async Task<IListener> CreateAsync(IFunctionExecutor executor, ListenerFactoryContext context)
        {
            List<IListener> listeners = new List<IListener>();

            foreach (IFunctionDefinition functionDefinition in _functionDefinitions)
            {
                IListenerFactory listenerFactory = functionDefinition.ListenerFactory;

                if (listenerFactory == null)
                {
                    continue;
                }

                IListener listener = await listenerFactory.CreateAsync(executor, context);
                listeners.Add(listener);
            }

            IListener sharedQueueListener = await _sharedQueueListenerFactory.CreateAsync(executor, context);

            if (sharedQueueListener != null)
            {
                listeners.Add(sharedQueueListener);
            }

            IListener instanceQueueListener = await _instanceQueueListenerFactory.CreateAsync(executor, context);

            if (instanceQueueListener != null)
            {
                listeners.Add(instanceQueueListener);
            }

            return new CompositeListener(listeners);
        }
    }
}

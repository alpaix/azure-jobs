﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Reflection;
using Microsoft.Azure.WebJobs.Host.Storage.Queue;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace Microsoft.Azure.WebJobs.Host.Queues.Triggers
{
    internal class CompositeArgumentBindingProvider : IQueueTriggerArgumentBindingProvider
    {
        private readonly IEnumerable<IQueueTriggerArgumentBindingProvider> _providers;

        public CompositeArgumentBindingProvider(params IQueueTriggerArgumentBindingProvider[] providers)
        {
            _providers = providers;
        }

        public ITriggerDataArgumentBinding<IStorageQueueMessage> TryCreate(ParameterInfo parameter)
        {
            foreach (IQueueTriggerArgumentBindingProvider provider in _providers)
            {
                ITriggerDataArgumentBinding<IStorageQueueMessage> binding = provider.TryCreate(parameter);

                if (binding != null)
                {
                    return binding;
                }
            }

            return null;
        }
    }
}

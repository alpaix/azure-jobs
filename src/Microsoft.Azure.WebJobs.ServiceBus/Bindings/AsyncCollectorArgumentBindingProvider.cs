﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Converters;
using Microsoft.ServiceBus.Messaging;

namespace Microsoft.Azure.WebJobs.ServiceBus.Bindings
{
    internal class AsyncCollectorArgumentBindingProvider : IQueueArgumentBindingProvider
    {
        public IArgumentBinding<ServiceBusEntity> TryCreate(ParameterInfo parameter)
        {
            Type parameterType = parameter.ParameterType;

            if (!parameterType.IsGenericType)
            {
                return null;
            }

            Type genericTypeDefinition = parameterType.GetGenericTypeDefinition();

            if (genericTypeDefinition != typeof(IAsyncCollector<>))
            {
                return null;
            }

            Type itemType = parameterType.GetGenericArguments()[0];
            return CreateBinding(itemType);
        }

        private static IArgumentBinding<ServiceBusEntity> CreateBinding(Type itemType)
        {
            MethodInfo method = typeof(AsyncCollectorArgumentBindingProvider).GetMethod("CreateBindingGeneric",
                BindingFlags.NonPublic | BindingFlags.Static);
            Debug.Assert(method != null);
            MethodInfo genericMethod = method.MakeGenericMethod(itemType);
            Debug.Assert(genericMethod != null);
            Func<IArgumentBinding<ServiceBusEntity>> lambda =
                (Func<IArgumentBinding<ServiceBusEntity>>)Delegate.CreateDelegate(
                typeof(Func<IArgumentBinding<ServiceBusEntity>>), genericMethod);
            return lambda.Invoke();
        }

        private static IArgumentBinding<ServiceBusEntity> CreateBindingGeneric<TItem>()
        {
            return new AsyncCollectorArgumentBinding<TItem>(MessageConverterFactory.Create<TItem>());
        }

        private class AsyncCollectorArgumentBinding<TItem> : IArgumentBinding<ServiceBusEntity>
        {
            private readonly IConverter<TItem, BrokeredMessage> _converter;

            public AsyncCollectorArgumentBinding(IConverter<TItem, BrokeredMessage> converter)
            {
                _converter = converter;
            }

            public Type ValueType
            {
                get { return typeof(IAsyncCollector<TItem>); }
            }

            public Task<IValueProvider> BindAsync(ServiceBusEntity value, ValueBindingContext context)
            {
                IAsyncCollector<TItem> collector = new MessageSenderAsyncCollector<TItem>(value, _converter,
                    context.FunctionInstanceId);
                IValueProvider provider = new CollectorValueProvider(value, collector, typeof(IAsyncCollector<TItem>));
                return Task.FromResult(provider);
            }
        }
    }
}

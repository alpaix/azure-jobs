﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Converters;
using Microsoft.ServiceBus.Messaging;

namespace Microsoft.Azure.WebJobs.ServiceBus.Bindings
{
    internal class ConverterValueBinder<TInput> : IOrderedValueBinder
    {
        private readonly ServiceBusEntity _entity;
        private readonly IConverter<TInput, BrokeredMessage> _converter;
        private readonly Guid _functionInstanceId;

        public ConverterValueBinder(ServiceBusEntity entity, IConverter<TInput, BrokeredMessage> converter,
            Guid functionInstanceId)
        {
            _entity = entity;
            _converter = converter;
            _functionInstanceId = functionInstanceId;
        }

        public int StepOrder
        {
            get { return BindStepOrders.Enqueue; }
        }

        public Type Type
        {
            get { return typeof(TInput); }
        }

        public object GetValue()
        {
            return default(TInput);
        }

        public string ToInvokeString()
        {
            return _entity.MessageSender.Path;
        }

        public Task SetValueAsync(object value, CancellationToken cancellationToken)
        {
            BrokeredMessage message = _converter.Convert((TInput)value);
            Debug.Assert(message != null);
            return _entity.SendAndCreateQueueIfNotExistsAsync(message, _functionInstanceId, cancellationToken);
        }
    }
}

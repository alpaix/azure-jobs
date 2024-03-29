﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Azure.WebJobs.Host.Protocols;

namespace Microsoft.Azure.WebJobs.Host.Indexers
{
    internal class FunctionIndex : IFunctionIndex, IFunctionIndexCollector
    {
        private readonly IDictionary<string, IFunctionDefinition> _functionsById;
        private readonly IDictionary<MethodInfo, IFunctionDefinition> _functionsByMethod;
        private readonly ICollection<FunctionDescriptor> _functionDescriptors;

        public FunctionIndex()
        {
            _functionsById = new Dictionary<string, IFunctionDefinition>();
            _functionsByMethod = new Dictionary<MethodInfo, IFunctionDefinition>();
            _functionDescriptors = new List<FunctionDescriptor>();
        }

        public void Add(IFunctionDefinition function, FunctionDescriptor descriptor, MethodInfo method)
        {
            string id = descriptor.Id;

            if (_functionsById.ContainsKey(id))
            {
                throw new InvalidOperationException("Method overloads are not supported. " +
                    "There are multiple methods with the name '" + id + "'.");
            }

            _functionsById.Add(id, function);
            _functionsByMethod.Add(method, function);
            _functionDescriptors.Add(descriptor);
        }

        public IFunctionDefinition Lookup(string functionId)
        {
            if (!_functionsById.ContainsKey(functionId))
            {
                return null;
            }

            return _functionsById[functionId];
        }

        public IFunctionDefinition Lookup(MethodInfo method)
        {
            if (!_functionsByMethod.ContainsKey(method))
            {
                return null;
            }

            return _functionsByMethod[method];
        }

        public IEnumerable<IFunctionDefinition> ReadAll()
        {
            return _functionsById.Values;
        }

        public IEnumerable<FunctionDescriptor> ReadAllDescriptors()
        {
            return _functionDescriptors;
        }

        public IEnumerable<MethodInfo> ReadAllMethods()
        {
            return _functionsByMethod.Keys;
        }
    }
}

﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Azure.WebJobs.Host.Indexers
{
    // Default policy for locating types. 
    internal class DefaultTypeLocator : ITypeLocator
    {
        private static readonly string _azureJobsAssemblyName = typeof(TableAttribute).Assembly.GetName().Name;
        private static readonly string _azureJobsServiceBusAssemblyName = "Microsoft.Azure.WebJobs.ServiceBus";

        private static Type[] EmptyTypeArray = new Type[0];

        // Helper to filter out assemblies that don't even reference this SDK.
        private static bool DoesAssemblyReferenceSdk(Assembly a)
        {
            // Don't index methods in our Host or ServiceBus assemblies.
            if (typeof(DefaultTypeLocator).Assembly == a)
            {
                return false;
            }
            else if (a.GetName().Name == "Microsoft.Azure.WebJobs.ServiceBus")
            {
                return false;
            }

            AssemblyName[] referencedAssemblyNames = a.GetReferencedAssemblies();
            foreach (var referencedAssemblyName in referencedAssemblyNames)
            {
                if (String.Equals(referencedAssemblyName.Name, _azureJobsAssemblyName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (String.Equals(referencedAssemblyName.Name, _azureJobsServiceBusAssemblyName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public IReadOnlyList<Type> GetTypes()
        {
            List<Type> allTypes = new List<Type>();

            var assemblies = GetUserAssemblies();
            foreach (var assembly in assemblies)
            {
                var assemblyTypes = FindTypes(assembly);

                if (assemblyTypes != null)
                {
                    allTypes.AddRange(assemblyTypes.Where(IsJobClass));
                }
            }

            return allTypes;
        }

        public static bool IsJobClass(Type type)
        {
            if (type == null)
            {
                return false;
            }

            return type.IsClass
                // For C# static keyword classes, IsAbstract and IsSealed both return true. Include C# static keyword
                // classes but not C# abstract keyword classes.
                && (!type.IsAbstract || type.IsSealed)
                // We only consider public top-level classes as job classes. IsPublic returns false for nested classes,
                // regardless of visibility modifiers. 
                && type.IsPublic
                && !type.ContainsGenericParameters;
        }

        private static IEnumerable<Assembly> GetUserAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        public Type[] FindTypes(Assembly a)
        {
            // Only try to index assemblies that reference this SDK.
            // This avoids trying to index through a bunch of FX assemblies that reflection may not be able to load anyways.
            if (!DoesAssemblyReferenceSdk(a))
            {
                return null;
            }

            Type[] types = null;

            try
            {
                types = a.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                // TODO: Log this somewhere?
                Console.WriteLine("Warning: Only got partial types from assembly: {0}", a.FullName);
                Console.WriteLine("Exception message: {0}", ex.ToString());

                // In case of a type load exception, at least get the types that did succeed in loading
                types = ex.Types;
            }
            catch (Exception ex)
            {
                // TODO: Log this somewhere?
                Console.WriteLine("Warning: Failed to get types from assembly: {0}", a.FullName);
                Console.WriteLine("Exception message: {0}", ex.ToString());
            }

            return types;
        }
    }
}

﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Indexers;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Loggers;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Queues;
using Microsoft.Azure.WebJobs.Host.Queues.Listeners;
using Microsoft.Azure.WebJobs.Host.Storage;
using Microsoft.Azure.WebJobs.Host.Storage.Queue;
using Microsoft.Azure.WebJobs.Host.Timers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Microsoft.Azure.WebJobs.Host.Executors
{
    internal class JobHostContextFactory
    {
        private readonly IStorageAccountProvider _storageAccountProvider;
        private readonly IFunctionIndexProvider _functionIndexProvider;
        private readonly INameResolver _nameResolver;
        private readonly IBindingProvider _bindingProvider;
        private readonly IHostIdProvider _hostIdProvider;
        private readonly IHostInstanceLoggerProvider _hostInstanceLoggerProvider;
        private readonly IFunctionInstanceLoggerProvider _functionInstanceLoggerProvider;
        private readonly IQueueConfiguration _queueConfiguration;
        private readonly IBackgroundExceptionDispatcher _backgroundExceptionDispatcher;
        private readonly CancellationToken _shutdownToken;

        public JobHostContextFactory(IStorageAccountProvider storageAccountProvider,
            IFunctionIndexProvider functionIndexProvider,
            INameResolver nameResolver,
            IBindingProvider bindingProvider,
            IHostIdProvider hostIdProvider,
            IHostInstanceLoggerProvider hostInstanceLoggerProvider,
            IFunctionInstanceLoggerProvider functionInstanceLoggerProvider,
            IQueueConfiguration queueConfiguration,
            IBackgroundExceptionDispatcher backgroundExceptionDispatcher,
            CancellationToken shutdownToken)
        {
            _storageAccountProvider = storageAccountProvider;
            _functionIndexProvider = functionIndexProvider;
            _nameResolver = nameResolver;
            _bindingProvider = bindingProvider;
            _hostIdProvider = hostIdProvider;
            _hostInstanceLoggerProvider = hostInstanceLoggerProvider;
            _functionInstanceLoggerProvider = functionInstanceLoggerProvider;
            _queueConfiguration = queueConfiguration;
            _backgroundExceptionDispatcher = backgroundExceptionDispatcher;
            _shutdownToken = shutdownToken;
        }

        public async Task<JobHostContext> CreateAndLogHostStartedAsync(CancellationToken cancellationToken)
        {
            using (CancellationTokenSource combinedCancellationSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _shutdownToken))
            {
                CancellationToken combinedCancellationToken = combinedCancellationSource.Token;

                IStorageAccount dashboardAccount = await _storageAccountProvider.GetDashboardAccountAsync(
                    combinedCancellationToken);
                CloudStorageAccount sdkDashboardAccount = dashboardAccount != null ? dashboardAccount.SdkObject : null;

                IHostInstanceLogger hostInstanceLogger = await _hostInstanceLoggerProvider.GetAsync(
                    combinedCancellationToken);
                IFunctionInstanceLogger functionInstanceLogger = await _functionInstanceLoggerProvider.GetAsync(
                    combinedCancellationToken);

                IFunctionOutputLogger functionOutputLogger;

                if (dashboardAccount != null)
                {
                    CloudBlobClient dashboardBlobClient = sdkDashboardAccount.CreateCloudBlobClient();
                    functionOutputLogger = new BlobFunctionOutputLogger(dashboardBlobClient);
                }
                else
                {
                    functionOutputLogger = new ConsoleFunctionOutputLogger();
                }

                IFunctionIndex functions = await _functionIndexProvider.GetAsync(combinedCancellationToken);

                string hostId = await _hostIdProvider.GetHostIdAsync(cancellationToken);

                if (!HostIdValidator.IsValid(hostId))
                {
                    throw new InvalidOperationException(HostIdValidator.ValidationMessage);
                }

                HostBindingContext bindingContext = new HostBindingContext(_backgroundExceptionDispatcher,
                    _bindingProvider, _nameResolver, _queueConfiguration);

                IListenerFactory sharedQueueListenerFactory;
                IListenerFactory instanceQueueListenerFactory;
                IRecurrentCommand heartbeatCommand;
                HostOutputMessage hostOutputMessage;

                if (dashboardAccount != null)
                {
                    string sharedQueueName = HostQueueNames.GetHostQueueName(hostId);
                    IStorageQueueClient dashboardQueueClient = dashboardAccount.CreateQueueClient();
                    IStorageQueue sharedQueue = dashboardQueueClient.GetQueueReference(sharedQueueName);
                    sharedQueueListenerFactory = new HostMessageListenerFactory(sharedQueue, functions,
                        functionInstanceLogger);

                    Guid hostInstanceId = Guid.NewGuid();
                    string instanceQueueName = HostQueueNames.GetHostQueueName(hostInstanceId.ToString("N"));
                    IStorageQueue instanceQueue = dashboardQueueClient.GetQueueReference(instanceQueueName);
                    instanceQueueListenerFactory = new HostMessageListenerFactory(instanceQueue, functions,
                        functionInstanceLogger);

                    HeartbeatDescriptor heartbeatDescriptor = new HeartbeatDescriptor
                    {
                        SharedContainerName = HostContainerNames.Hosts,
                        SharedDirectoryName = HostDirectoryNames.Heartbeats + "/" + hostId,
                        InstanceBlobName = hostInstanceId.ToString("N"),
                        ExpirationInSeconds = (int)HeartbeatIntervals.ExpirationInterval.TotalSeconds
                    };
                    heartbeatCommand = new UpdateHostHeartbeatCommand(new HeartbeatCommand(sdkDashboardAccount,
                        heartbeatDescriptor.SharedContainerName,
                        heartbeatDescriptor.SharedDirectoryName + "/" + heartbeatDescriptor.InstanceBlobName));

                    IEnumerable<MethodInfo> indexedMethods = functions.ReadAllMethods();
                    Assembly hostAssembly = GetHostAssembly(indexedMethods);
                    string displayName = hostAssembly != null ? hostAssembly.GetName().Name : "Unknown";

                    hostOutputMessage = new DataOnlyHostOutputMessage
                    {
                        HostInstanceId = hostInstanceId,
                        HostDisplayName = displayName,
                        SharedQueueName = sharedQueueName,
                        InstanceQueueName = instanceQueueName,
                        Heartbeat = heartbeatDescriptor,
                        WebJobRunIdentifier = WebJobRunIdentifier.Current
                    };

                    // Publish this to Azure logging account so that a web dashboard can see it. 
                    await LogHostStartedAsync(functions, hostOutputMessage, hostInstanceLogger,
                        combinedCancellationToken);
                }
                else
                {
                    sharedQueueListenerFactory = new NullListenerFactory();
                    instanceQueueListenerFactory = new NullListenerFactory();
                    heartbeatCommand = new NullRecurrentCommand();
                    hostOutputMessage = new DataOnlyHostOutputMessage();
                }

                IFunctionExecutor executor = new FunctionExecutor(new FunctionExecutorContext(functionInstanceLogger,
                    functionOutputLogger, bindingContext, hostOutputMessage));
                IListenerFactory allFunctionsListenerFactory = new HostListenerFactory(functions.ReadAll(),
                    sharedQueueListenerFactory, instanceQueueListenerFactory);

                IFunctionExecutor hostCallExecutor = CreateHostCallExecutor(hostId, instanceQueueListenerFactory,
                    bindingContext, heartbeatCommand, _shutdownToken, executor);

                IListener listener = CreateHostListener(hostId, allFunctionsListenerFactory, bindingContext,
                    heartbeatCommand, _shutdownToken, executor);

                IEnumerable<FunctionDescriptor> descriptors = functions.ReadAllDescriptors();
                int descriptorsCount = descriptors.Count();

                if (descriptorsCount == 0)
                {
                    Console.WriteLine(
                        "No functions found. Try making job classes public and methods public static.");
                }
                else
                {
                    Console.WriteLine("Found the following functions:");

                    foreach (FunctionDescriptor descriptor in descriptors)
                    {
                        Console.WriteLine(descriptor.FullName);
                    }
                }

                return new JobHostContext(functions, hostCallExecutor, listener);
            }
        }

        private static IFunctionExecutor CreateHostCallExecutor(string hostId,
            IListenerFactory instanceQueueListenerFactory, HostBindingContext bindingContext,
            IRecurrentCommand heartbeatCommand, CancellationToken shutdownToken, IFunctionExecutor innerExecutor)
        {
            IFunctionExecutor heartbeatExecutor = new HeartbeatFunctionExecutor(heartbeatCommand,
                bindingContext.BackgroundExceptionDispatcher, innerExecutor);
            IFunctionExecutor abortListenerExecutor = new AbortListenerFunctionExecutor(instanceQueueListenerFactory,
                innerExecutor, bindingContext, hostId, heartbeatExecutor);
            IFunctionExecutor shutdownFunctionExecutor = new ShutdownFunctionExecutor(shutdownToken,
                abortListenerExecutor);
            return shutdownFunctionExecutor;
        }

        private static IListener CreateHostListener(string hostId, IListenerFactory allFunctionsListenerFactory,
            HostBindingContext bindingContext, IRecurrentCommand heartbeatCommand, CancellationToken shutdownToken,
            IFunctionExecutor executor)
        {
            IListener factoryListener = new ListenerFactoryListener(allFunctionsListenerFactory, executor,
                bindingContext, hostId);
            IListener heartbeatListener = new HeartbeatListener(heartbeatCommand,
                bindingContext.BackgroundExceptionDispatcher, factoryListener);
            IListener shutdownListener = new ShutdownListener(shutdownToken, heartbeatListener);
            return shutdownListener;
        }

        private static Assembly GetHostAssembly(IEnumerable<MethodInfo> methods)
        {
            // 1. Try to get the assembly name from the first method.
            MethodInfo firstMethod = methods.FirstOrDefault();

            if (firstMethod != null)
            {
                return firstMethod.DeclaringType.Assembly;
            }

            // 2. If there are no function definitions, try to use the entry assembly.
            Assembly entryAssembly = Assembly.GetEntryAssembly();

            if (entryAssembly != null)
            {
                return entryAssembly;
            }

            // 3. If there's no entry assembly either, we don't have anything to use.
            return null;
        }

        private static Task LogHostStartedAsync(IFunctionIndex functionIndex, HostOutputMessage hostOutputMessage,
            IHostInstanceLogger logger, CancellationToken cancellationToken)
        {
            IEnumerable<FunctionDescriptor> functions = functionIndex.ReadAllDescriptors();

            HostStartedMessage message = new HostStartedMessage
            {
                HostInstanceId = hostOutputMessage.HostInstanceId,
                HostDisplayName = hostOutputMessage.HostDisplayName,
                SharedQueueName = hostOutputMessage.SharedQueueName,
                InstanceQueueName = hostOutputMessage.InstanceQueueName,
                Heartbeat = hostOutputMessage.Heartbeat,
                WebJobRunIdentifier = hostOutputMessage.WebJobRunIdentifier,
                Functions = functions
            };

            return logger.LogHostStartedAsync(message, cancellationToken);
        }

        private class DataOnlyHostOutputMessage : HostOutputMessage
        {
            internal override void AddMetadata(IDictionary<string, string> metadata)
            {
                throw new NotSupportedException();
            }
        }
    }
}

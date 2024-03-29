﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Timers;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace Microsoft.Azure.WebJobs.Host.Loggers
{
    internal sealed class UpdateParameterLogCommand : IRecurrentCommand
    {
        private readonly IReadOnlyDictionary<string, IWatcher> _watches;
        private readonly CloudBlockBlob _parameterLogBlob;
        private readonly TextWriter _consoleOutput;

        private string _lastContent;

        public UpdateParameterLogCommand(IReadOnlyDictionary<string, IWatcher> watches, CloudBlockBlob parameterLogBlob,
            TextWriter consoleOutput)
        {
            if (parameterLogBlob == null)
            {
                throw new ArgumentNullException("parameterLogBlob");
            }
            else if (consoleOutput == null)
            {
                throw new ArgumentNullException("consoleOutput");
            }
            else if (watches == null)
            {
                throw new ArgumentNullException("watches");
            }

            _parameterLogBlob = parameterLogBlob;
            _consoleOutput = consoleOutput;
            _watches = watches;
        }

        public static void AddLogs(IReadOnlyDictionary<string, IWatcher> watches,
            IDictionary<string, ParameterLog> collector)
        {
            foreach (KeyValuePair<string, IWatcher> item in watches)
            {
                IWatcher watch = item.Value;

                if (watch == null)
                {
                    continue;
                }

                ParameterLog status = watch.GetStatus();

                if (status == null)
                {
                    continue;
                }

                collector.Add(item.Key, status);
            }
        }

        public async Task<bool> TryExecuteAsync(CancellationToken cancellationToken)
        {
            Dictionary<string, ParameterLog> logs = new Dictionary<string, ParameterLog>();
            AddLogs(_watches, logs);
            string content = JsonConvert.SerializeObject(logs, JsonSerialization.Settings);

            try
            {
                if (_lastContent == content)
                {
                    // If it hasn't change, then don't re upload stale content.
                    return true;
                }

                _lastContent = content;
                await _parameterLogBlob.UploadTextAsync(content, cancellationToken);
                return true;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                // Not fatal if we can't update parameter status. 
                // But at least log what happened for diagnostics in case it's an infrastructure bug.                 
                _consoleOutput.WriteLine("---- Parameter status update failed ---");
                _consoleOutput.WriteLine(e.ToDetails());
                _consoleOutput.WriteLine("-------------------------");
                return false;
            }
        }
    }
}

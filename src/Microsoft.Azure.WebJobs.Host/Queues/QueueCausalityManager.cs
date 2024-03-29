﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.WebJobs.Host.Queues
{
    /// <summary>
    /// Tracks causality via JSON formatted queue message content. 
    /// Adds an extra field to the JSON object for the parent guid name.
    /// </summary>
    /// <remarks>
    /// Important that this class can interoperate with external queue messages, 
    /// so be resilient to a missing guid marker. 
    /// Can we switch to some auxillary table? Beware, CloudQueueMessage. 
    /// Id is not filled out until after the message is queued, 
    /// but then there's a race between updating the aux storage and another function picking up the message.
    /// </remarks>
    internal static class QueueCausalityManager
    {
        const string parentGuidFieldName = "$AzureWebJobsParentId";

        public static void SetOwner(Guid functionOwner, JObject token)
        {
            if (token == null)
            {
                throw new ArgumentNullException("token");
            }

            if (!Guid.Equals(Guid.Empty, functionOwner))
            {
                token[parentGuidFieldName] = functionOwner.ToString();
            }
        }

        [DebuggerNonUserCode]
        public static Guid? GetOwner(IStorageQueueMessage msg)
        {
            string text = msg.TryGetAsString();

            if (text == null)
            {
                return null;
            }

            IDictionary<string, JToken> json;
            try
            {
                json = JsonSerialization.ParseJObject(text);
            }
            catch (Exception)
            {
                return null;
            }

            if (!json.ContainsKey(parentGuidFieldName) || json[parentGuidFieldName].Type != JTokenType.String)
            {
                return null;
            }

            string val = (string)json[parentGuidFieldName];

            Guid guid;
            if (Guid.TryParse(val, out guid))
            {
                return guid;
            }
            return null;
        }
    }
}

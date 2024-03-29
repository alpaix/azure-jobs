﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.WebJobs.Host.Blobs.Listeners
{
    internal class BlobReceipt
    {
        private const string IncompleteKey = "Incomplete";

        private static readonly BlobReceipt _completedInstance = new BlobReceipt(incomplete: false);
        private static readonly BlobReceipt _incompleteInstance = new BlobReceipt(incomplete: true);

        private readonly bool _incomplete;

        private BlobReceipt(bool incomplete)
        {
            _incomplete = incomplete;
        }

        public static BlobReceipt Complete
        {
            get { return _completedInstance; }
        }

        public static BlobReceipt Incomplete
        {
            get { return _incompleteInstance; }
        }

        public bool IsCompleted
        {
            get { return !_incomplete; }
        }

        public void ToMetadata(IDictionary<string, string> metadata)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }

            if (_incomplete)
            {
                // re-use the key as the value, ala HTML (the presence of the key is what matters, not its value).
                metadata[IncompleteKey] = IncompleteKey;
            }
            else
            {
                metadata.Remove(IncompleteKey);
            }
        }

        public static BlobReceipt FromMetadata(IDictionary<string, string> metadata)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }

            bool incomplete = metadata.ContainsKey(IncompleteKey);

            return incomplete ? Incomplete : Complete;
        }
    }
}

﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Dashboard.Data
{
    public class FunctionIndexManager : IFunctionIndexManager
    {
        private readonly IVersionedDocumentStore<FunctionSnapshot> _store;

        [CLSCompliant(false)]
        public FunctionIndexManager(CloudBlobClient client)
            : this(VersionedDocumentStore.CreateJsonBlobStore<FunctionSnapshot>(
                client, DashboardContainerNames.Dashboard, DashboardDirectoryNames.FunctionsFlat))
        {
        }

        private FunctionIndexManager(IVersionedDocumentStore<FunctionSnapshot> store)
        {
            _store = store;
        }

        public IEnumerable<VersionedMetadata> List(string hostId)
        {
            // Blob name under function/flat dircetory is hostId_FunctionId.
            return _store.List(hostId + "_");
        }

        public bool CreateOrUpdateIfLatest(FunctionSnapshot snapshot)
        {
            return _store.CreateOrUpdateIfLatest(snapshot.Id, snapshot.HostVersion, CreateOtherMetadata(snapshot),
                snapshot);
        }

        public bool UpdateOrCreateIfLatest(FunctionSnapshot snapshot, string currentETag, DateTimeOffset currentVersion)
        {
            return _store.UpdateOrCreateIfLatest(snapshot.Id, snapshot.HostVersion, CreateOtherMetadata(snapshot),
                snapshot, currentETag, currentVersion);
        }

        public bool DeleteIfLatest(string id, DateTimeOffset deleteThroughVersion)
        {
            return _store.DeleteIfLatest(id, deleteThroughVersion);
        }

        public bool DeleteIfLatest(string id, DateTimeOffset deleteThroughVersion, string currentETag,
            DateTimeOffset currentVersion)
        {
            return _store.DeleteIfLatest(id, deleteThroughVersion, currentETag, currentVersion);
        }

        private IDictionary<string, string> CreateOtherMetadata(FunctionSnapshot snapshot)
        {
            return FunctionIndexEntry.CreateOtherMetadata(snapshot);
        }
    }
}

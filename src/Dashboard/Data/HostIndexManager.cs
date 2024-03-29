﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.WindowsAzure.Storage.Blob;

namespace Dashboard.Data
{
    internal class HostIndexManager : IHostIndexManager
    {
        private readonly IVersionedDocumentStore<HostSnapshot> _store;

        public HostIndexManager(CloudBlobClient client)
            : this(VersionedDocumentStore.CreateJsonBlobStore<HostSnapshot>(
                client, DashboardContainerNames.Dashboard, DashboardDirectoryNames.Hosts))
        {
        }

        private HostIndexManager(IVersionedDocumentStore<HostSnapshot> store)
        {
            _store = store;
        }

        public HostSnapshot Read(string id)
        {
            VersionedMetadataDocument<HostSnapshot> versionedDocument = _store.Read(id);

            if (versionedDocument == null)
            {
                return null;
            }

            return versionedDocument.Document;
        }

        public bool UpdateOrCreateIfLatest(string id, HostSnapshot snapshot)
        {
            return _store.UpdateOrCreateIfLatest(id, snapshot.HostVersion, otherMetadata: null, document: snapshot);
        }
    }
}

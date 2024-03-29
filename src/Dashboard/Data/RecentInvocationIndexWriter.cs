﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Dashboard.Data
{
    public class RecentInvocationIndexWriter : IRecentInvocationIndexWriter
    {
        private readonly IConcurrentMetadataTextStore _store;

        [CLSCompliant(false)]
        public RecentInvocationIndexWriter(CloudBlobClient client)
            : this(ConcurrentTextStore.CreateBlobStore(
                client, DashboardContainerNames.Dashboard, DashboardDirectoryNames.RecentFunctionsFlat))
        {
        }

        private RecentInvocationIndexWriter(IConcurrentMetadataTextStore store)
        {
            _store = store;
        }

        public void CreateOrUpdate(FunctionInstanceSnapshot snapshot, DateTimeOffset timestamp)
        {
            string innerId = CreateInnerId(timestamp, snapshot.Id);
            _store.CreateOrUpdate(innerId, RecentInvocationEntry.CreateMetadata(snapshot), String.Empty);
        }

        public void DeleteIfExists(DateTimeOffset timestamp, Guid id)
        {
            string innerId = CreateInnerId(timestamp, id);
            _store.DeleteIfExists(innerId);
        }

        private static string CreateInnerId(DateTimeOffset timestamp, Guid id)
        {
            return RecentInvocationEntry.CreateBlobName(timestamp, id);
        }
    }
}

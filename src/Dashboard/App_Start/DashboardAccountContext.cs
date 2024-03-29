﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Dashboard.Data;
using Microsoft.WindowsAzure.Storage;

namespace Dashboard
{
    public class DashboardAccountContext
    {
        public const string ConnectionStringName = ConnectionStringNames.Dashboard;
        public static readonly string PrefixedConnectionStringName = 
            ConnectionStringProvider.GetPrefixedConnectionStringName(ConnectionStringName); 

        public ConnectionStringState ConnectionStringState { get; internal set; }
        
        public bool HasSetupError
        {
            get { return ConnectionStringState != ConnectionStringState.Valid; }
        }

        public string SdkStorageAccountName { get; internal set; }

        [CLSCompliant(false)]
        public CloudStorageAccount StorageAccount { get; internal set; }

        public DashboardAccountContext()
        {
            ConnectionStringState = ConnectionStringState.Unknown;
            StorageAccount = null;
        }
    }
}
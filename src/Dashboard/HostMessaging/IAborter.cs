﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Dashboard.HostMessaging
{
    public interface IAborter
    {
        void RequestHostInstanceAbort(string queueName);

        bool HasRequestedHostInstanceAbort(string queueName);
    }
}

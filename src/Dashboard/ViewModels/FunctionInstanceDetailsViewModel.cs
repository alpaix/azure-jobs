﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Dashboard.ViewModels
{
    public class FunctionInstanceDetailsViewModel
    {
        public InvocationLogViewModel Invocation { get; set; }

        public ParamModel[] Parameters { get; set; }

        public IEnumerable<Guid> ChildrenIds { get; set; }

        public InvocationLogViewModel Ancestor { get; set; }

        public TriggerReasonViewModel TriggerReason { get; set; }

        public string Trigger { get; set; }

        public bool IsAborted { get; set; }
    }
}

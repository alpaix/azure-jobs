﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Dashboard.ViewModels
{
    public class RunFunctionViewModel
    {
        public string QueueName { get; set; }
        public string FunctionId { get; set; }
        public IEnumerable<FunctionParameterViewModel> Parameters { get; set; }
        public Guid? ParentId { get; set; }

        public string FunctionName { get; set; }
        public bool HostIsNotRunning { get; set; }
        public string SubmitText { get; set; }
        public string FunctionFullName { get; set; }
    }
}

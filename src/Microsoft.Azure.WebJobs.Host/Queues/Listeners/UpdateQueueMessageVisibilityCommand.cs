﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Storage;
using Microsoft.Azure.WebJobs.Host.Storage.Queue;
using Microsoft.Azure.WebJobs.Host.Timers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Microsoft.Azure.WebJobs.Host.Queues.Listeners
{
    internal class UpdateQueueMessageVisibilityCommand : ITaskSeriesCommand
    {
        private readonly IStorageQueue _queue;
        private readonly IStorageQueueMessage _message;
        private readonly TimeSpan _visibilityTimeout;
        private readonly IDelayStrategy _speedupStrategy;

        public UpdateQueueMessageVisibilityCommand(IStorageQueue queue, IStorageQueueMessage message,
            TimeSpan visibilityTimeout, IDelayStrategy speedupStrategy)
        {
            if (queue == null)
            {
                throw new ArgumentNullException("queue");
            }

            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (speedupStrategy == null)
            {
                throw new ArgumentNullException("speedupStrategy");
            }

            _queue = queue;
            _message = message;
            _visibilityTimeout = visibilityTimeout;
            _speedupStrategy = speedupStrategy;
        }

        public async Task<TaskSeriesCommandResult> ExecuteAsync(CancellationToken cancellationToken)
        {
            TimeSpan delay;

            try
            {
                await _queue.UpdateMessageAsync(_message, _visibilityTimeout, MessageUpdateFields.Visibility,
                    cancellationToken);
                // The next execution should occur after a normal delay.
                delay = _speedupStrategy.GetNextDelay(executionSucceeded: true);
            }
            catch (StorageException exception)
            {
                // For consistency, the exceptions handled here should match PollQueueCommand.DeleteMessageAsync.
                if (exception.IsServerSideError())
                {
                    // The next execution should occur more quickly (try to update the visibility before it expires).
                    delay = _speedupStrategy.GetNextDelay(executionSucceeded: false);
                }
                if (exception.IsBadRequestPopReceiptMismatch())
                {
                    // There's no point to executing again. Once the pop receipt doesn't match, we've permanently lost
                    // ownership.
                    delay = Timeout.InfiniteTimeSpan;
                }
                else if (exception.IsNotFoundMessageOrQueueNotFound() ||
                    exception.IsConflictQueueBeingDeletedOrDisabled())
                {
                    // There's no point to executing again. Once the message or queue is deleted, we've permanently lost
                    // ownership.
                    // For queue disabled, in theory it's possible the queue could be re-enabled, but the scenarios here
                    // are currently unclear.
                    delay = Timeout.InfiniteTimeSpan;
                }
                else
                {
                    throw;
                }
            }

            return new TaskSeriesCommandResult(wait: Task.Delay(delay));
        }
    }
}

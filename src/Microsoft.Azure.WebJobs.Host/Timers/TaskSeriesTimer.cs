﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Host.Timers
{
    /// <summary>Represents a timer that executes one task after another in a series.</summary>
    internal sealed class TaskSeriesTimer : ITaskSeriesTimer
    {
        private readonly ITaskSeriesCommand _command;
        private readonly IBackgroundExceptionDispatcher _backgroundExceptionDispatcher;
        private readonly Task _initialWait;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private bool _started;
        private bool _stopped;
        private Task _run;
        private bool _disposed;

        public TaskSeriesTimer(ITaskSeriesCommand command, IBackgroundExceptionDispatcher backgroundExceptionDispatcher,
            Task initialWait)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            if (backgroundExceptionDispatcher == null)
            {
                throw new ArgumentNullException("backgroundExceptionDispatcher");
            }

            if (initialWait == null)
            {
                throw new ArgumentNullException("initialWait");
            }

            _command = command;
            _backgroundExceptionDispatcher = backgroundExceptionDispatcher;
            _initialWait = initialWait;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            ThrowIfDisposed();

            if (_started)
            {
                throw new InvalidOperationException("The timer has already been started; it cannot be restarted.");
            }

            _run = RunAsync(_cancellationTokenSource.Token);
            _started = true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (!_started)
            {
                throw new InvalidOperationException("The timer has not yet been started.");
            }

            if (_stopped)
            {
                throw new InvalidOperationException("The timer has already been stopped.");
            }

            _cancellationTokenSource.Cancel();
            return StopAsyncCore(cancellationToken);
        }

        private async Task StopAsyncCore(CancellationToken cancellationToken)
        {
            await Task.Delay(0);
            TaskCompletionSource<object> cancellationTaskSource = new TaskCompletionSource<object>();

            using (cancellationToken.Register(() => cancellationTaskSource.SetCanceled()))
            {
                // Wait for all pending command tasks to complete (or cancellation of the token) before returning.
                await Task.WhenAny(_run, cancellationTaskSource.Task);
            }

            _stopped = true;
        }

        public void Cancel()
        {
            ThrowIfDisposed();
            _cancellationTokenSource.Cancel();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                // Running callers might still be using the cancellation token.
                // Mark it canceled but don't dispose of the source while the callers are running.
                // Otherwise, callers would receive ObjectDisposedException when calling token.Register.
                // For now, rely on finalization to clean up _cancellationTokenSource's wait handle (if allocated).
                _cancellationTokenSource.Cancel();

                _disposed = true;
            }
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Allow Start to return immediately without waiting for any initial iteration work to start.
                await Task.Yield();

                Task wait = _initialWait;

                // Execute tasks one at a time (in a series) until stopped.
                while (!cancellationToken.IsCancellationRequested)
                {
                    TaskCompletionSource<object> cancellationTaskSource = new TaskCompletionSource<object>();

                    using (cancellationToken.Register(() => cancellationTaskSource.SetCanceled()))
                    {
                        try
                        {
                            await Task.WhenAny(wait, cancellationTaskSource.Task);
                        }
                        catch (OperationCanceledException)
                        {
                            // When Stop fires, don't make it wait for wait before it can return.
                        }
                    }

                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    try
                    {
                        TaskSeriesCommandResult result = await _command.ExecuteAsync(cancellationToken);
                        wait = result.Wait;
                    }
                    catch (OperationCanceledException)
                    {
                        // Don't fail the task, throw a background exception, or stop looping when a task cancels.
                    }
                }
            }
            catch (Exception exception)
            {
                // Immediately report any unhandled exception from this background task.
                // (Don't capture the exception as a fault of this Task; that would delay any exception reporting until
                // Stop is called, which might never happen.)
                _backgroundExceptionDispatcher.Throw(ExceptionDispatchInfo.Capture(exception));
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(null);
            }
        }
    }
}

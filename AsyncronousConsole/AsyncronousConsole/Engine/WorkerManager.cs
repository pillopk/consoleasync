using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AsyncronousConsole.Engine
{
    public enum WorkerStateEnum
    {
        Stopped, Running, Suspended
    }

    internal class WorkerManager : IConsoleWorker
    {
        private Task acutalTask;
        private readonly Stopwatch watch;

        public ConsoleWriter console;
        public ConsoleWorker worker;

        public Guid UniqueID { get; internal set; }
        public WorkerStateEnum State { get; internal set; }

        public WorkerManager(ConsoleWriter consoleWriter, ConsoleWorker newWorker)
        {
            UniqueID = Guid.NewGuid();
            State = WorkerStateEnum.Stopped;
            console = consoleWriter;
            worker = newWorker;
            watch = new Stopwatch();
        }

        public void Start()
        {
            if (State == WorkerStateEnum.Running)
                throw new InvalidOperationException("Worker already started");

            State = WorkerStateEnum.Running;
            worker.OnStart(console);
            ExecuteCicle();
        }

        public void Stop()
        {
            if (State == WorkerStateEnum.Stopped)
                throw new InvalidOperationException("Worker already stopped");

            StopAsync().Wait();
        }

        public void Execute()
        {
            if (State != WorkerStateEnum.Running)
                throw new InvalidOperationException("Attempted to force execute a non running worker");

            ExecuteAsync();
        }

        public void Suspend()
        {
            if (State != WorkerStateEnum.Running)
                throw new InvalidOperationException("Attempted to suspend a non running worker");

            State = WorkerStateEnum.Suspended;
        }

        public void Resume()
        {
            if (State != WorkerStateEnum.Suspended)
                throw new InvalidOperationException("Attempted to resume a non suspended worker");

            State = WorkerStateEnum.Running;
        }

        private async void ExecuteCicle()
        {
            if (State != WorkerStateEnum.Running)
                throw new InvalidOperationException("Attempted to execute a non running worker");

            watch.Start();

            if (worker.IntervalBetweenExecution.HasValue)
            {
                while (State != WorkerStateEnum.Stopped) { await ExecuteAsync(); }
            }
        }

        private async Task ExecuteAsync()
        {
            long elapsed = watch.ElapsedMilliseconds;
            watch.Restart();

            if (State == WorkerStateEnum.Running)
            {
                acutalTask = Task.Run(() => worker.OnExecute(console, TimeSpan.FromMilliseconds(elapsed)));
                await acutalTask;
            }

            // ReSharper disable once PossibleInvalidOperationException
            if (worker.IntervalBetweenExecution.Value.TotalMilliseconds < 10)
                throw new ArgumentException("Interval between execution must be greater than 10 milliseconds");

            await Task.Delay(worker.IntervalBetweenExecution.Value);
        }

        public async Task StopAsync()
        {
            if (State == WorkerStateEnum.Stopped) return;

            State = WorkerStateEnum.Stopped;
            if (acutalTask == null) return;
            if (acutalTask.Status.HasFlag(TaskStatus.Running) || acutalTask.Status.HasFlag(TaskStatus.RanToCompletion))
                await acutalTask;

            worker.OnStop(console, TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds));
            watch.Stop();
        }

    }


}

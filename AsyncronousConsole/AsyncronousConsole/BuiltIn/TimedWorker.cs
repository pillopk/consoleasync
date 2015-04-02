using System;

namespace AsyncronousConsole.BuiltIn
{
    public class TimedWorker : ConsoleWorker
    {
        private readonly Action<IConsoleWriter> timedStart;
        private readonly Action<IConsoleWriter, TimeSpan> timedExecute;
        private readonly Action<IConsoleWriter> timedStop;

        private readonly TimeSpan? executionInterval = TimeSpan.FromSeconds(1);

        public override TimeSpan? IntervalBetweenExecution
        {
            get { return executionInterval; }
        }

        #region Constructor

        public TimedWorker(Action<IConsoleWriter, TimeSpan> onExecute)
        {
            executionInterval = null;
            timedExecute = onExecute;
        }

        public TimedWorker(Action<IConsoleWriter, TimeSpan> onExecute, Action<IConsoleWriter> onStart)
        {
            executionInterval = null;
            timedExecute = onExecute;
            timedStart = onStart;
        }

        public TimedWorker(Action<IConsoleWriter, TimeSpan> onExecute, Action<IConsoleWriter> onStart, Action<IConsoleWriter> onStop)
        {
            executionInterval = null;
            timedExecute = onExecute;
            timedStart = onStart;
            timedStop = onStop;
        }

        public TimedWorker(TimeSpan interval, Action<IConsoleWriter, TimeSpan> onExecute)
        {
            executionInterval = interval;
            timedExecute = onExecute;
        }

        public TimedWorker(TimeSpan interval, Action<IConsoleWriter, TimeSpan> onExecute, Action<IConsoleWriter> onStart)
        {
            executionInterval = interval;
            timedExecute = onExecute;
            timedStart = onStart;
        }

        public TimedWorker(TimeSpan interval, Action<IConsoleWriter, TimeSpan> onExecute, Action<IConsoleWriter> onStart, Action<IConsoleWriter> onStop)
        {
            executionInterval = interval;
            timedExecute = onExecute;
            timedStart = onStart;
            timedStop = onStop;
        }

        #endregion

        public override void OnStart(IConsoleWriter writer)
        {
            if (timedStart != null) timedStart(writer);
        }

        public override void OnExecute(IConsoleWriter writer, TimeSpan elapsed)
        {
            if (timedExecute != null) timedExecute(writer, elapsed);
        }

        public override void OnStop(IConsoleWriter writer, TimeSpan elapsed)
        {
            if (timedStop != null) timedStop(writer);
        }
    }

}

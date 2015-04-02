using System;

namespace AsyncronousConsole
{
    public abstract class ConsoleWorker
    {
        /// <summary>
        /// Indicate a interval between calling OnExecute method
        /// </summary>
        public abstract TimeSpan? IntervalBetweenExecution { get; }

        /// <summary>
        /// Called at start of worker cicle
        /// </summary>
        /// <param name="writer">Writer of the console associated at the worker</param>
        public abstract void OnStart(IConsoleWriter writer);

        /// <summary>
        /// Called repeatedly enery worker cicle
        /// </summary>
        /// <param name="writer">Writer of the console associated at the worker</param>
        /// <param name="elapsed">Interval from last callof OnExecute</param>
        public abstract void OnExecute(IConsoleWriter writer, TimeSpan elapsed);

        /// <summary>
        /// Called at end of worker cicle
        /// </summary>
        /// <param name="writer">Writer of the console associated at the worker</param>
        /// <param name="elapsed">Interval from last callof OnExecute</param>
        public abstract void OnStop(IConsoleWriter writer, TimeSpan elapsed);
    }
}

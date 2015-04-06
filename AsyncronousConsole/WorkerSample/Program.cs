using System;
using System.Collections.Generic;
using AsyncronousConsole;
using AsyncronousConsole.BuiltIn;
using AsyncronousConsole.Support;

namespace WorkerSample
{
    class Program
    {
        // Create worker collection
        private static List<TestWorker> workers;

        static void Main(string[] args)
        {
            // Initialize worker collection
            workers = new List<TestWorker>();

            // create control console
            CreateControlConsole();

            // Create variuos worker console
            CreateWorkerConsole("First Worker", TimeSpan.FromMilliseconds(550), .5);
            CreateWorkerConsole("Second Worker", TimeSpan.FromMilliseconds(800), .3);
            CreateWorkerConsole("Third Worker", TimeSpan.FromMilliseconds(1100), .7);

            // Wait for user input, the true parameter starts every worker in every console
            ConsoleAsync.Run(true);
        }

        static void CreateControlConsole()
        {
            // Define an interval
            TimeSpan interval = TimeSpan.FromMilliseconds(500);

            // Create the console
            IConsole control = ConsoleAsync.CreateConsole("Control Console");

            // Add quit command
            control.AddCommand("quit", (writer, list) => ConsoleAsync.Quit());

            // Create worker from builtin type TimedWorker
            ConsoleWorker controlWorker = new TimedWorker(interval, (writer, span) =>
            {
                // Clear console and write title and info
                writer.Clear();
                writer.Info("CONTROL CONSOLE").NewLine();
                writer.Muted("Elapsed: {0}", span).NewLine().NewLine();

                // Cicle workers
                foreach (TestWorker worker in workers)
                {
                    // Calculate percentage of success over 60 characters
                    int success = (60 * worker.Successes + 1) / (worker.Successes + worker.Failures + 1);
                    int failure = 60 - success;

                    // Write worker name fitted on 16 char (to make bars aligned)
                    writer.Text(worker.Name.Fit(16));

                    // Write success bar
                    writer.Success("".Fit(success, '\u25A0'));

                    // Write error bar
                    writer.Error("".Fit(failure, '\u25A0'));

                    writer.NewLine();
                }

                // Write a message and commands help
                writer.NewLine().NewLine().NewLine().Info("Available commands").NewLine();
                writer.Text(@"
suspend     : suspend worker (not in control console)
resume      : resume worker (not in control console)
quit        : close application (in any console)
");
            });

            // Add worker to control console
            control.AddWorker(controlWorker);
        }

        static void CreateWorkerConsole(string name, TimeSpan interval, double margin)
        {
            // Create console
            IConsole console = ConsoleAsync.CreateConsole(name);

            // Create new instance of worker
            TestWorker worker = new TestWorker(interval, console.Name, margin);

            // Add worker to collection
            workers.Add(worker);

            // Add worker to newly created console
            IConsoleWorker workerInterface = console.AddWorker(worker);

            // Add command suspend
            console.AddCommand("suspend", (writer, list) =>
            {
                // Suspend worker and write a message
                workerInterface.Suspend();
                writer.Info("Worker suspended").NewLine();
            });

            // Add command resume
            console.AddCommand("resume", (writer, list) =>
            {
                // Resume worker and write a message
                workerInterface.Resume();
                writer.Info("Worker resumed").NewLine();
            });

            // Add command quit
            console.AddCommand("quit", (writer, list) => ConsoleAsync.Quit());
        }
    }

    // Custom Worker
    public class TestWorker : ConsoleWorker
    {
        // Initialize internal field
        private TimeSpan? interval;
        private Random rnd;
        private readonly double successMargin;

        // Initialize public properties
        public string Name { get; private set; }
        public int Successes { get; private set; }
        public int Failures { get; private set; }

        // Initialize value from constructor parameters
        public TestWorker(TimeSpan? intervalInMillisecond, string workerName, double margin)
        {
            Successes = 0;
            Failures = 0;
            interval = intervalInMillisecond;
            Name = workerName;
            successMargin = margin;
        }

        // Set the interval between executiob
        public override TimeSpan? IntervalBetweenExecution
        {
            get { return interval; }
        }

        // OnStart will be called before first execution
        public override void OnStart(IConsoleWriter writer)
        {
            // Create random object
            rnd = new Random();
        }

        // OnExecute will be called every interval specified in IntervalBetweenExecution property
        public override void OnExecute(IConsoleWriter writer, TimeSpan elapsed)
        {
            // Generate random value
            double value = rnd.NextDouble();

            // Check value to succes margin
            if (value > successMargin)
            {
                // If error write a message and increment fail count
                writer.Error("Error from: {0} with value {1}", Name, value);
                Failures++;
            }
            else
            {
                // If success write a message and increment success count
                writer.Success("Success from: {0} with value {1}", Name, value);
                Successes++;
            }
            writer.NewLine();
        }

        // OnStop will be called once after last execution (after IConsoleWorker.Stop)
        public override void OnStop(IConsoleWriter writer, TimeSpan elapsed)
        {
            Name = null;
            interval = null;
        }
    }

}

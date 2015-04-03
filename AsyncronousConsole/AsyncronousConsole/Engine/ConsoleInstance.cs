using System;
using System.Collections.Generic;
using System.Linq;
using AsyncronousConsole.Support;

namespace AsyncronousConsole.Engine
{
    internal class ConsoleInstance : IConsole
    {
        private Func<IConsoleWriter, ConsoleKeyInfo, bool> keyFilter;

        internal ConsoleWriter Writer { get; private set; }
        internal List<WorkerManager> Workers { get; private set; }
        internal List<ConsoleCommand> Commands { get; private set; }
        internal KeyCommand CommandKeys { get; private set; }

        public string Name { get; private set; }

        public IConsoleWriter GetWriter()
        {
            return Writer;
        }

        public ConsoleInstance(string consoleName)
        {
            Name = consoleName;

            Workers = new List<WorkerManager>();
            Commands = new List<ConsoleCommand>();
            CommandKeys = new KeyCommand();
            Writer = new ConsoleWriter(Name);
        }

        public void AddCommand(string commandName, Action<IConsoleWriter, string[]> action)
        {
            ConsoleCommand exist =
                Commands.FirstOrDefault(
                    c => String.Equals(c.Text, commandName, StringComparison.InvariantCultureIgnoreCase));

            if (exist != null)
                throw new Exception(string.Format("Command with name '{0}' already exist", commandName));

            Commands.Add(new ConsoleCommand(Writer, commandName, action));
        }

        public void RemoveCommand(string commandName)
        {
            ConsoleCommand toRemove =
                Commands.FirstOrDefault(
                    c => String.Equals(c.Text, commandName, StringComparison.InvariantCultureIgnoreCase));

            if (toRemove != null)
                Commands.Remove(toRemove);
        }

        public IConsoleWorker AddWorker(ConsoleWorker newWorker)
        {
            WorkerManager manager = new WorkerManager(Writer, newWorker);
            Workers.Add(manager);
            return manager;
        }

        public void RemoveWorker(IConsoleWorker existingWorker)
        {
            WorkerManager worker = Workers.FirstOrDefault(w => w.UniqueID.Equals(existingWorker.UniqueID));

            if (worker == null)
                throw new InvalidOperationException("Worker is not in console");

            Workers.Remove(worker);
            worker.StopAsync().Wait();
        }

        public void AddKeyCommand(KeyCommandEnum key, string command)
        {
            CommandKeys.AddKeyCommand(key, command, true);
        }

        public void AddKeyCommand(KeyCommandEnum key, string command, bool autoSend)
        {
            CommandKeys.AddKeyCommand(key, command, autoSend);
        }

        public void ClearKeyCommand(KeyCommandEnum key)
        {
            CommandKeys.ClearKeyCommand(key);
        }

        public void Execute(Action<IConsoleWriter> action)
        {
            ConsoleAsync.Manager.Execute(this, action);
        }

        public void Show()
        {
            ConsoleAsync.ShowConsole(Name);
        }

        public void Destroy()
        {
            ConsoleAsync.Manager.DestroyConsole(this);
        }


        public void AddKeyFilter(Func<IConsoleWriter, ConsoleKeyInfo, bool> filter)
        {
            keyFilter = filter;
        }

        internal bool ApplyKeyFilter(ConsoleKeyInfo key)
        {
            return keyFilter != null && keyFilter(Writer, key);
        }
    }

}

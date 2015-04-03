using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsyncronousConsole.Support;

namespace AsyncronousConsole.Engine
{
    internal class ConsoleManager
    {
        private readonly GlobalInput input;

        private bool isRunning = true;
        private bool initializing = true;
        private string actualConsoleName = null;
        private Action<string, bool> commandReceivedAction;
        private readonly StringBuilder consoleStandardOutput;


        internal string ConsoleToStandardOutput { get; set; }
        internal GlobalRenderer Renderer { get; private set; }
        internal List<ConsoleInstance> Consoles { get; private set; }

        internal ConsoleInstance GetConsole(string consoleName, bool withException = true)
        {
            ConsoleInstance console = Consoles.FirstOrDefault(c => c.Name == consoleName);
            if ((console == null) && withException) throw new InvalidOperationException(string.Format("Console '{0}' not found!", consoleName));
            return console;
        }

        internal ConsoleInstance ActiveConsole
        {
            get { return GetConsole(actualConsoleName); }
        }

        public ConsoleManager()
        {
            ConsoleToStandardOutput = null;
            Consoles = new List<ConsoleInstance>();
            Renderer = new GlobalRenderer(ConsoleAsync.ConsoleWidth, ConsoleAsync.ConsoleHeight);

            input = new GlobalInput(Renderer);
            input.CommandReceived += InputCommandReceived;
            input.CicleConsole += InputCicleConsole;
            input.EscapePressed += InputEscapePressed;

            consoleStandardOutput = new StringBuilder();
            Console.SetOut(new StringWriter(consoleStandardOutput));
        }

        private void InputEscapePressed(object sender, EventArgs e)
        {
            Renderer.RestoreConsoleSize();
        }

        public ConsoleInstance CreateConsole(string consoleName)
        {
            if (string.IsNullOrEmpty(consoleName))
                throw new Exception("Console name is null");

            if (IsConsoleNameValid(consoleName) == false)
                throw new Exception("Console name contains invalid chars");

            ConsoleInstance console = new ConsoleInstance(consoleName);
            Consoles.Add(console);

            if (initializing)
            {
                actualConsoleName = consoleName;
                UpdateTitle();
            }

            initializing = false;
            return console;
        }

        public void DestroyConsole(string consoleName)
        {
            ConsoleInstance console = GetConsole(consoleName, false);
            if (console != null) DestroyConsole(console);
        }

        internal void DestroyConsole(ConsoleInstance console)
        {
            if (Consoles.Count < 2)
                throw new InvalidOperationException("Cannot destroy last console. Use quit command instead");

            if (ActiveConsole.Name == console.Name)
                CicleConsole(-1);

            Consoles.Remove(console);

            if (console.Workers.Count > 0)
                Task.WaitAll(console.Workers.Select(w => w.StopAsync()).ToArray());

            // ReSharper disable once RedundantAssignment
            console = null;
        }

        private bool IsConsoleNameValid(string consoleName)
        {
            bool result = true;
            foreach (char ch in consoleName.ToLowerInvariant())
            {
                if (ConsoleAsync.AvailableInputChars.Contains(ch) == false)
                    result = false;
            }
            return result;
        }

        private void InputCommandReceived(object sender, InputEventArgs e)
        {
            bool managed = ExecuteCommand(ActiveConsole, e.Command);

            if (commandReceivedAction != null)
                commandReceivedAction(e.Command, managed);
        }

        public bool ExecuteCommand(ConsoleInstance console, string commandText)
        {
            foreach (ConsoleCommand command in console.Commands)
            {
                if (command.TryExecute(commandText))
                    return true;
            }
            return false;
        }

        private void InputCicleConsole(object sender, CicleConsoleEventArgs e)
        {
            CicleConsole(e.Direction);
        }

        private void CicleConsole(int direction)
        {
            List<string> names = Consoles.Select(c => c.Name).ToList();
            int index = names.IndexOf(actualConsoleName) + direction;

            if (index == names.Count) index = 0;
            if (index == -1) index = names.Count - 1;

            actualConsoleName = names[index];
            Renderer.ForceRender();
            UpdateTitle();
        }

        private void UpdateTitle()
        {
            Console.Title = string.Concat(actualConsoleName, " - ", ConsoleAsync.SYSTEM_CONSOLE_TITLE);
        }

        public void SetReceivedAction(Action<string, bool> action)
        {
            commandReceivedAction = action;
        }

        public void ShowConsole(string consoleName)
        {
            ConsoleInstance console = GetConsole(consoleName);
            actualConsoleName = console.Name;
        }

        public void Run(bool startAllWorker = false)
        {
            consoleStandardOutput.Clear();

            Stopwatch watch = new Stopwatch();
            watch.Start();

            if (startAllWorker)
            {
                Consoles.ForEach(c => c.Workers.ForEach(w => w.Start()));
            }

            if (Consoles.Count==0)
                throw new InvalidOperationException("Cannot run ConsoleAsync without at least one console");
            
            while (isRunning)
            {
                input.Execute(ActiveConsole, watch.ElapsedMilliseconds);
                Renderer.Render(ActiveConsole);
                watch.Restart();
                ManageStandardOutput();
                Task.Delay(10).Wait();
            }
        }

        private void ManageStandardOutput()
        {
            string update = consoleStandardOutput.ToString();
            consoleStandardOutput.Clear();

            if (ConsoleToStandardOutput == null) return;

            ConsoleInstance console = GetConsole(ConsoleToStandardOutput, false);
            if (console == null) return;

            if (string.IsNullOrEmpty(update) == false)
                console.GetWriter().Text(update);
        }

        public void Execute(ConsoleInstance console, Action<IConsoleWriter> action)
        {
            action(console.Writer);
            if (console.Name == actualConsoleName) Renderer.Render(ActiveConsole);
        }

        public void AddCommandToAll(string commandName, Action<IConsoleWriter, List<string>> action)
        {
            foreach (ConsoleInstance console in Consoles)
            {
                ConsoleCommand exist = console.Commands.FirstOrDefault(c => c.Text == commandName);

                if (exist != null)
                    throw new InvalidOperationException(string.Format("Command '{0}' already exist in console '{1}'", commandName, console.Name));

                console.AddCommand(commandName, action);
            }
        }

        public void RemoveCommandFromAll(string commandName)
        {
            foreach (ConsoleInstance console in Consoles)
            {
                ConsoleCommand exist = console.Commands.FirstOrDefault(c => c.Text == commandName);
                if (exist != null) console.RemoveCommand(commandName);
            }
        }

        public void ExecuteCommandToAll(Action<IConsoleWriter> action)
        {
            foreach (ConsoleInstance console in Consoles)
            {
                console.Execute(action);
            }
        }

        public void Quit()
        {
            isRunning = false;

            List<WorkerManager> workers = Consoles.SelectMany(c => c.Workers).ToList();

            Task.WaitAll(
                workers.Select(w => w.StopAsync()).ToArray()
                );
        }
    }

}



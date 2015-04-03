using System;
using System.Collections.Generic;
using System.Linq;

namespace AsyncronousConsole.Engine
{
    internal class ConsoleCommand
    {
        public string Text { get; private set; }

        private readonly IConsoleWriter writer;
        private readonly Action<IConsoleWriter, List<string>> action;

        public ConsoleCommand(IConsoleWriter consoleWriter, string command, Action<IConsoleWriter, List<string>> function)
        {
            writer = consoleWriter;
            Text = command;
            action = function;
        }

        public bool TryExecute(string command)
        {
            string[] items = command.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (String.Equals(items[0], Text, StringComparison.InvariantCultureIgnoreCase) == false) return false;

            List<string> parameters = items.Skip(1).Take(items.Length - 1).ToList();
            action(writer, parameters);
            return true;
        }
    }
}

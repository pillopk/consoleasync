using System;
using System.Linq;

namespace AsyncronousConsole.Engine
{
    internal class ConsoleCommand
    {
        public string Text { get; private set; }

        private readonly IConsoleWriter writer;
        private readonly Action<IConsoleWriter, string[]> action;

        public ConsoleCommand(IConsoleWriter consoleWriter, string command, Action<IConsoleWriter, string[]> function)
        {
            writer = consoleWriter;
            Text = command;
            action = function;
        }

        public bool TryExecute(string command)
        {
            string[] items = command.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (String.Equals(items[0], Text, StringComparison.InvariantCultureIgnoreCase) == false) return false;

            string[] parameters = items.Skip(1).Take(items.Length - 1).ToArray();
            action(writer, parameters);
            return true;
        }
    }
}

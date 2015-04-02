using System;

namespace AsyncronousConsole.Support
{
    public enum KeyCommandEnum : byte
    {
        F1 = 0, F2 = 1, F3 = 2, F4 = 3, F5 = 4, F6 = 5, F7 = 6, F8 = 7, F9 = 8, F10 = 9
    }

    internal class KeyCommand
    {
        private readonly KeyCommandDefinition[] functions;

        public KeyCommand()
        {
            functions = new KeyCommandDefinition[10];
        }

        public void AddKeyCommand(KeyCommandEnum key, string command, bool autoEnter)
        {
            functions[(int)key] = new KeyCommandDefinition(command, autoEnter);
        }

        public void ClearKeyCommand(KeyCommandEnum key)
        {
            functions[(int)key] = null;
        }

        public KeyCommandDefinition GetCommand(ConsoleKey key)
        {
            int index = ((int)key) - 112;
            return functions[index];
        }
    }

    internal class KeyCommandDefinition
    {
        public string Command;
        public bool AutoEnter;

        public KeyCommandDefinition(string command, bool auto)
        {
            Command = command;
            AutoEnter = auto;
        }
    }
}

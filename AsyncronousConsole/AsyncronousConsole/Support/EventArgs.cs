using System;

namespace AsyncronousConsole.Support
{

    internal class WriterOutputEventArgs : EventArgs
    {
        public string Line { get; private set; }

        public WriterOutputEventArgs(string line)
        {
            Line = line;
        }
    }

    internal class InputEventArgs : EventArgs
    {
        public string Command { get; private set; }

        public InputEventArgs(string command)
        {
            Command = command;
        }
    }

    internal class CicleConsoleEventArgs : EventArgs
    {
        public int Direction { get; private set; }

        public CicleConsoleEventArgs(int direction)
        {
            Direction = direction;
        }
    }

}

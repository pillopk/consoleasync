using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AsyncronousConsole.Support;

namespace AsyncronousConsole.Engine
{
    internal class GlobalInput
    {
        private readonly GlobalRenderer renderer;
        private int cursorPos;
        private string text = string.Empty;

        private List<string> previousCommands;
        private int previousIndex;
        private string incompleteCommand;

        #region Function Key Array

        private ConsoleKey[] functionKeyArray = new[]
        {
            ConsoleKey.F1,
            ConsoleKey.F2,
            ConsoleKey.F3,
            ConsoleKey.F4,
            ConsoleKey.F5,
            ConsoleKey.F6,
            ConsoleKey.F7,
            ConsoleKey.F8,
            ConsoleKey.F9,
            ConsoleKey.F10,
            ConsoleKey.F11,
            ConsoleKey.F12
        };

        #endregion

        #region Events

        internal event EventHandler EscapePressed;
        internal event EventHandler<InputEventArgs> CommandReceived;
        internal event EventHandler<CicleConsoleEventArgs> CicleConsole;

        private void OnEscapePressed()
        {
            EventHandler handler = EscapePressed;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private void OnCommandReceived(string completedCommand)
        {
            EventHandler<InputEventArgs> handler = CommandReceived;
            if (handler != null) handler(this, new InputEventArgs(completedCommand));
        }

        private void OnCicleConsole(int direction)
        {
            EventHandler<CicleConsoleEventArgs> handler = CicleConsole;
            if (handler != null) handler(this, new CicleConsoleEventArgs(direction));
        }

        #endregion

        public GlobalInput(GlobalRenderer Renderer)
        {
            renderer = Renderer;
            previousCommands = new List<string>();
        }

        public void Execute(ConsoleInstance console, long elapsedMillisecond)
        {
            string issuedCommand = null;

            while (Console.KeyAvailable)
            {
                ConsoleKeyInfo input = Console.ReadKey(true);
                var ignore = console.ApplyKeyFilter(input);
                if (ignore) continue;

                if (input.Key == ConsoleKey.Enter) // Send Command
                {
                    if (string.IsNullOrWhiteSpace(text) == false)
                    {
                        issuedCommand = text;
                        AddPreviousCommand(text);
                        SetUncompletedCommand(string.Empty);
                        ResetCommandIndex();
                        text = string.Empty;
                        cursorPos = 0;
                    }
                }
                else if (functionKeyArray.Contains(input.Key)) // Function Keys
                {
                    KeyCommandDefinition fk = console.CommandKeys.GetCommand(input.Key);
                    if (fk != null)
                    {
                        if (fk.AutoEnter)
                        {
                            issuedCommand = fk.Command;
                        }
                        else
                        {
                            text = fk.Command;
                            cursorPos = text.Length;
                        }
                    }
                }
                else if (input.Key == ConsoleKey.Tab) // Cicle Console
                {
                    OnCicleConsole((input.Modifiers == ConsoleModifiers.Shift) ? -1 : 1);
                }
                else if (input.Key == ConsoleKey.Escape) // Escape Pressed
                {
                    OnEscapePressed();
                }
                else if (input.Key == ConsoleKey.Delete) // Delete Char
                {
                    if (text.Length > 0)
                    {
                        text = text.RemoveChar(cursorPos);
                    }
                }
                else if (input.Key == ConsoleKey.Backspace) // Delete Previous Char
                {
                    if ((text.Length > 0) && (cursorPos > 0))
                    {
                        cursorPos--;
                        text = text.RemoveChar(cursorPos);
                    }
                }
                else if (input.Key == ConsoleKey.LeftArrow) // Move cursor to the Left
                {
                    if (cursorPos > 0)
                    {
                        if (input.Modifiers == ConsoleModifiers.Control) FindWord(false);
                        else cursorPos--;
                    }
                }
                else if (input.Key == ConsoleKey.RightArrow) // Move cursor to the Right
                {
                    if (cursorPos < text.Length)
                    {
                        if (input.Modifiers == ConsoleModifiers.Control) FindWord(true);
                        else cursorPos++;
                    }
                }
                else if ((input.Key == ConsoleKey.PageUp) && (input.Modifiers.HasFlag(ConsoleModifiers.Control))) // Get Previous Command
                {
                    text = GetPreviousCommand();
                    cursorPos = text.Length;
                }
                else if ((input.Key == ConsoleKey.PageDown) && (input.Modifiers.HasFlag(ConsoleModifiers.Control))) // Get Next Command
                {
                    text = GetNextCommand();
                    cursorPos = text.Length;
                }
                else if ((input.Key == ConsoleKey.Home) && (!input.Modifiers.HasFlag(ConsoleModifiers.Control)))  // Move cursor at Start
                {
                    cursorPos = 0;
                }
                else if ((input.Key == ConsoleKey.End) && (!input.Modifiers.HasFlag(ConsoleModifiers.Control)))  // Move cursor at End
                {
                    cursorPos = text.Length;
                }
                else if (console.Writer.ApplyKey(input))
                {
                    // Exclude scroll keys
                }
                else if (ConsoleAsync.AvailableInputChars.Contains(input.KeyChar.ToString(CultureInfo.InvariantCulture).ToLowerInvariant()) == false)
                {
                    // Exclude non allowed chars
                }
                else
                {
                    if ((input.Key != ConsoleKey.Spacebar) || (cursorPos != 0))
                    {
                        if (cursorPos == text.Length)
                        {
                            text += input.KeyChar;
                            cursorPos = text.Length;
                        }
                        else
                        {
                            text = string.Concat(text.Left(cursorPos), input.KeyChar, text.LeftRest(cursorPos));
                            cursorPos++;
                        }

                        SetUncompletedCommand(text);
                        ResetCommandIndex();
                    }
                }

                renderer.InputCommand = text;
                renderer.InputCommandCursorPosition = cursorPos;
            }


            if (string.IsNullOrEmpty(issuedCommand) == false)
            {
                OnCommandReceived(issuedCommand);
                // ReSharper disable once RedundantAssignment
                issuedCommand = null;
            }
        }

        private void FindWord(bool forward)
        {
            char[] separators = new[] { ' ', ',', '.', '-' };

            List<int> positions = new List<int> { 0 };

            if (text.Length > 1)
            {
                for (int i = 0; i < text.Length - 1; i++)
                {
                    foreach (char sep in separators)
                    {
                        if ((text[i] == sep) && (text[i + 1] != sep)) { positions.Add(i + 1); }
                    }
                }

                if (positions.Contains(cursorPos) == false)
                {
                    positions.Add(cursorPos);
                }

                positions.Add(text.Length);
                positions.Sort();

                int actualIndex = positions.IndexOf(cursorPos);
                if (forward == false && actualIndex > 0) cursorPos = positions[positions.IndexOf(cursorPos) - 1];
                if (forward == false && actualIndex == 0) cursorPos = 0;
                if (forward && actualIndex < positions.Count - 1) cursorPos = positions[positions.IndexOf(cursorPos) + 1];
                if (forward && actualIndex == positions.Count - 1) cursorPos = positions[actualIndex];
                return;
            }

            cursorPos = 0;
        }

        #region Previous Commands

        private void ResetCommandIndex()
        {
            previousIndex = 0;
        }

        private string GetPreviousCommand()
        {
            if (previousCommands.Count == 0) return text;
            if (previousIndex == 0) incompleteCommand = text;
            if (previousIndex < previousCommands.Count) previousIndex++;
            return previousCommands[previousIndex - 1];
        }

        private string GetNextCommand()
        {
            if (previousCommands.Count == 0) return text;
            if (previousIndex > 0) previousIndex--;
            if (previousIndex == 0) return incompleteCommand;
            return previousCommands[previousIndex - 1];
        }

        private void AddPreviousCommand(string command)
        {
            previousCommands.Insert(0, command);
            if (previousCommands.Count > 20) previousCommands.RemoveAt(20);
        }

        private void SetUncompletedCommand(string uncomplete)
        {
            incompleteCommand = uncomplete;
        }

        #endregion
    }

}

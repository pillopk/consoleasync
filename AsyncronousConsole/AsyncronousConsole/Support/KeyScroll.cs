using System;
using System.Collections.Generic;
using System.Linq;

namespace AsyncronousConsole.Support
{
    internal enum KeyScrollEnum
    {
        RowUp, RowDown, PageUp, PageDown, Top, Bottom
    }

    internal class KeyScroll
    {
        private readonly List<KeyOperation> definitions;

        public KeyScroll()
        {
            definitions = new List<KeyOperation>();

            definitions.Clear();
            definitions.Add(new KeyOperation(ConsoleKey.UpArrow, KeyScrollEnum.RowUp));
            definitions.Add(new KeyOperation(ConsoleKey.DownArrow, KeyScrollEnum.RowDown));
            definitions.Add(new KeyOperation(ConsoleKey.PageUp, KeyScrollEnum.PageUp));
            definitions.Add(new KeyOperation(ConsoleKey.PageDown, KeyScrollEnum.PageDown));
            definitions.Add(new KeyOperation(ConsoleKey.Home, KeyScrollEnum.Top, true));
            definitions.Add(new KeyOperation(ConsoleKey.End, KeyScrollEnum.Bottom, true));
        }

        public KeyScrollEnum? TestKey(ConsoleKeyInfo keyInfo)
        {
            KeyOperation item =
                definitions.FirstOrDefault(
                    d => d.Key == keyInfo.Key && d.Control == keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control));

            return (item != null) ? item.Operation : new KeyScrollEnum?();
        }

        #region Support Class

        private class KeyOperation
        {
            public ConsoleKey Key { get; set; }
            public KeyScrollEnum Operation { get; set; }
            public bool Control { get; set; }

            public KeyOperation(ConsoleKey key, KeyScrollEnum operation, bool? control = null)
            {
                Key = key;
                Operation = operation;
                Control = control.HasValue && control.Value;
            }
        }

        #endregion
    }
}

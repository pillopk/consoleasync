using System;
using System.Collections.Generic;

namespace AsyncronousConsole.Support
{
    internal class RowCollection
    {
        private ConsoleColor lastColor;

        private readonly List<Row> rows;

        public RowCollection()
        {
            rows = new List<Row>();
            Add(string.Empty);
        }

        public Row this[int index]
        {
            get { return rows[index]; }
        }

        public Row Last
        {
            get { return rows[rows.Count - 1]; }
        }

        public int Count
        {
            get { return rows.Count; }
        }

        public void Add(string line)
        {
            rows.Add(new Row { Line = line });
        }

        public void SetColor(ConsoleColor color)
        {
            if ((color == lastColor) && Last.Colors.Count > 0) return;

            lastColor = color;
            Last.Colors.Add(new RowColorIndex
            {
                Color = color,
                Index = Last.Line.Length
            });
        }

        public void Clear()
        {
            rows.Clear();
            Add(string.Empty);
        }

    }

    internal class Row
    {
        public string Line { get; set; }
        public List<RowColorIndex> Colors { get; private set; }

        public Row()
        {
            Colors = new List<RowColorIndex>();
        }

        public ConsoleColor? GetColorAt(int charIndex)
        {
            ConsoleColor? result = null;
            foreach (RowColorIndex color in Colors)
            {
                if (color.Index <= charIndex) result = color.Color;
                else break;
            }
            return result;
        }
    }

    internal class RowColorIndex
    {
        public int Index { get; set; }
        public ConsoleColor Color { get; set; }
    }
}

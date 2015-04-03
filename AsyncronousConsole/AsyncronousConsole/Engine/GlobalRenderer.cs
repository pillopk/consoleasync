using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using AsyncronousConsole.Support;
using Microsoft.Win32.SafeHandles;

namespace AsyncronousConsole.Engine
{
    internal class GlobalRenderer
    {
        #region DllImport

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] uint fileAccess,
            [MarshalAs(UnmanagedType.U4)] uint fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] int flags,
            IntPtr template);


        [DllImport("kernel32.dll", EntryPoint = "WriteConsoleOutputW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool WriteConsoleOutput(
            SafeFileHandle hConsoleOutput,
            CharInfo[] lpBuffer,
            Coord dwBufferSize,
            Coord dwBufferCoord,
            ref ShortRect lpWriteRegion);

        [StructLayout(LayoutKind.Sequential)]
        public struct Coord
        {
            public short X;
            public short Y;
        };

        [StructLayout(LayoutKind.Explicit)]
        public struct CharInfo
        {
            [FieldOffset(0)]
            public ushort UnicodeChar;
            [FieldOffset(2)]
            public short Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ShortRect
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        #endregion

        private const string PREFIX = ">";

        private readonly SafeFileHandle fileHandle;
        private int lastWidth;
        private int lastHeight;
        private bool forceRender;
        private bool suspended;

        internal int ScreenWidth;
        internal int ScreenHeight;
        internal int ViewWidth;
        internal int ViewHeight;
        internal int ScrollColumnIndex;
        internal int InputRowIndex;
        internal int FooterRowIndex;

        // INPUT
        private string inputCommand = string.Empty;
        private int inputCommandOffset;
        private int inputCommandCursorPosition;
        private bool changeInput = true;

        // FOOTER
        private string footerContent;
        private bool changeFooter = true;

        #region Properties

        public string InputCommand
        {
            get { return inputCommand; }
            set
            {
                if (inputCommand != value) changeInput = true;
                inputCommand = value;
            }
        }

        public int InputCommandCursorPosition
        {
            get { return inputCommandCursorPosition; }
            set
            {
                if (inputCommandCursorPosition != value) changeInput = true;
                inputCommandCursorPosition = value;
            }
        }

        #endregion

        public GlobalRenderer(int width, int height)
        {
            inputCommandOffset = 0;

            Console.WindowWidth = width;
            Console.WindowHeight = height;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.CursorVisible = false;

            UpdateConsoleData();

            fileHandle = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
        }

        private void UpdateConsoleData()
        {
            if ((lastWidth != Console.WindowWidth) || (lastHeight != Console.WindowHeight))
            {
                ScreenWidth = Console.WindowWidth;
                ScreenHeight = Console.WindowHeight;
                ViewWidth = ScreenWidth - 1;
                ViewHeight = ScreenHeight - 2;
                ScrollColumnIndex = ScreenWidth - 1;
                InputRowIndex = ScreenHeight - 2;
                FooterRowIndex = ScreenHeight - 1;
                forceRender = true;
            }

            lastWidth = ScreenWidth;
            lastHeight = ScreenHeight;

            Console.SetCursorPosition(0, 0);
            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
        }

        public void RestoreConsoleSize()
        {
            Console.SetWindowSize(ScreenWidth, ScreenHeight);
        }

        public void ForceRender()
        {
            forceRender = true;
        }

        public void Render(ConsoleInstance console)
        {
            if (HasValidDimension() == false) return;
            UpdateConsoleData();

            UpdateFooter(console);

            ScreenBuffer buffer = new ScreenBuffer(ScreenWidth, ScreenHeight);

            if (forceRender || console.Writer.HasUpdate) RenderView(buffer, console);
            if (forceRender || changeInput) RenderInput(buffer);
            if (forceRender || changeFooter) RenderFooter(buffer);

            IEnumerable<ShortRect> rects = GetRects(
                forceRender | console.Writer.HasUpdate,
                forceRender | changeInput,
                forceRender | changeFooter);

            foreach (ShortRect rect in rects)
            {
                ShortRect refRect = rect;
                bool b = WriteConsoleOutput(fileHandle, buffer.Buffer,
                    new Coord() { X = (byte)ScreenWidth, Y = (byte)ScreenHeight },
                    new Coord() { X = rect.Left, Y = rect.Top },
                    ref refRect);
            }

            console.Writer.ResetUpdate();
            changeFooter = false;
            changeInput = false;
            forceRender = false;
        }

        private void RenderView(ScreenBuffer buffer, ConsoleInstance console)
        {
            RowCollection rows = console.Writer.Lines;
            int scrollRowOffset = console.Writer.OffsetRowIndex;
            int totalRows = (rows.Count < ViewHeight) ? rows.Count : ViewHeight;

            for (int rowIndex = 0; rowIndex < totalRows; rowIndex++)
            {
                Row row = (rows.Count < ViewHeight)
                    ? rows[rowIndex]
                    : rows[rows.Count - ViewHeight + rowIndex + scrollRowOffset];

                buffer.WriteAt(rowIndex, row);
            }

            buffer.SetColumnColors(ScrollColumnIndex, 0, ViewHeight, ConsoleColor.DarkGray, ConsoleColor.Black);
            buffer.WriteAt(ViewHeight - 1, ScrollColumnIndex, "\u25BC",
                (scrollRowOffset < 0) ? ConsoleColor.White : ConsoleColor.DarkGray);
            buffer.WriteAt(0, ScrollColumnIndex, "\u25B2",
                (rows.Count > ViewHeight - scrollRowOffset) ? ConsoleColor.White : ConsoleColor.DarkGray);

            int barSpace = ViewHeight - 2;
            int barStart = 0;
            int barEnd = barSpace;

            if (rows.Count > ViewHeight)
            {
                int barHeight = (barSpace * ViewHeight) / rows.Count;
                if (barHeight < 1) barHeight = 1;

                int positiveOffset = rows.Count + scrollRowOffset;
                barEnd = (barSpace * positiveOffset) / rows.Count;

                barStart = barEnd - barHeight;
            }

            for (int i = barStart; i < barEnd; i++)
            {
                buffer.WriteAt(i + 1, ScrollColumnIndex, "\u2588", ConsoleColor.DarkGray);
            }
        }

        private void RenderInput(ScreenBuffer buffer)
        {
            int inputWidth = ViewWidth - 1;

            if (inputCommandCursorPosition > inputWidth + inputCommandOffset) inputCommandOffset = inputCommandCursorPosition - inputWidth;
            if (inputCommandCursorPosition < inputCommandOffset) inputCommandOffset = inputCommandCursorPosition;

            string line = string.Format("{0}{1}", PREFIX, InputCommand.LeftRest(inputCommandOffset).Fit(inputWidth));
            buffer.SetRowColors(InputRowIndex, 0, ScreenWidth, ConsoleColor.Black, ConsoleColor.Gray);
            buffer.WriteAt(InputRowIndex, line);
            buffer.SetCharColors(inputCommandCursorPosition + PREFIX.Length - inputCommandOffset, InputRowIndex, null, ConsoleColor.DarkGray);
        }

        private void RenderFooter(ScreenBuffer buffer)
        {
            buffer.SetRowColors(FooterRowIndex, 0, ScreenWidth, ConsoleColor.Gray, ConsoleColor.Black);
            buffer.WriteAt(FooterRowIndex, footerContent);
        }

        public void UpdateFooter(ConsoleInstance console)
        {
            string otherUpdate = string.Empty;

            foreach (ConsoleInstance instance in ConsoleAsync.Manager.Consoles)
            {
                if ((instance.Name != console.Name) && instance.Writer.HasUpdate)
                {
                    otherUpdate = "?";
                    break;
                }
            }

            List<string> consoleNames = ConsoleAsync.Manager.Consoles.Select(c => c.Name).ToList();
            string counters = string.Format(" {0}/{1}", consoleNames.IndexOf(console.Name) + 1, consoleNames.Count).PadRight(7);

            string footerData = string.Concat(
                (Console.CapsLock ? "-CAPS" : string.Empty).PadRight(6),
                (Console.NumberLock ? "-NUM" : string.Empty).PadRight(6),
                otherUpdate.PadRight(3)
                );

            int spaceForName = ScreenWidth - counters.Length - footerData.Length;
            string name = console.Name.Truncate(spaceForName).PadRight(spaceForName);

            string footerTemp = string.Concat(counters, name, footerData);
            changeFooter = footerTemp != footerContent;
            footerContent = footerTemp;
        }

        private IEnumerable<ShortRect> GetRects(bool view, bool input, bool footer)
        {
            List<ShortRect> rects = new List<ShortRect>();

            if (view & input & footer)
            {
                rects.Add(CreateRect(0, 0, ScreenWidth, ScreenHeight));
            }
            else if (view & input)
            {
                rects.Add(CreateRect(0, 0, ScreenWidth, ScreenHeight - 2));
            }
            else if (input & footer)
            {
                rects.Add(CreateRect(0, ScreenHeight - 2, ScreenWidth, ScreenHeight));
            }
            else
            {
                if (input)
                {
                    rects.Add(CreateRect(0, ScreenHeight - 2, ScreenWidth, ScreenHeight - 2));
                }

                if (footer)
                {
                    rects.Add(CreateRect(0, ScreenHeight - 1, ScreenWidth, ScreenHeight - 1));
                }

                if (view)
                {
                    rects.Add(CreateRect(0, 0, ScreenWidth, ScreenHeight - 3));
                }
            }

            return rects;
        }

        private ShortRect CreateRect(int left, int top, int width, int height)
        {
            return new ShortRect()
            {
                Left = (byte)left,
                Top = (byte)top,
                Right = (byte)width,
                Bottom = (byte)height
            };
        }

        public bool HasValidDimension()
        {
            //bool supported = (Console.WindowWidth == ScreenWidth) && (Console.WindowHeight == ScreenHeight);
            bool supported = (Console.WindowWidth > 59) && (Console.WindowHeight > 29);

            if (!supported)
            {
                suspended = true;
                ScreenBuffer buffer = new ScreenBuffer(Console.WindowWidth, Console.WindowHeight);
                buffer.SetRowColors(0, 0, Console.WindowWidth, ConsoleColor.Cyan, ConsoleColor.Black);
                buffer.SetRowColors(1, 0, Console.WindowWidth, ConsoleColor.Gray, ConsoleColor.Black);
                buffer.SetRowColors(2, 0, Console.WindowWidth, ConsoleColor.Gray, ConsoleColor.Black);
                buffer.SetRowColors(3, 0, Console.WindowWidth, ConsoleColor.DarkGray, ConsoleColor.Black);
                buffer.WriteAt(0, "ConsoleAsync");
                buffer.WriteAt(1, "Support minumum 60x30 console window");
                buffer.WriteAt(3, "Press escape key to resize window");

                ShortRect rect = CreateRect(0, 0, Console.WindowWidth, Console.WindowHeight);

                bool b = WriteConsoleOutput(fileHandle, buffer.Buffer,
                    new Coord() { X = (byte)Console.WindowWidth, Y = (byte)Console.WindowHeight },
                    new Coord() { X = 0, Y = 0 },
                    ref rect);
            }

            if (suspended && supported)
            {
                forceRender = true;
                suspended = false;
            }

            return supported;
        }


        #region Support Class

        private class ScreenBuffer
        {
            private readonly int width;
            private readonly int height;
            private readonly CharInfo[] buffer;

            public CharInfo[] Buffer
            {
                get { return buffer; }
            }

            public ScreenBuffer(int w, int h)
            {
                width = w;
                height = h;
                buffer = CreateDefaultBuffer();
            }

            public void WriteAt(int rowNumber, string content, ConsoleColor? fore = null, ConsoleColor? back = null)
            {
                WriteAt(rowNumber, 0, content, fore, back);
            }

            public void WriteAt(int rowNumber, int charNumber, string content, ConsoleColor? fore = null, ConsoleColor? back = null)
            {
                string cont = content.Left(width);

                for (int charIndex = 0; charIndex < cont.Length; charIndex++)
                {
                    int rowIndex = rowNumber * width;
                    ApplyChar(rowIndex + charNumber + charIndex, cont[charIndex]);
                    ApplyColors(rowIndex + charNumber + charIndex, fore, back);
                }
            }

            public void WriteAt(int rowNumber, Row row)
            {
                string cont = row.Line.Left(width);

                for (int charIndex = 0; charIndex < cont.Length; charIndex++)
                {
                    int rowIndex = rowNumber * width;
                    ApplyChar(rowIndex + charIndex, cont[charIndex]);

                    ConsoleColor? color = row.GetColorAt(charIndex);
                    if (color.HasValue)
                    {
                        ApplyColors(rowIndex + charIndex, color.Value);
                    }
                }
            }

            public void SetCharColors(int charNumber, int rowNumber, ConsoleColor? fore = null, ConsoleColor? back = null)
            {
                int index = (rowNumber * width) + charNumber;
                ApplyColors(index, fore, back);
            }

            public void SetRowColors(int rowNumber, int start, int end, ConsoleColor fore, ConsoleColor back)
            {
                for (int charIndex = start; charIndex < end; charIndex++)
                {
                    int rowIndex = rowNumber * width;
                    buffer[rowIndex + charIndex].Attributes = (short)(((short)back * 16) + fore);
                }
            }

            public void SetColumnColors(int colNumber, int start, int end, ConsoleColor fore, ConsoleColor back)
            {
                for (int rowIndex = start; rowIndex < end; rowIndex++)
                {
                    buffer[(rowIndex * width) + colNumber].Attributes = (short)(((short)back * 16) + fore);
                }
            }

            private void ApplyChar(int index, char chr)
            {
                buffer[index].UnicodeChar = chr;
            }

            private void ApplyColors(int index, ConsoleColor? fore = null, ConsoleColor? back = null)
            {
                if (fore.HasValue && back.HasValue)
                {
                    buffer[index].Attributes = (short)(((short)back.Value * 16) + fore.Value);
                }
                else
                {
                    int actual = buffer[index].Attributes;

                    if (back.HasValue)
                    {
                        actual = (actual & 15) + (byte)(back.Value) * 16;
                        buffer[index].Attributes = (short)actual;
                    }

                    if (fore.HasValue)
                    {
                        actual = (actual & 240) + (byte)fore.Value;
                        buffer[index].Attributes = (short)actual;
                    }
                }
            }

            private CharInfo[] CreateDefaultBuffer()
            {
                CharInfo[] temp = new CharInfo[width * height];

                for (int i = 0; i < temp.Length; ++i)
                {
                    temp[i].Attributes = ((byte)ConsoleColor.Black * 16) + (byte)ConsoleColor.White;
                    temp[i].UnicodeChar = (byte)ConsoleKey.Spacebar;
                }

                return temp;
            }
        }

        #endregion

    }
}

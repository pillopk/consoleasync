using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AsyncronousConsole.Support;

namespace AsyncronousConsole.Engine
{
    internal class ConsoleWriter : IConsoleWriter
    {
        private bool hasUpdate;

        private ConsoleWriterFile writerFile;
        private readonly KeyScroll keys;

        internal int OffsetRowIndex { get; private set; }
        internal RowCollection Lines { get; private set; }
        internal bool HasUpdate
        {
            get { return hasUpdate; }
        }

        public string ConsoleName { get; private set; }

        #region Events

        public event EventHandler<WriterOutputEventArgs> WriterOutput = null;

        private void OnWriterOutput(string line)
        {
            EventHandler<WriterOutputEventArgs> handler = WriterOutput;
            if (handler != null) handler(this, new WriterOutputEventArgs(line));
        }

        #endregion

        public ConsoleWriter(string consoleName)
        {
            keys = new KeyScroll();
            OffsetRowIndex = 0;

            Lines = new RowCollection();
            Lines.Add(string.Empty);
            ConsoleName = consoleName;
        }

        public IConsoleWriter Info(string text)
        {
            WriteInner(ConsoleColor.Cyan, text);
            return this;
        }

        public IConsoleWriter Info(string format, params object[] parameters)
        {
            WriteInner(ConsoleColor.Cyan, format, parameters);
            return this;
        }

        public IConsoleWriter Warning(string text)
        {
            WriteInner(ConsoleColor.Yellow, text);
            return this;
        }

        public IConsoleWriter Warning(string format, params object[] parameters)
        {
            WriteInner(ConsoleColor.Yellow, format, parameters);
            return this;
        }

        public IConsoleWriter Error(string text)
        {
            WriteInner(ConsoleColor.Red, text);
            return this;
        }

        public IConsoleWriter Error(string format, params object[] parameters)
        {
            WriteInner(ConsoleColor.Red, format, parameters);
            return this;
        }

        public IConsoleWriter Muted(string text)
        {
            WriteInner(ConsoleColor.DarkGray, text);
            return this;
        }

        public IConsoleWriter Muted(string format, params object[] parameters)
        {
            WriteInner(ConsoleColor.DarkGray, format, parameters);
            return this;
        }

        public IConsoleWriter Text(string text)
        {
            WriteInner(ConsoleColor.White, text);
            return this;
        }

        public IConsoleWriter Text(string format, params object[] parameters)
        {
            WriteInner(ConsoleColor.White, format, parameters);
            return this;
        }

        public IConsoleWriter NewLine()
        {
            Lines.Add(string.Empty);
            hasUpdate = true;
            UpdateScroll();
            return this;
        }

        public IConsoleWriter ClearCurrentLine()
        {
            Lines.Last.Line = string.Empty;
            hasUpdate = true;
            return this;
        }

        public IConsoleWriter Clear()
        {
            Clear(true);
            return this;
        }

        internal void Clear(bool resetScroll)
        {
            Lines.Clear();
            if (resetScroll) OffsetRowIndex = 0;
            hasUpdate = true;
        }

        private void WriteInner(ConsoleColor color, string format, params object[] parameters)
        {
            string append = format;

            if ((parameters != null) && (parameters.Length > 0))
                append = string.Format(format, parameters);

            WriteInner(color, append);
        }

        private void WriteInner(ConsoleColor color, string text)
        {
            string line = string.Concat(Lines.Last.Line, text);

            if (line.Length > ConsoleAsync.Manager.Renderer.ViewWidth)
            {
                string[] newLines = line.FitMultiline(ConsoleAsync.Manager.Renderer.ViewWidth);
                Lines.Last.Line = newLines[0];

                for (int i = 1; i < newLines.Length; i++)
                {
                    Lines.Add(string.Empty);
                    SetNextColor(color);
                    Lines.Last.Line = newLines[i];
                    UpdateScroll();
                }
            }
            else
            {
                SetNextColor(color);
                Lines.Last.Line = line;
            }

            OnWriterOutput(Lines.Last.Line);
            hasUpdate = true;
        }

        private void SetNextColor(ConsoleColor color)
        {
            Lines.SetColor(color);
        }

        public void ScrollBottom()
        {
            OffsetRowIndex = 0;
            hasUpdate = true;
        }

        public void ScrollTop()
        {
            if (Lines.Count < ConsoleAsync.Manager.Renderer.ViewHeight) return;
            OffsetRowIndex = ConsoleAsync.Manager.Renderer.ViewHeight - Lines.Count;
            hasUpdate = true;
        }

        private void UpdateScroll()
        {
            if (OffsetRowIndex < 0) OffsetRowIndex--;
        }

        public void ResetUpdate()
        {
            hasUpdate = false;
        }

        public void SaveOutputToFile(string directory, string name)
        {
            writerFile = new ConsoleWriterFile(this, directory, name);
        }

        public void SaveOutputToFile(string directory, string name, int linesPerFile, int linesPerFlush)
        {
            writerFile = new ConsoleWriterFile(this, directory, name, linesPerFile, linesPerFlush);
        }

        public void CancelSaveOutputToFile()
        {
            if (writerFile == null)
                throw new InvalidOperationException("SaveOutputToFile procedure is not started");

            writerFile.Dispose();
            writerFile = null;
        }


        #region Key Methods

        internal bool ApplyKey(ConsoleKeyInfo key)
        {
            int viewHeight = ConsoleAsync.Manager.Renderer.ViewHeight;
            KeyScrollEnum? keyFunction = keys.TestKey(key);
            if (keyFunction.HasValue == false) return false;

            if (Lines.Count > viewHeight)
            {
                switch (keyFunction.Value)
                {
                    case KeyScrollEnum.RowUp:
                        OffsetRowIndex--;
                        break;

                    case KeyScrollEnum.RowDown:
                        OffsetRowIndex++;
                        break;

                    case KeyScrollEnum.PageUp:
                        OffsetRowIndex -= viewHeight;
                        break;

                    case KeyScrollEnum.PageDown:
                        OffsetRowIndex += viewHeight;
                        break;

                    case KeyScrollEnum.Top:
                        OffsetRowIndex = viewHeight - Lines.Count;
                        break;

                    case KeyScrollEnum.Bottom:
                        OffsetRowIndex = 0;
                        break;
                }

                if (OffsetRowIndex > 0) OffsetRowIndex = 0;
                if (OffsetRowIndex < -(Lines.Count - viewHeight))
                    OffsetRowIndex = -(Lines.Count - viewHeight);
            }
            else
            {
                OffsetRowIndex = 0;
            }

            hasUpdate = true;
            return true;
        }

        #endregion

    }
}

using System;
using System.IO;

namespace AsyncronousConsole.Engine
{
    internal class ConsoleWriterFile : IDisposable
    {
        private StreamWriter fileWriter;
        private ConsoleWriter consoleWriter;

        private readonly string fileName;
        private readonly string fileDirectory;
        private readonly int fileFlushRowCount;
        private readonly int fileMaxRowCount;

        private int fileCounter;
        private int rowCount;
        private int rowFlushCount;

        public string FilePathComplete { get; private set; }

        public ConsoleWriterFile(ConsoleWriter writer, string directory, string name, int linesPerFile = 1000, int linesPerFlush = 50)
        {
            if (string.IsNullOrEmpty(directory))
                throw new ArgumentNullException("name");

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (Directory.Exists(directory) == false)
                throw new ArgumentException(string.Format("Directory '{0}' not exist", directory));

            if (name.Length > 40)
                throw new ArgumentException("Name must be max 40 character");

            if (linesPerFile < 100)
                throw new ArgumentException("LinePerFile parameter must be greater than 100");

            if ((linesPerFlush < 1) || (linesPerFlush >= linesPerFile))
                throw new ArgumentException("LinePerFlush parameter must be greater than 0 and lesser than LinePerFile parameter");

            fileDirectory = directory;
            fileName = name;
            fileMaxRowCount = linesPerFile;
            fileFlushRowCount = linesPerFlush;
            FilePathComplete = null;
            fileCounter = 0;
            rowCount = 0;

            CreateFile();

            consoleWriter = writer;
            consoleWriter.WriterOutput += WriterOutput;
        }

        private void WriterOutput(object sender, Support.WriterOutputEventArgs e)
        {
            WriteToFile(e.Line);
        }

        private void WriteToFile(string line)
        {
            if (rowCount >= fileMaxRowCount)
            {
                CloseFile();
                CreateFile();
            }

            fileWriter.WriteLine(line);

            rowCount++;
            rowFlushCount++;

            if (rowFlushCount > fileFlushRowCount)
            {
                fileWriter.Flush();
                rowFlushCount = 0;
            }
        }

        private void CreateFile()
        {
            fileCounter++;

            FilePathComplete = Path.Combine(fileDirectory,
                string.Format("{0}-{1:yyyy-MM-dd-hh-mm}-{2}.txt", fileName, DateTime.Now, fileCounter));

            fileWriter = File.CreateText(FilePathComplete);
        }

        private void CloseFile()
        {
            if (fileWriter == null)
                throw new InvalidOperationException("Unable to close writer stream, object is null");

            fileWriter.Close();
            fileWriter.Dispose();

            rowFlushCount = 0;
            rowCount = 0;
        }

        public void Dispose()
        {
            consoleWriter.WriterOutput -= WriterOutput;
            consoleWriter = null;

            if (fileWriter != null)
            {
                fileWriter.Flush();
                fileWriter.Close();
                fileWriter.Dispose();
                fileWriter = null;
            }
        }

    }
}

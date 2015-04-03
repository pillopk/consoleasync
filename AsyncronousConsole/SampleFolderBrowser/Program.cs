using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AsyncronousConsole;
using AsyncronousConsole.Support;

namespace SampleFolderBrowser
{
    class Program
    {
        private static string actualFolder = @"c:\";

        static void Main(string[] args)
        {
            IConsole console = ConsoleAsync.CreateConsole("Folder Browser");

            ConsoleAsync.AddCommandToAllConsole("quit", (writer, strings) => ConsoleAsync.Quit());

            console.AddCommand("help", (writer, strings) => writer.Text(@"
Switch to second console with TAB button
the available command are:

dir                          show content of a current directory
cd ..                        back to parent directory
cd <directory name>          enter in specified directory
md <directory name>          create a directory
rd <directory name>          delete a directory
").NewLine().NewLine());

            console.AddCommand("cd", (writer, strings) =>
            {
                if (strings.Count > 0)
                    ChangeFolder(writer, strings[0]);
            });

            console.AddCommand("cd..", (writer, strings) => ChangeFolder(writer, ".."));
            console.AddCommand("dir", (writer, strings) => WriteFolder(writer));

            console.AddCommand("md", (writer, strings) =>
            {
                if (strings.Count < 1)
                {
                    WriteError(writer, new ArgumentException("makedir require a parameter"));
                    return;
                }

                try
                {
                    Directory.CreateDirectory(Path.Combine(actualFolder, strings[0]));
                    writer.Info("Directory created").NewLine();
                }
                catch (Exception ex)
                {
                    WriteError(writer, ex);
                }
            });

            console.AddCommand("rd", (writer, strings) =>
            {
                if (strings.Count < 1)
                {
                    WriteError(writer, new ArgumentException("removedir require a parameter"));
                    return;
                }

                try
                {
                    Directory.Delete(Path.Combine(actualFolder, strings[0]));
                    writer.Info("Directory deleted").NewLine();
                }
                catch (Exception ex)
                {
                    WriteError(writer, ex);
                }
            });

            console.Execute(writer =>
            {
                writer.Info("Folder Explorer sample").NewLine().NewLine();
                console.SendCommand("help");
            });

            ConsoleAsync.Run();
        }

        static void ChangeFolder(IConsoleWriter writer, string newFolder)
        {
            if (newFolder == "..")
            {
                try
                {
                    actualFolder = Directory.GetParent(actualFolder).FullName;
                }
                catch (Exception ex)
                {
                    WriteError(writer, ex);
                }
            }
            else
            {
                string newPath = Path.Combine(actualFolder, newFolder);
                if (Directory.Exists(newPath) == false)
                    writer.NewLine().Error("Folder '{0}' not found in current path", newFolder).NewLine().NewLine();

                actualFolder = newPath;
            }

            writer.Info("FOLDER: {0}", actualFolder).NewLine().NewLine();
        }

        static void WriteFolder(IConsoleWriter writer)
        {
            Dictionary<string, FileInfo> items = new Dictionary<string, FileInfo>();
            string[] files = Directory.GetFiles(actualFolder);
            string[] dirs = Directory.GetDirectories(actualFolder);

            foreach (string file in files)
            {
                FileInfo info = new FileInfo(file);
                items.Add(info.Name, info);
            }

            foreach (string dir in dirs)
            {
                FileInfo info = new FileInfo(dir);
                items.Add(info.Name, info);
            }

            List<FileInfo> infos = items.Values.ToList();
            infos.Sort(new FileInfoComparer());

            foreach (FileInfo info in infos)
            {
                bool isDirectory = info.Attributes.HasFlag(FileAttributes.Directory);

                if (isDirectory) writer.Warning(LineRenderer(info));
                else writer.Text(LineRenderer(info));

                writer.NewLine();
            }
            writer.NewLine();
        }

        static string LineRenderer(FileInfo info)
        {
            bool isDirectory = info.Attributes.HasFlag(FileAttributes.Directory);
            string result = string.Format("{0:yyyy/MM/dd}", info.LastWriteTime);

            if (isDirectory)
            {
                result = string.Concat(result, " <directory>").PadRight(24);
            }
            else
            {
                string length = info.Length.ToFileSize().PadLeft(12);
                result = string.Format("{0} {1} ", result, length);
            }

            int width = result.Length;
            string[] names = info.Name.FitMultiline(ConsoleAsync.ConsoleWidth - width);
            result = string.Concat(result, names[0]);

            if (names.Length > 1)
            {
                for (int i = 1; i < names.Length; i++)
                {
                    result = string.Concat(result, string.Empty.PadRight(width), names[i]);
                }
            }

            return result;
        }

        static void WriteError(IConsoleWriter writer, Exception ex)
        {
            writer.NewLine().Error(ex.Message).NewLine().Error(ex.StackTrace).NewLine().NewLine();
        }

        #region Support Class

        private class FileInfoComparer : IComparer<FileInfo>
        {
            public int Compare(FileInfo x, FileInfo y)
            {
                StringComparer sc = StringComparer.Create(CultureInfo.InvariantCulture, true);
                bool xDir = x.Attributes.HasFlag(FileAttributes.Directory);
                bool yDir = y.Attributes.HasFlag(FileAttributes.Directory);

                if ((xDir && yDir) || (!xDir && !yDir))
                {
                    return sc.Compare(x.Name, y.Name);
                }

                return xDir ? -1 : 1;
            }
        }

        #endregion

    }
}

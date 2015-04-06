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
            // Create console
            IConsole console = ConsoleAsync.CreateConsole("Folder Browser");

            // Add quit command to console
            ConsoleAsync.AddCommandToAllConsole("quit", (writer, strings) => ConsoleAsync.Quit());

            // Add help command to console
            console.AddCommand("help", (writer, strings) => writer.Text(@"
Switch to second console with TAB button
the available command are:

dir                          show content of a current directory
cd ..                        back to parent directory
cd <directory name>          enter in specified directory
md <directory name>          create a directory
rd <directory name>          delete a directory
").NewLine().NewLine());

            // Add cd commands to console
            console.AddCommand("cd", (writer, strings) =>
            {
                // check if there at least one parameter
                if (strings.Count > 0)
                    ChangeFolder(writer, strings[0]);
            });

            // Add cd.. commands to console
            console.AddCommand("cd..", (writer, strings) => ChangeFolder(writer, ".."));

            // Add cd commands to console
            console.AddCommand("dir", (writer, strings) => WriteFolder(writer));

            // Add md commands to console
            console.AddCommand("md", (writer, strings) =>
            {
                // check if there at least one parameter
                if (strings.Count < 1)
                {
                    // If not throw an exception
                    WriteError(writer, new ArgumentException("makedir require a parameter"));
                    return;
                }

                try
                {
                    // Try to create directory with passed parameter
                    Directory.CreateDirectory(Path.Combine(actualFolder, strings[0]));
                    writer.Info("Directory created").NewLine();
                }
                catch (Exception ex)
                {
                    // Write exception
                    WriteError(writer, ex);
                }
            });

            console.AddCommand("rd", (writer, strings) =>
            {
                // check if there at least one parameter
                if (strings.Count < 1)
                {
                    // If not throw an exception
                    WriteError(writer, new ArgumentException("removedir require a parameter"));
                    return;
                }

                try
                {
                    // Try to delete directory with passed parameter
                    Directory.Delete(Path.Combine(actualFolder, strings[0]));
                    writer.Info("Directory deleted").NewLine();
                }
                catch (Exception ex)
                {
                    // Write exception
                    WriteError(writer, ex);
                }
            });

            // Call an execute to write initial message to console
            console.Execute(writer =>
            {
                writer.Info("Folder Explorer sample").NewLine().NewLine();
                console.SendCommand("help");
            });

            // Wait for user commands
            ConsoleAsync.Run();
        }

        // Change actual folder
        static void ChangeFolder(IConsoleWriter writer, string newFolder)
        {
            // Check if parent folder
            if (newFolder == "..")
            {
                // Try to move to parent
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
                // Create new path
                string newPath = Path.Combine(actualFolder, newFolder);

                // If path not exist throw exception
                if (Directory.Exists(newPath) == false)
                    writer.NewLine().Error("Folder '{0}' not found in current path", newFolder).NewLine().NewLine();

                // Update path
                actualFolder = newPath;
            }

            // Write folder path to console
            writer.Info("FOLDER: {0}", actualFolder).NewLine().NewLine();
        }

        static void WriteFolder(IConsoleWriter writer)
        {
            // Create support collection
            List<FileInfo> infos = new List<FileInfo>();

            // Read files and directories
            string[] files = Directory.GetFiles(actualFolder);
            string[] dirs = Directory.GetDirectories(actualFolder);

            // Write each dir in collection
            infos.AddRange(dirs.Select(dir => new FileInfo(dir)));

            // Write each file in collection
            infos.AddRange(files.Select(file => new FileInfo(file)));

            // Sort collection
            infos.Sort(new FileInfoComparer());

            // Cicle every item
            foreach (FileInfo info in infos)
            {
                // Check if item is a directory
                bool isDirectory = info.Attributes.HasFlag(FileAttributes.Directory);

                // Write item in correct format
                if (isDirectory) writer.Warning(LineRenderer(info));
                else writer.Text(LineRenderer(info));

                // New line
                writer.NewLine();
            }
            // New line for separation
            writer.NewLine();
        }

        // Render the single line of a dir
        static string LineRenderer(FileInfo info)
        {
            // Check if item is directory
            bool isDirectory = info.Attributes.HasFlag(FileAttributes.Directory);

            // Start line with write time
            string result = string.Format("{0:yyyy/MM/dd}", info.LastWriteTime);

            if (isDirectory)
            {
                // Write only a static string for directory
                result = string.Concat(result, " <directory>").PadRight(24);
            }
            else
            {
                // Write the size of the file
                string length = info.Length.ToFileSize().PadLeft(12);
                result = string.Format("{0} {1} ", result, length);
            }

            // Write file name, multiline if needed
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
            // Write an error to console
            writer.NewLine().Error(ex.Message).NewLine().Error(ex.StackTrace).NewLine().NewLine();
        }

        #region Support Class

        // Compare FileInfo
        private class FileInfoComparer : IComparer<FileInfo>
        {
            public int Compare(FileInfo x, FileInfo y)
            {
                // Create string comparer
                StringComparer sc = StringComparer.Create(CultureInfo.InvariantCulture, true);

                // Create flag to check if one of the file is a directory
                bool xDir = x.Attributes.HasFlag(FileAttributes.Directory);
                bool yDir = y.Attributes.HasFlag(FileAttributes.Directory);

                // String compare only if the fileinfo are of same type
                if ((xDir && yDir) || (!xDir && !yDir))
                {
                    return sc.Compare(x.Name, y.Name);
                }

                // If not directory become first
                return xDir ? -1 : 1;
            }
        }

        #endregion

    }
}

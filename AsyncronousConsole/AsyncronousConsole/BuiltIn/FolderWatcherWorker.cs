using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AsyncronousConsole.Support;

namespace AsyncronousConsole.BuiltIn
{
    public class FolderWatcherWorker : ConsoleWorker
    {
        private bool hasError;
        private bool firstRender = true;
        private string errorMessage = null;
        private string path;
        private FileSystemWatcher watch;
        private Dictionary<string, FileInfo> items;
        private Func<FileInfo, string> renderer;
        private bool hasChange;

        public override TimeSpan? IntervalBetweenExecution
        {
            get { return TimeSpan.FromMilliseconds(500); }
        }

        public FolderWatcherWorker(string directoryPath, Func<FileInfo, string> lineRenderer = null)
        {
            if (CheckExist(directoryPath) == false)
                throw new ArgumentException("Directory not exist");


            renderer = lineRenderer ?? DefautRenderer;
            path = directoryPath;
        }

        public override void OnStart(IConsoleWriter writer)
        {
            if (CheckExist(path) == false) return;

            ReadDirectory();
            watch = new FileSystemWatcher(path);
            watch.Created += WatchCreated;
            watch.Deleted += WatchDeleted;
            watch.Changed += WatchChanged;
            watch.Renamed += WatchRenamed;
            watch.Error += WatchError;
            watch.EnableRaisingEvents = true;
            hasChange = true;
        }

        private void WatchError(object sender, ErrorEventArgs e)
        {
            SetError(e.GetException().Message);
            hasChange = true;
        }

        private void SetError(string message)
        {
            hasError = true;
            errorMessage = message;
        }

        public override void OnExecute(IConsoleWriter writer, TimeSpan elapsed)
        {
            if ((CheckExist(path) == false) && (hasError == false))
            {
                SetError("Directory not exist");
                hasChange = true;
            }

            if ((string.IsNullOrEmpty(errorMessage) == false) && hasChange)
            {
                DisposeWatcher();
                writer.Clear();
                writer.Info("CONTENT OF FOLDER: {0}", path).NewLine().NewLine();
                writer.Error("Error in FileSystemWatcher object:").NewLine();
                writer.Error(errorMessage);
                hasChange = false;
                return;
            }

            if (hasChange)
            {
                writer.Clear();
                writer.Info("CONTENT OF FOLDER: {0}", path).NewLine().NewLine();

                List<FileInfo> infos = items.Values.ToList();
                infos.Sort(new FileInfoComparer());

                foreach (FileInfo info in infos)
                {
                    bool isDirectory = info.Attributes.HasFlag(FileAttributes.Directory);

                    if (isDirectory) writer.Warning(renderer(info));
                    else writer.Text(renderer(info));

                    writer.NewLine();
                }

                if (firstRender)
                {
                    firstRender = false;
                    writer.ScrollTop();
                }

                hasChange = false;
            }
        }

        public override void OnStop(IConsoleWriter writer, TimeSpan elapsed)
        {
            if (CheckExist(path) == false) return;
            DisposeWatcher();
            items.Clear();
        }

        private void DisposeWatcher()
        {
            watch.EnableRaisingEvents = false;
            watch.Created -= WatchCreated;
            watch.Deleted -= WatchDeleted;
            watch.Changed -= WatchChanged;
            watch.Renamed -= WatchRenamed;
            watch.Dispose();
            watch = null;
        }

        private void WatchCreated(object sender, FileSystemEventArgs e)
        {
            AddInfo(e.FullPath);
            hasChange = true;
        }

        private void WatchChanged(object sender, FileSystemEventArgs e)
        {
            UpdateInfo(e.FullPath);
            hasChange = true;
        }

        private void WatchDeleted(object sender, FileSystemEventArgs e)
        {
            items.Remove(e.Name);
            hasChange = true;
        }

        private void WatchRenamed(object sender, RenamedEventArgs e)
        {
            AddInfo(e.FullPath);
            items.Remove(e.OldName);
            hasChange = true;
        }

        private void ReadDirectory()
        {
            items = new Dictionary<string, FileInfo>();
            string[] files = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);

            foreach (string file in files) { AddInfo(file); }
            foreach (string dir in dirs) { AddInfo(dir); }
        }

        private void AddInfo(string filePath)
        {
            FileInfo info = new FileInfo(filePath);
            items.Add(info.Name, info);
        }

        private void UpdateInfo(string filePath)
        {
            FileInfo info = new FileInfo(filePath);
            items[info.Name] = info;
        }

        private string DefautRenderer(FileInfo info)
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
            string[] names = info.Name.FitMultiline(ConsoleAsync.Manager.Renderer.ViewWidth - width);
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

        private bool CheckExist(string directoryPath)
        {
            return Directory.Exists(directoryPath);
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

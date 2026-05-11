using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SpiroStockManagement
{
    //public class FileChangedEventArgs : EventArgs
    //{
    //    public string FilePath { get; set; }
    //    public DateTime TimeOfChange { get; set; }

    //}
    public class Syncing
    {
        //public event EventHandler<FileChangedEventArgs> DatabaseFileWasUpdated;
        //public Dictionary<string, FileSystemWatcher> FileSystemWatchers;

        //public List<string> FilesToCheckPaths;


        //public void Initialize(List<string> fileToCheckPaths)
        //{
        //    FilesToCheckPaths = fileToCheckPaths;
        //    FileSystemWatchers = new Dictionary<string, FileSystemWatcher>();

        //    //Initialize files watch
        //    foreach (string _fileToCheck in FilesToCheckPaths)
        //    {
        //        if (!FileSystemWatchers.ContainsKey(_fileToCheck))
        //        {
        //            var watcher = new FileSystemWatcher();
        //            watcher.Path = Path.GetDirectoryName(_fileToCheck);
        //            watcher.Filter = Path.GetFileName(_fileToCheck);
        //            watcher.NotifyFilter = NotifyFilters.LastWrite;
        //            watcher.EnableRaisingEvents = true;
        //            watcher.Changed += (s, e) =>
        //            {
        //                FileChangedEventArgs _FileChangedEventArgs = new FileChangedEventArgs();
        //                _FileChangedEventArgs.FilePath = _fileToCheck;
        //                _FileChangedEventArgs.TimeOfChange = DateTime.Now;
        //                OnDatabaseFileWasUpdated(_FileChangedEventArgs);
        //            };
        //            FileSystemWatchers.Add(_fileToCheck, watcher);
        //        }
        //    }
        //}


        //public void Initialize(string directory)
        //{
        //    FileSystemWatchers = new Dictionary<string, FileSystemWatcher>();

        //    //Initialize files watch
        //    FileSystemWatcher watcher = new FileSystemWatcher();
        //    watcher.Path = Path.GetDirectoryName(directory);
        //    watcher.Filter = "DatabaseChangedByAndroidApp.txt";
        //    watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName;


        //    watcher.EnableRaisingEvents = true;

        //    watcher.Created += watcher_Created;
        //    FileSystemWatchers.Add(directory, watcher);


        //}

        DateTime lastDatabaseChangedDate;
        string DatabaseFilePath;
        public void Initialize(string databaseFilePath)
        {
            lastDatabaseChangedDate = System.IO.File.GetLastWriteTime(databaseFilePath);
            DatabaseFilePath = databaseFilePath;
        }

        public bool HasTheDatabaseChanged()
        {
            DateTime _ft = GlobalVariables.LastTimeDatabaseWasChangedByMe;
            DateTime _currentLastWrite = System.IO.File.GetLastWriteTime(DatabaseFilePath);
            if (_currentLastWrite != lastDatabaseChangedDate)
            {
                lastDatabaseChangedDate = System.IO.File.GetLastWriteTime(DatabaseFilePath);

                //if ft and lastwrite are really similar is beacause it was this program
                double _secondsOfDifference = 0;
                if (_ft > _currentLastWrite)
                    _secondsOfDifference = (_ft - _currentLastWrite).TotalSeconds;
                if (_currentLastWrite > _ft)
                    _secondsOfDifference = (_currentLastWrite - _ft).TotalSeconds;

                if (_secondsOfDifference == 0) return false;
                
                Console.WriteLine(_ft.ToString());
                //check if the last change is near of our variable
                
                //OnDatabaseFileWasUpdated(new FileChangedEventArgs());
                return true;
            }
            
            return false;
        }

        //void watcher_Created(object sender, FileSystemEventArgs e)
        //{
        //    if (e.Name == "DatabaseChangedByAndroidApp")
        //        OnDatabaseFileWasUpdated(new FileChangedEventArgs());
            
        //}

        //protected virtual void OnDatabaseFileWasUpdated(FileChangedEventArgs e)
        //{
        //    EventHandler<FileChangedEventArgs> eh = DatabaseFileWasUpdated;
        //    if (eh != null)
        //    {
        //        eh(this, e);
        //    }
        //}
    }
}

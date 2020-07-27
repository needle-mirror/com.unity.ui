using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine.UIElements;

namespace UnityEditor.UIElements.GameObjects
{
    internal class FileSystemWatcher : System.IO.FileSystemWatcher
    {
        public FileSystemWatcher(string path)
            : base(path)
        {}

        public void SimulateChange(string path)
        {
            var fullPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), path);
            var dir = System.IO.Path.GetDirectoryName(fullPath) ?? String.Empty;
            var name = System.IO.Path.GetFileName(fullPath);
            FileSystemEventArgs args = new FileSystemEventArgs(WatcherChangeTypes.Changed, dir, name);
            OnChanged(args);
        }
    }

    internal class FileWatch : IEquatable<FileWatch>
    {
        private readonly string m_Path;
        private readonly IFileChangedNotify m_Watcher;

        public IFileChangedNotify watcher => m_Watcher;
        public string path => m_Path;
        public int pathHash { get; }
        public bool enabled { get; set; }
        public bool needsNotification { get; set; }

        public static int ComputePathHash(string path)
        {
            string fullPath = Path.GetFullPath(path);
            fullPath = fullPath.Replace("\\", "/");
            return fullPath.GetHashCode();
        }

        public FileWatch(IFileChangedNotify watcher, string assetPath)
        {
            m_Watcher = watcher;
            m_Path = assetPath;
            pathHash = ComputePathHash(assetPath);
            enabled = false;
            needsNotification = false;
        }

        public bool Equals(FileWatch other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return pathHash == other.pathHash && watcher.Equals(other.watcher);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FileWatch)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (pathHash * 397) ^ watcher.GetHashCode();
            }
        }

        public static bool operator==(FileWatch left, FileWatch right)
        {
            return Equals(left, right);
        }

        public static bool operator!=(FileWatch left, FileWatch right)
        {
            return !Equals(left, right);
        }
    }

    [InitializeOnLoad]
    internal class FileWatcher : IFileWatcher
    {
        private readonly FileSystemWatcher m_FileWatcher;
        private bool m_GlobalShouldNotify;
        private List<FileWatch> m_WatchedPaths = new List<FileWatch>();
        private List<FileWatch> m_WatchersToNotify = new List<FileWatch>();

        private void OnEditorApplicationUpdate()
        {
            if (m_GlobalShouldNotify)
            {
                m_GlobalShouldNotify = false;

                foreach (var e in m_WatchedPaths)
                {
                    if (e.needsNotification)
                    {
                        e.needsNotification = false;
                        // Store entries to notify in m_WatchersToNotify to ensure
                        // m_WatchedPaths is not modified while we iterate on it
                        m_WatchersToNotify.Add(e);
                    }
                }

                try
                {
                    foreach (var entry in m_WatchersToNotify)
                    {
                        entry.watcher.OnFileChanged(entry.path);
                    }
                }
                finally
                {
                    m_WatchersToNotify.Clear();
                }
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            int pathHash = FileWatch.ComputePathHash(e.FullPath);
            var entries = m_WatchedPaths.FindAll(element => element.pathHash == pathHash);
            foreach (var entry in entries)
            {
                entry.needsNotification = entry.enabled;
            }
            m_GlobalShouldNotify = entries.Count > 0;
        }

        private static FileWatcher s_Instance;

        public static FileWatcher Instance()
        {
            if (s_Instance == null)
            {
                s_Instance = new FileWatcher();
            }

            return s_Instance;
        }

        private FileWatcher()
        {
            m_FileWatcher = new FileSystemWatcher(Application.dataPath);
            m_FileWatcher.IncludeSubdirectories = true;
            m_FileWatcher.NotifyFilter = NotifyFilters.LastWrite;
            m_FileWatcher.Created += OnChanged;
            m_FileWatcher.Changed += OnChanged;
            m_FileWatcher.Deleted += OnChanged;
            EditorApplication.update += OnEditorApplicationUpdate;
        }

        static FileWatcher()
        {
            Reset();
        }

        internal static void Reset()
        {
            s_Instance = null;
            UnityEngine.UIElements.FileWatcher.SetFileWatcherImplementation(Instance());
        }

        internal void SimulateChange(string path)
        {
            m_FileWatcher.SimulateChange(path);
        }

        public void AddFile(IFileChangedNotify watcher, string path)
        {
            if (watcher == null || path == null)
                throw new ArgumentNullException();

            if (path.Length == 0)
                throw new ArgumentOutOfRangeException();

            var entry = new FileWatch(watcher, path);
            if (!m_WatchedPaths.Contains(entry))
            {
                m_WatchedPaths.Add(entry);
            }
        }

        public void RemoveAllFiles(IFileChangedNotify watcher)
        {
            m_WatchedPaths.RemoveAll(element => element.watcher == watcher);

            bool hasEnabled = false;
            foreach (var e in m_WatchedPaths)
            {
                if (e.enabled)
                {
                    hasEnabled = true;
                    break;
                }
            }

            m_FileWatcher.EnableRaisingEvents = hasEnabled;
        }

        public void EnableWatcher(IFileChangedNotify watcher)
        {
            bool hasEnabled = false;
            foreach (var e in m_WatchedPaths)
            {
                if (e.watcher == watcher)
                    e.enabled = true;

                hasEnabled |= e.enabled;
            }

            m_FileWatcher.EnableRaisingEvents = hasEnabled;
        }

        public void DisableWatcher(IFileChangedNotify watcher)
        {
            bool hasEnabled = false;
            foreach (var e in m_WatchedPaths)
            {
                if (e.watcher == watcher)
                    e.enabled = false;

                hasEnabled |= e.enabled;
            }

            m_FileWatcher.EnableRaisingEvents = hasEnabled;
        }
    }
}

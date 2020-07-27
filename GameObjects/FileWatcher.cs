using UnityEngine;

namespace UnityEngine.UIElements
{
    internal interface IFileChangedNotify
    {
        void OnFileChanged(string path);
    }

    interface IFileWatcher
    {
        void AddFile(IFileChangedNotify watcher, string path);
        void RemoveAllFiles(IFileChangedNotify watcher);
        void EnableWatcher(IFileChangedNotify watcher);
        void DisableWatcher(IFileChangedNotify watcher);
    }

    class FileWatcher : IFileWatcher
    {
        private static IFileWatcher m_Implementation;

        internal static void SetFileWatcherImplementation(IFileWatcher watcher)
        {
            m_Implementation = watcher;
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
        }

        public void AddFile(IFileChangedNotify watcher, string path)
        {
            m_Implementation?.AddFile(watcher, path);
        }

        public void RemoveAllFiles(IFileChangedNotify watcher)
        {
            m_Implementation?.RemoveAllFiles(watcher);
        }

        public void EnableWatcher(IFileChangedNotify watcher)
        {
            m_Implementation?.EnableWatcher(watcher);
        }

        public void DisableWatcher(IFileChangedNotify watcher)
        {
            m_Implementation?.DisableWatcher(watcher);
        }
    }
}

namespace blog.dachs.ServerManager
{
    using System;
    using System.Collections;
    using System.Linq;

    public class BackupSourceFileCollection : GlobalExtention, IList
    {
        private BackupSource backupSource;
        private List<BackupSourceFile> collection;

        public BackupSourceFileCollection(Configuration configuration, BackupSource backupSource) : base(configuration)
        {
            this.Collection = new List<BackupSourceFile>();
            this.BackupSource = backupSource;
        }

        private BackupSource BackupSource
        {
            get { return this.backupSource; }
            set { this.backupSource = value; }
        }

        private List<BackupSourceFile> Collection
        {
            get { return this.collection; }
            set { this.collection = value; }
        }

        public void CreateBackup()
        {
            FileInfo fileInfo;
            BackupSourceFile sourceFile;
            string path;
            string name;

            foreach (string filePath in Directory.EnumerateFiles(this.BackupSource.FullAbsolutePath, "*.*", SearchOption.AllDirectories))
            {
                path = string.Empty;
                name = string.Empty;

                fileInfo = new FileInfo(filePath);

                if (fileInfo.DirectoryName != null)
                {
                    if (fileInfo.DirectoryName != BackupSource.FullAbsolutePath)
                    {
                        path = fileInfo.DirectoryName.Replace(BackupSource.FullAbsolutePath + "\\", string.Empty);
                    }

                    name = fileInfo.Name;

                    sourceFile = new BackupSourceFile(this.Configuration, this.backupSource, path, name);
                    
                    // set additional data
                    sourceFile.CreationDate = fileInfo.CreationTime;
                    sourceFile.LastAccessDate = fileInfo.LastAccessTime;
                    sourceFile.LastWriteDate = fileInfo.LastWriteTime;
                    sourceFile.LastSeenDate = DateTime.Now;
                    sourceFile.Size = fileInfo.Length;

                    sourceFile.PrepareOnDisc();
                    sourceFile.ReadFromDisc();
                    sourceFile.WriteToDisc();
                    this.Add(sourceFile);
                }
            }
        }

        #region implementing IList
        public object? this[int index] { get => ((IList)Collection)[index]; set => ((IList)Collection)[index] = value; }

        public bool IsFixedSize => ((IList)Collection).IsFixedSize;

        public bool IsReadOnly => ((IList)Collection).IsReadOnly;

        public int Count => ((ICollection)Collection).Count;

        public bool IsSynchronized => ((ICollection)Collection).IsSynchronized;

        public object SyncRoot => ((ICollection)Collection).SyncRoot;

        public int Add(object? value)
        {
            return ((IList)Collection).Add(value);
        }

        public void Clear()
        {
            ((IList)Collection).Clear();
        }

        public bool Contains(object? value)
        {
            return ((IList)Collection).Contains(value);
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)Collection).CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Collection).GetEnumerator();
        }

        public int IndexOf(object? value)
        {
            return ((IList)Collection).IndexOf(value);
        }

        public void Insert(int index, object? value)
        {
            ((IList)Collection).Insert(index, value);
        }

        public void Remove(object? value)
        {
            ((IList)Collection).Remove(value);
        }

        public void RemoveAt(int index)
        {
            ((IList)Collection).RemoveAt(index);
        }
        #endregion

        public void Add(BackupSourceFile backupFile)
        {
            this.Collection.Add(backupFile);
        }

        public void Add(BackupSourceFileCollection collection)
        {
            this.AddRange(collection);
        }

        public void AddRange(BackupSourceFileCollection collection)
        {
            foreach (Backup backup in collection)
            {
                this.Add(backup);
            }
        }

        public BackupSourceFile ElementAt(int index)
        {
            return this.Collection[index];
        }

        public BackupSourceFileCollection GetFilesToBackup()
        {
            BackupSourceFileCollection backupSourceFileCollection = new BackupSourceFileCollection(Configuration, this.BackupSource);
            
            foreach(BackupSourceFile backupSourceFile in this.Collection)
            {
                if (backupSourceFile.LastWriteDate > backupSourceFile.LastBackupDate)
                {
                    backupSourceFileCollection.Add(backupSourceFile);
                }
            }

            return backupSourceFileCollection;
        }
    }
}

namespace blog.dachs.ServerManager
{
    using System;
    using System.Collections;
    using System.Linq;

    public class BackupSourceFileCollection : GlobalExtention, IList
    {
        private BackupSource backupSource;
        private Dictionary<string, BackupSourceFile> collection;

        public BackupSourceFileCollection(Configuration configuration, BackupSource backupSource) : base(configuration)
        {
            this.Collection = new Dictionary<string, BackupSourceFile>();
            this.BackupSource = backupSource;
        }

        private BackupSource BackupSource
        {
            get { return this.backupSource; }
            set { this.backupSource = value; }
        }

        private Dictionary<string, BackupSourceFile> Collection
        {
            get { return this.collection; }
            set { this.collection = value; }
        }

        public void ReadFromFilesystem()
        {
            foreach (string filePath in Directory.EnumerateFiles(this.BackupSource.FullAbsolutePath, "*.*", SearchOption.AllDirectories))
            {
                FileInfo fileInfo = new FileInfo(filePath);

                if (fileInfo.DirectoryName != null)
                {
                    BackupSourceFile backupFile = new BackupSourceFile(this.Configuration);
                    backupFile.BackupSource = this.backupSource;
                    
                    if (fileInfo.DirectoryName == BackupSource.FullAbsolutePath)
                    {
                        backupFile.Path = string.Empty;
                    }
                    else
                    {
                        backupFile.Path = fileInfo.DirectoryName.Replace(BackupSource.FullAbsolutePath + "\\", string.Empty);
                    }
                    
                    backupFile.Name = fileInfo.Name;
                    backupFile.CreationDate = fileInfo.CreationTime;
                    backupFile.LastAccessDate = fileInfo.LastAccessTime;
                    backupFile.LastWriteDate = fileInfo.LastWriteTime;
                    backupFile.LastSeenDate = DateTime.Now;
                    backupFile.Size = fileInfo.Length;

                    if (this.Collection.ContainsKey(backupFile.FullRelativePath))
                    {
                        this.Collection[backupFile.FullRelativePath].CreationDate = backupFile.CreationDate;
                        this.Collection[backupFile.FullRelativePath].LastAccessDate = backupFile.LastAccessDate;
                        this.Collection[backupFile.FullRelativePath].LastWriteDate = backupFile.LastWriteDate;
                        this.Collection[backupFile.FullRelativePath].LastSeenDate = backupFile.LastSeenDate;
                        this.Collection[backupFile.FullRelativePath].Size = backupFile.Size;
                    }
                    else
                    {
                        backupFile.PrepareOnDisc();
                        backupFile.ReadFromDisc();
                        this.Add(backupFile);
                    }
                }
            }
        }

        public void WriteToDisc()
        {
            BackupSourceFile backupSourceFile;

            for (int i=0; i < this.Collection.Count; i++)
            {
                backupSourceFile = this.ElementAt(i);
                
                if (backupSourceFile.Changed)
                {
                    backupSourceFile.WriteToDisc();
                }
            }
        }

        public void CreateBackup()
        {
            BackupSourceFileCollection filesToBackup = this.GetFilesToBackup();
            BackupSourceFile fileToBackup;

            for (int i = 0; i < filesToBackup.Count; i++)
            {
                fileToBackup = filesToBackup.ElementAt(i);

                
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
            if (!this.Collection.ContainsKey(backupFile.FullRelativePath))
                this.Collection.Add(backupFile.FullRelativePath, backupFile);
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
            return this.Collection[this.Collection.ElementAt(index).Key];
        }

        public BackupSourceFileCollection GetFilesToBackup()
        {
            BackupSourceFileCollection backupSourceFileCollection = new BackupSourceFileCollection(Configuration, this.BackupSource);
            BackupSourceFile backupSourceFile;

            for (int i = 0; i < this.Collection.Count; i++)
            {
                backupSourceFile = this.ElementAt(i);

                if (backupSourceFile.LastWriteDate > backupSourceFile.LastBackupDate)
                {
                    backupSourceFileCollection.Add(backupSourceFile);
                }
            }

            return backupSourceFileCollection;
        }
    }
}

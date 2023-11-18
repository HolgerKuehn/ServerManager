namespace blog.dachs.ServerManager
{
    using System;
    using System.Collections;

    public class BackupSourceCollection : GlobalExtention, IList
    {
        private Backup backup;
        private Dictionary<string, BackupSource> sourceCollection;

        public BackupSourceCollection(Configuration configuration, Backup backup) : base(configuration)
        {
            this.SourceCollection = new Dictionary<string, BackupSource>();
            this.backup = backup;
        }

        private Backup Backup
        {
            get { return this.backup; }
            set { this.backup = value; }
        }

        private Dictionary<string, BackupSource> SourceCollection
        {
            get { return this.sourceCollection; }
            set { this.sourceCollection = value; }
        }

        public void ReadFromFilesystem()
        {
            BackupSource backupSource;

            List<string> baseDirectoryCollection = new List<string>();
            List<string> sourceDirectoryCollection = new List<string>();

            // read all directories for requiered depth; start on baseDirectory
            sourceDirectoryCollection.Add(this.Backup.SourceBasePath);

            for (int i=0; i < this.Backup.SourceNameDepth; i++)
            {
                baseDirectoryCollection.Clear();
                baseDirectoryCollection.AddRange(sourceDirectoryCollection);
                sourceDirectoryCollection.Clear();

                foreach(string baseDirectory in baseDirectoryCollection)
                {
                    foreach (string directoryPath in Directory.EnumerateDirectories(baseDirectory, "*", SearchOption.TopDirectoryOnly))
                    {
                        sourceDirectoryCollection.Add(directoryPath);
                    }
                }
            }

            foreach (string directoryPath in sourceDirectoryCollection)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);

                backupSource = new BackupSource(this.Configuration, this.Backup);
                backupSource.Backup = backup;
                backupSource.Path = directoryInfo.FullName.Replace(backup.SourceBasePath + "\\", string.Empty);
 
                if (!this.SourceCollection.ContainsKey(backupSource.FullRelativePath))
                {
                    backupSource.PrepareOnDisc();
                    backupSource.ReadFromDisc();
                    this.Add(backupSource);
                }
            }


            // propergate to Source
            for (int i = 0; i < this.SourceCollection.Count; i++)
            {
                backupSource = this.ElementAt(i);
                backupSource.ReadFromFilesystem();
            }
        }

        public void WriteToDisc()
        {
            BackupSource backupSource;

            for (int i = 0; i < this.SourceCollection.Count; i++)
            {
                backupSource = this.ElementAt(i);

                if (backupSource.Changed)
                {
                    backupSource.WriteToDisc();
                }
            }
        }

        #region implementing IList
        public object? this[int index] { get => ((IList)SourceCollection)[index]; set => ((IList)SourceCollection)[index] = value; }

        public bool IsFixedSize => ((IList)SourceCollection).IsFixedSize;

        public bool IsReadOnly => ((IList)SourceCollection).IsReadOnly;

        public int Count => ((ICollection)SourceCollection).Count;

        public bool IsSynchronized => ((ICollection)SourceCollection).IsSynchronized;

        public object SyncRoot => ((ICollection)SourceCollection).SyncRoot;

        public int Add(object? value)
        {
            return ((IList)SourceCollection).Add(value);
        }

        public void Clear()
        {
            ((IList)SourceCollection).Clear();
        }

        public bool Contains(object? value)
        {
            return ((IList)SourceCollection).Contains(value);
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)SourceCollection).CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)SourceCollection).GetEnumerator();
        }

        public int IndexOf(object? value)
        {
            return ((IList)SourceCollection).IndexOf(value);
        }

        public void Insert(int index, object? value)
        {
            ((IList)SourceCollection).Insert(index, value);
        }

        public void Remove(object? value)
        {
            ((IList)SourceCollection).Remove(value);
        }

        public void RemoveAt(int index)
        {
            ((IList)SourceCollection).RemoveAt(index);
        }
        #endregion

        public void Add(BackupSource backupSource)
        {
            if (!this.SourceCollection.ContainsKey(backupSource.FullRelativePath))
                this.SourceCollection.Add(backupSource.FullRelativePath, backupSource);
        }

        public void Add(BackupSourceCollection collection)
        {
            this.AddRange(collection);
        }

        public void AddRange(BackupSourceCollection collection)
        {
            foreach (BackupSource backupSource in collection)
            {
                this.Add(backupSource);
            }
        }

        public BackupSource ElementAt(int index)
        {
            return this.SourceCollection[this.SourceCollection.ElementAt(index).Key];
        }
    }
}

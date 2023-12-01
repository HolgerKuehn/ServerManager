namespace blog.dachs.ServerManager
{
    using System;
    using System.Collections;

    public class BackupSetSourceFileCollection : GlobalExtention, IList
    {
        private BackupSet backupSet;
        private List<BackupSetSourceFile> setSourceFileCollection;

        public BackupSetSourceFileCollection(Configuration configuration, BackupSet backupSet) : base(configuration)
        {
            this.SetSourceFileCollection = new List<BackupSetSourceFile>();
            this.BackupSet = backupSet;
        }

        private BackupSet BackupSet
        {
            get { return this.backupSet; }
            set { this.backupSet = value; }
        }

        private List<BackupSetSourceFile> SetSourceFileCollection
        {
            get { return this.setSourceFileCollection; }
            set { this.setSourceFileCollection = value; }
        }

        public void CreateBackup()
        {
            foreach (BackupSetSourceFile backupSetSourceFile in this.SetSourceFileCollection)
            {
                backupSetSourceFile.CreateBackup();
            }
        }

        #region implementing IList
        public object? this[int index] { get => ((IList)SetSourceFileCollection)[index]; set => ((IList)SetSourceFileCollection)[index] = value; }

        public bool IsFixedSize => ((IList)SetSourceFileCollection).IsFixedSize;

        public bool IsReadOnly => ((IList)SetSourceFileCollection).IsReadOnly;

        public int Count => ((ICollection)SetSourceFileCollection).Count;

        public bool IsSynchronized => ((ICollection)SetSourceFileCollection).IsSynchronized;

        public object SyncRoot => ((ICollection)SetSourceFileCollection).SyncRoot;

        public int Add(object? value)
        {
            return ((IList)SetSourceFileCollection).Add(value);
        }

        public void Clear()
        {
            ((IList)SetSourceFileCollection).Clear();
        }

        public bool Contains(object? value)
        {
            return ((IList)SetSourceFileCollection).Contains(value);
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)SetSourceFileCollection).CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)SetSourceFileCollection).GetEnumerator();
        }

        public int IndexOf(object? value)
        {
            return ((IList)SetSourceFileCollection).IndexOf(value);
        }

        public void Insert(int index, object? value)
        {
            ((IList)SetSourceFileCollection).Insert(index, value);
        }

        public void Remove(object? value)
        {
            ((IList)SetSourceFileCollection).Remove(value);
        }

        public void RemoveAt(int index)
        {
            ((IList)SetSourceFileCollection).RemoveAt(index);
        }
        #endregion

        public void Add(BackupSetSourceFile backupSetSourceFile)
        {
            this.SetSourceFileCollection.Add(backupSetSourceFile);
        }

        public void Add(BackupSetSourceFileCollection collection)
        {
            this.AddRange(collection);
        }

        public void AddRange(BackupSetSourceFileCollection collection)
        {
            foreach (BackupSetSourceFile backupSetSourceFile in collection)
            {
                this.Add(backupSetSourceFile);
            }
        }

        public BackupSetSourceFile ElementAt(int index)
        {
            return this.SetSourceFileCollection[index];
        }
    }
}

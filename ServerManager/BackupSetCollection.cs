namespace blog.dachs.ServerManager
{
    using System;
    using System.Collections;

    public class BackupSetCollection : GlobalExtention, IList
    {
        private BackupSource backupSource;
        private List<BackupSet> setCollection;

        public BackupSetCollection(Configuration configuration, BackupSource backupSource) : base(configuration)
        {
            this.SetCollection = new List<BackupSet>();
            this.backupSource = backupSource;
        }

        private BackupSource BackupSource
        {
            get { return this.backupSource; }
            set { this.backupSource = value; }
        }

        private List<BackupSet> SetCollection
        {
            get { return this.setCollection; }
            set { this.setCollection = value; }
        }

        #region implementing IList
        public object? this[int index] { get => ((IList)SetCollection)[index]; set => ((IList)SetCollection)[index] = value; }

        public bool IsFixedSize => ((IList)SetCollection).IsFixedSize;

        public bool IsReadOnly => ((IList)SetCollection).IsReadOnly;

        public int Count => ((ICollection)SetCollection).Count;

        public bool IsSynchronized => ((ICollection)SetCollection).IsSynchronized;

        public object SyncRoot => ((ICollection)SetCollection).SyncRoot;

        public int Add(object? value)
        {
            return ((IList)SetCollection).Add(value);
        }

        public void Clear()
        {
            ((IList)SetCollection).Clear();
        }

        public bool Contains(object? value)
        {
            return ((IList)SetCollection).Contains(value);
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)SetCollection).CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)SetCollection).GetEnumerator();
        }

        public int IndexOf(object? value)
        {
            return ((IList)SetCollection).IndexOf(value);
        }

        public void Insert(int index, object? value)
        {
            ((IList)SetCollection).Insert(index, value);
        }

        public void Remove(object? value)
        {
            ((IList)SetCollection).Remove(value);
        }

        public void RemoveAt(int index)
        {
            ((IList)SetCollection).RemoveAt(index);
        }
        #endregion

        public void Add(BackupSet backupSet)
        {
            this.SetCollection.Add(backupSet);
        }

        public void Add(BackupSetCollection collection)
        {
            this.AddRange(collection);
        }

        public void AddRange(BackupSetCollection collection)
        {
            foreach (BackupSet backupSet in collection)
            {
                this.Add(backupSet);
            }
        }

        public BackupSet ElementAt(int index)
        {
            return this.SetCollection[index];
        }
    }
}

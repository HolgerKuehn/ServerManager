namespace blog.dachs.ServerManager
{
    using System;
    using System.Collections;
    using System.Data;

    public class BackupCollection : GlobalExtention, IList
    {
        private List<Backup> backupCollection;

        public BackupCollection(Configuration configuration) : base(configuration)
        {
            this.Collection = new List<Backup>();

            // read backups from disk
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupCollection_BackupCollection, "reading BackupCollection"));

            string sqlCommand = this.Database.GetCommand(Command.BackupCollection_BackupCollection);
            sqlCommand = sqlCommand.Replace("<ConfigurationID>", this.Configuration.ConfigurationID.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupCollection_BackupCollection, sqlCommand));

            DataTable dataTable = this.Database.GetDataTable(sqlCommand);
            DataRow dataRow = null;
            int backupId;

            // get BackupID 
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                backupId = Convert.ToInt32(dataRow[0].ToString());

                this.Collection.Add(new Backup(this.Configuration, backupId));
            }
        }

        private List<Backup> Collection
        {
            get { return this.backupCollection; }
            set { this.backupCollection = value; }
        }

        public void RunBackup()
        {
            foreach (Backup backup in this.Collection)
            {
                backup.ReadFileList();
                backup.WriteChangedToDisc();
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

        public void Add(Backup backup)
        {
            this.Collection.Add(backup);
        }

        public void Add(BackupCollection collection)
        {
            this.AddRange(collection);
        }

        public void AddRange(BackupCollection collection)
        {
            foreach (Backup backup in collection)
            {
                this.Add(backup);
            }
        }

        public Backup ElementAt(int index)
        {
            return this.Collection[index];
        }
    }
}

namespace blog.dachs.ServerManager
{
    using System;
    using System.Collections;
    using System.Data;

    public class KeePassDatabaseCollection : GlobalExtention, IList
    {
        private List<KeePassDatabase> collection;

        public KeePassDatabaseCollection(Configuration configuration) : base(configuration)
        {
            KeePassDatabase primaryKeePassDatabase;
            KeePassDatabase keePassDatabase;
            KeePassEntry keePassEntry;

            this.Collection = new List<KeePassDatabase>();

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.KeePassDatabaseCollection_KeePassDatabaseCollection, "reading primary KeePassDatabase"));

            string sqlCommand = this.Database.GetCommand(Command.KeePassDatabaseCollection_KeePassDatabaseCollection);
            sqlCommand = sqlCommand.Replace("<ConfigurationID>", this.Configuration.ConfigurationID.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.KeePassDatabaseCollection_KeePassDatabaseCollection, sqlCommand));

            DataTable dataTable = this.Database.GetDataTable(sqlCommand);
            DataRow dataRow = null;
            int keePassDatabaseID;

            // reading values
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                keePassDatabaseID = Convert.ToInt32(dataRow[0].ToString());

                primaryKeePassDatabase = new KeePassDatabase(this.Configuration, keePassDatabaseID);
                this.Collection.Add(primaryKeePassDatabase);
            }


            // read Database inside primary DB
            primaryKeePassDatabase = this.GetKeePassDatabase("ServerManager.kdbx");

            foreach (string title in primaryKeePassDatabase.GetListOfEntries(""))
            {
                keePassEntry = primaryKeePassDatabase.GetEntry(title);
                
                if (keePassEntry.Title != "ServerManager.kdbx")
                {
                    keePassDatabase = new KeePassDatabase(this.Configuration);
                    keePassDatabase.Name = keePassEntry.Title;
                    keePassDatabase.Password = keePassEntry.Password;
                    this.Collection.Add(keePassDatabase);
                }
            }
        }

        public List<KeePassDatabase> Collection
        {
            get { return this.collection; }
            set { this.collection = value; }
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

        public void Add(KeePassDatabase keePassDatabase)
        {
            this.Collection.Add(keePassDatabase);
        }

        public void Add(KeePassDatabaseCollection collection)
        {
            this.AddRange(collection);
        }

        public void AddRange(KeePassDatabaseCollection collection)
        {
            foreach (KeePassDatabase keePassDatabase in collection)
            {
                this.Add(keePassDatabase);
            }
        }

        public KeePassDatabase ElementAt(int index)
        {
            return this.Collection[index];
        }

        public KeePassDatabase GetKeePassDatabase(string name)
        {
            KeePassDatabase result = new KeePassDatabase(this.Configuration);

            foreach (KeePassDatabase keePassDatabase in this.Collection)
            {
                if (keePassDatabase.Name == name)
                {
                    result = keePassDatabase;
                }
            }

            return result;
        }
    }
}

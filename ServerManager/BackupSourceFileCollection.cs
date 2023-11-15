namespace blog.dachs.ServerManager
{
    using System;
    using System.Collections;
    using System.Linq;

    public class BackupSourceFileCollection : GlobalExtention, IList
    {
        private Backup backup;
        private Dictionary<string, BackupSourceFile> collection;

        public BackupSourceFileCollection(Configuration configuration, Backup backup) : base(configuration)
        {
            this.Collection = new Dictionary<string, BackupSourceFile>();
            this.backup = backup;

            // read backups from disk
            //this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupFileCollection_BackupFileCollection, "reading BackupSourceFileCollection"));

            //string sqlCommand = this.Database.GetCommand(Command.BackupFileCollection_BackupFileCollection);
            //sqlCommand = sqlCommand.Replace("<ConfigurationID>", this.Configuration.ConfigurationID.ToString());

            //this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupFileCollection_BackupFileCollection, sqlCommand));

            //DataTable dataTable = this.Database.GetDataTable(sqlCommand);
            //DataRow dataRow = null;
            //int backupId;

            //// get BackupID 
            //for (int row = 0; row < dataTable.Rows.Count; row++)
            //{
            //    dataRow = dataTable.Rows[row];
            //    backupId = Convert.ToInt32(dataRow[0].ToString());

            //    this.Collection.Add(new Backup(this.Configuration, backupId));
            //}
        }

        private Backup Backup
        {
            get { return this.backup; }
            set { this.backup = value; }
        }

        private Dictionary<string, BackupSourceFile> Collection
        {
            get { return this.collection; }
            set { this.collection = value; }
        }

        public void ReadFromFilesystem()
        {
            foreach (string filePath in Directory.EnumerateFiles(this.Backup.SourceBasePath, "*.*", SearchOption.AllDirectories))
            {
                FileInfo fileInfo = new FileInfo(filePath);

                if (fileInfo.DirectoryName != null)
                {
                    BackupSourceFile backupFile = new BackupSourceFile(this.Configuration);
                    backupFile.Backup = backup;
                    backupFile.RelativePath = fileInfo.DirectoryName.Replace(backup.SourceBasePath + "\\", string.Empty);
                    backupFile.FileName = fileInfo.Name;
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
                        backupFile.ReadBackupFileIdFromDisc();
                        this.Add(backupFile);
                    }
                }
                else
                { 
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
    }
}

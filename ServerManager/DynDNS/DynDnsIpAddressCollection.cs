namespace blog.dachs.ServerManager.DynDNS
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using blog.dachs.ServerManager;

    public class DynDnsIpAddressCollection : GlobalExtention, IList
    {
        private DynDnsIpAddressReferenceType referenceType;
        private int referenceId;
        private List<DynDnsIpAddress> ipAddressCollection;

        public DynDnsIpAddressCollection(Configuration configuration) : base(configuration)
        {
            this.IpAddressCollection = new List<DynDnsIpAddress>();
        }

        public DynDnsIpAddressReferenceType ReferenceType
        {
            get { return referenceType; }
            set
            {
                referenceType = value;

                foreach (DynDnsIpAddress ipAddress in this.ipAddressCollection)
                {
                    ipAddress.ReferenceType = referenceType;
                }
            }
        }

        public int ReferenceId
        {
            get { return referenceId; }
            set
            {
                referenceId = value;

                foreach (DynDnsIpAddress ipAddress in this.ipAddressCollection)
                {
                    ipAddress.ReferenceId = referenceId;
                }
            }
        }

        private List<DynDnsIpAddress> IpAddressCollection
        {
            get { return ipAddressCollection; }
            set { ipAddressCollection = value; }
        }

        public string IpAddresses
        {
            get
            {
                string ipAddresses = string.Empty;

                foreach (DynDnsIpAddress ipAddress in this.IpAddressCollection)
                {
                    if (!ipAddresses.Contains(ipAddress.IpAddress))
                    {
                        if (ipAddresses != string.Empty)
                        {
                            ipAddresses = ipAddresses + ", ";
                        }

                        ipAddresses = ipAddresses + ipAddress.IpAddress;
                    }
                }

                return ipAddresses;
            }
            set { }
        }

        public string NetworkAddresses
        {
            get
            {
                string ipAddresses = string.Empty;

                foreach (DynDnsIpAddress ipAddress in this.IpAddressCollection)
                {
                    if (!ipAddresses.Contains(ipAddress.NetworkAddress))
                    {
                        if (ipAddresses != string.Empty)
                        {
                            ipAddresses = ipAddresses + ", ";
                        }

                        ipAddresses = ipAddresses + ipAddress.NetworkAddress;
                    }
                }

                return ipAddresses;
            }
            set { }
        }

        public DynDnsIpAddress NewIpAddress()
        {
            DynDnsIpAddress ipAddress;
            ipAddress = new DynDnsIpAddress(this.Configuration);
            ipAddress.ReferenceType = this.ReferenceType;
            ipAddress.ReferenceId = this.ReferenceId;

            return ipAddress;
        }

        /// <summary>
        /// reads all IP Addresses for the reference-object
        /// </summary>
        public void ReadIpAddressCollection()
        {
            List<DynDnsIpAddressObject> ipAddressObjects;

            ipAddressObjects = [
                DynDnsIpAddressObject.DNSServer,
                DynDnsIpAddressObject.ServiceDNS,
                DynDnsIpAddressObject.ServiceNetworkAdapter
            ];

            this.ReadIpAddressCollection(ipAddressObjects);
        }

        /// <summary>
        /// reads all IP Addresses fitting the parameters for the reference-object
        /// </summary>
        /// <param name="ipAddressObjects">List of Objects to include</param>
        /// <remarks>
        /// includes both Versions; IPv4 and IPv6
        /// includes all Types; Public, Private, LinkLocal and UniqueLocal
        /// </remarks>
        public void ReadIpAddressCollection(List<DynDnsIpAddressObject> ipAddressObjects)
        {
            List<DynDnsIpAddressType> ipAddressTypes;

            ipAddressTypes = [
                DynDnsIpAddressType.Public,
                DynDnsIpAddressType.Private,
                DynDnsIpAddressType.LinkLocal,
                DynDnsIpAddressType.UniqueLocal
            ];

            this.ReadIpAddressCollection(ipAddressObjects, ipAddressTypes);
        }

        /// <summary>
        /// reads all IP Addresses fitting the parameters for the reference-object
        /// </summary>
        /// <param name="ipAddressObject">List of Objects to include</param>
        /// <remarks>includes both Versions, Types, IPv4 and IPv6</remarks>
        public void ReadIpAddressCollection(DynDnsIpAddressObject ipAddressObject)
        {
            List<DynDnsIpAddressObject> ipAddressObjects;

            ipAddressObjects = [
                ipAddressObject
            ];

            this.ReadIpAddressCollection(ipAddressObjects);
        }

        /// <summary>
        /// reads all IP Addresses fitting the parameters for the reference-object
        /// </summary>
        /// <param name="ipAddressObject">List of Objects to include</param>
        /// <param name="ipAddressType">List of Types to include</param>
        /// <remarks>includes both Versions; IPv4 and IPv6</remarks>
        public void ReadIpAddressCollection(DynDnsIpAddressObject ipAddressObject, DynDnsIpAddressType ipAddressType)
        {
            List<DynDnsIpAddressObject> ipAddressObjects;

            ipAddressObjects = [
                ipAddressObject
            ];

            List<DynDnsIpAddressType> ipAddressTypes;

            ipAddressTypes = [
                ipAddressType
            ];

            this.ReadIpAddressCollection(ipAddressObjects, ipAddressTypes);
        }

        /// <summary>
        /// reads all IP Addresses fitting the parameters for the reference-object
        /// </summary>
        /// <param name="ipAddressObject">List of Objects to include</param>
        /// <param name="ipAddressType">List of Types to include</param>
        /// <remarks>includes both Versions; IPv4 and IPv6</remarks>
        public void ReadIpAddressCollection(DynDnsIpAddressObject ipAddressObject, DynDnsIpAddressType ipAddressType, DynDnsIpAddressVersion ipAddressVersion)
        {
            List<DynDnsIpAddressObject> ipAddressObjects;

            ipAddressObjects = [
                ipAddressObject
            ];

            List<DynDnsIpAddressType> ipAddressTypes;

            ipAddressTypes = [
                ipAddressType
            ]; 
            
            List<DynDnsIpAddressVersion> ipAddressVersions;

            ipAddressVersions = [
                ipAddressVersion
            ];

            this.ReadIpAddressCollection(ipAddressObjects, ipAddressTypes, ipAddressVersions);
        }

        /// <summary>
        /// reads all IP Addresses fitting the parameters for the reference-object
        /// </summary>
        /// <param name="ipAddressObject">List of Objects to include</param>
        /// <param name="ipAddressType">List of Types to include</param>
        /// <remarks>includes both Versions; IPv4 and IPv6</remarks>
        public void ReadIpAddressCollection(DynDnsIpAddressObject ipAddressObject, List<DynDnsIpAddressType> ipAddressTypes)
        {
            List<DynDnsIpAddressObject> ipAddressObjects;

            ipAddressObjects = [
                ipAddressObject
            ];

            this.ReadIpAddressCollection(ipAddressObjects, ipAddressTypes);
        }

        /// <summary>
        /// reads all IP Addresses fitting the parameters for the reference-object
        /// </summary>
        /// <param name="ipAddressObjects">List of Objects to include</param>
        /// <param name="ipAddressTypes">List of Types to include</param>
        /// <remarks>includes both Versions; IPv4 and IPv6</remarks>
        public void ReadIpAddressCollection(List<DynDnsIpAddressObject> ipAddressObjects, List<DynDnsIpAddressType> ipAddressTypes)
        {
            List<DynDnsIpAddressVersion> ipAddressVersions;

            ipAddressVersions = [
                DynDnsIpAddressVersion.IPv4,
                DynDnsIpAddressVersion.IPv6
            ];

            this.ReadIpAddressCollection(ipAddressObjects, ipAddressTypes, ipAddressVersions);
        }

        /// <summary>
        /// reads all IP Addresses fitting the parameters for the reference-object
        /// </summary>
        /// <param name="ipAddressObjects">List of Objects to include</param>
        /// <param name="ipAddressTypes">List of Types to include</param>
        /// <param name="ipAddressVersions">List of Versions to include</param>
        public void ReadIpAddressCollection(List<DynDnsIpAddressObject> ipAddressObjects, List<DynDnsIpAddressType> ipAddressTypes, List<DynDnsIpAddressVersion> ipAddressVersions)
        {
            DataRow dataRow;

            int ipAddressId = 0;
            DynDnsIpAddress ipAddress = null;
            string ipAddressObjectsString = string.Empty;
            string ipAddressTypesString = string.Empty;
            string ipAddressVersionsString = string.Empty;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsIpCollection_ReadIpAddressCollection, "read IPs for ReferenceType " + ReferenceType + " and ReferenceID " + ReferenceId + " from disc"));

            string sqlCommand = Database.GetCommand(Command.DynDnsIpCollection_ReadIpAddressCollection);

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsIpCollection_ReadIpAddressCollection, sqlCommand));

            sqlCommand = sqlCommand.Replace("<DynDnsIpAddressReferenceTypeID>", Convert.ToString((byte)ReferenceType));
            sqlCommand = sqlCommand.Replace("<DynDnsIpAddressReferenceID>", Convert.ToString(ReferenceId));
            
            for (int i = 0; i < ipAddressObjects.Count; i++)
            {
                if (i != 0)
                {
                    ipAddressObjectsString += ", ";
                }

                ipAddressObjectsString += (byte)ipAddressObjects[i];
            }

            sqlCommand = sqlCommand.Replace("<DynDnsIpAddressObjectID>", ipAddressObjectsString);


            for (int i = 0; i < ipAddressTypes.Count; i++)
            {
                if (i != 0)
                {
                    ipAddressTypesString += ", ";
                }

                ipAddressTypesString += (byte)ipAddressTypes[i];
            }

            sqlCommand = sqlCommand.Replace("<DynDnsIpAddressTypeID>", ipAddressTypesString);


            for (int i = 0; i < ipAddressVersions.Count; i++)
            {
                if (i != 0)
                {
                    ipAddressVersionsString += ", ";
                }

                ipAddressVersionsString += (byte)ipAddressVersions[i];
            }

            sqlCommand = sqlCommand.Replace("<DynDnsIpAddressVersionID>", ipAddressVersionsString);


            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsIpCollection_ReadIpAddressCollection, sqlCommand));

            int row = 0;
            while (true)
            {
                dataRow = this.Database.GetDataRow(sqlCommand, row);

                if (dataRow != null )
                {
                    ipAddressId = Convert.ToInt32(dataRow[0].ToString());
                    ipAddress = new DynDnsIpAddress(this.Configuration, ipAddressId);
                    ipAddress.ReferenceType = this.ReferenceType;
                    ipAddress.ReferenceId = this.ReferenceId;

                    ipAddressCollection.Add(ipAddress);
                }
                else
                {
                    break;
                }

                row++; 
            }

            this.IpAddressCollection.Sort();
        }

        public void SetIpAddressPrefix()
        {
            foreach (DynDnsIpAddress ipAddress in this)
            {
                ipAddress.SetIpAddressPrefix();
            }
        }

        /// <summary>
        /// Writes IpAddresses to disc
        /// </summary>
        public void WriteIpAddressCollection()
        {
            // determine Index of IP-Address
            this.Remove(DynDnsIpAddressType.NotValid);
            this.IpAddressCollection.Sort();

            DynDnsIpAddress ipAddress;
            DynDnsIpAddressObject lastIpAddressObject = DynDnsIpAddressObject.NotValid;
            DynDnsIpAddressType lastIpAddressType = DynDnsIpAddressType.NotValid;
            DynDnsIpAddressVersion lastIpAddressVersion = DynDnsIpAddressVersion.NotValid;
            int lastIpAddressIndex = 1;

            for (int i = 0; i < this.IpAddressCollection.Count; i++)
            {
                ipAddress = this.IpAddressCollection.ElementAt(i);

                if (
                    ipAddress.IpAddressObject != lastIpAddressObject ||
                    ipAddress.IpAddressType != lastIpAddressType ||
                    ipAddress.IpAddressVersion != lastIpAddressVersion
                )
                {
                    lastIpAddressIndex = 1;
                    lastIpAddressObject = ipAddress.IpAddressObject;
                    lastIpAddressType = ipAddress.IpAddressType;
                    lastIpAddressVersion = ipAddress.IpAddressVersion;
                }

                // set Index
                ipAddress.IpAddressIndex = lastIpAddressIndex;
                lastIpAddressIndex++;


                // write IP to disk
                ipAddress.WriteIpAddress();
            }
        }

        #region implementing IList
        public object? this[int index] { get => ((IList)IpAddressCollection)[index]; set => ((IList)IpAddressCollection)[index] = value; }

        public bool IsFixedSize => ((IList)IpAddressCollection).IsFixedSize;

        public bool IsReadOnly => ((IList)IpAddressCollection).IsReadOnly;

        public int Count => ((ICollection)IpAddressCollection).Count;

        public bool IsSynchronized => ((ICollection)IpAddressCollection).IsSynchronized;

        public object SyncRoot => ((ICollection)IpAddressCollection).SyncRoot;

        public int Add(object? value)
        {
            return ((IList)IpAddressCollection).Add(value);
        }

        public void Clear()
        {
            ((IList)IpAddressCollection).Clear();
        }

        public bool Contains(object? value)
        {
            return ((IList)IpAddressCollection).Contains(value);
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)IpAddressCollection).CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)IpAddressCollection).GetEnumerator();
        }

        public int IndexOf(object? value)
        {
            return ((IList)IpAddressCollection).IndexOf(value);
        }

        public void Insert(int index, object? value)
        {
            ((IList)IpAddressCollection).Insert(index, value);
        }

        public void Remove(object? value)
        {
            ((IList)IpAddressCollection).Remove(value);
        }

        public void RemoveAt(int index)
        {
            ((IList)IpAddressCollection).RemoveAt(index);
        }
        #endregion

        public void Add(DynDnsIpAddress ipAddress)
        {
            ipAddress.ReferenceType = this.ReferenceType;
            ipAddress.ReferenceId = this.ReferenceId;

            this.IpAddressCollection.Add(ipAddress);
        }

        public void Add(DynDnsIpAddressCollection ipAddressCollection)
        {
            this.AddRange(ipAddressCollection);
        }

        public void AddRange(DynDnsIpAddressCollection ipAddressCollection)
        {
            foreach (DynDnsIpAddress ipAddress in ipAddressCollection)
            {
                this.Add(ipAddress);
            }
        }

        public bool Contains(DynDnsIpAddress ipAddress)
        {
            bool containsIpAddress = false;

            foreach (DynDnsIpAddress ipAddressTest in this.IpAddressCollection)
            {
                if (ipAddressTest.IpAddress == ipAddress.IpAddress)
                {
                    containsIpAddress = true;
                    break;
                }
            }

            return containsIpAddress;
        }

        public void Sort()
        {
            this.IpAddressCollection.Sort();
        }

        public void Remove(DynDnsIpAddressType ipAddressType)
        {
            List<DynDnsIpAddress> ipAddressCollection = new List<DynDnsIpAddress>();

            foreach (DynDnsIpAddress ipAddress in IpAddressCollection)
            {
                if (ipAddress.IpAddressType != ipAddressType)
                {
                    ipAddressCollection.Add(ipAddress);
                }
            }

            this.IpAddressCollection = ipAddressCollection;
        }

        public DynDnsIpAddress ElementAt(int index)
        {
            return this.IpAddressCollection[index];
        }
    }
}

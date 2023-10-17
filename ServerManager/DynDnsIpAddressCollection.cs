namespace blog.dachs.ServerManager
{
    using System;
    using System.Collections;

    public class DynDnsIpAddressCollection : GlobalExtention, IList
    {
        private List<DynDnsIpAddress> ipAddressCollection;

        public DynDnsIpAddressCollection(Configuration configuration) : base(configuration)
        {
            this.IpAddressCollection = new List<DynDnsIpAddress>();
        }

        private List<DynDnsIpAddress> IpAddressCollection
        {
            get { return this.ipAddressCollection; }
            set { this.ipAddressCollection = value; }
        }

        public DynDnsIpAddressCollection GetIpAddressCollection(DynDnsIpAddressType ipAddressType)
        {
            DynDnsIpAddressCollection ipAddressCollection = new DynDnsIpAddressCollection(this.Configuration);

            ipAddressCollection.AddRange(this.GetIpAddressCollection(ipAddressType, DynDnsIpAddressVersion.IPv4));
            ipAddressCollection.AddRange(this.GetIpAddressCollection(ipAddressType, DynDnsIpAddressVersion.IPv6));

            return ipAddressCollection;
        }

        public DynDnsIpAddressCollection GetIpAddressCollection(DynDnsIpAddressVersion ipAddressVersion)
        {
            DynDnsIpAddressCollection ipAddressCollection = new DynDnsIpAddressCollection(this.Configuration);

            ipAddressCollection.AddRange(this.GetIpAddressCollection(DynDnsIpAddressType.Public, ipAddressVersion));
            ipAddressCollection.AddRange(this.GetIpAddressCollection(DynDnsIpAddressType.Private, ipAddressVersion));
            ipAddressCollection.AddRange(this.GetIpAddressCollection(DynDnsIpAddressType.UniqueLocal, ipAddressVersion));
            ipAddressCollection.AddRange(this.GetIpAddressCollection(DynDnsIpAddressType.LinkLocal, ipAddressVersion));

            ipAddressCollection.AddRange(this.GetIpAddressCollection(DynDnsIpAddressType.DnsServerPublic, ipAddressVersion));
            ipAddressCollection.AddRange(this.GetIpAddressCollection(DynDnsIpAddressType.DnsServerPrivate, ipAddressVersion));
            ipAddressCollection.AddRange(this.GetIpAddressCollection(DynDnsIpAddressType.DnsServerUniqueLocal, ipAddressVersion));
            ipAddressCollection.AddRange(this.GetIpAddressCollection(DynDnsIpAddressType.DnsServerLinkLocal, ipAddressVersion));

            return ipAddressCollection;
        }

        public DynDnsIpAddressCollection GetIpAddressCollection(DynDnsIpAddressType ipAddressType, DynDnsIpAddressVersion ipAddressVersion)
        {
            DynDnsIpAddressCollection ipAddressCollection = new DynDnsIpAddressCollection(this.Configuration);

            foreach (DynDnsIpAddress ipAddress in this.IpAddressCollection)
            {
                if (ipAddress.IpAddressType == ipAddressType && ipAddress.IpAddressVersion == ipAddressVersion)
                {
                    ipAddressCollection.Add(ipAddress);
                }
            }

            return ipAddressCollection;
        }

        public List<DynDnsIpAddressType> GetAvailableIpTypes()
        {
            List<DynDnsIpAddressType> ipAddressTypeCollection = new List<DynDnsIpAddressType>();

            foreach (DynDnsIpAddress ipAddress in this.IpAddressCollection)
            {
                if (!ipAddressTypeCollection.Contains(ipAddress.IpAddressType))
                {
                    ipAddressTypeCollection.Add(ipAddress.IpAddressType);
                }
            }

            return ipAddressTypeCollection;
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

        public void Add(string ipAddress)
        {
            this.IpAddressCollection.Add(new DynDnsIpAddress(this.Configuration, ipAddress));
        }

        public void Add(DynDnsIpAddress ipAddress)
        {
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

        public void Remove(DynDnsIpAddressType ipAddressType)
        {
            List<DynDnsIpAddress> ipAddressCollection = new List<DynDnsIpAddress>();

            foreach (DynDnsIpAddress ipAddress in this.IpAddressCollection)
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

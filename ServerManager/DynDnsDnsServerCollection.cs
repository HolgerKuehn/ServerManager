namespace blog.dachs.ServerManager
{
    using System;
    using System.Collections;

    public class DynDnsDnsServerCollection : GlobalExtention, IList
    {
        private List<DynDnsDnsServer> dnsServerCollection;

        public DynDnsDnsServerCollection(Configuration configuration) : base(configuration)
        {
            this.DnsServerCollection = new List<DynDnsDnsServer>();
        }

        private List<DynDnsDnsServer> DnsServerCollection
        {
            get { return this.dnsServerCollection; }
            set { this.dnsServerCollection = value; }
        }

        public DynDnsDnsServer GetDnsServer(DnsServerType dnsServerType)
        {
            DynDnsDnsServer dynDnsDnsServerResult = this.DnsServerCollection[0];

            foreach (DynDnsDnsServer dynDnsDnsServer in this.DnsServerCollection)
            {
                if (dynDnsDnsServer.DynDnsDnsServerType == dnsServerType)
                    dynDnsDnsServerResult = dynDnsDnsServer;
            }

            return dynDnsDnsServerResult;
        }

        #region implementing IList
        public object? this[int index] { get => ((IList)DnsServerCollection)[index]; set => ((IList)DnsServerCollection)[index] = value; }

        public bool IsFixedSize => ((IList)DnsServerCollection).IsFixedSize;

        public bool IsReadOnly => ((IList)DnsServerCollection).IsReadOnly;

        public int Count => ((ICollection)DnsServerCollection).Count;

        public bool IsSynchronized => ((ICollection)DnsServerCollection).IsSynchronized;

        public object SyncRoot => ((ICollection)DnsServerCollection).SyncRoot;

        public int Add(object? value)
        {
            return ((IList)DnsServerCollection).Add(value);
        }

        public void Clear()
        {
            ((IList)DnsServerCollection).Clear();
        }

        public bool Contains(object? value)
        {
            return ((IList)DnsServerCollection).Contains(value);
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)DnsServerCollection).CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)DnsServerCollection).GetEnumerator();
        }

        public int IndexOf(object? value)
        {
            return ((IList)DnsServerCollection).IndexOf(value);
        }

        public void Insert(int index, object? value)
        {
            ((IList)DnsServerCollection).Insert(index, value);
        }

        public void Remove(object? value)
        {
            ((IList)DnsServerCollection).Remove(value);
        }

        public void RemoveAt(int index)
        {
            ((IList)DnsServerCollection).RemoveAt(index);
        }
        #endregion
    }
}

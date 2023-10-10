namespace blog.dachs.ServerManager
{
    using System;
    using System.Collections;

    public class DynDnsIpAddressCollection : GlobalExtention, IList
    {
        private List<DynDnsIpAddress> dynDnsIpAddresses;

        public DynDnsIpAddressCollection(Configuration configuration) : base(configuration)
        {
            this.DynDnsIpAddresses = new List<DynDnsIpAddress>();
        }

        private List<DynDnsIpAddress> DynDnsIpAddresses
        {
            get { return this.dynDnsIpAddresses; }
            set { this.dynDnsIpAddresses = value; }
        }

        public List<DynDnsIpAddress> GetDynDnsIpAddress(DynDnsIpAddressType DynDnsIpAddressType)
        {
            return this.DynDnsIpAddresses;
        }

        #region implementing IList
        public object? this[int index] { get => ((IList)DynDnsIpAddresses)[index]; set => ((IList)DynDnsIpAddresses)[index] = value; }

        public bool IsFixedSize => ((IList)DynDnsIpAddresses).IsFixedSize;

        public bool IsReadOnly => ((IList)DynDnsIpAddresses).IsReadOnly;

        public int Count => ((ICollection)DynDnsIpAddresses).Count;

        public bool IsSynchronized => ((ICollection)DynDnsIpAddresses).IsSynchronized;

        public object SyncRoot => ((ICollection)DynDnsIpAddresses).SyncRoot;

        public int Add(object? value)
        {
            return ((IList)DynDnsIpAddresses).Add(value);
        }

        public void Clear()
        {
            ((IList)DynDnsIpAddresses).Clear();
        }

        public bool Contains(object? value)
        {
            return ((IList)DynDnsIpAddresses).Contains(value);
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)DynDnsIpAddresses).CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)DynDnsIpAddresses).GetEnumerator();
        }

        public int IndexOf(object? value)
        {
            return ((IList)DynDnsIpAddresses).IndexOf(value);
        }

        public void Insert(int index, object? value)
        {
            ((IList)DynDnsIpAddresses).Insert(index, value);
        }

        public void Remove(object? value)
        {
            ((IList)DynDnsIpAddresses).Remove(value);
        }

        public void RemoveAt(int index)
        {
            ((IList)DynDnsIpAddresses).RemoveAt(index);
        }
        #endregion
    }
}

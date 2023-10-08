namespace blog.dachs.ServerManager
{
    using System;
    using System.Collections;

    public class DynDnsDnsServerCollection : IList
    {
        private List<DynDnsDnsServer> dynDnsDnsServers;

        public DynDnsDnsServerCollection()
        {
            this.DynDnsDnsServers = new List<DynDnsDnsServer>();
        }

        public List<DynDnsDnsServer> DynDnsDnsServers
        {
            get { return this.dynDnsDnsServers; }
            set { this.dynDnsDnsServers = value; }
        }

        #region implementing IList
        public object? this[int index] { get => ((IList)DynDnsDnsServers)[index]; set => ((IList)DynDnsDnsServers)[index] = value; }

        public bool IsFixedSize => ((IList)DynDnsDnsServers).IsFixedSize;

        public bool IsReadOnly => ((IList)DynDnsDnsServers).IsReadOnly;

        public int Count => ((ICollection)DynDnsDnsServers).Count;

        public bool IsSynchronized => ((ICollection)DynDnsDnsServers).IsSynchronized;

        public object SyncRoot => ((ICollection)DynDnsDnsServers).SyncRoot;

        public int Add(object? value)
        {
            return ((IList)DynDnsDnsServers).Add(value);
        }

        public void Clear()
        {
            ((IList)DynDnsDnsServers).Clear();
        }

        public bool Contains(object? value)
        {
            return ((IList)DynDnsDnsServers).Contains(value);
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)DynDnsDnsServers).CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)DynDnsDnsServers).GetEnumerator();
        }

        public int IndexOf(object? value)
        {
            return ((IList)DynDnsDnsServers).IndexOf(value);
        }

        public void Insert(int index, object? value)
        {
            ((IList)DynDnsDnsServers).Insert(index, value);
        }

        public void Remove(object? value)
        {
            ((IList)DynDnsDnsServers).Remove(value);
        }

        public void RemoveAt(int index)
        {
            ((IList)DynDnsDnsServers).RemoveAt(index);
        }
        #endregion
    }
}

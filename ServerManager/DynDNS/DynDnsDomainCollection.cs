namespace blog.dachs.ServerManager.DynDNS
{
    using System;
    using System.Collections;

    public class DynDnsDomainCollection : GlobalExtention, IList
    {
        private List<DynDnsDomain> dynDnsDomains;

        public DynDnsDomainCollection(Configuration configuration) : base(configuration)
        {
            dynDnsDomains = new List<DynDnsDomain>();
        }

        public List<DynDnsDomain> DynDnsDomains
        {
            get { return dynDnsDomains; }
            set { dynDnsDomains = value; }
        }

        #region implementing IList
        public object? this[Int32 index] { get => ((IList)DynDnsDomains)[index]; set => ((IList)DynDnsDomains)[index] = value; }


        public bool IsFixedSize => ((IList)DynDnsDomains).IsFixedSize;

        public bool IsReadOnly => ((IList)DynDnsDomains).IsReadOnly;

        public Int32 Count => ((ICollection)DynDnsDomains).Count;

        public bool IsSynchronized => ((ICollection)DynDnsDomains).IsSynchronized;

        public object SyncRoot => ((ICollection)DynDnsDomains).SyncRoot;

        public Int32 Add(object? value)
        {
            return ((IList)DynDnsDomains).Add(value);
        }

        public void Clear()
        {
            ((IList)DynDnsDomains).Clear();
        }

        public bool Contains(object? value)
        {
            return ((IList)DynDnsDomains).Contains(value);
        }

        public void CopyTo(Array array, Int32 index)
        {
            ((ICollection)DynDnsDomains).CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)DynDnsDomains).GetEnumerator();
        }

        public Int32 IndexOf(object? value)
        {
            return ((IList)DynDnsDomains).IndexOf(value);
        }

        public void Insert(Int32 index, object? value)
        {
            ((IList)DynDnsDomains).Insert(index, value);
        }

        public void Remove(object? value)
        {
            ((IList)DynDnsDomains).Remove(value);
        }

        public void RemoveAt(Int32 index)
        {
            ((IList)DynDnsDomains).RemoveAt(index);
        }
        #endregion
    }
}

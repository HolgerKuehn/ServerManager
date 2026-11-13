using blog.dachs.ServerManager;

namespace blog.dachs.ServerManager.DynDNS
{
    using System;
    using System.Collections;

    public class DynDnsServiceCollection : GlobalExtention, IList
    {
        private List<DynDnsService> serviceCollection;

        public DynDnsServiceCollection(Configuration configuration) : base(configuration)
        {
            serviceCollection = new List<DynDnsService>();
        }


        public List<DynDnsService> ServiceCollection
        {
            get { return serviceCollection; }
            set { serviceCollection = value; }
        }

        #region implementing IList
        public object? this[Int32 index] { get => ((IList)ServiceCollection)[index]; set => ((IList)ServiceCollection)[index] = value; }

        public bool IsFixedSize => ((IList)ServiceCollection).IsFixedSize;

        public bool IsReadOnly => ((IList)ServiceCollection).IsReadOnly;

        public Int32 Count => ((ICollection)ServiceCollection).Count;

        public bool IsSynchronized => ((ICollection)ServiceCollection).IsSynchronized;

        public object SyncRoot => ((ICollection)ServiceCollection).SyncRoot;

        public Int32 Add(object? value)
        {
            return ((IList)ServiceCollection).Add(value);
        }

        public void Clear()
        {
            ((IList)ServiceCollection).Clear();
        }

        public bool Contains(object? value)
        {
            return ((IList)ServiceCollection).Contains(value);
        }

        public void CopyTo(Array array, Int32 index)
        {
            ((ICollection)ServiceCollection).CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)ServiceCollection).GetEnumerator();
        }

        public Int32 IndexOf(object? value)
        {
            return ((IList)ServiceCollection).IndexOf(value);
        }

        public void Insert(Int32 index, object? value)
        {
            ((IList)ServiceCollection).Insert(index, value);
        }

        public void Remove(object? value)
        {
            ((IList)ServiceCollection).Remove(value);
        }

        public void RemoveAt(Int32 index)
        {
            ((IList)ServiceCollection).RemoveAt(index);
        }
        #endregion
    }
}

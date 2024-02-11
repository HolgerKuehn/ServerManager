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
        public object? this[int index] { get => ((IList)ServiceCollection)[index]; set => ((IList)ServiceCollection)[index] = value; }

        public bool IsFixedSize => ((IList)ServiceCollection).IsFixedSize;

        public bool IsReadOnly => ((IList)ServiceCollection).IsReadOnly;

        public int Count => ((ICollection)ServiceCollection).Count;

        public bool IsSynchronized => ((ICollection)ServiceCollection).IsSynchronized;

        public object SyncRoot => ((ICollection)ServiceCollection).SyncRoot;

        public int Add(object? value)
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

        public void CopyTo(Array array, int index)
        {
            ((ICollection)ServiceCollection).CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)ServiceCollection).GetEnumerator();
        }

        public int IndexOf(object? value)
        {
            return ((IList)ServiceCollection).IndexOf(value);
        }

        public void Insert(int index, object? value)
        {
            ((IList)ServiceCollection).Insert(index, value);
        }

        public void Remove(object? value)
        {
            ((IList)ServiceCollection).Remove(value);
        }

        public void RemoveAt(int index)
        {
            ((IList)ServiceCollection).RemoveAt(index);
        }
        #endregion
    }
}

using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using MathCore.Annotations;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable once CheckNamespace
namespace System.Collections.Generic
{
    public class ObservableLinkedList<T> : ICollection<T>, ICollection, ISerializable, IDeserializationCallback, IList<T>,
        INotifyCollectionChanged, INotifyPropertyChanged
    {
        private const string IndexerName = "Item[]";

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        protected virtual void OnCollectionItemAdded(T ChangedItem, int Index) => OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, ChangedItem, Index));
        protected virtual void OnCollectionItemRemoved(T ChangedItem, int Index) => OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, ChangedItem, Index));
        protected virtual void OnCollectionChanged(NotifyCollectionChangedAction action = NotifyCollectionChangedAction.Reset) => OnCollectionChanged(new NotifyCollectionChangedEventArgs(action));
        protected virtual void OnCollectionChanged(T NewValue, int index) => OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, NewValue, index));
        protected virtual void OnCollectionChanged(T OldValue, T NewValue, int index) => OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, NewValue, OldValue, index));
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.ThreadSafeInvoke(this, e);
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(IndexerName);
        }

        #endregion

        #region INotiftPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

        #endregion

        #region Поля

        /// <summary>Связный список элементов</summary>
        private readonly LinkedList<T> _List;

        #endregion

        #region Свойства

        /// <summary>Число элементов</summary>
        public int Count => _List.Count;

        /// <summary>Первый элемент списка</summary>
        public LinkedListNode<T> First => _List.First;

        /// <summary>Последний элемент списка</summary>
        public LinkedListNode<T> Last => _List.Last;

        bool ICollection<T>.IsReadOnly => false;

        object ICollection.SyncRoot => ((ICollection)_List).SyncRoot;

        bool ICollection.IsSynchronized => ((ICollection)_List).IsSynchronized;

        public T this[int index]
        {
            get => NodeAt(index).Value;
            set
            {
                var node = NodeAt(index);
                var old_value = node.Value;
                node.Value = value;
                OnCollectionChanged(old_value, value, index);
            }
        }

        #endregion

        #region Конструкторы

        /// <summary>Инициализация нового экземпляра <see cref="ObservableLinkedList{T}"/></summary>
        public ObservableLinkedList() => _List = new LinkedList<T>();

        /// <summary>Инициализация нового экземпляра <see cref="ObservableLinkedList{T}"/></summary>
        /// <param name="collection">Исходная последовательность элементов, добавляемая в список при инициализации</param>
        public ObservableLinkedList(IEnumerable<T> collection) => _List = new LinkedList<T>(collection);

        #endregion

        #region Методы 

        public void Add(T value) => AddLast(value);

        /// <summary>Добавить элемент после указанного элемента</summary>
        /// <param name="PrevNode">Элемент, после которого надо добавить значение в список</param>
        /// <param name="value">Добавляемое значение</param>
        /// <returns>Новый элемент списка</returns>
        public LinkedListNode<T> AddAfter(LinkedListNode<T> PrevNode, T value)
        {
            var result = _List.AddAfter(PrevNode, value);
            OnCollectionItemAdded(value, IndexOf(result));
            if (ReferenceEquals(result, _List.Last)) OnPropertyChanged(nameof(Last));
            return result;
        }

        public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> NewNode)
        {
            _List.AddAfter(node, NewNode);
            OnCollectionItemAdded(NewNode.Value, IndexOf(NewNode));
            if (ReferenceEquals(node, _List.Last)) OnPropertyChanged(nameof(Last));
        }

        public LinkedListNode<T> AddBefore(LinkedListNode<T> node, T value)
        {
            var result = _List.AddBefore(node, value);
            OnCollectionItemAdded(value, IndexOf(result));
            if (ReferenceEquals(node, _List.First)) OnPropertyChanged(nameof(First));
            return result;
        }

        public void AddBefore(LinkedListNode<T> node, LinkedListNode<T> NewNode)
        {
            _List.AddBefore(node, NewNode);
            OnCollectionItemAdded(NewNode.Value, IndexOf(NewNode));
            if (ReferenceEquals(node, _List.First)) OnPropertyChanged(nameof(First));
        }

        public LinkedListNode<T> AddFirst(T value)
        {
            var result = _List.AddFirst(value);
            OnCollectionItemAdded(value, 0);
            OnPropertyChanged(nameof(First));
            return result;
        }

        public void AddFirst(LinkedListNode<T> node)
        {
            _List.AddFirst(node);
            OnCollectionItemAdded(node.Value, 0);
            OnPropertyChanged(nameof(First));
        }

        public LinkedListNode<T> AddLast(T value)
        {
            var result = _List.AddLast(value);
            OnCollectionItemAdded(value, _List.Count - 1);
            OnPropertyChanged(nameof(Last));
            return result;
        }

        public void AddLast(LinkedListNode<T> node)
        {
            _List.AddLast(node);
            OnCollectionItemAdded(node.Value, _List.Count - 1);
            OnPropertyChanged(nameof(Last));
        }

        public void Clear()
        {
            _List.Clear();
            OnCollectionChanged();
            OnPropertyChanged(nameof(First));
            OnPropertyChanged(nameof(Last));
        }

        public int IndexOf(LinkedListNode<T> node)
        {
            var root = _List.First;
            for (var n = 0; root != null; root = root.Next, n++)
                if (node == root) return n;
            return -1;
        }

        public int IndexOf(T value)
        {
            var node = _List.First;
            for (var n = 0; node != null; node = node.Next, n++)
                if (Equals(node.Value, value)) return n;
            return -1;
        }

        public int IndexOf(T value, out LinkedListNode<T> node)
        {
            node = _List.First;
            for (var n = 0; node != null; node = node.Next, n++)
                if (Equals(node.Value, value)) return n;
            node = null;
            return -1;
        }

        public LinkedListNode<T> NodeAt(int index)
        {
            var count = Count;
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException(nameof(index), @"Индекс вышел за границы списка");
            LinkedListNode<T> node;
            if (index > count / 2)
            {
                node = _List.Last;
                while (++index < count) node = node.Previous;
            }
            else
            {
                node = _List.First;
                while (index-- > 0) node = node.Next;
            }
            return node;
        }

        public bool Contains(T value) => _List.Contains(value);

        public void CopyTo(T[] array, int index) => _List.CopyTo(array, index);

        public void CopyTo(Array array, int index) => ((ICollection)_List).CopyTo(array, index);

        public bool LinkedListEquals(object obj) => _List.Equals(obj);

        public LinkedListNode<T> Find(T value) => _List.Find(value);

        public LinkedListNode<T> FindLast(T value) => _List.FindLast(value);

        public bool Remove(T value)
        {
            var index = IndexOf(value, out var node);
            var is_first = ReferenceEquals(_List.First, node);
            var is_last = ReferenceEquals(_List.Last, node);
            if (!_List.Remove(value)) return false;
            OnCollectionItemRemoved(value, index);
            if (is_first) OnPropertyChanged(nameof(First));
            if (is_last) OnPropertyChanged(nameof(Last));
            return true;
        }

        public void Remove([NotNull] LinkedListNode<T> node)
        {
            if (node is null) throw new ArgumentNullException(nameof(node));
            var index = IndexOf(node);
            var is_first = ReferenceEquals(_List.First, node);
            var is_last = ReferenceEquals(_List.Last, node);
            _List.Remove(node);
            OnCollectionItemRemoved(node.Value, index);
            if (is_first) OnPropertyChanged(nameof(First));
            if (is_last) OnPropertyChanged(nameof(Last));
        }

        public void RemoveFirst()
        {
            if (_List.Count == 0) return;
            var item = _List.First.Value;
            _List.RemoveFirst();
            OnCollectionItemRemoved(item, 0);
            OnPropertyChanged(nameof(First));
        }

        public void RemoveLast()
        {
            var count = _List.Count;
            if (count == 0) return;
            var item = _List.First.Value;
            _List.RemoveLast();
            OnCollectionItemRemoved(item, count);
            OnPropertyChanged(nameof(Last));
        }

        public Type GetLinkedListType() => _List.GetType();

        #endregion

        #region IEnumerable

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => (_List as IEnumerable<T>).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => (_List as IEnumerable).GetEnumerator();

        #endregion

        /// <inheritdoc />
        public void GetObjectData(SerializationInfo info, StreamingContext context) => _List.GetObjectData(info, context);

        /// <inheritdoc />
        public void OnDeserialization(object sender) => _List.OnDeserialization(sender);

        public void Insert(int index, T item) => AddAfter(NodeAt(index), item);

        public void RemoveAt(int index) => Remove(NodeAt(index));
    }
}
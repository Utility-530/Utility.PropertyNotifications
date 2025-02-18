using AnyClone;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Utility.Interfaces.Generic;
using Utility.Trees.Abstractions;

namespace Utility.Trees
{
    /// <summary>
    /// <a href="https://github.com/yuramag/ObservableTreeDemo"></a>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Tree<T> : Tree, ITree<T> /*where T : class*/
    {
        private T data;

        public Tree(T data, params object[] items) : base(data, items)
        {
            this.data = data;
            if (items.Any())
                Add(items);
        }

        public Tree(params object[] items) : base(null, items)
        {
            if (items.Any())
                Add(items);
        }

        public int Id { get; private set; }

        //public ITree<T>? this[T item]
        //{
        //    get
        //    {
        //        var x = TreeHelper.Match(this, a => a.Data?.Equals(item) == true);
        //        if (x == null)
        //        {
        //            throw new Exception("4sd ss");
        //            //x = new Tree<T>(item);
        //            //this.Add(x);
        //        }
        //        return x;
        //    }
        //    set
        //    {
        //        throw new NotImplementedException("rfgf 3422");
        //    }
        //}

        //public new ITree<T>? this[Guid key]
        //{
        //    get
        //    {
        //        var x = TreeHelper.Match(this, a => a.Key.Equals(key) == true);
        //        if (x == null)
        //        {
        //            throw new Exception("4sd ss");
        //            //x = new Tree<T>(item);
        //            //this.Add(x);
        //        }
        //        return x;
        //    }
        //    set
        //    {
        //        var x = TreeHelper.Match(this, a => a.Key.Equals(key) == true);
        //        if (x == null)
        //        {
        //            throw new Exception("4sd ss");
        //            //x = new Tree<T>(item);
        //            //this.Add(x);
        //        }
        //        x.Parent[x.Key] = value;
        //    }
        //}

        public new ITree<T> this[int index]
        {
            get
            {
                return m_items.Count == 0 ? throw new Exception("vfd4") : m_items[index] as ITree<T>;
            }
            set
            {
                if (m_items.Count == 0)
                    throw new Exception(" rere4");
                else
                    m_items[index] = value;
            }
        }

        public void Add(T data)
        {
            if (data == null)
                return;

            var tree = new Tree<T>(data) { Id = m_items.Count };

            m_items.Add(tree);
            return;
        }

        void IAdd<ITree<T>>.Add(ITree<T> tree)
        {
            m_items.Add(tree);
            return;
        }


        public override void Add(object data)
        {
            if (data == null)
                return;

            if (data is ITree<T> tree)
            {
                m_items.Add((tree));
                return;
            }


            if (data is T t)
            {
                m_items.Add(new Tree<T>(t));
                return;
            }
            throw new InvalidOperationException("Cannot add unknown content type.");
        }


        public override Task<ITree> Add()
        {
            var data = this.Data.Clone();
            ITree tree = new Tree<T>(data);
            this.m_items.Add(tree);
            tree.Parent = this;
            return Task.FromResult(tree);
        }

        public void Remove(T data)
        {
            var single = m_items.OfType<ITree<T>>().Single(a => a.Data.Equals(data));
            m_items.Remove(single);
            return;
        }

        public override object Data { get => data; set => data = (T)value; }

        //public override ITree<T> Parent { get => (ITree<T>)parent; set => parent = value; }

        public new IReadOnlyList<ITree<T>> Items
        {
            get
            {
                var x = m_items as ObservableCollection<ITree<T>>;
                return x;
            }
        }


        protected override IList CreateChildren()
        {
            var m_items = new ObservableCollection<ITree<T>>();
            m_items.CollectionChanged += ItemsOnCollectionChanged;
            return m_items;
        }

        private void ResetOnCollectionChangedEvent()
        {
            if (m_items != null)
                (m_items as ObservableCollection<ITree<T>>).CollectionChanged -= ItemsOnCollectionChanged;
        }

        private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add && args.NewItems != null)
            {
                foreach (var item in args.NewItems.Cast<Tree<T>>())
                {
                    item.Parent = this;
                }
            }
            else if (args.Action != NotifyCollectionChangedAction.Move && args.OldItems != null)
            {
                foreach (var item in args.OldItems.Cast<Tree<T>>())
                {
                    item.Parent = null;
                    item.ResetOnCollectionChangedEvent();
                }
            }
        }

        public override IEnumerator<ITree> GetEnumerator()
        {
            return m_items == null ? Enumerable.Empty<ITree<T>>().GetEnumerator() : Items.GetEnumerator();
        }

        public int IndexOf(ITree<T> tree)
        {
            int i = 0;
            foreach (var item in m_items)
            {
                if (item.Equals(tree))
                    return i;
                i++;
            }
            return -1;
        }

        IEnumerator<ITree<T>> IEnumerable<ITree<T>>.GetEnumerator()
        {
            return m_items == null ? Enumerable.Empty<ITree<T>>().GetEnumerator() : Items.GetEnumerator();
        }


    }
}
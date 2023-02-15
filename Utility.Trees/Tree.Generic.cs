using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Utility.Trees
{

    /// <summary>
    /// <a href="https://github.com/yuramag/ObservableTreeDemo"></a>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Tree<T> : Tree, ITree<T> /*where T : class*/
    {
        //private ObservableCollection<ITree<T>> m_items;
        private T data;


        public Tree(T data) : this(data, null)
        {
        }

        public Tree(T data, params object[] items) : base(data, items)
        {
            this.data = data;
            Add(items);
        }

        public Tree(T data, params ITree<T>[] items) : base(data, items)
        {
            this.data = data;
            Add(items);
        }


        public ITree<T>? this[T item]
        {
            get
            {
                var x = TreeHelper.Match(this, a => a.Data?.Equals(item) == true);
                if (x == null)
                {
                    throw new Exception("4sd ss");
                    //x = new Tree<T>(item);
                    //this.Add(x);
                }
                return x;
            }
            set
            {
                throw new NotImplementedException("rfgf 3422");
            }
        }


        public new ITree<T>? this[Guid key]
        {
            get
            {
                var x = TreeHelper.Match(this, a => a.Key.Equals(key) == true);
                if (x == null)
                {
                    throw new Exception("4sd ss");
                    //x = new Tree<T>(item);
                    //this.Add(x);
                }
                return x;
            }
            set
            {
                var x = TreeHelper.Match(this, a => a.Key.Equals(key) == true);
                if (x == null)
                {
                    throw new Exception("4sd ss");
                    //x = new Tree<T>(item);
                    //this.Add(x);
                }
                x.Parent[x.Key] = value;
            }
        }

        public new ITree<T> this[int index]
        {
            get
            {
                return m_items.Count == 0 ? this : m_items[index] as ITree<T>;
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

            var tree = new Tree<T>(data);

            m_items.Add(tree);
            return;


       
        }

        public override void Add(object data)
        {
            if (data == null)
                return;


            if (data is ITree<T> tree)
            {
                m_items.Add(CloneTree(tree));
                return;
            }

            if (data is ITree )
            {
                throw new Exception("t 44 redsdssd");
            }

            if (data is T t)
            {
                m_items.Add(CloneNode(new Tree(t)));
                return;
            }

            //var o = data as object[];
            //if (o != null)
            //{
            //    foreach (var obj in o)
            //        Add(obj);
            //    return;
            //}

            //var e = data as IEnumerable;
            //if (e != null && !(data is ITree))
            //{
            //    foreach (var obj in e)
            //        Add(obj);
            //    return;
            //}

            throw new InvalidOperationException("Cannot add unknown content type.");
        }

        public void Remove(T data)
        {
            var single = m_items.OfType<ITree<T>>().Single(a => a.Data.Equals(data));
            m_items.Remove(single);
            return;
        }

        public ITree<T> CloneTree(ITree<T> item)
        {
            var result = CloneNode(item);
            if (item.HasItems)
                result.Add(item.Items);
            return result;
        }

        protected virtual ITree<T> CloneNode(ITree<T> item)
        {
            return new Tree<T>(item.Data);
        }


        public new T Data { get => data; private set => data = value; }


        public new ITree<T> Parent { get; private set; }


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

        public new IEnumerable<ITree<T>> GetParents(bool includingThis)
        {
            if (includingThis)
                yield return this;

            var parent = Parent;
            while (parent != null)
            {
                yield return parent;
                parent = parent.Parent;
            }
        }

        public new IEnumerable<ITree<T>> GetChildren(bool includingThis)
        {
            if (includingThis)
                yield return this;

            if (m_items != null)
                foreach (var child in m_items.OfType<ITree>().SelectMany(item => item.GetChildren(true)))
                    yield return child as ITree<T>;
        }

        public override IEnumerator<ITree<T>> GetEnumerator()
        {
            return m_items == null ? Enumerable.Empty<ITree<T>>().GetEnumerator() : Items.GetEnumerator();
        }

        public int IndexOf(ITree<T> tree)
        {

            return this.m_items.IndexOf(tree);

        }

        ITree<T> ITree<T>.CloneTree()
        {
            throw new NotImplementedException();
        }
    }

    public static class TreeHelper
    {
        public static ITree<T> Create<T>(T data)
        {
            return new Tree<T>(data);
        }

        public static ITree<T> Create<T>(T data, params ITree<T>[] items)
        {
            return new Tree<T>(data, items);
        }

        public static ITree<T> Create<T>(T data, params object[] items)
        {
            return new Tree<T>(data, items);
        }

        public static void Visit<T>(ITree<T> tree, Action<ITree<T>> action)
        {
            action(tree);
            if (tree.HasItems)
                foreach (var item in tree as IEnumerable<ITree<T>>)
                    Visit(item, action);
        }

        public static ITree<T>? Match<T>(ITree<T> tree, Predicate<ITree<T>> action)
        {
            if (action(tree))
            {
                return tree;
            }
            else if (tree.HasItems)
            {
                foreach (var item in tree as IEnumerable<ITree<T>>)
                {
                    if (Match(item, action) is ITree<T> sth)
                    {
                        return sth;
                    }
                }

            }


            return null;

        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Utility.Trees
{



    public abstract class Tree : ITree
    {
        public abstract bool HasItems { get; }

        public abstract IList Items { get; }

        public abstract object Data { get; }

        public abstract void Add(object data);
    }


    /// <summary>
    /// <a href="https://github.com/yuramag/ObservableTreeDemo"></a>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Tree<T> : Tree, ITree<T>
    {
        private ObservableCollection<ITree<T>> m_items;
        private T data;


        public Tree(T data)
            : this(data, null)
        {
        }

        public Tree(T data, params object[] items)
        {
            this.data = data;
            Add(items);
        }

        public Tree(T data, params ITree<T>[] items)
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

        public override void Add(object data)
        {
            if (data == null)
                return;

            if (data is T)
            {
                Items.Add(CloneNode(new Tree<T>((T)data)));
                return;
            }

            var t = data as ITree<T>;
            if (t != null)
            {
                Items.Add(CloneTree(t));
                return;
            }

            var o = data as object[];
            if (o != null)
            {
                foreach (var obj in o)
                    Add(obj);
                return;
            }

            var e = data as IEnumerable;
            if (e != null && !(data is ITree))
            {
                foreach (var obj in e)
                    Add(obj);
                return;
            }

            throw new InvalidOperationException("Cannot add unknown content type.");
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
            return new Tree<T>(item.GenericData);
        }

        public override object Data { get => data; }

        public T GenericData { get => data; private set => data = value; }

        public override bool HasItems
        {
            get { return m_items != null && m_items.Count > 0; }
        }

        public ITree<T> Parent { get; private set; }


        public override IList Items
        {
            get
            {
                if (m_items == null)
                {
                    m_items = new ObservableCollection<ITree<T>>();
                    m_items.CollectionChanged += ItemsOnCollectionChanged;
                }
                return m_items;
            }
        }

        private void ResetOnCollectionChangedEvent()
        {
            if (m_items != null)
                m_items.CollectionChanged -= ItemsOnCollectionChanged;
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

        public IEnumerable<ITree<T>> GetParents(bool includingThis)
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

        public IEnumerable<ITree<T>> GetChildren(bool includingThis)
        {
            if (includingThis)
                yield return this;

            if (m_items != null)
                foreach (var child in m_items.SelectMany(item => item.GetChildren(true)))
                    yield return child;
        }

        public IEnumerator<ITree<T>> GetEnumerator()
        {
            return m_items == null ? Enumerable.Empty<ITree<T>>().GetEnumerator() : m_items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            if (typeof(T).IsValueType)
                return Data.ToString();
            return EqualityComparer<T>.Default.Equals(GenericData, default) ? string.Empty : Data.ToString();
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
                foreach (var item in tree)
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
                foreach (var item in tree)
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
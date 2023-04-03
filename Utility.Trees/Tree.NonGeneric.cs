using Jellyfish;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Trees
{
    public static class TreeHelper2
    {
        public static ITree Create(object data)
        {
            return new Tree(data);
        }

        public static ITree Create(object data, params ITree[] items)
        {
            return new Tree(data, items);
        }

        public static ITree Create(object data, params object[] items)
        {
            return new Tree(data, items);
        }

        public static void Visit(ITree tree, Action<ITree> action)
        {
            action(tree);
            if (tree.HasItems)
                foreach (var item in tree)
                    Visit(item, action);
        }

        public static ITree? Match(ITree tree, Predicate<ITree> action)
        {
            if (action(tree))
            {
                return tree;
            }

            foreach (var item in tree)
            {
                if (Match(item, action) is ITree sth)
                {
                    return sth;
                }
            }

            return null;

        }
    }

    /// <summary>
    /// <a href="https://github.com/yuramag/ObservableTreeDemo"></a>
    /// </summary>
    /// <typeparam name="object"></typeparam>
    public class Tree : ViewModel, ITree, IEnumerable<ITree>
    {
        private IList items;
        private object data;

        protected ITree parent;
        bool flag;
        private State state;

        protected IList m_items
        {
            get
            {
                if (flag == false)
                {
                    items = CreateChildren();
                    flag = true;
                }
                return items;
            }
        }


        public Tree(object data, params object[] items)
        {
            this.data = data;
            if (items.Any())
                Add(items);
        }

        public Guid Key { get; set; }

        public ITree? this[object item]
        {
            get
            {
                var x = TreeHelper2.Match(this, a => a.Data?.Equals(item) == true);
                if (x == null)
                {
                    throw new Exception("4sd ss");
                    //x = new Tree(item);
                    //this.Add(x);
                }
                return x;
            }
            set
            {
                throw new NotImplementedException("rfgf 3422");
            }
        }


        public new ITree? this[Guid key]
        {
            get
            {
                var x = TreeHelper2.Match(this, a => a.Key.Equals(key) == true);
                if (x == null)
                {
                    throw new Exception("4sd ss");
                    //x = new Tree(item);
                    //this.Add(x);
                }
                return x;
            }
            set
            {
                var x = TreeHelper2.Match(this, a => a.Key.Equals(key) == true);
                if (x == null)
                {
                    throw new Exception("4sd ss");
                    //x = new Tree(item);
                    //this.Add(x);
                }
                x.Parent[x.Key] = value;
            }
        }

        public new ITree this[int index]
        {
            get
            {
                return m_items.Count == 0 ? throw new Exception(" 434 ") : m_items[index] as ITree;
            }
            set
            {
                if (m_items.Count == 0)
                    throw new Exception(" rere4");
                else
                    m_items[index] = value as ITree;
            }
        }





        public virtual void Add(object data)
        {
            if (data == null)
                return;

            if (data is ITree tree)
            {
                m_items.Add(tree);
                return;
            }
            if (data is IEnumerable<ITree> treeCollection)
            {
                foreach (var item in treeCollection)
                    m_items.Add(item);
                return;
            }
            if (data is not null)
            {
                m_items.Add(new Tree((object)data));
                return;
            }

            throw new Exception("t 44 redsdssd");
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

        public void Remove(object data)
        {
            if (data == null)
                return;


            if (data is Guid key)
            {
                var single = m_items.OfType<ITree>().Single(t => t.Key == key);
                m_items.Remove(single);
                return;
            }

            if (data is ITree tree)
            {
                var single = m_items.OfType<ITree>().Single(t => t == tree);
                m_items.Remove(single);
                return;
            }

            if (data is not null)
            {
                var single = m_items.OfType<ITree>().Single(t => t.Data == data);
                m_items.Remove(single);
                return;
            }


            throw new Exception("t 44 redsdssd");
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

        public ITree CloneTree()
        {
            var result = CloneNode(this);
            if (this.HasItems)
                result.Add(this.Items);
            return result;
        }

        protected virtual ITree CloneNode(ITree item)
        {
            return new Tree(item.Data);
        }


        public object Data { get => data; private set => data = value; }

        public virtual bool HasItems
        {
            get { return m_items != null && m_items.Count > 0; }
        }

        public ITree Parent { get => parent; set => parent = value; }


        public IReadOnlyList<ITree> Items
        {
            get
            {

                if (m_items is IReadOnlyList<ITree> list)
                {
                    return list;
                }
                else
                {
                    return m_items.Cast<ITree>().ToArray();
                }
            }
        }

        public State State { get => state; set => Set(ref state, value); }

        public Index Index
        {
            get
            {
                var indices = Indices();
                return new Index { Collection = indices.Reverse().ToArray() };
                IEnumerable<int> Indices()
                {
                    ITree parent = this.Parent;
                    ITree child = this;
                    while (parent != null)
                    {
                        yield return parent.IndexOf(child);
                        child = parent;
                        if (parent.Parent == parent)
                            throw new Exception("r sdfsd3232 bf");
                        parent = parent.Parent;
                    }
                }
            }
        }

        private void ResetOnCollectionChangedEvent()
        {
            if (m_items != null)
                (m_items as ObservableCollection<ITree>).CollectionChanged -= ItemsOnCollectionChanged;
        }

        private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add && args.NewItems != null)
            {
                foreach (var item in args.NewItems.Cast<Tree>())
                {
                    item.Parent = this;
                }
            }
            else if (args.Action != NotifyCollectionChangedAction.Move && args.OldItems != null)
            {
                foreach (var item in args.OldItems.Cast<Tree>())
                {
                    item.Parent = null;
                    item.ResetOnCollectionChangedEvent();
                }
            }
        }

        public IEnumerable<ITree> GetParents(bool includingThis)
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

        public IEnumerable<ITree> GetChildren(bool includingThis)
        {
            if (includingThis)
                yield return this;

            if (m_items != null)
                foreach (var child in m_items.OfType<ITree>().SelectMany(item => item.GetChildren(true)))
                    yield return child;
        }

        public virtual IEnumerator<ITree> GetEnumerator()
        {
            return m_items == null ? Enumerable.Empty<ITree>().GetEnumerator() : m_items.OfType<ITree>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Equals(ITree? other)
        {
            return other?.Key.Equals(this.Key) == true;
        }

        public void Remove(Guid index)
        {
            var single = m_items.OfType<ITree>().Single(t => t.Key == index);
            m_items.Remove(single);
        }


        protected virtual IList CreateChildren()
        {
            var collection = new ObservableCollection<ITree>();
            collection.CollectionChanged += ItemsOnCollectionChanged;
            return collection;
        }

        public int IndexOf(ITree tree)
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

        public override string ToString()
        {
            if (Data.GetType().IsValueType)
                return Data?.ToString();
            return Data == default ? string.Empty : Data.ToString();
        }
    }

}

using AnyClone;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Trees.Abstractions;

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

        public static void Visit(this ITree tree, Action<ITree> action)
        {
            action(tree);
            if (tree.HasItems)
                foreach (var item in tree)
                    Visit(item, action);
        }

        public static void VisitAncestors(this ITree tree, Action<ITree> action)
        {
            action(tree);
            if (tree.Parent is ITree parent)
                parent.VisitAncestors(action);
        }


        public static IReadOnlyTree? MatchAncestor(this IReadOnlyTree tree, Predicate<IReadOnlyTree> action)
        {
            if (action(tree))
            {
                return tree;
            }

            if (tree.Parent is IReadOnlyTree parent)
            {
                return parent.MatchAncestor(action);
            }


            return null;
        }


        public static IReadOnlyTree? MatchDescendant(this IReadOnlyTree tree, Predicate<IReadOnlyTree> action)
        {
            if (action(tree))
            {
                return tree;
            }
            List<IReadOnlyTree> trees = new();
            foreach (var child in tree.Items)
                if (child is IReadOnlyTree tChild)
                {
                    if (action(tChild))
                    {
                        return tChild;
                    }
                    else
                        trees.Add(tChild);
                }
                else
                    throw new Exception("c 333211");
                

            foreach(var c in trees)
            {
                 if (c.MatchDescendant(action) is { } match)
                    return match;
            }
            return null;
        }

        public static bool IsRoot(this ITree tree)
        {
            return tree.Index.IsEmpty;
        }

        public static IEnumerable<IReadOnlyTree> Ancestors(this IReadOnlyTree tree)
        {
            IReadOnlyTree parent = tree.Parent;
            while (parent != null)
            {
                yield return parent;
                parent = parent.Parent;
            }
        }

        public static IEnumerable<IReadOnlyTree> MatchAncestors(this IReadOnlyTree tree, Predicate<IReadOnlyTree> predicate)
        {
            if (predicate(tree))
            {
                yield return tree;
            }
            IReadOnlyTree parent = tree.Parent;
            while (parent != null)
            {
                if (predicate(parent))
                    yield return parent;
                parent = parent.Parent;
            }
        }


        //public static IEnumerable<IReadOnlyTree> MatchDescendants(this IReadOnlyTree tree, Predicate<IReadOnlyTree> action)
        //{
        //    if (action(tree))
        //    {
        //        return tree;
        //    }

        //    foreach (var child in tree.Items)
        //        if (child is IReadOnlyTree tChild)
        //        {
        //            if (action(tChild))
        //            {
        //                return tChild;
        //            }
        //            else if (tChild.MatchDescendant(action) is { } match)
        //                return match;
        //        }
        //        else
        //            throw new Exception("c 333211");



        //    return null;
        //}

    }



    /// <summary>
    /// <a href="https://github.com/yuramag/ObservableTreeDemo"></a>
    /// </summary>
    /// <typeparam name="object"></typeparam>
    public class Tree : ITree, IEnumerable<ITree>, INotifyPropertyChanged, IEquatable
    {
        private IList items;
        protected ITree? parent;
        private bool flag;
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

        public Tree(object? data = null, params object[] items)
        {
            if (data != null)
                this.Data = data;
            //else
            //    this.data = "root";
            if (items.Any())
                Add(items);
        }

        public virtual IEquatable Key { get; set; }



        public ITree? this[object key]
        {
            get
            {
                var match = TreeHelper2.MatchDescendant(this, a => key.Equals(a) == true) as ITree;
                if (match is null)
                    return TreeHelper2.MatchAncestor(this, a => key.Equals(a) == true) as ITree;
                return match;
            }
            set
            {
                var x = TreeHelper2.MatchDescendant(this, a => key.Equals(a) == true);
                if (x is not ITree match)
                {
                    throw new Exception("4sd ss");
                    //x = new Tree(item);
                    //this.Add(x);
                }
                match.Parent[x.Key] = value;
            }
        }

        public virtual ITree? this[int index]
        {
            get
            {
                return m_items.Count == 0 ? null : m_items[index] as ITree;
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
            if (data is IEnumerable treeCollection)
            {
                foreach (var item in treeCollection)
                    if (item is ITree)
                        m_items.Add(item);
                    else
                        m_items.Add(new Tree(item));
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

        public virtual void Remove(object data)
        {
            if (data == null)
                return;

            if (data is IEquatable key)
            {
                var single = m_items.OfType<ITree>().Single(t => t.Key.Equals(key));
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

        protected virtual object Clone(object data)
        {
            if (data is IClone clone)
            {
                return clone.Clone();
            }
            return data.Clone();
        }

        public virtual ITree Add()
        {
            var data = Clone(this.Data);
            var tree = new Tree(data);
            this.items.Add(tree);
            tree.Parent = this;
            return tree;
        }

        public virtual ITree Remove()
        {
            this.Parent = null;
            this.Parent?.Remove(this);
            return this.Parent;
        }

        public virtual object Data { get; set; }

        public virtual bool HasItems => Items != null && Items.Count() > 0;

        public ITree? Parent { get => parent; set => parent = value; }

        public virtual IEnumerable Items
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

        public State State
        {
            get => state;
            set { state = value; OnPropertyChanged(); }
        }

        public Index Index
        {
            get
            {
                var indices = Indices();
                return new Index(indices.Reverse().ToArray());
                IEnumerable<int> Indices()
                {
                    ITree? parent = this.Parent;
                    ITree child = this;
                    while (parent != null)
                    {
                        yield return parent.IndexOf(child);
                        child = parent;
                        if (parent.Parent == parent)
                            throw new Exception("r sdfsd3232 bf");
                        parent = parent.Parent;
                    }
                    yield return 0;
                }
            }
        }

        public int Depth
        {
            get
            {
                ITree parent = this;
                int depth = 0;
                while (parent.Parent != null)
                {
                    depth++;
                    parent = parent.Parent;
                }
                return depth;
            }
        }

        IReadOnlyTree IReadOnlyTree.Parent { get => Parent; set => Parent = value as ITree; }

        private void ResetOnCollectionChangedEvent()
        {
            if (m_items != null)
                ((ObservableCollection<ITree>)m_items).CollectionChanged -= ItemsOnCollectionChanged;
        }

        private void ItemsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
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
            return other?.Key?.Equals(this.Key) == true;
        }

        public void Remove(Guid index)
        {
            var single = m_items.OfType<ITree>().Single(t => t.Key.Equals(index));
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
            foreach (var item in Items)
            {
                if (item.Equals(tree))
                    return i;
                i++;
            }
            return -1;
        }

        public override string ToString()
        {
            if (Data?.GetType().IsValueType == true)
                return Data?.ToString();
            return Data == default ? this.GetType().Name : Data.ToString();
        }

        #region propertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        #endregion propertyChanged

        bool IEquatable<IReadOnlyTree>.Equals(IReadOnlyTree? other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IEquatable? other)
        {
            return this.Equals(other as ITree);
        }
    }
}
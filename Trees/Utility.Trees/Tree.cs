using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyNotifications;
using Utility.Trees.Abstractions;

namespace Utility.Trees
{
    /// <summary>
    /// <a href="https://github.com/yuramag/ObservableTreeDemo"></a>
    /// </summary>
    /// <typeparam name="object"></typeparam>
    public class Tree : NotifyPropertyClass, ITree, IEquatable, IParent<ITree>
    {
        private IList items;
        protected ITree? parent;
        private bool flag;
        private string key;
        private bool? hasItems;

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        protected IList m_items
        {
            get
            {
                if (flag == false)
                {
                    items = CreateChildren();
                    if (items is INotifyCollectionChanged cc)
                    {
                        cc.CollectionChanged += ItemsOnCollectionChanged;
                    }
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

        public virtual string Key
        {
            get => key; set
            {
                if (key != value)
                {
                    var previous = this.key;
                    key = value;
                    this.RaisePropertyChanged(previous, value);
                }

            }
        }



        //public ITree? this[object key]
        //{
        //    get
        //    {
        //        if (TreeHelper2.MatchDescendant(this, a => key.Equals(a) == true) is not ITree match)
        //            return TreeHelper2.MatchAncestor(this, a => key.Equals(a) == true) as ITree;
        //        return match;
        //    }
        //    set
        //    {
        //        var x = TreeHelper2.MatchDescendant(this, a => key.Equals(a) == true);
        //        if (x is not ITree match)
        //        {
        //            throw new Exception("4sd ss");
        //            //x = new Tree(item);
        //            //this.Add(x);
        //        }
        //        match.Parent[x.Key] = value;
        //    }
        //}

        public virtual Task<ITree> ToTree(object data)
        {
            return Task.FromResult((ITree)new Tree((object)data));
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



        public virtual async void Add(object data)
        {
            if (data == null)
                return;

            if (data is ITree tree)
            {
                m_items.Add(tree);
                tree.Parent ??= this;
                return;
            }
            if (data is IEnumerable treeCollection)
            {
                foreach (var item in treeCollection)
                    if (item is ITree)
                        Add(item);
                    else
                        Add(await ToTree(item));
                return;
            }
            if (data is not null)
            {
                if (this.All(a => a.Data.Equals(data)==false || a.Data != data))
                {
                    Add(await ToTree(data));
                }
                else
                {

                }
                return;
            }

            throw new InvalidOperationException("Cannot add unknown content type.");
        }

        public virtual void Remove(object data)
        {
            if (data == null)
                return;

            if (data is IEquatable key)
            {
                var single = m_items.OfType<IReadOnlyTree>().Single(t => t.Equals(key));
                m_items.Remove(single);
                return;
            }

            if (data is ITree tree)
            {
                var single = m_items.OfType<IReadOnlyTree>().Single(t => t == tree);
                m_items.Remove(single);
                return;
            }

            if (data is not null)
            {
                var single = m_items.OfType<IReadOnlyTree>().Single(t => t.Data == data);
                m_items.Remove(single);
                return;
            }

            throw new InvalidOperationException("Cannot add unknown content type.");
        }

        public virtual void Clear()
        {
            m_items.Clear();
        }


        public virtual async Task<Tree> AsyncClone()
        {
            object clone = Data;
            if (Data is IClone cln)
            {
                clone = cln.Clone();
            }
            if (Data is IAsyncClone asyncClone)
            {
                clone = await asyncClone.AsyncClone();
            }

            var cloneTree = await ToTree(clone) as Tree;
            cloneTree.Key = this.Key;
            return cloneTree;
        }

        //public virtual async Task<ITree> Add()
        //{
        //    object clone = Data;
        //    if (Data is IClone cln)
        //    {
        //        clone = cln.Clone();
        //    }

        //    var tree = await ToTree(clone);
        //    this.items.Add(tree);
        //    tree.Parent = this;
        //    return tree as ITree;
        //}

        public virtual Task<ITree> Remove()
        {
            this.Parent = null;
            this.Parent?.Remove(this);
            return Task.FromResult(this.Parent);
        }

        public void Remove(Guid index)
        {
            var single = m_items.OfType<ITree>().Single(t => t.Key.Equals(index));
            m_items.Remove(single);
        }

        public virtual object Data { get; set; }

        public virtual bool HasItems { get => hasItems ?? Items != null && Items.Count() > 0; set => hasItems = value; }

        public virtual ITree? Parent { get => parent; set => parent = value; }

        public virtual IEnumerable Items
        {
            get
            {
                return m_items;
            }
        }


        public IIndex Index
        {
            get
            {
                var indices = Indices();
                return new Index(indices.Reverse().ToArray());
                IEnumerable<int> Indices()
                {
                    IReadOnlyTree? parent = this.Parent;
                    ITree child = this;
                    while (parent is ITree _parent)
                    {
                        yield return _parent.IndexOf(child);
                        child = _parent;
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
                IReadOnlyTree parent = this;
                int depth = 0;
                while (parent.Parent != null)
                {
                    depth++;
                    parent = parent.Parent;
                }
                return depth;
            }
        }

        public virtual Task<bool> HasMoreChildren() => Task.FromResult(false);

        IReadOnlyTree IParent<IReadOnlyTree>.Parent { get => Parent; set => Parent = value as ITree; }

        object IValue.Value => Data;

        Type IType.Type => Data?.GetType();

        private void ResetOnCollectionChangedEvent()
        {
            if (m_items != null)
                ((INotifyCollectionChanged)m_items).CollectionChanged -= ItemsOnCollectionChanged;
        }

        protected virtual void ItemsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add && args.NewItems != null)
            {
                foreach (var item in args.NewItems.Cast<Tree>())
                {
                    if (item.Parent == null)
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
            this.InvokeCollectionChanged(sender, args);
        }

        protected void InvokeCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
        {
            this.CollectionChanged?.Invoke(sender, args);

        }

        public virtual IEnumerator<ITree> GetEnumerator()
        {
            return m_items == null ? Enumerable.Empty<ITree>().GetEnumerator() : m_items.OfType<ITree>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        protected virtual IList CreateChildren()
        {
            var collection = new ObservableCollection<ITree>();
            return collection;
        }


        public override string ToString()
        {
            if (Data?.GetType().IsValueType == true)
                return Data?.ToString();
            return Data == default ? this.GetType().Name : Data.ToString();
        }

        public virtual bool Equals(ITree? other)
        {
            return other?.Key?.Equals(this.Key) == true;
        }


        bool IEquatable<IReadOnlyTree>.Equals(IReadOnlyTree? other)
        {
            return other?.Key?.Equals(this.Key) == true;
        }

        public virtual bool Equals(IEquatable? other)
        {
            return other?.Equals(this.Key) == true;
        }

        public int Count => m_items.Count;
        public virtual bool IsReadOnly { get; set; }

        public void RemoveAt(int index)
        {
            m_items.RemoveAt(index);
        }


        public bool Contains(ITree item)
        {
            return m_items.Contains(item);
        }

        public void CopyTo(ITree[] array, int arrayIndex)
        {
            m_items.CopyTo(array, arrayIndex);
        }

        public bool Remove(ITree item)
        {
            m_items.Remove(item);
            return true;
        }

        void ICollection<ITree>.Add(ITree item)
        {
            this.Add(item);
        }

        async Task<object> IAsyncClone.AsyncClone()
        {
            return await this.AsyncClone();
        }
    }

    internal static class TreeHelper
    {
        public static int IndexOf(this IReadOnlyTree tree, IReadOnlyTree _item)
        {
            int i = 0;
            foreach (var item in tree.Items)
            {
                if (item.Equals(_item))
                    return i;
                i++;
            }
            return -1;
        }

    }
}
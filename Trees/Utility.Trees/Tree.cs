using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyNotifications;
using Utility.Trees.Abstractions;
using IIndex = Utility.Interfaces.Trees.IIndex;

namespace Utility.Trees
{
    /// <summary>
    /// <a href="https://github.com/yuramag/ObservableTreeDemo"></a>
    /// </summary>
    /// <typeparam name="object"></typeparam>
    public class Tree : NotifyPropertyClass, ITree, IEquatable
    {
        private IList items;
        private IReadOnlyTree? parent;
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

        public virtual Task<ITree> ToTree(object data)
        {
            if (data is ITree)
                throw new Exception("sd 3333333333311");
            var tree = (ITree)new Tree((object)data);
            this.Add(tree);
            (tree as ISetParent<IReadOnlyTree>).Parent = this;
            return Task.FromResult(tree);
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
                if ((tree as IGetParent<IReadOnlyTree>).Parent == null)
                    (tree as ISetParent<IReadOnlyTree>).Parent = this;
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
                if (this.All(a => (a as IGetData).Data.Equals(data) == false || (a as IGetData).Data != data))
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
                var single = m_items.OfType<IReadOnlyTree>().Single(t => (t as IGetData).Data == data);
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

        public virtual Task<IReadOnlyTree> Remove()
        {
            this.Parent = null;
            (this.Parent as ITree)?.Remove(this);
            return Task.FromResult(this.Parent);
        }

        public void Remove(Guid index)
        {
            var single = m_items.OfType<ITree>().Single(t => (t as IGetKey).Key.Equals(index));
            m_items.Remove(single);
        }

        public virtual object? Value { get; set; }

        public virtual object Data { get; set; }

        public virtual bool HasChildren { get => hasItems ?? Children != null && Children.Count() > 0; }

        public virtual IReadOnlyTree Parent
        {
            get => parent;
            set
            {
                if (parent != value)
                {
                    var previous = parent;
                    parent = value;
                    RaisePropertyChanged(previous, value);
                }
            }
        }


        public virtual IEnumerable Children
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
                return new Utility.Structs.Index(indices.Reverse().ToArray());
                IEnumerable<int> Indices()
                {
                    IReadOnlyTree? parent = this.Parent;
                    ITree child = this;
                    while (parent is ITree _parent)
                    {
                        yield return _parent.IndexOf(child);
                        child = _parent;
                        if ((parent as IGetParent<IReadOnlyTree>).Parent == parent)
                            throw new Exception("r sdfsd3232 bf");
                        parent = (parent as IGetParent<IReadOnlyTree>).Parent;
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
                while ((parent as IGetParent<IReadOnlyTree>).Parent != null)
                {
                    depth++;
                    parent = (parent as IGetParent<IReadOnlyTree>).Parent;
                }
                return depth;
            }
        }

        public virtual Task<bool> HasMoreChildren() => Task.FromResult(false);

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
            return (other as IGetKey)?.Key?.Equals(this.Key) == true;
        }


        bool IEquatable<IReadOnlyTree>.Equals(IReadOnlyTree? other)
        {
            return (other as IGetKey)?.Key?.Equals(this.Key) == true;
        }

        public virtual bool Equals(IEquatable? other)
        {
            return other?.Equals(this.Key) == true;
        }

        public virtual int Count => m_items.Count;
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
            foreach (var item in tree.Children)
            {
                if (item.Equals(_item))
                    return i;
                i++;
            }
            return -1;
        }

    }
}
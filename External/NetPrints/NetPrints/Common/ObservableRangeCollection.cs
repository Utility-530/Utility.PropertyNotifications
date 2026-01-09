using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

// http://stackoverflow.com/a/670579

namespace NetPrints.Core
{
    public static class CollectionHelper
    {
        public static T AddRange<T, TR>(this T t, IEnumerable<TR> items) where T : IEnumerable<TR>
        {

            if (t is IRangeCollection<TR> addRange)
            {
                addRange.AddRange(items);
                return t;

            }
            else if (t is IList<TR> list)
            {
                foreach (var item in items)
                    list.Add(item);
                return t;
            }

            return (T)emr();

            IEnumerable<TR> emr()
            {
                foreach (var item in t)
                    yield return item;
                foreach (var item in items)
                    yield return item;
            }
        }

        public static T RemoveRange<T, TR>(this T t, IEnumerable<TR> items) where T : IEnumerable<TR>
        {

            if (t is IRangeCollection<TR> addRange)
            {
                addRange.RemoveRange(items);
                return t;

            }
            else if (t is IList<TR> list)
            {
                foreach (var item in items)
                    list.Remove(item);
                return t;
            }

            return (T)emr();

            IEnumerable<TR> emr()
            {
                foreach (var item in t)
                {
                    if (items.Contains(item) == false)
                        yield return item;
                }
            }
        }
    }

    /// <summary> 
    /// Represents a dynamic data collection that provides notifications when items get added, removed, or when the whole list is refreshed. 
    /// </summary> 
    /// <typeparam name="T"></typeparam> 
    [Serializable]
    public class ObservableRangeCollection<T> : ObservableCollection<T>, IObservableCollection<T>, IRangeCollection<T>
    {
        /// <summary> 
        /// Adds the elements of the specified collection to the end of the ObservableCollection(Of T). 
        /// </summary> 
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            foreach (var item in collection)
            {
                Items.Add(item);
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary> 
        /// Removes the first occurence of each item in the specified collection from ObservableCollection(Of T). 
        /// </summary> 
        public void RemoveRange(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            foreach (var item in collection)
            {
                Items.Remove(item);
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary> 
        /// Clears the current collection and replaces it with the specified item. 
        /// </summary> 
        public void Replace(T item)
        {
            ReplaceRange(new T[] { item });
        }

        /// <summary> 
        /// Clears the current collection and replaces it with the specified collection. 
        /// </summary> 
        public void ReplaceRange(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            Items.Clear();

            foreach (var item in collection)
            {
                Items.Add(item);
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected override void ClearItems()
        {
            while (Count > 0)
                base.RemoveAt(Count - 1);
            //List<T> removed = new List<T>(this);
            //base.ClearItems();
            //base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed));
        }

        /// <summary> 
        /// Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection(Of T) class. 
        /// </summary> 
        public ObservableRangeCollection()
            : base()
        {
        }

        /// <summary> 
        /// Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection(Of T) class that contains elements copied from the specified collection. 
        /// </summary> 
        /// <param name="collection">collection: The collection from which the elements are copied.</param> 
        /// <exception cref="System.ArgumentNullException">The collection parameter cannot be null.</exception> 
        public ObservableRangeCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }
    }
}

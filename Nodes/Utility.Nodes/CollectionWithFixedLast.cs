using System;
using System.Collections.ObjectModel;

namespace Utility.Nodes
{
    public class CollectionWithFixedLast<T> : RangeObservableCollection<T>
    {
        private readonly T _fixedLastItem;

        public CollectionWithFixedLast(T fixedLastItem)
        {
            _fixedLastItem = fixedLastItem;
            Add(_fixedLastItem);
        }


        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(Count > 0 ? Math.Min(index, Count - 1) : index, item);
        }


    }
}

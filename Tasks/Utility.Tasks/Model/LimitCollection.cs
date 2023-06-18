
using System;


namespace Utility.Tasks.Model
{
    public struct Capacity
    {
        public Capacity(uint value)
        {
            Value = value;
        }
        public uint Value { get; }
    }

    public interface IBasicObservableCollection<T> : IObservable<T>
    {
        void Remove(T obj);

        void Add(T obj);


    }

}

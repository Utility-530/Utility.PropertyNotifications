
using System;


namespace ReactiveAsyncWorker.Model
{
    public struct Capacity
    {
        public Capacity(uint value)
        {
            Value = value;
        }
        public uint Value { get; }
    }

    public interface IBasicCollection<T> : IObservable<T>
    {
        void Remove(T obj);

        void Add(T obj);


    }

}

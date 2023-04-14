using System;
using System.Collections.Generic;
using System.Linq;

namespace Utility.WPF.Demo.Data.Common;

public class Enumerator<T> : IEnumerator<T>
{
    private int position = -1;
    private readonly List<T> collection;

    public Enumerator(IEnumerable<T> list)
    {
        collection = list.ToList();
    }

    public void Add(T value)
    {
        collection.Add(value);
    }

    public IReadOnlyCollection<T> Collection => collection;

    public bool MoveNext()
    {
        position++;
        return position < collection.Count;
    }

    public void Reset()
    {
        position = -1;
    }

    T IEnumerator<T>.Current => (T)Current;

    public object Current
    {
        get
        {
            try
            {
                return collection[position];
            }
            catch (IndexOutOfRangeException)
            {
                throw new InvalidOperationException();
            }
        }
    }

    public void Dispose()
    {
    }
}
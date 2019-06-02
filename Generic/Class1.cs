using System;
using System.Collections.Generic;

namespace UtilityInterface.Generic
{

    public interface IParent<T>
    {
        IEnumerable<T> Children { get; set; }

    }

    public interface IService<R>
    {
        IObservable<R> Resource { get; }
    }

    public interface ISave<T>
    {
        bool Save(T o);
    }

    public interface ILoad<T>
    {
        T Load();
    }

    public interface IPermanent<T>:ISave<T>, ILoad<T>
    {

    }
   
    public interface IFilter<T>
    {
        bool Filter(T o);

    }

   
    public interface ISort<T>
    {
        bool Sort(T o);

    }

    public interface IContain<T>
    {
        T Object { get; }
    }

   
    public interface IMethod<T,R>
    {
        Func<T,R> Method  { get; }
    }

  
    public interface IFunction<T, R>
    {
        R Function(T t);
    }
}

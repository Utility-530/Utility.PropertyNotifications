using System;
using System.Collections.Generic;

namespace UtilityInterface.Generic
{

    public interface IParent<T>
    {
        IEnumerable<T> Children { get; set; }

    }

    public interface IObservableService<R>
    {
        IObservable<R> Service { get; }
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

    public interface IObject<T>
    {
        T Object { get; }
    }

   
    public interface IFunction<T,R>
    {
        Func<T,R> Function { get; }
    }

  
    public interface IMethod<T, R>
    {
        R Method(T t);
    }

    public interface IPair<T>
    {
        T One { get; }

        T Two { get;  }
    }


    public interface IKey<T>
    {
        T Key { get; }
    }

    public interface IRange<T>
    {
        T Min { get; }

        T Max { get; }
    }

}

using System;
using System.Collections.Generic;

namespace UtilityInterface
{





    public interface IPeriod
    {

        DateTime Start { get; set; }
        DateTime End { get; set; }


    }


    public interface IParent<T>
    {
        IEnumerable<T> Children { get; set; }

    }



    public interface ITimeValue
    {

        double Value { get; set; }
        DateTime Time { get; set; }


    }

    public interface IService<R>
    {
        IObservable<R> Resource { get; }
    }

    public interface IPeriodic
    {
        IEnumerable<DateTime> Dates { get; set; } 
    }

    public interface IKey
    {
        String Key { get; set; }

    }


    public interface IDistributed
    { 
        double Weight { get; set; }
    }

    public interface IPermanent<T>
    {

        bool Save(T o);
        T Load();

    }
    public interface IPermanent
    {

        bool Save(object o);
        object Load();
    }

    public interface IPlayer
    {
        void Pause();

        void Resume();

        void Cancel();
    }


    public interface IFilter
    {
        bool Filter(object o);

    }

    public interface IFilter<T>
    {
        bool Filter(T o);

    }

    public interface ISorter
    {
        bool Sort(object o);

    }

    public interface ISorter<T>
    {
        bool Sort(T o);

    }

    public interface IDelayedConstuctor
    {
         void Init(object o);
    }

    public interface IContainer<T>
    {
        T Object { get; }
    }

    public interface IContainer
    {
        object Object { get; }
    }

    public interface IMethod
    {
        Func<object,object> Method  { get; }
    }

    public interface IMethod<T,R>
    {
        Func<T,R> Method  { get; }
    }

    public interface IFunction
    {
        object Function(object t);
    }

    public interface IFunction<T, R>
    {
        R Function(T t);
    }
}

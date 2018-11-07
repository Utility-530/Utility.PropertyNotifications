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
        IList<T> Children { get; set; }

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
        IList<DateTime> Dates { get; set; } 
    }


    public interface IDistributed
    { 

        double Weight { get; set; }


    }

    public interface IPermanent<T>
    {

        bool Save(object o);
        T Load();

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
        Object Object { get; }
    }
}

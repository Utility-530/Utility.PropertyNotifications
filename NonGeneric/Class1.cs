using System;
using System.Collections.Generic;

namespace UtilityInterface.NonGeneric
{

    public interface IStart
    {
        DateTime Start { get; set; }

    }

    public interface IEnd
    {
        DateTime End { get; set; }

    }

    public interface ITime
    {
        DateTime Time { get; set; }
    }
    public interface IValue
    {
        double Value { get; set; }
    }


    public interface IPeriod : IStart, IEnd
    {

    }


    public interface ITimeValue : ITime, IValue
    {

    }

    public interface IPeriodic
    {
        IEnumerable<DateTime> DateTimes { get; }
    }

    public interface IKey
    {
        string Key { get; }

    }
    public interface IName
    {
        string Name { get; }

    }

    public interface IWeight
    {
        double Weight { get; }
    }

    public interface ISave
    {
        bool Save(object o);
    }

    public interface ILoad
    {
        object Load();
    }

    public interface IPermanent : ISave, ILoad
    {

    }



    public interface IPlay
    {
        void Play();
    }
    public interface IPause
    {
        void Pause();
    }
    public interface ICancel
    {
        void Cancel();
    }

    public interface IPlayer : IPlay, IPause, ICancel
    {
    }


    public interface IFilter
    {
        bool Filter(object o);

    }

    public interface ISort
    {
        bool Sort(object o);

    }



    public interface IInitialise
    {
        void Initialise(object o);
    }



    public interface IObject
    {
        object Object { get; }
    }

    public interface IFunction
    {
        Func<object, object> Function { get; }
    }


    public interface IMethod
    {
        object Method(object t);
    }

    public interface IPair
    {
        string One { get; }

        string Two { get; }
    }

    public interface IRange
    {
        double Min { get; }

        double Max { get; }
    }

    public interface INameGetter
    {
        string GetName(object value);
    }
}

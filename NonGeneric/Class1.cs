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


    public interface IPeriod:IStart,IEnd
    { 

    }


    public interface ITimeValue:ITime,IValue
    {

    }

    public interface IPeriodic
    {
        IEnumerable<DateTime> DateTimes { get; set; } 
    }

    public interface IKey
    {
        string Key { get; set; }

    }
    public interface IName
    {
        string Name { get; set; }

    }

    public interface IDistributed
    { 
        double Weight { get; set; }
    }

   
    public interface IPermanent
    {

        bool Save(object o);
        object Load();
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

    public interface IPlayer:IPlay,IPause,ICancel
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

    public interface IMethod
    {
        Func<object,object> Method  { get; }
    }


    public interface IFunction
    {
        object Function(object t);
    }

    public interface IPair
    {
        string One { get; set; }

        string Two { get; set; }
    }

    public interface IRange
    {
        double Min { get; set; }

        double Max { get; set; }
    }

}

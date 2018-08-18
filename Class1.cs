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



    public interface IPeriodic
    {
        IList<DateTime> Dates { get; set; } 
    }


    public interface IDistributed
    { 

        double Weight { get; set; }


    }





}

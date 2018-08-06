using System;
using System.Collections.Generic;

namespace UtilityInterface
{
<<<<<<< HEAD




    public interface IPeriod
    {

        DateTime Start { get; set; }
        DateTime End { get; set; }
=======
 
        public interface IKey
        {
            string Key { get; set; }
        }

        public interface IDbRow
        {
            Int64 Id { get; set; }
            Int64 ParentId { get; set; }
        }
        
        public interface IPeriodic
        {
            DateTime Start { get; set; }
            DateTime End { get; set; }
        }
>>>>>>> 99229becea2a21d949094c6c6bb4adb9ab58fb21

    }

<<<<<<< HEAD
    public interface IParent<T>
    {
        IList<T> Children { get; set; }

    }



    public interface ITimeValue
    {

        double Value { get; set; }
        DateTime Time { get; set; }

=======
        public interface ITimeValue
        {
            double Value { get; set; }
            DateTime Time { get; set; }
        }
>>>>>>> 99229becea2a21d949094c6c6bb4adb9ab58fb21

    }


<<<<<<< HEAD
    public interface IPeriodic
    {
        IList<DateTime> Dates { get; set; } 
    }


    public interface IDistributed
    {
=======
        public interface IDistributed
        {
            double Weight { get; set; }
        }
>>>>>>> 99229becea2a21d949094c6c6bb4adb9ab58fb21

        double Weight { get; set; }

<<<<<<< HEAD
    }




=======
>>>>>>> 99229becea2a21d949094c6c6bb4adb9ab58fb21
}

using System;

namespace UtilityInterface
{
    public class Class1
    {
        public interface IKey
        {
            string Key { get; set; }

        }


        public interface IPeriodic
        {

            DateTime Start { get; set; }
            DateTime End { get; set; }

        }





        public interface ITimeValue
        {

            double Value { get; set; }
            DateTime Time { get; set; }


        }



        public interface IDistibuted
        {

            double Weight { get; set; }

        }


        public interface IDbRow
        {
            Int64 Id { get; set; }
            Int64 ParentId { get; set; }
        }
    }
}

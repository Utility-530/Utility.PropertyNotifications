using System.ComponentModel;

namespace SoftFluent.Windows.Samples
{
    [TypeConverter(typeof(PointConverter))]
    public struct Point
    {
        public Point(int x, int y)
            : this()
        {
            X = x;
            Y = y;
        }

        public int X { get; private set; }
        public int Y { get; private set; }
    }

    [Flags]
    public enum DaysOfWeek
    {
        NoDay = 0,
        Monday = 1,
        Tuesday = 2,
        Wednesday = 4,
        Thursday = 8,
        Friday = 16,
        Saturday = 32,
        Sunday = 64,
        WeekDays = Monday | Tuesday | Wednesday | Thursday | Friday
    }

    public enum Gender
    {
        Male,
        Female
    }

    public enum Status
    {
        //[PropertyTrees(Name = "Foreground", Value = "Black")]
        //[PropertyTrees(Name = "Background", Value = "Orange")]
        Unknown,

        //[PropertyTrees(Name = "Foreground", Value = "White")]
        //[PropertyTrees(Name = "Background", Value = "Red")]
        Invalid,

        //[PropertyTrees(Name = "Foreground", Value = "White")]
        //[PropertyTrees(Name = "Background", Value = "Green")]
        Valid
    }
}
using System.Windows.Media;

namespace Utility.WPF.Converters
{
    public class BooleanToBrushConverter : BaseConverter<Brush, bool>
    {
        public BooleanToBrushConverter() : base(Brushes.Black, Brushes.Red)
        {
        }

        protected override bool Check(bool value) => value;

        protected override bool Convert(object value)
        {
            return System.Convert.ToBoolean(value);
        }

        public static BooleanToBrushConverter Instance => new BooleanToBrushConverter();
    }
}
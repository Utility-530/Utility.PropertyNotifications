using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace Utility.WPF.Converters
{
    public class ByteToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType().IsEnum || value is byte)
                return GetColor(System.Convert.ToByte(value));
            return Colors.BlanchedAlmond;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static Color GetColor(byte key) => NiceColors.Value[key];

        private static readonly Lazy<Dictionary<int, Color>> NiceColors = new Lazy<Dictionary<int, Color>>(() =>
                   niceColorsDictionary
                    .Select((a, i) => Tuple.Create(i, (Color)System.Windows.Media.ColorConverter.ConvertFromString(a.Value)))
                    .ToDictionary(a => a.Item1, a => a.Item2));

        private static readonly Dictionary<string, string> niceColorsDictionary = new Dictionary<string, string> {
            { "navy", "#001F3F"} ,
             { "blue", "#0074D9"} ,
              { "aqua", "#7FDBFF"} ,
                                  { "teal", "#39CCCC"} ,
                                  { "olive", "#3D9970"} ,
                                  { "green", "#2ECC40"} ,
                                  { "d", "#d59aea"} ,
                                  {  "yellow", "#FFDC00"} ,
                         { "black", "#111111"},
                                  { "red", "#FF4136"} ,
               { "fuchsia", "#F012BE"} ,

             { "purple", "#B10DC9"} ,

             { "maroon", "#85144B"} ,
                                  { "gray", "#AAAAAA"} ,
                                                 { "silver", "#DDDDDD"} ,
                                           {  "orange", "#FF851B"} ,
                                  { "a", "#ff035c"},
                                  { "b", "#9eb4cc"},
                                  { "c", "#fbead3"},
                                  };
    }
}
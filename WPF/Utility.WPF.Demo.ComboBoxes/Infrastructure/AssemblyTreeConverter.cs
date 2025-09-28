using System.Globalization;
using System.Windows.Data;
using Utility.Trees.Abstractions;
using System.Collections;

namespace Utility.WPF.Demo.ComboBoxes
{
    public class AssemblyTreeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string sParameter = null;
            if (parameter is string str)
            {
                sParameter = str;
            }
            ITree tree = Utility.WPF.Controls.ComboBoxes.Ex.ToTree(new[] { typeof(Utility.WPF.Demo.Data.Model.Character).Assembly }, new Predicate<DictionaryEntry>(a =>
            {
                return a.Value.GetType().ToString().Contains(sParameter);

            }));
            return tree.Children;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static AssemblyTreeConverter Instance { get; } = new AssemblyTreeConverter();
    }

}

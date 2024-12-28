using System;
using System.Globalization;
using System.Windows.Data;
using Utility.Interfaces.NonGeneric;

namespace Utility.WPF.Controls.ComboBoxes
{
    public class KeyValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                //case Assembly assemblyKeyValue:
                //    return assemblyKeyValue.Key.Split(",").First();
                //case ResourceDictionary resourceDictionaryKeyValue:
                //    return resourceDictionaryKeyValue.Key;
                //case DictionaryEntry dictionaryEntryKeyValue:
                //    return dictionaryEntryKeyValue.Key.Contains(nameof(DataTemplateKey)) ? "DTK" : dictionaryEntryKeyValue.Key;
                //case KeyValue keyValue:

                case IName name:
                    return name.Name;
                case IKey keyValue:
                    return keyValue.Key;   
 


            }
            return string.Empty;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static KeyValueConverter Instance { get; } = new();
    }
}

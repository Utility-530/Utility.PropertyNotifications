//using System;
//using System.Globalization;
//using System.Windows;
//using System.Windows.Data;
//using Utility.Helpers.NonGeneric;
//using Utility.ProjectStructure;

//namespace Utility.WPF.Demo.Trees
//{
//    public class KeyValueConverter : IValueConverter
//    {
//        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            switch(value)
//            {
//                case AssemblyKeyValue assemblyKeyValue:
//                    return assemblyKeyValue.Key.Split(",").First();
//                case ResourceDictionaryKeyValue resourceDictionaryKeyValue:
//                    return resourceDictionaryKeyValue.Key;
//                case DictionaryEntryKeyValue dictionaryEntryKeyValue:
//                    return dictionaryEntryKeyValue.Key.Contains(nameof(DataTemplateKey))? "DTK": dictionaryEntryKeyValue.Key;
//                case KeyValue keyValue:
//                    return keyValue.Key;      
         

//            }
//            return string.Empty;

//        }

//        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }

//        public static KeyValueConverter Instance { get; } = new();
//    }
//}

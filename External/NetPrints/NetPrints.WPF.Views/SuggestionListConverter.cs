using NetPrints.Core;
using NetPrints.ViewModels;
using NetPrintsEditor.Converters;
using Splat;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace NetPrints.WPF.Views
{


    public class SuggestionListConverter : IValueConverter
    {
        ISpecifierConverter specifierConverter = Locator.Current.GetService<ISpecifierConverter>();

        public object Convert(object tupleObject, Type targetType, object parameter, CultureInfo culture)
        {
            if (tupleObject == null)
                return DependencyProperty.UnsetValue;

            if (tupleObject is not SearchableItemViewModel { Value: ISpecifier specifier })
            {
                throw new Exception("311 3");
            }
            if (targetType == typeof(ImageSource))
            {
                var iconPath = specifierConverter.ConvertToIconPath(specifier);
                var fullIconPath = $"pack://application:,,,/{Assembly.GetEntryAssembly().GetName().Name};component/Resources/{iconPath}";
                return fullIconPath;
            }
            else
            {
                return specifierConverter.ConvertToText(specifier);
            }
            // See https://docs.microsoft.com/en-us/dotnet/framework/wpf/app-development/pack-uris-in-wpf for format
         

            return DependencyProperty.UnsetValue;
            //return new SuggestionListItem(text, fullIconPath);
        }



        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //public class SuggestionListItem
    //{
    //    public string Text { get; }
    //    public string IconPath { get; }

    //    public SuggestionListItem(string text, string iconPath)
    //    {
    //        Text = text;
    //        IconPath = iconPath;
    //    }
    //}
}

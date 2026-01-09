using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using NetPrints;
using NetPrints.Core;
using NetPrints.Core.Converters;
using NetPrints.Interfaces;
using NetPrints.Reflection;
using NetPrintsEditor.Converters;
using Splat;
using Tiny.Toolkits;
using Utility.PropertyNotifications;

namespace Utility.Nodes.Demo.Lists
{
    internal class SpecifierConverter : ISpecifierConverter
    {
        public string ConvertToIconPath(ISpecifier value)
        {
            if (value is MethodSpecifier methodSpecifier)
            {
                return OperatorUtil.IsOperator(methodSpecifier) ? "Operator_16x.png" : "Method_16x.png";

            }
            throw new Exception("Unsupported specifier type");
        }

        public string ConvertToText(ISpecifier value)
        {
            if (value is MethodSpecifier methodSpecifier)
            {
                return MethodSpecifierConverter.Convert(value);

            }
            throw new Exception("Unsupported specifier type");
        }
    }


    public class SuggestionListConverter : IValueConverter
    {
        ISpecifierConverter specifierConverter = Locator.Current.GetService<ISpecifierConverter>();

        public object Convert(object tupleObject, Type targetType, object parameter, CultureInfo culture)
        {
            if (tupleObject == null)
                return DependencyProperty.UnsetValue;

            if (tupleObject is not  ISpecifier specifier)
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


    //public class SuggestionListViewModel : NotifyPropertyClass
    //{
    //    ISpecifierConverter specifierConverter = Locator.Current.GetService<ISpecifierConverter>();
    //    private readonly ITypesEnumerable types = Locator.Current.GetService<ITypesEnumerable>();

    //    private string searchText = "";
    //    private string[] splitSearchText = Array.Empty<string>();
    //    private Lazy<IEnumerable<SearchableItemViewModel>> items;

    //    public SuggestionListViewModel()
    //    {
    //        items = new Lazy<IEnumerable<SearchableItemViewModel>>(() => types.Types.ToItems().ToArray());
    //    }

    //    public IEnumerable<SearchableItemViewModel> Items => items.Value;

    //    public string SearchText
    //    {
    //        get => searchText; set
    //        {
    //            searchText = value;
    //            OnSearchTextChanged();
    //            RaisePropertyChanged(nameof(Predicate));
    //        }
    //    }

    //    public Predicate<object> Predicate => new Predicate<object>(ItemFilter);

    //    public bool ItemFilter(object item)
    //    {
    //        if (string.IsNullOrEmpty(SearchText))
    //        {
    //            return true;
    //        }

    //        //object convertedItem = suggestionConverter.Convert(item, typeof(string), null, CultureInfo.CurrentUICulture);
    //        if (item is SearchableItemViewModel { Category: { } cat, Value: ISpecifier value } _item)
    //        {
    //            var (text, _) = specifierConverter.Convert(value);
    //            var listItemText = $"{_item.Category} {text}";
    //            return splitSearchText.All(searchTerm =>
    //                listItemText.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0);
    //        }
    //        throw new NotImplementedException();
    //    }

    //    private void OnSearchTextChanged() => splitSearchText = SearchText?.Split(' ');


    //    public void OnItemSelected(object selectedValue)
    //    {

    //    }
    //}


    //public class SearchableItemViewModel(string c, ISpecifier v)
    //{
    //    public string Category { get; set; } = c;
    //    public ISpecifier Value { get; set; } = v;
    //}

    //static class Helper
    //{
    //    public static IEnumerable<SearchableItemViewModel> ToItems(this IEnumerable<ITypesProvider> providers)
    //    {
    //        return providers.SelectMany(a => a.types().Select(x => new SearchableItemViewModel(a.Name, x))).DistinctBy(a => a.Value);
    //    }

    //}
}

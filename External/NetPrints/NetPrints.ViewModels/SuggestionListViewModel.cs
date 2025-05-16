using GalaSoft.MvvmLight;
using NetPrints.Core;
using NetPrints.Interfaces;
using NetPrints.Reflection;
using NetPrintsEditor.Converters;
using Splat;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MoreLinq;

namespace NetPrints.ViewModels
{
    public class SuggestionListViewModel : ViewModelBase
    {
        ISpecifierConverter specifierConverter = Locator.Current.GetService<ISpecifierConverter>();
        private readonly ITypesEnumerable types = Locator.Current.GetService<ITypesEnumerable>();

        private string searchText = "";
        private string[] splitSearchText = Array.Empty<string>();
        private Lazy<IEnumerable<SearchableItemViewModel>> items;

        public SuggestionListViewModel()
        {
            items = new Lazy<IEnumerable<SearchableItemViewModel>>(() => types.Types.ToItems().ToArray());
        }

        public IEnumerable<SearchableItemViewModel> Items => items.Value;

        public string SearchText
        {
            get => searchText; set
            {
                searchText = value;
                OnSearchTextChanged();
                RaisePropertyChanged(nameof(Predicate));
            }
        }

        public Predicate<object> Predicate => new Predicate<object>(ItemFilter);

        public bool ItemFilter(object item)
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                return true;
            }

            //object convertedItem = suggestionConverter.Convert(item, typeof(string), null, CultureInfo.CurrentUICulture);
            if (item is SearchableItemViewModel { Category: { } cat, Value: ISpecifier value } _item)
            {
                var (text, _) = specifierConverter.Convert(value);
                var listItemText = $"{_item.Category} {text}";
                return splitSearchText.All(searchTerm =>
                    listItemText.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            throw new NotImplementedException();
        }

        private void OnSearchTextChanged() => splitSearchText = SearchText?.Split(' ');


        public void OnItemSelected(object selectedValue)
        {

        }
    }


    public class SearchableItemViewModel(string c, ISpecifier v)
    {
        public string Category { get; set; } = c;
        public ISpecifier Value { get; set; } = v;
    }

    static class Helper
    {
        public static IEnumerable<SearchableItemViewModel> ToItems(this IEnumerable<ITypesProvider> providers)
        {
            return providers.SelectMany(a => a.types().Select(x => new SearchableItemViewModel(a.Name, x))).DistinctBy(a => a.Value);
        }

    }
}

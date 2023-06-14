using System.Collections;
using System.Linq;
using Utility.WPF.Helpers;
using Utility.WPF.Demo.Common.ViewModels;

namespace Utility.WPF.Demo.Dragablz.ViewModels
{
    public class ResourceDictionariesViewModel : Utility.WPF.Demo.Common.ViewModels.ResourceDictionariesViewModel
    {
        private readonly TickViewModel[] collection;

        public ResourceDictionariesViewModel()
        {
            var dictionaries = typeof(Utility.WPF.Demo.Common.ViewModels.ResourceDictionariesViewModel)
                 .Assembly
                 .SelectResourceDictionaries(a => a.Key.ToString().EndsWith("themes.baml", System.StringComparison.CurrentCultureIgnoreCase))
                 .Single()
                 .resourceDictionary
                 .MergedDictionaries;

            collection = ThemesViewModelFactory
                .CreateViewModels(dictionaries)
                .ToArray();

            ResourceDictionaryService service = new(dictionaries);

            foreach (var item in collection)
            {
                service.OnNext(item as TickViewModel);
            }
        }

        public override IEnumerable Collection => collection;
    }
}
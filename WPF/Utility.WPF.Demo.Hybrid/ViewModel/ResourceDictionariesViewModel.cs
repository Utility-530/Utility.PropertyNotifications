using System;
using System.Collections;
using System.Linq;
using System.Reactive.Linq;
using Utility.Persists;
using Utility.Services;
using Utility.Interfaces.NonGeneric.Data;
using Utility.WPF.Demo.Common.ViewModels;
using Utility.WPF.Helpers;
using Utility.WPF.Meta;

namespace Utility.WPF.Demo.Hybrid.ViewModels
{
    public class ResourceDictionariesViewModel : Utility.WPF.Demo.Common.ViewModels.ResourceDictionariesViewModel
    {
        private readonly CollectionService collectionService = new();

        public ResourceDictionariesViewModel()
        {
            var dictionaries = typeof(Utility.WPF.Demo.Common.ViewModels.ResourceDictionariesViewModel)
                .Assembly
                .SelectResourceDictionaries(a => a.Key.ToString().EndsWith("themes.baml", System.StringComparison.CurrentCultureIgnoreCase))
                .Single()
                .ResourceDictionary
                .MergedDictionaries;

            var themes = ThemesViewModelFactory
                .CreateViewModels(dictionaries)
                .ToArray();

            ResourceDictionaryService service = new(dictionaries);

            //foreach(var item in collectionService.Items)
            //{
            //    service.OnNext(item as TickViewModel);
            //}

            collectionService
                .Select(a => a.change)
                .Subscribe(a =>
                {
                    if (a.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add ||
                    a.Action ==  System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
                    {
                        foreach (var item in a.Items)
                        {
                            service.OnNext(item as TickViewModel);
                        }
                        return;
                    }
                    throw new Exception("gd77fdfwe");
                });

            collectionService.OnNext(new(Repository()));

            foreach (var item in themes)
            {
                if (collectionService.Items.Contains(item) == false)
                    collectionService.Items.Add(item);
                //else
                //    collectionService.Items.Remove(item);
            }
        }

        public override IEnumerable Collection => collectionService.Items;

        private IRepository Repository() => new LiteDbRepository(new(typeof(TickViewModel), nameof(TickViewModel.Id)));
    }
}
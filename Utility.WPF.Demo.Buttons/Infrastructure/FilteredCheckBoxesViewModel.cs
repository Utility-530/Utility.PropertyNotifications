using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Utility.Service;
using Utility.ViewModels;
using Utility.Models.Filters;
using UtilityWpf.Demo.Data.Factory;
using UtilityWpf.Demo.Data.Model;
using Utility.Models;

namespace Utility.WPF.Demo.Buttons.Infrastructure
{
    internal class FilteredCheckBoxesViewModel : ViewModel
    {
        public FilteredCheckBoxesViewModel() : base(nameof(FilteredCheckBoxesViewModel))
        {
            FilterDictionaryService<Profile> filter = new(a => a.Name);
            var changeSet = new ProfileCollectionObservable(10, 5)
                            .Take(20)
                            .ToObservableChangeSet();

            //FilterCollectionViewModel = new(changeSet, filter, new(true));

            //var filtered = changeSet.Filter(filter);
            //CollectionViewModel = filtered.SubscribeTo(()=> new CollectionViewModel<Profile>(), out var dis);
            //CountViewModel = new(changeSet);

            //filtered.Subscribe(CollectionViewModel);
            //filtered.Subscribe(CollectionViewModel);
        }

        public override Property Model { get; }

        //public FilterCollectionViewModel<Profile> FilterCollectionViewModel { get; }
        //public CollectionViewModel<Profile> CollectionViewModel { get; }
        //public CountViewModel CountViewModel { get; }
    }

    internal class FilteredCustomCheckBoxesViewModel
    {
        private readonly FilterService<Profile> filterService = new();

        public FilteredCustomCheckBoxesViewModel()
        {
            var filters = ProfileFilterCollection
                                .Filters
                                .ToObservable()
                                .ToObservableChangeSet();

            var profiles = new ProfileCollectionObservable(10, 5)
                            .Take(20)
                            .ToObservableChangeSet();

            Dictionary<Filter, IDisposable> dictionary = new();
            filters
                .OnItemAdded(filter =>
                {
                    CompositeDisposable composite = new();
                    if (filter is ObserverFilter<Profile> oFilter)
                        composite.Add(profiles.Subscribe(oFilter));
                    if (filter is IRefreshObservable filterObservable)
                        composite.Add(filterObservable.Subscribe(filterService));
                    dictionary[filter] = composite;
                })
                .OnItemRemoved(filter =>
                {
                    if (dictionary.ContainsKey(filter))
                    {
                        dictionary[filter].Dispose();
                        dictionary.Remove(filter);
                    }
                })
                .Subscribe();

            var filtered = profiles.Filter(filterService);
            var checkFilters = filters.Transform(a => new CheckViewModel(a.Header, false) {/* a */});
            //FilterCollectionViewModel = new(checkFilters, filterService, new(false));
            //CollectionViewModel = filtered.SubscribeTo(()=> new CollectionViewModel<Profile>(), out var disposable );
            //CountViewModel = profiles.SubscribeTo(()=> new CountViewModel(), out var disposable2);
            //FilteredCountViewModel = filtered.SubscribeTo(() => new CountViewModel(), out var disposable3);
        }

        //public CheckedCollectionCommandViewModel<Profile, Filter> FilterCollectionViewModel { get; }
        //public CollectionViewModel<Profile> CollectionViewModel { get; }
        //public CountViewModel CountViewModel { get; }
        //public CountViewModel FilteredCountViewModel { get; }
    }
}
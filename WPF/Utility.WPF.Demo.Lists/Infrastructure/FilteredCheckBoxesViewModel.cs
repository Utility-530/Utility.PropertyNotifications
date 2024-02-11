using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Utility.Services;
using Utility.ViewModels;
using Utility.Helpers.Ex;
using Utility.WPF.Demo.Data.Factory;
using Utility.WPF.Demo.Data.Model;
using Utility.Models.Filters;
using Utility.Reactives;

namespace Utility.WPF.Demo.Lists.Infrastructure
{
    internal class FilteredCheckBoxesViewModel
    {
        public FilteredCheckBoxesViewModel()
        {
            FilterDictionaryService<Profile> filter = new(a => a.Name);
            var changeSet = new ProfileCollectionObservable(10, 5)
                            .Take(20)
                            .ToObservableChangeSet();

            FilterCollectionViewModel = new(changeSet, filter, new(true));

            var subject = changeSet.Filter(filter).ToReplaySubject();

            CollectionViewModel = new();
            CountViewModel = new();
        }

        public FilterCollectionViewModel<Profile> FilterCollectionViewModel { get; }
        public CollectionViewModel<Profile> CollectionViewModel { get; }
        public CountViewModel CountViewModel { get; }
    }

    internal class FilteredCustomCheckBoxesViewModel : IDisposable
    {
        private readonly FilterService<Profile> filterService = new();
        private readonly Dictionary<Filter, IDisposable> dictionary = new();

        public FilteredCustomCheckBoxesViewModel()
        {
            var filters = ProfileFilterCollection
                                .Filters
                                .ToObservable()
                                .ToObservableChangeSet();

            var profiles = new ProfileCollectionObservable(10, 5)
                                .Take(20)
                                .ToObservableChangeSet();

            filters
                .OnItemAdded(filter =>
                {
                    CompositeDisposable composite = new();
                    if (filter is ObserverFilter<Profile> oFilter)
                        composite.Add(profiles.Subscribe(oFilter));
                    if (filter is IRefreshObservable refresh)
                        composite.Add(refresh.Subscribe(filterService));
                    dictionary[filter] = composite;
                })
                .OnItemRemoved(filter =>
                {
                    //if (dictionary.ContainsKey(filter))
                    //{
                    //    dictionary[filter].Dispose();
                    //    dictionary.Remove(filter);
                    //}
                })
                .Subscribe();

            var checkFilters = filters.Transform(a => (ViewModel<Filter>)new CheckContentViewModel(a, a.Header, false));
            FilterCollectionViewModel = new(checkFilters, filterService, new(false));

            var subjects = profiles.Filter(filterService).ToReplaySubject(100);

            CollectionViewModel = new();
            CountViewModel = new();
            FilteredCountViewModel = new();
        }

        public CheckedCollectionCommandViewModel<Profile, Filter> FilterCollectionViewModel { get; }
        public CollectionViewModel<Profile> CollectionViewModel { get; }
        public CountViewModel CountViewModel { get; }
        public CountViewModel FilteredCountViewModel { get; }

        public void Dispose()
        {
            foreach (var item in dictionary)
            {
                item.Value.Dispose();
            }
        }
    }
}
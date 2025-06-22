using DynamicData;
using System;
using System.Collections.Generic;
using Utility.Services;
using Utility.Models.Filters;
using Utility.ViewModels;
using System.Reactive.Disposables;
using Utility.Reactives;
using System.Reactive;
using Utility.Services.Deprecated;

namespace Utility.WPF.Controls.Meta.ViewModels
{
    public class FilteredCustomCheckBoxesViewModel : IDisposable
    {
        private readonly BaseFilterService<ViewModelEntity> filterService = new();
        private readonly Dictionary<Filter, IDisposable> dictionary = new();

        public FilteredCustomCheckBoxesViewModel(
            IObservable<IChangeSet<ViewModelEntity>> profiles,
            IObservable<IChangeSet<Filter>> filters, 
            IObservable<Unit> deselections)
        {
            filters
                .OnItemAdded(async filter =>
                {
                    CompositeDisposable composite = new();
                    if (filter is ObserverFilter<ViewModelEntity> oFilter)
                        composite.Add(profiles.Subscribe(oFilter));
                    if (filter is IRefreshObservable refresh)
                    {
                        composite.Add(refresh.Subscribe(filterService));
                        composite.Add(refresh.Subscribe(a =>
                        {
                        }));
                    }
                    //var first = await FilterEntity.Select.Where(a => a.Key == filter.Key).FirstAsync();
                    //if (first != null)
                    //{
                    //    filter.Value = first.Value;
                    //}
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

            var checkFilters = filters.Transform(e => new ViewModel<Filter>(e.Header, e));
            //var checkProfiles = profiles.Transform(e => new ViewModel<ViewModelEntity>(e.Key, e));
            FilterCollectionViewModel = new(checkFilters, filterService, new(false));

            //var subjects = profiles/*.Filter(filterService)*/.ToReplaySubject(1000);

            CollectionViewModel = new();
            profiles.Subscribe(CollectionViewModel);
            CountViewModel = new();
            profiles.Subscribe(CountViewModel);
            FilteredCountViewModel = new();
            profiles.Subscribe(FilteredCountViewModel);

            deselections.Subscribe(a => DeselectAll());
        }

        public void DeselectAll()
        {
            foreach (var item in CollectionViewModel.Children)
            {
                item.IsSelected = false;
            }
        }

        public CheckedCollectionCommandViewModel<ViewModelEntity, Filter> FilterCollectionViewModel { get; }
        public CollectionViewModel<ViewModelEntity> CollectionViewModel { get; }
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
using ReactiveUI;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Utility.Services;
using Utility.ViewModels;
using Utility.WPF.Demo.Data.Factory;
using Utility.WPF.Demo.Data.Model;
using System.Reactive.Linq;
using DynamicData;
using System.Linq;
using System.Reactive.Disposables;
using Utility.Models.Filters;
using Utility.Services.Deprecated;

namespace Utility.WPF.Demo.Lists.Infrastructure
{
    public class StringCheckMatchFilter : StringMatchFilter<CheckViewModel>
    {
    }

    public class CheckBoxesViewModel
    {
        public CheckBoxesViewModel()
        {
            var filters = new Filter[] { new StringCheckMatchFilter() };

            CompositeDisposable composite = new();

            Data = new ObservableCollection<CheckViewModel>
            {
                new("1", true),
                new("2", false),
                new("3", false),
            };
            var dataChangeSet = Data.ToObservable().ToObservableChangeSet();

            var filterService = new BaseFilterService<CheckViewModel>();

            foreach (var filter in filters)
            {
                if (filter is ObserverFilter<CheckViewModel> oFilter)
                     composite.Add(dataChangeSet.Subscribe(oFilter));
                 if (filter is IRefreshObservable refresh)
                     composite.Add(refresh.Subscribe(filterService));
            }

            var changeSet = filters.Select(a => (ViewModel<Filter>)new CheckContentViewModel(a, a.Header, false)).ToObservable().ToObservableChangeSet();
            FilterCollectionViewModel = new(changeSet, filterService, default);
            Command = ReactiveCommand.Create<object, object>(a =>
            {
                return a;
            });
        }

        public ObservableCollection<CheckViewModel> Data { get; }

        public CheckedCollectionCommandViewModel<CheckViewModel, Filter> FilterCollectionViewModel { get; }

        public ICommand Command { get; }
    }

    //internal class FilteredCustomCheckBoxesViewModel : IDisposable
    //{
    //    private readonly FilterService<Profile> filterService = new();
    //    private readonly Dictionary<Filter, IDisposable> dictionary = new();

    //    public FilteredCustomCheckBoxesViewModel()
    //    {
    //        var filters = ProfileFilterCollection
    //                            .Filters
    //                            .ToObservable()
    //                            .ToObservableChangeSet();

    //        var profiles = new ProfileCollectionObservable(10, 5)
    //                            .Take(20)
    //                            .ToObservableChangeSet();

    //        filters
    //            .OnItemAdded(filter =>
    //            {
    //                CompositeDisposable composite = new();
    //                if (filter is ObserverFilter<Profile> oFilter)
    //                    composite.Add(profiles.Subscribe(oFilter));
    //                if (filter is IRefreshObservable refresh)
    //                    composite.Add(refresh.Subscribe(filterService));
    //                dictionary[filter] = composite;
    //            })
    //            .OnItemRemoved(filter =>
    //            {
    //                //if (dictionary.ContainsKey(filter))
    //                //{
    //                //    dictionary[filter].Dispose();
    //                //    dictionary.Remove(filter);
    //                //}
    //            })
    //            .Subscribe();

    //        FilterCollectionViewModel = new(filters, filterService, new(false));

    //        var subjects = profiles.Filter(filterService).ToReplaySubject();

    //        CollectionViewModel = new(subjects);
    //        CountViewModel = new(profiles);
    //        FilteredCountViewModel = new(subjects);
    //    }

    //    public FilterCollectionCommandViewModel<Profile, Filter> FilterCollectionViewModel { get; }
    //    public CollectionViewModel<Profile> CollectionViewModel { get; }
    //    public CountViewModel CountViewModel { get; }
    //    public CountViewModel FilteredCountViewModel { get; }

    //    public void Dispose()
    //    {
    //        foreach (var item in dictionary)
    //        {
    //            item.Value.Dispose();
    //        }
    //    }
    //}
}
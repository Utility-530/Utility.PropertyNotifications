using DynamicData;
using System;
using System.Collections.Generic;
using Utility.Service;
using Utility.Models.Filters;
using Utility.WPF.Meta;
using static Utility.WPF.Controls.Meta.ViewModels.AssemblyComboBoxViewModel;

namespace Utility.WPF.Controls.Meta.ViewModels
{
    internal class FilteredCustomCheckBoxesViewModel : IDisposable
    {
        private readonly FilterService<AssemblyKeyValue> filterService = new();
        private readonly Dictionary<Filter, IDisposable> dictionary = new();

        public FilteredCustomCheckBoxesViewModel(
            IObservable<IChangeSet<ViewModelEntity>> profiles,
            IObservable<IChangeSet<Filter>> filters)
        {
            //filters
            //    .OnItemAdded(async filter =>
            //    {
            //        CompositeDisposable composite = new();
            //        if (filter is ObserverFilter<AssemblyKeyValue> oFilter)
            //            composite.Add(profiles.Subscribe(oFilter));
            //        if (filter is IRefreshObservable refresh)
            //        {
            //            composite.Add(refresh.Subscribe(filterService));
            //            composite.Add(refresh.Subscribe(a =>
            //            {
            //            }));
            //        }
            //        //var first = await FilterEntity.Select.Where(a => a.Key == filter.Key).FirstAsync();
            //        //if (first != null)
            //        //{
            //        //    filter.Value = first.Value;
            //        //}
            //        dictionary[filter] = composite;
            //    })
            //    .OnItemRemoved(filter =>
            //    {
            //        //if (dictionary.ContainsKey(filter))
            //        //{
            //        //    dictionary[filter].Dispose();
            //        //    dictionary.Remove(filter);
            //        //}
            //    })
            //    .Subscribe();

            //var checkFilters = filters.Transform(a => new ViewModel(a, a.Header, false));
            //var checkProfiles = profiles.Transform(a => new ViewModel(a, a.Key, false));
            //FilterCollectionViewModel = new(checkFilters, filterService, new(false));

            //var subjects = profiles.Filter(filterService).ToReplaySubject(1000);

            //CollectionViewModel = new(subjects);
            //CountViewModel = new(profiles);
            //FilteredCountViewModel = new(subjects);
        }

        //public CheckedCollectionCommandViewModel<ViewModelEntity, Filter> FilterCollectionViewModel { get; }
        //public CollectionViewModel<ViewModelEntity> CollectionViewModel { get; }
        //public CountViewModel CountViewModel { get; }
        //public CountViewModel FilteredCountViewModel { get; }

        public void Dispose()
        {
            foreach (var item in dictionary)
            {
                item.Value.Dispose();
            }
        }
    }
}
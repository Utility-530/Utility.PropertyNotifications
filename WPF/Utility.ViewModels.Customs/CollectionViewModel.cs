using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using DynamicData.PLinq;
using ReactiveUI;
using Utility.Models;
using Utility.Services.Deprecated;
using Utility.ViewModels.Customs.Infrastructure;

namespace Utility.ViewModels;

//public class CollectionViewModel : ReactiveObject
//{
//    public CollectionViewModel()
//    {
//        SelectionChanged = ReactiveCommand.Create<object, object>(a =>
//        {
//            return a;
//        });
//    }

//    public virtual ICollection Collection { get; }

//    public virtual ICommand SelectionChanged { get; }
//}

public class CollectionViewModel<T> : ViewModel, IObserver<IChangeSet<T>>
{
    private ReplaySubject<IChangeSet<T>> changeSet = new();
    private readonly ReadOnlyObservableCollection<T> collection;

    public CollectionViewModel() : base("Collection")
    {
        changeSet
            .Select(a => a)
            .Bind(out collection)
            .Subscribe();
    }

    public override ICollection<T> Children => collection;

    public override Property Model => throw new NotImplementedException();

    public void OnNext(IChangeSet<T> value)
    {
        changeSet.OnNext(value);
    }
}

//public class CollectionViewModel<T, TKey> : CollectionViewModel
//{
//    private readonly ReadOnlyObservableCollection<T> collection;

//    public CollectionViewModel(IObservable<IChangeSet<T, TKey>> changeSet)
//    {
//        changeSet
//            .Bind(out collection)
//            .Subscribe();
//    }

//    public override ICollection Collection => collection;
//}

//public class CollectionViewModel<T, TKey, TGroupKey> : CollectionViewModel
//{
//    public CollectionViewModel(IObservable<IChangeSet<T, TKey>> changeSet, Func<T, TGroupKey> group)
//    {
//        Collection = changeSet
//                        .ToGroupViewModel(group)
//                        .Collection;
//    }

//    public override ICollection Collection { get; }
//}

public class CollectionGroupViewModel<T, TKey> : ViewModel, IObserver<ClassProperty?> where T : class
{
    private readonly ReplaySubject<ClassProperty?> subject = new(1);
    private readonly ReadOnlyObservableCollection<object> collection;

    public CollectionGroupViewModel(IObservable<IChangeSet<T, TKey>> changeSet, FilterDictionaryService<T> filter, string propertyName) : base(propertyName)
    {
        var type = typeof(T).Name;

        var ungrouped = changeSet
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Filter(filter)
                        .AsObservableCache();

        var grouped = GroupHelper.ConvertGroups<Groupable<T>, T, TKey, string>(changeSet
                        .Filter(filter)
                        .Transform(a => new Groupable<T>(a, new ClassProperty(propertyName, type), subject.WhereNotNull().Select(a => a.Value)))
                        .GroupOnProperty(a => a.GroupProperty))
                        .OnSelectableItemAdded()
                        .AsObservableCache();

        var combined = new SourceList<IObservableList<object>>();

        filter.OnNext(new Dictionary<string, bool>());

        _ = ObservableChangeSet.Create<object>(sourceList =>
        {
            CompositeDisposable compositeDisposable = new();
            IDisposable? disposable = default;
            var switcher = new CacheSourceGroupSwitcher(sourceList, ungrouped, grouped);

            subject
                .StartWith(default(ClassProperty?))
                .DistinctUntilChanged()
                .Subscribe(a =>
                {
                    if (disposable != default)
                    {
                        disposable.Dispose();
                        compositeDisposable.Remove(disposable);
                    }

                    disposable = switcher
                                    .Switch(a != default)
                                    .DisposeWith(compositeDisposable);
                }).DisposeWith(compositeDisposable);
            return compositeDisposable;
        })
        .Bind(out collection)
        .Subscribe();
    }

    public IReadOnlyCollection<ClassProperty> Properties => typeof(T).GetProperties().Select(a => new ClassProperty(a.Name, typeof(T).Name)).ToArray();

    public override ICollection Children => collection;

    public override Property Model => throw new NotImplementedException();
    //public override Model Model { get; }

    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(ClassProperty? value)
    {
        subject.OnNext(value);
    }

    private class CacheSourceGroupSwitcher : CacheSourceConverter
    {
        private readonly IObservableCache<T, TKey> ungrouped;
        private readonly IObservableCache<GroupViewModel<T, TKey, string>, string> grouped;

        public CacheSourceGroupSwitcher(
            ISourceList<object> sourceList,
            IObservableCache<T, TKey> ungrouped,
            IObservableCache<GroupViewModel<T, TKey, string>, string> grouped) : base(sourceList)
        {
            this.ungrouped = ungrouped;
            this.grouped = grouped;
        }

        public IDisposable Switch(bool useGroup)
        {
            SourceList.Clear();
            return Choose()
                .Subscribe(a =>
                {
                    SourceList.EditDiff(a);
                });

            IObservable<IEnumerable<object>> Choose()
            {
                return useGroup == false
                    ? ungrouped
                        .Connect()
                        .ToCollection()
                    : grouped
                        .Connect()
                        .ToCollection();
            }
        }
    }
}

public class CacheSourceConverter
{
    public CacheSourceConverter(ISourceList<object> sourceList)
    {
        SourceList = sourceList;
    }

    public ISourceList<object> SourceList { get; }
}

public class CollectionGroupViewModel<T> : ViewModel, IObserver<Dictionary<string, bool>>, IObserver<ClassProperty> where T : class
{
    private ReplaySubject<ClassProperty> subject = new(1);
    private ReplaySubject<Dictionary<string, bool>> dictionarySubject = new(1);
    private ReplaySubject<Func<T, string>> predicateSubject = new(1);
    //private readonly GroupCollection2ViewModel<Groupable<T>, T, string> viewModel;
    //private readonly FilterDictionaryService<T> filterDictionaryService = new();

    public CollectionGroupViewModel(IObservable<IChangeSet<T>> changeSet, string propertyName) :
        base(propertyName)
    {
        var type = typeof(T).Name;

        predicateSubject
            .Select(a => new FilterDictionaryService<T>(a))
            .Select(filter =>
            {
                var viewModel = changeSet
                     .Filter(filter)
                     .Transform(a => new Groupable<T>(a, new ClassProperty(propertyName, type), subject))
                     .ToGroupOnViewModel(out var disposable);
                return viewModel;
            })
            .Subscribe();
    }

    public override Property Model => throw new NotImplementedException();

    //public override ICollection Children => viewModel.Collection;

    //public IReadOnlyCollection<ClassProperty> Properties => viewModel.Properties;

    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(ClassProperty value)
    {
        subject.OnNext(value);
    }

    public void OnNext(Dictionary<string, bool> value)
    {
        dictionarySubject.OnNext(value);
    }

    public void OnNext(Func<T, string> value)
    {
        predicateSubject.OnNext(value);
    }
}
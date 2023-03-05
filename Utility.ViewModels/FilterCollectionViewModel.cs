using DynamicData;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using Utility.Common.Models;
using Utility.Service;
using Utility.Interfaces.NonGeneric;
using Utility.Models;

namespace Utility.ViewModels;

public class ViewModel<TValue> : ViewModel
{
    public ViewModel(string key, TValue value) : base(key)
    {
        Model = new Property<TValue>(value);
    }

    public override Property<TValue> Model { get; }
}

public class FilterCollectionBaseViewModel<TR> : ReactiveObject
{
    protected readonly ReadOnlyObservableCollection<CheckViewModel> filterCollection;

    public FilterCollectionBaseViewModel(IObservable<IChangeSet<TR>> changeSet, Func<TR, string> keySelector)
    {
        changeSet
           .DistinctValues(keySelector)
           .Transform(a => new CheckViewModel(a, false))
           .Bind(out filterCollection)
           .Subscribe();
    }

    public virtual ICollection FilterCollection => filterCollection;
}

public class FilterCollectionKeyBaseViewModel<TR> : FilterCheckContentCollectionViewModel<TR> where TR : IPredicate, IKey
{
    public FilterCollectionKeyBaseViewModel(IObservable<IChangeSet<TR>> changeSet, Settings settings) :
        base(changeSet
            .Transform(a => (ViewModel<TR>)new ViewModel<TR>(a.Key, a) { /*Settings = settings*/ }), settings)
    {
    }
}

public class FilterCheckContentCollectionViewModel<TR> : ReactiveObject where TR : IPredicate, IKey
{
    protected readonly ReadOnlyObservableCollection<ViewModel<TR>> filterCollection;
    protected readonly ReplaySubject<IChangeSet<ViewModel<TR>, string>> replaySubject = new();

    public FilterCheckContentCollectionViewModel(IObservable<IChangeSet<ViewModel<TR>>> changeSet, Settings settings)
    {
        changeSet
            .Bind(out filterCollection)
            .OnItemAdded(item => item
                                .WhenAnyValue(c => c.IsChecked)
                                .Select(b => item)
                                .ToObservableChangeSet(r => ((TR)r.Model.Value).Key)
                                .Subscribe(replaySubject))
            .Subscribe();
    }

    public virtual ICollection Collection => filterCollection;
}

public class FilterCollectionViewModel<T> : FilterCollectionBaseViewModel<T>
{
    private readonly ReactiveCommand<Dictionary<object, bool?>, Dictionary<string, bool>> command;

    public FilterCollectionViewModel(IObservable<IChangeSet<T>> changeSet, FilterDictionaryService<T> filter, Settings settings) :
        base(changeSet, filter.KeySelector)
    {
        command = ReactiveCommand.Create<Dictionary<object, bool?>, Dictionary<string, bool>>(dictionary =>
        {
            var output = dictionary
                            .Where(a => a.Key != default && a.Value.HasValue)
                            .ToDictionary(a => (string)a.Key, a => a.Value!.Value);
            return output;
        });

        command.Subscribe(filter);
    }

    public ICommand Command => command;
}

public class CheckedCollectionCommandViewModel<T, TR> : FilterCheckContentCollectionViewModel<TR> where TR : IPredicate, IKey
{
    private readonly ReactiveCommand<Dictionary<object, bool?>, Func<T, bool>> command;

    public CheckedCollectionCommandViewModel(IObservable<IChangeSet<ViewModel<TR>>> changeSet, FilterService<T> filterService, Settings settings) : base(changeSet, settings)
    {
        command = ReactiveCommand.Create<Dictionary<object, bool?>, Func<T, bool>>(dictionary =>
        {
            return a => filterCollection.All(o => !(o.IsChecked ?? false) || ((TR)o.Model.Value).Invoke(a));
        });

        replaySubject
            .ToCollection()
            .Select(a => new Func<T, bool>(o => a.All(ob => !(ob.IsChecked ?? false) || ((TR)ob.Model.Value).Invoke(o))))
            .Subscribe(filterService);
    }

    public ICommand Command => command;
}

public class FilterCollectionViewModel<T, TR> : FilterCollectionKeyBaseViewModel<TR> where TR : IPredicate, IKey
{
    public FilterCollectionViewModel(IObservable<IChangeSet<TR>> changeSet, Func<T, bool> filter, FilterService<T> filterService, Settings settings) : base(changeSet, settings)
    {
        Observable.Return(filter).Subscribe(filterService);
    }
}

public record Settings(bool DefaultValue = true);
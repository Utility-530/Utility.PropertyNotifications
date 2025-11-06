using DynamicData;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Utility.Common.Contract;
using Utility.Common.Models;
using Utility.Helpers.Ex;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.ViewModels.Customs.Infrastructure;

namespace Utility.ViewModels;

public static class GroupCollectionBuilder
{
    /// <summary>
    /// For when the group property changes
    /// </summary>
    public static GroupCollectionViewModel<T, TKey, TGroupKey> ToGroupOnViewModel<T, TKey, TGroupKey>(this IObservable<IChangeSet<T, TKey>> changeSet, Expression<Func<T, TGroupKey>> func, out IDisposable disposable)
        where T : INotifyPropertyChanged
    {
        var viewModel = new GroupCollectionViewModel<T, TKey, TGroupKey>("DSFsd");
        disposable = changeSet.GroupOnProperty(func).Subscribe(viewModel);
        return viewModel;
    }

    /// <summary>
    /// For when the group property changes
    /// </summary>
    public static GroupCollectionViewModel<TGroup, TR, TKey, string> ToGroupOnViewModel<TGroup, TR, TKey>(this IObservable<IChangeSet<TGroup, TKey>> changeSet, out IDisposable disposable)
        where TGroup : Groupable<TR>
        where TR : class
    {
        var viewModel = new GroupCollectionViewModel<TGroup, TR, TKey, string>("sdfs");
        disposable = changeSet.GroupOnProperty(a => a.GroupProperty).Subscribe(viewModel);
        return viewModel;
    }

    /// <summary>
    /// For when the group property changes
    /// </summary>
    public static GroupCollectionViewModel<Groupable<TR>, TR, TKey, string> ToGroupOnViewModel<TR, TKey>(this IObservable<IChangeSet<Groupable<TR>, TKey>> changeSet, out IDisposable disposable)
        where TR : class
    {
        return changeSet.ToGroupOnViewModel<Groupable<TR>, TR, TKey>(out disposable);
    }

    /// <summary>
    /// For when the group property changes and have no key
    /// </summary>
    public static GroupCollection2ViewModel<Groupable<TR>, TR, string> ToGroupOnViewModel<TR>(this IObservable<IChangeSet<Groupable<TR>>> changeSet, out IDisposable disposable)
        where TR : class
    {
        var viewModel = new GroupCollection2ViewModel<Groupable<TR>, TR, string>("dsfs");
        disposable = changeSet.GroupOnProperty(a => a.GroupProperty).Subscribe(viewModel);
        return viewModel;
    }

    /// <summary>
    /// For when the group property changes
    /// </summary>
    public static GroupCollectionViewModel<Groupable, object, TKey, string> ToGroupOnViewModel<TKey>(this IObservable<IChangeSet<Groupable, TKey>> changeSet, out IDisposable disposable)
    {
        var viewModel = new GroupCollectionViewModel<Groupable, object, TKey, string>("dsfs");
        disposable = changeSet.GroupOnProperty(a => a.GroupProperty).Subscribe(viewModel);
        return viewModel;
    }

    /// <summary>
    /// For when the group property never changes
    /// </summary>
    public static GroupCollectionViewModel<T, TKey, TGroupKey> ToGroupViewModel<T, TKey, TGroupKey>(this IObservable<IChangeSet<T, TKey>> changeSet, Func<T, TGroupKey> func, out IDisposable disposable)
    {
        var viewModel = new GroupCollectionViewModel<T, TKey, TGroupKey>("dsfs");
        disposable = changeSet.Group(func).Subscribe(viewModel);
        return viewModel;
    }

    public static IObservable<IChangeSet<T>> OnSelectableItemAdded<T>(this IObservable<IChangeSet<T>> objects)
        where T : class, IIsSelected
    {
        return objects
            .OnItemAdded(addItem =>
                                addItem
                                    .WhenAnyValue(a => (a as IGetIsSelected).IsSelected)
                                    .Where(a => a)
                                    .WithLatestFrom(objects.ToCollection(), (a, b) => b)
                                    .Subscribe(collection =>
                                    {
                                        foreach (var item in collection.ToArray())
                                        {
                                            (item as ISetIsSelected).IsSelected = item == addItem;
                                        }
                                    }));
    }

    public static IObservable<IChangeSet<T, TKey>> OnSelectableItemAdded<T, TKey>(this IObservable<IChangeSet<T, TKey>> objects)
    where T : class, IIsSelected
    {
        return objects
            .OnItemAdded(addItem =>
                                addItem
                                    .WhenAnyValue(a => (a as IGetIsSelected).IsSelected)
                                    .Where(a => a)
                                    .WithLatestFrom(objects.ToCollection(), (a, b) => b)
                                    .Subscribe(collection =>
                                    {
                                        foreach (var item in collection.ToArray())
                                        {
                                            (item as ISetIsSelected).IsSelected = item == addItem;
                                        }
                                    }));
    }
}

public class GroupCollectionViewModel<TGroupable, T, TKey, TGroupKey> : GroupCollectionViewModel,
    IObserver<IGroupChangeSet<TGroupable, TKey, TGroupKey>>
    where TGroupable : IGroupable<T>
    where T : class
{
    private readonly ReadOnlyObservableCollection<GroupViewModel<T, TKey, TGroupKey>> collection;
    private ReplaySubject<IGroupChangeSet<TGroupable, TKey, TGroupKey>> replaySubject = new(1);

    public GroupCollectionViewModel(string key) : base(key)
    {
        collection = GroupHelper
                            .ConvertGroups<TGroupable, T, TKey, TGroupKey>(replaySubject)
                            .OnSelectableItemAdded()
                            .ToCollection(out var disposable);

        Connections.Add(disposable);
    }

    public override IReadOnlyCollection<ClassProperty> Properties => typeof(T).GetProperties().Select(a => new ClassProperty(a.Name, typeof(T).Name)).ToArray();

    public override ICollection Children => collection;

    public override Property Model => throw new NotImplementedException();

    public void OnNext(IGroupChangeSet<TGroupable, TKey, TGroupKey> value)
    {
        replaySubject.OnNext(value);
    }
}

public class GroupCollectionViewModel<T, TKey, TGroupKey> : GroupCollectionViewModel, IObserver<IGroupChangeSet<T, TKey, TGroupKey>>
{
    private ReplaySubject<IGroupChangeSet<T, TKey, TGroupKey>> replaySubject = new(1);

    public GroupCollectionViewModel(string key) : base(key)
    {
        Children = GroupHelper.ConvertGroups(replaySubject, Create).ToCollection(Connections);
    }

    public override IReadOnlyCollection<ClassProperty> Properties => typeof(T).GetProperties().Select(a => new ClassProperty(a.Name, typeof(T).Name)).ToArray();

    public override ICollection Children { get; }
    public override Property Model { get; }

    public virtual GroupViewModel<T, TKey, TGroupKey> Create(IGroup<T, TKey, TGroupKey> group)
    {
        return new GroupViewModel<T, TKey, TGroupKey>(group);
    }

    public void OnNext(IGroupChangeSet<T, TKey, TGroupKey> value)
    {
        replaySubject.OnNext(value);
    }
}

public class GroupCollection2ViewModel<TGroupable, T, TGroupKey> : GroupCollectionViewModel, IObserver<IChangeSet<IGroup<TGroupable, TGroupKey>>>
    where TGroupable : IGroupable<T>
    where T : class
{
    private ReplaySubject<IChangeSet<IGroup<TGroupable, TGroupKey>>> replaySubject = new(1);

    public GroupCollection2ViewModel(string key) : base(key)
    {
        Children = GroupHelper.ConvertGroups<TGroupable, T, TGroupKey>(replaySubject).ToCollection(Connections);
    }

    public override IReadOnlyCollection<ClassProperty> Properties => typeof(T).GetProperties().Select(a => new ClassProperty(a.Name, typeof(T).Name)).ToArray();

    public override ICollection Children { get; }
    public override Property Model { get; }

    public virtual GroupViewModel<T, TGroupKey> Create(IGroup<T, TGroupKey> group)
    {
        return new GroupViewModel<T, TGroupKey>(group);
    }

    public void OnNext(IChangeSet<IGroup<TGroupable, TGroupKey>> value)
    {
        replaySubject.OnNext(value);
    }
}

public abstract class GroupCollectionViewModel : ViewModel
{
    protected GroupCollectionViewModel(string key) : base(key)
    {
    }

    public virtual IReadOnlyCollection<ClassProperty> Properties { get; }

    //public virtual ICollection Collection { get; }
}
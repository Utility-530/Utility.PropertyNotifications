using DynamicData;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using Utility.Common.Collection;
using Utility.Common.Contract;
using Utility.Common.Model;
using Utility.Common.Models;
using Utility.Helpers;
using Utility.Helpers.Ex;
using Utility.Models;
using Utility.Observables;

namespace Utility.ViewModels
{
    public record Message(string From, object Content)
    {
        public Guid Guid { get; } = Guid.NewGuid();

        public DateTime Created { get; } = DateTime.Now;
    }

    public record ChildMessage(string From, IEnumerable<IChangeSet<ViewModel, string>> Children) : Message(From, Children);

    public record PropertyChangeMessage(string From, IEnumerable<PropertyChange> Changes) : Message(From, Changes);

    public record PropertyChange<TValue>(string Name, TValue Generic) : PropertyChange(Name, Generic);
    public record PropertyChangeBoolean(string Name, bool Bool) : PropertyChange<bool>(Name, Bool);
    public record PropertyChange(string Name, object Value);

    public record struct TimeChange(DateTime? DateTime);

    //public class ReactiveProperty
    //{
    //}

    public abstract class ViewModel :
        GuidModel<ViewModel>,
        ISubject<Message>,
        IDisposable,
        IIsSelected,
        ISetIsSelected
    {
        private ReplaySubject<Message> messages = new();
        private IDisposable propertyConnection;
        private IDisposable childConnection;

        //Dictionary<string,  childConnection;
        private IObservable<IChangeSet<PropertyChange, string>> changeSet;

        private ReadOnlyObservableCollection<PropertyChange> propertyCollection;
        private ReadOnlyObservableCollection<ViewModel> childCollection;
        protected Utility.Observables.Generic.Disposable disposer = new();
        private ReplaySubject<Message> childMessages;
        private ReplaySubject<Message> parentMessages;
        private int count;
        private readonly List<string> siblingKeys;

        public ViewModel(string key)
        {
            //Guid = Guid.NewGuid();
            changeSet = messages
                .OfType<PropertyChangeMessage>()
                .SelectMany(a => a.Changes)
                .ToObservableChangeSet(a => a.Name)
                .Replay()
                .AutoConnect();
            Key = key;
        }

        public string Key { get; }

        //public Settings Settings { get; set; }

        public abstract Property Model { get; }

        public virtual IEnumerable Children
        {
            get
            {
                childConnection ??= messages
                                        .OfType<ChildMessage>()
                                        .SelectMany(a => a.Children)
                                        .OnItemAdded(item =>
                                        {
                                            disposer.Add(item.Where(a => a.From != this.Key).Subscribe(childMessages));
                                            item.AddParent(this.Key);
                                            disposer.Add(this.Where(a => a.From != item.Key).Subscribe(item));
                                        })
                                        .Bind(out childCollection)
                                        .Subscribe()
                                        .DisposeWith(disposer);
                return childCollection;
            }
        }

        private ViewModel? selectedChild;
        private int selectedIndex;

        public ViewModel? SelectedChild
        {
            get => selectedChild;
            set => this.RaiseAndSetIfChanged(ref selectedChild, value);
        }

        public int SelectedIndex
        {
            get => selectedIndex;
            set => this.RaiseAndSetIfChanged(ref selectedIndex, value);
        }

        public void Replace(IList<ViewModel> replacements, ViewModel selectedNote)
        {
            //notes.SelectChanges()
            //    .Subscribe(a =>
            //    {
            //        if (a.Action == NotifyCollectionChangedAction.Add)
            //            this.notes.AddRange(a.NewItems.OfType<NoteViewModel>().ToArray());
            //    });

            for (int i = this.childCollection.Count - 1; i >= 0; i--)
            {
                //await Task.Delay(50);
                //this.childCollection.RemoveAt(i);

                messages.OnNext(new ChildMessage(this.Key,
                    new IChangeSet<ViewModel, string>[] {
                        new ChangeSet<ViewModel, string>(new[] {
                        new Change<ViewModel, string>(ChangeReason.Remove, childCollection[i].Key, childCollection[i]) }) }));
            }

            for (int i = 0; i < replacements.Count; i++)
            {
                messages.OnNext(new ChildMessage(this.Key,
                  new IChangeSet<ViewModel, string>[] {
                        new ChangeSet<ViewModel, string>(new[] {
                        new Change<ViewModel, string>(ChangeReason.Add, replacements[i].Key, childCollection[i]) }) }));
            }
            //for (int i = notes.Count - 1; i >= 0; i--)
            //{
            //    //await Task.Delay(50);
            //    this.notes.Add(notes[i]);
            //}

            this.selectedChild = default;
            //await Task.Delay(500);
            this.selectedChild = selectedNote;

            this.WhenAnyValue(a => a.SelectedChild)
                .Subscribe(a => SelectedIndex = replacements.IndexOf(a));

            this.WhenAnyValue(a => a.SelectedIndex)
                .Subscribe(a =>
                {
                    if (a < 0)
                    {
                        if (replacements.Count > 0)
                            SelectedIndex = 0;
                        return;
                    }

                    if (replacements.Count == 1)
                    {
                        if (SelectedChild.Equals(default) == false)
                            return;
                        SelectedChild = replacements[0];
                        SelectedIndex = 0;
                        return;
                    }

                    if (replacements.Count > a)
                    {
                        SelectedChild = replacements[a];
                        SelectedIndex = a;
                    }
                });
        }

        public bool IsSelected { get; set; }

        public string? ParentKey { get; private set; }

        //public Guid Guid { get; } = Guid.NewGuid();

        public DateTime Created { get; } = DateTime.Now;

        public IEnumerable PropertyCollection { get; }

        public bool? IsChecked => Connect<bool>()?.Generic;

        public bool IsChildrenConnected => childConnection != default;

        public bool IsPropertiesConnected => propertyConnection != default;

        public virtual int Count => count;

        public void AddParent(string key)
        {
            ParentKey = key;
        }

        public void AddSibling(string key)
        {
            siblingKeys.Add(key);
        }

        public IObservableCollection<IDisposable> Connections => disposer;

        private PropertyChange<T>? Connect<T>([CallerMemberName] string? name = null)
        {
            disposer.Add(propertyConnection ??= changeSet
                                    .Bind(out propertyCollection)
                                    .Subscribe());
            return propertyCollection
                    .FirstOrDefault(a => a.Name == name) as PropertyChange<T>;
        }

        public void OnCompleted()
        {
            throw new NotImplementedException("sd33f2 vff");
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException("sdf2 vff");
        }

        public void OnNext(Message value)
        {
            messages.OnNext(value);

            if (value is ChildMessage { Children: var children })
            {
                foreach (var child in children)
                {
                    count += child.Adds - child.Removes;
                }
                this.RaisePropertyChanged(nameof(Count));
            }
        }

        public IDisposable Subscribe(IObserver<Message> observer)
        {
            return
                this.Changes()
                .Select(a => new PropertyChangeMessage(this.Key, new[] { new PropertyChange(a.PropertyName, a.NewValue) }))
                .Merge(childMessages)
                .Merge(parentMessages)
                .Subscribe(observer);
        }

        //public PropertyChange ChildChange { get; set; }

        //public bool IsReadOnly { get => isReadOnly; set => isReadOnly = value; }

        //    public static IObservable<IChangeSet<GroupViewModel<T, TGroupKey>>> ConvertGroups<TGroup, T, TGroupKey>(this IObservable<IChangeSet<IGroup<TGroup, TGroupKey>>> groups)
        //where TGroup : IGroupable<T>
        //where T : class
        //    {
        //        return groups
        //            .Transform(t => new GroupViewModel<T, TGroupKey>(new Group<T, TGroupKey>(t.GroupKey, t.List.Items.Select(a => a.Value))))
        //            .ObserveOn(RxApp.MainThreadScheduler);
        //    }

        //public class GroupCollectionViewModel<T, TKey, TGroupKey> : GroupCollectionViewModel
        //{
        //    public GroupCollectionViewModel(IObservable<IGroupChangeSet<T, TKey, TGroupKey>> changeSet)
        //    {
        //        Collection = GroupHelper.ConvertGroups(changeSet, Create).ToCollection(CompositeDisposable);
        //    }

        //    public override IReadOnlyCollection<ClassProperty> Properties => typeof(T).GetProperties().Select(a => new ClassProperty(a.Name, typeof(T).Name)).ToArray();

        //    public override ICollection Collection { get; }

        //    public virtual GroupViewModel<T, TKey, TGroupKey> Create(IGroup<T, TKey, TGroupKey> group)
        //    {
        //        return new GroupViewModel<T, TKey, TGroupKey>(group);
        //    }
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

        #region dispose

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            disposer.Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        #endregion dispose
    }

    //public class Disposer : ObservableCollection<IDisposable>
    //{
    //    //private readonly List<IDisposable> disposables = new();
    //    private bool disposed;

    //    public Disposer()
    //    {
    //        new System.Reactive.Disposables.CompositeDisposable();
    //    }

    //    public Disposer(IDisposable disposable)
    //    {
    //        Add(disposable);
    //    }

    //    //public void Add(IDisposable disposable)
    //    //{
    //    //    Insert(0, disposable);
    //    //}

    //    public void Dispose(bool disposing)
    //    {
    //        // Check to see if Dispose has already been called.
    //        if (!this.disposed)
    //        {
    //            // If disposing equals true, dispose all managed and unmanaged resources.
    //            if (disposing)
    //            {
    //                // Dispose managed resources.
    //                foreach (var disposable in this)
    //                    disposable.Dispose();
    //            }
    //        }
    //        disposed = true;
    //    }
    //}
}
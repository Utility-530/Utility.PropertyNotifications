using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using DynamicData;
using ReactiveUI;
using Utility.Common.Models;
using Utility.Helpers;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.PropertyNotifications;

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
        protected Utility.Observables.Disposable disposer = new();
        private ReplaySubject<Message> childMessages;
        private ReplaySubject<Message> parentMessages;
        private int count;
        private readonly List<string> siblingKeys;
        private ViewModel? selectedChild;
        private int selectedIndex;

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

        public virtual string Key { get; }

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
            if (childCollection == null)
                return;
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
                if (childCollection.Count > i)
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

        public virtual bool IsSelected { get; set; }

        public string? ParentKey { get; private set; }

        //public Guid Guid { get; } = Guid.NewGuid();

        public DateTime Created { get; } = DateTime.Now;

        public IEnumerable PropertyCollection { get; }

        public virtual bool? IsChecked
        {
            get
            {
                return Connect<bool>()?.Generic;
            }
            set => throw new NotImplementedException();
        }

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

        public virtual void OnCompleted()
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
                this.WhenChanged()
                .Select(a => new PropertyChangeMessage(this.Key, new[] { new PropertyChange(a.Name, a.Value) }))
                .Merge(childMessages)
                .Merge(parentMessages)
                .Subscribe(observer);
        }

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
}
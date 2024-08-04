

using Splat;
using System.Reactive;
using System.Reactive.Subjects;
using Utility.Helpers.NonGeneric;
using Utility.Repos;

namespace Utility.Descriptors
{
    public interface IValueCollection
    {
        IEnumerable Collection { get; }
    }

    internal record CollectionDescriptor(Descriptor PropertyDescriptor, Type ElementType, IEnumerable Collection) : BasePropertyDescriptor(PropertyDescriptor, Collection),
        ICollectionDescriptor,
        IValueCollection,
        IRefresh
    {
        ReplaySubject<Change<IDescriptor>> replaySubject = new();
        List<CollectionItemDescriptor> descriptors = new();

        private static ITreeRepository repo => Locator.Current.GetService<ITreeRepository>();

        public static string _Name => "Collection";

        public override string? Name => _Name;
        
        public override IObservable<object> Children
        {
            get
            {
                return Observable.Create<Change<IDescriptor>>(async observer =>
                {
                    AddHeaderDescriptor(observer, ElementType, Instance.GetType());
                    AddFromInstance(observer);
                    AddFromRefreshes(observer);
                    return AddFromChanges(observer);
                });


                IDisposable AddFromChanges(IObserver<Change<IDescriptor>> observer)
                {
                    if (Instance is not INotifyCollectionChanged collectionChanged)
                    {
                        return Disposable.Empty;
                    }

                    return collectionChanged
                        .SelectChanges()
                        .Subscribe(a =>
                        {
                            switch (a.Action)
                            {
                                case NotifyCollectionChangedAction.Add:
                                    {
                                        int lastIndex = repo.MaxIndex(Guid) ?? 0 + 1;
                                        foreach (var item in a.NewItems)
                                            Next(observer, item, item.GetType(), Type, Changes.Type.Add, lastIndex);
                                        break;
                                    }
                                case NotifyCollectionChangedAction.Remove:
                                    foreach (var item in a.OldItems)
                                    {
                                        var find = descriptors.Single(a => a.Item == item);
                                        repo.Remove(find.Guid);
                                        descriptors.Remove(find);
                                        observer.OnNext(new(find, Changes.Type.Remove));
                                    }
                                    break;
                                case NotifyCollectionChangedAction.Reset:
                                    observer.OnNext(new(null, Changes.Type.Reset));
                                    break;
                            }
                        });
                }
            }
        }

        public void Refresh()
        {
            replaySubject.OnNext(new(default, Utility.Changes.Type.Reset));
            AddHeaderDescriptor(replaySubject, ElementType, Instance.GetType());
            AddFromInstance(replaySubject);
        }

        // collection
        public virtual int Count => Instance is IEnumerable enumerable ? enumerable.Count() : 0;

        public override async void Initialise(object? item = null)
        {
            var observer = Observer.Create<Change<IDescriptor>>((a) =>
            {
                if (a.Type == Changes.Type.Add && a.Value is IInitialise initialise)
                {
                    initialise.Initialise();
                }
            });
            var tables = await repo.SelectKeys(Guid);
            AddFromDataSource(observer, tables);
        }

        public override void Finalise(object? item = null)
        {

            var observer = Observer.Create<Change<IDescriptor>>((a) =>
            {
                if (a.Type == Changes.Type.Add && a.Value is IFinalise finalise)
                {
                    finalise.Finalise();
                }
            });
            AddFromInstance(observer);
        }

        void AddFromRefreshes(IObserver<Change<IDescriptor>> observer)
        {
            replaySubject.Subscribe(observer);
        }

        async void AddHeaderDescriptor(IObserver<Change<IDescriptor>> observer, Type elementType, Type componentType)
        {
            var descriptor = new CollectionHeadersDescriptor(elementType, componentType);
            var _guid = await repo.Find(Guid, elementType.Name, elementType, 0);
            descriptor.Guid = _guid;
            observer.OnNext(new(descriptor, Changes.Type.Add));
        }

        async void AddFromInstance(IObserver<Change<IDescriptor>> observer)
        {
            int i = 1;
            foreach (var item in Collection)
            {
                var type = item.GetType();
                var _guid = await repo.Find(Guid, type.Name, type, i);
                Next(observer, item, item.GetType(), Type, Changes.Type.Add, i++);
            }
        }

        void AddFromDataSource(IObserver<Change<IDescriptor>> observer, IEnumerable<Key> tables)
        {
            foreach (var table in tables)
            {
                //&& descriptors.SingleOrDefault(a => a.Index == table.Index) is not { } x
                if (table.Index is not 0 && table.Type is { } elementType)
                {
                    if (elementType.IsValueType || elementType.GetConstructor(System.Type.EmptyTypes) != null)
                    {
                        var item = Activator.CreateInstance(elementType);
                        if (Instance is IList collection)
                        {
                            if (collection.Count < table.Index)
                                //collection.Insert(table.Index ?? throw new Exception(" 3 efsd"), item);
                                collection.Add(item);
                        }
                        else
                        {
                            throw new Exception(" s898d");
                        }
                        Next(observer, item, elementType, Type, Changes.Type.Add, table.Index.Value);
                    }
                    else
                    {
                        throw new Exception("s;)dfsd979797");
                    }
                }
            }
        }

        async void Next(IObserver<Change<IDescriptor>> observer, object item, Type type, Type parentType, Changes.Type changeType, int i)
        {
            var descriptor = await DescriptorFactory.CreateItem(item, i, type, parentType, Guid);
            observer.OnNext(new(descriptor, changeType));
        }
    }
}



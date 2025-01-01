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
        List<ICollectionItemDescriptor> descriptors = new();

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
                    repo
                    .SelectKeys(Guid)
                    .Subscribe(tables =>
                    {
                        AddFromDataSource(observer, tables);
                    });
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
                                    descriptors.Clear();
                                    observer.OnNext(new(null, Changes.Type.Reset));
                                    break;
                            }
                        });
                }
            }
        }

        public void Refresh()
        {
            AddFromInstance(replaySubject);
        }

        // collection
        public virtual int Count => Instance is IEnumerable enumerable ? enumerable.Count() : 0;

        public override void Initialise(object? item = null)
        {
            //var observer = Observer.Create<Change<IDescriptor>>((a) =>
            //{
            //    if (a.Type == Changes.Type.Add && a.Value is IInitialise initialise)
            //    {
            //        initialise.Initialise();
            //    }
            //});    
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

        void AddHeaderDescriptor(IObserver<Change<IDescriptor>> observer, Type elementType, Type componentType)
        {
            var descriptor = new CollectionHeadersDescriptor(elementType, componentType);
            repo.Find(Guid, elementType.Name, elementType, 0)
                .Subscribe(c =>
                {
                    descriptor.Guid = c.Guid;
                    observer.OnNext(new(descriptor, Changes.Type.Add));
                });
        }

        void AddFromInstance(IObserver<Change<IDescriptor>> observer)
        {
            foreach (var item in Collection)
            {
                if (descriptors.Any(a => a.Item == item) == false)
                {
                    int i = (descriptors.LastOrDefault()?.Index ?? 0) + 1;
                    Next(observer, item, item.GetType(), Type, Changes.Type.Add, i);
                }
            }

            foreach (var descriptor in descriptors.ToArray())
            {
                //i++;
                if (Contains(Collection, descriptor.Item) == false)
                {
                    observer.OnNext(new(descriptor, Changes.Type.Remove));
                    repo.Remove(descriptor.Guid);
                    descriptors.Remove(descriptor);
                }
            }

            bool Contains(IEnumerable source, object value)
            {
                foreach (var i in source)
                {
                    if (Equals(i, value))
                        return true;
                }
                return false;
            }
        }

        void AddFromDataSource(IObserver<Change<IDescriptor>> observer, IEnumerable<Key> keys)
        {
            foreach (var key in keys)
            {
                //&& descriptors.SingleOrDefault(a => a.Index == table.Index) is not { } x
                if (key.Index is > 0 && key.Instance is { } item)
                {
                    if (Instance is IList collection)
                    {
                        if (collection.Count < key.Index)
                            //collection.Insert(table.Index ?? throw new Exception(" 3 efsd"), item);
                            collection.Add(item);
                    }
                    else
                    {
                        throw new Exception(" s898d");
                    }
                    Next(observer, item, item.GetType(), Type, Changes.Type.Add, key.Index.Value, false, key.Removed);
                }
            }
        }

        void Next(IObserver<Change<IDescriptor>> observer, object item, Type type, Type parentType, Changes.Type changeType, int i, bool refresh = false, DateTime? removed = null)
        {
            DescriptorFactory.CreateItem(item, i, type, parentType, Guid)
                .Subscribe(descriptor =>
                {
                    var refDescriptor = (CollectionItemReferenceDescriptor)descriptor;
                    refDescriptor.Removed = removed;
                    observer.OnNext(new(descriptor, changeType));
                    descriptors.Add(refDescriptor);
                    if (refresh)
                        descriptor.Initialise();
                });
        }
    }
}



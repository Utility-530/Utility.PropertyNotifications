using Splat;

namespace Utility.Descriptors;


public record ChildrenResponse(IMemberDescriptor Descriptor);


public partial class ChildPropertyExplorer
{
    static Dictionary<Guid, DateValue> _descriptorsCache = new();
    private static ITreeRepository repo => Locator.Current.GetService<ITreeRepository>();

    public static IObservable<Change<IMemberDescriptor>> Explore(object Instance, IMemberDescriptor memberDescriptor)
    {

        var descriptors = TypeDescriptor.GetProperties(Instance);
        int count = descriptors.Count;

        CompositeDisposable composite = new();

        return Observable.Create<Change<IMemberDescriptor>>(async observer =>
        {
            if (count != 0)
            {
                foreach (Descriptor descriptor in descriptors)
                {


                    var _guid = await repo.Find(memberDescriptor.Guid, descriptor.Name, descriptor.PropertyType);

                    var propertyDescriptor = ObjectConverter.ToValue(Instance, descriptor);
                    propertyDescriptor.Guid = _guid;

                    //var propertyDescriptor = _descriptorsCache.GetValueOrCreate(_guid, () =>
                    //{               
                    //    return propertyDescriptor;
                    //});

                    try
                    {
                        if (propertyDescriptor is StructValue)
                        {

                        }
                        if (propertyDescriptor is not ObjectValue objectValue && propertyDescriptor is not TypeValue && propertyDescriptor.IsReadOnly == false)
                        {
                            var valueFromRepo = repo.Get(_guid);
                            if (valueFromRepo is DateValue { DateTime: { } added, Value: { } value } dateValue)
                            {
                                if (_descriptorsCache.ContainsKey(_guid) == false)
                                {
                                    _descriptorsCache[_guid] = new(dateValue.DateTime, value);
                                    propertyDescriptor.SetValue(value);
                                    if (propertyDescriptor is IRaisePropertyChanged changed)
                                        changed.RaisePropertyChanged(value);
                                }
                            }

                            if (propertyDescriptor.GetValue() is { } _value && _value.Equals(_descriptorsCache.GetValueOrDefault(_guid).Value ?? Activator.CreateInstance(propertyDescriptor.Type)) == false)
                            {
                                _descriptorsCache[_guid] = new(DateTime.Now, _value);
                                repo.Set(_guid, _value, _descriptorsCache[_guid].DateTime);
                            }

                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    Register(_guid, propertyDescriptor);
                    observer.OnNext(new Change<IMemberDescriptor>(propertyDescriptor, Changes.Type.Add));
                }
            }
            return composite;
        });
    }

    static void Register(Guid guid, PropertyDescriptor propertyDescriptor)
    {
        //await initialisationTask;
        if (propertyDescriptor is INotifyPropertyReceived propertyReceived)
            propertyReceived
                .WhenReceived()
                .Subscribe(a =>
                {
                    //propertyDescriptor.LastPersistence = new(DateTime.Now, a.Value);
                    repo.Set(guid, a.Value, DateTime.Now);
                });

        if (propertyDescriptor is INotifyPropertyCalled propertyCalled)
            propertyCalled
                .WhenCalled()
                .Subscribe(a =>
                {
                    //if (repo.Get(guid) is { } value)
                    //    if (value.Equals(a.Value) == false)
                    //        if (propertyCalled is IRaisePropertyChanged changed)
                    //        {
                    //            changed.RaisePropertyChanged(value);
                    //        }
                });
    }

    public static IObservable<Change<IMemberDescriptor>> CollectionItemDescriptors(object data, IMemberDescriptor memberDescriptor)
    {
        List<CollectionItemDescriptor> descriptors = new();
        return Observable.Create<Change<IMemberDescriptor>>(async observer =>
        {
            int i = 1;
            if (data is not IEnumerable enumerable || data is string s || data.GetType() is not Type _type || _type.GetCollectionElementType() is not Type elementType)
            {
                return Disposable.Empty; ;
            }

            var tables = await repo.Select(memberDescriptor.Guid, elementType.Name);

            AddHeaderDescriptor(observer, elementType, _type);

            AddFromInstance(observer, enumerable, i, tables);

            AddFromDataSource(observer, tables, elementType);

            var lastIndex = repo.MaxIndex(memberDescriptor.Guid) + 1 ?? i;

            return AddFromChanges(observer, lastIndex);
        });


        async void AddFromInstance(IObserver<Change<IMemberDescriptor>> observer, IEnumerable enumerable, int i, IEnumerable<Selection> tables)
        {
            foreach (var item in enumerable)
            {
                var type = item.GetType();
                var _guid = await repo.Find(memberDescriptor.Guid, type.Name, type, i);
                Next(observer, item, item.GetType(), Changes.Type.Add, i++);
            }
        }

        async void AddHeaderDescriptor(IObserver<Change<IMemberDescriptor>> observer, Type elementType, Type componentType)
        {
            var descriptor = new CollectionHeaderDescriptor(elementType, componentType);
            var _guid = await repo.Find(memberDescriptor.Guid, elementType.Name, elementType, 0);
            descriptor.Guid = _guid;
            observer.OnNext(new(descriptor, Changes.Type.Add));
        }

        void AddFromDataSource(IObserver<Change<IMemberDescriptor>> observer, IEnumerable<Selection> tables, Type elementType)
        {
            foreach (var table in tables)
            {
                if (table.Index is not 0 && descriptors.SingleOrDefault(a => a.Index == table.Index) is not { } x)
                {
                    if (elementType.IsValueType || elementType.GetConstructor(System.Type.EmptyTypes) != null)
                    {
                        var item = Activator.CreateInstance(elementType);
                        if (data is IList collection)
                        {
                            collection.Add(item);
                        }
                        Next(observer, item, elementType, Changes.Type.Add, table.Index.Value);
                    }
                    else
                    {
                        throw new Exception("s;)dfsd979797");
                    }
                }
            }
        }

        async void Next(IObserver<Change<IMemberDescriptor>> observer, object item, Type type, Changes.Type changeType, int i)
        {
            var descriptor = new CollectionItemDescriptor(item, i, type);
            descriptors.Add(descriptor);
            var _guid = await repo.Find(memberDescriptor.Guid, type.Name, type, i);
            descriptor.Guid = _guid;
            observer.OnNext(new(descriptor, changeType));
        }

        IDisposable AddFromChanges(IObserver<Change<IMemberDescriptor>> observer, int lastIndex)
        {
            if (data is not INotifyCollectionChanged collectionChanged)
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
                                foreach (var item in a.NewItems)
                                    Next(observer, item, item.GetType(), Changes.Type.Add, lastIndex++);
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
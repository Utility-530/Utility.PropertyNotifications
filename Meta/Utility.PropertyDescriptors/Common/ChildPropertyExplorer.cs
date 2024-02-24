using System.Collections.Specialized;
using Utility.Helpers.Ex;
using Utility.Observables.NonGeneric;

namespace Utility.Descriptors
{

    public record ChildrenResponse(IMemberDescriptor Descriptor);


    public partial class ChildPropertyExplorer
    {
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
                        var _guid = await GuidRepository.Instance.Find(memberDescriptor.Guid, descriptor.Name, descriptor.PropertyType);
                        var propertyDescriptor = ObjectConverter.ToValue(Instance, descriptor);
                        propertyDescriptor.Guid = _guid;

                        try
                        {
                            if (propertyDescriptor is not ObjectValue objectValue && propertyDescriptor is not TypeValue)
                            {
                                if (GuidRepository.Instance.Get(_guid) is { } value)
                                {
                                    propertyDescriptor.SetValue(value);
                                }
                                else if (propertyDescriptor.GetValue() is { } _value)
                                    GuidRepository.Instance.Set(_guid, _value);
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        GuidRepository.Instance.Register(_guid, propertyDescriptor as INotifyPropertyCalled);
                        GuidRepository.Instance.Register(_guid, propertyDescriptor as INotifyPropertyReceived);
                        observer.OnNext(new Change<IMemberDescriptor>(propertyDescriptor, Changes.Type.Add));
                    }
                }
                return composite;
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
                    return Disposer.Empty; ;
                }

                var tables = await GuidRepository.Instance.Select(memberDescriptor.Guid, elementType.Name);

                AddHeaderDescriptor(observer, elementType, _type);

                AddFromInstance(observer, enumerable, i, tables);

                AddFromDataSource(observer, tables, elementType);

                var lastIndex = GuidRepository.Instance.MaxIndex(memberDescriptor.Guid) + 1 ?? i;

                return AddFromChanges(observer, lastIndex);
            });


            async void AddFromInstance(IObserver<Change<IMemberDescriptor>> observer, IEnumerable enumerable, int i, IEnumerable<(Guid guid, Type type, int? index)> tables)
            {
                foreach (var item in enumerable)
                {
                    var type = item.GetType();
                    var _guid = await GuidRepository.Instance.Find(memberDescriptor.Guid, type.Name, type, i);
                    Next(observer, item, item.GetType(), Changes.Type.Add, i++);
                }
            }

            async void AddHeaderDescriptor(IObserver<Change<IMemberDescriptor>> observer, Type elementType, Type componentType)
            {
                var descriptor = new CollectionHeaderDescriptor(elementType, componentType);
                var _guid = await GuidRepository.Instance.Find(memberDescriptor.Guid, elementType.Name, elementType, 0);
                descriptor.Guid = _guid;
                observer.OnNext(new(descriptor, Changes.Type.Add));
            }

            void AddFromDataSource(IObserver<Change<IMemberDescriptor>> observer, IEnumerable<(Guid guid, Type type, int? index)> tables, Type elementType)
            {
                foreach (var table in tables)
                {
                    if (table.index is not 0 && descriptors.SingleOrDefault(a => a.Index == table.index) is not { } x)
                    {
                        if (elementType.IsValueType || elementType.GetConstructor(System.Type.EmptyTypes) != null)
                        {
                            var item = Activator.CreateInstance(elementType);
                            if (data is IList collection)
                            {
                                collection.Add(item);
                            }
                            Next(observer, item, elementType, Changes.Type.Add, table.index.Value);
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
                var _guid = await GuidRepository.Instance.Find(memberDescriptor.Guid, type.Name, type, i);
                descriptor.Guid = _guid;
                observer.OnNext(new(descriptor, changeType));
            }

            IDisposable AddFromChanges(IObserver<Change<IMemberDescriptor>> observer, int lastIndex)
            {
                if (data is not INotifyCollectionChanged collectionChanged)
                {
                    return Disposer.Empty;
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
                                    GuidRepository.Instance.Remove(find.Guid);
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

}
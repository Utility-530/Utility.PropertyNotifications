using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Utility.Changes;
using Utility.Helpers.Ex;
using Utility.Nodes.Reflections;
using Utility.Observables.NonGeneric;
using Utility.PropertyNotifications;
using Descriptor = System.ComponentModel.PropertyDescriptor;
using Utility.Helpers;

namespace Utility.PropertyDescriptors
{

    public record ChildrenResponse(IMemberDescriptor Descriptor);


    public partial class ChildPropertyExplorer
    {
        public static IObservable<Change<IMemberDescriptor>> Explore(object Instance, IMemberDescriptor memberDescriptor)
        {

            var descriptors = TypeDescriptor.GetProperties(Instance);//  descriptor.GetChildProperties().Cast<Descriptor>().ToArray();
            int count = descriptors.Count;

            CompositeDisposable composite = new();

            return Observable.Create<Change<IMemberDescriptor>>(async observer =>
              {
                  if (count != 0)
                  {
                      foreach (Descriptor descriptor in descriptors)
                      {
                          var _guid = await GuidRepository.Instance.Find(memberDescriptor.Guid, descriptor.Name);
                          var value = ObjectConverter.ToValue(Instance, descriptor);
                          value.Guid = _guid;
                          ValueRepository.Instance.Register(_guid, value as INotifyPropertyCalled);
                          ValueRepository.Instance.Register(_guid, value as INotifyPropertyReceived);
                          observer.OnNext(new Change<IMemberDescriptor>(value, Changes.Type.Add));
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
                int i = 0;
                if (data is not IEnumerable enumerable || data is string s || data.GetType() is not System.Type _type || _type.GetCollectionElementType() is not System.Type elementType)
                {
                    return Disposer.Empty; ;
                }             

                var tables = await GuidRepository.Instance.Select(memberDescriptor.Guid, elementType.Name);

                foreach (var item in enumerable)
                {
                    var _guid = await GuidRepository.Instance.Find(memberDescriptor.Guid, item.GetType().Name, i);

                    if (tables.SingleOrDefault(a => a.Guid == _guid) is { Removed: { } removed } table)
                    {

                    }
                    else
                    {
                        Next(observer, item, elementType, Changes.Type.Add, i++);
                    }
                }

                foreach (var table in tables)
                {
                    if (table.Removed is not { } removed && descriptors.SingleOrDefault(a => a.Index == table._Index) is not { } x)
                    {
                        if (elementType.IsValueType || elementType.GetConstructor(System.Type.EmptyTypes) != null)
                        {
                            var item = Activator.CreateInstance(elementType);
                            Next(observer, item, elementType, Changes.Type.Add, table._Index.Value);
                        }
                        else
                        {
                            throw new Exception("s;)dfsd979797");
                        }
                    }
                }

                var lastIndex = GuidRepository.Instance.MaxIndex(memberDescriptor.Guid) + 1 ?? i;
     
                if (data is INotifyCollectionChanged collectionChanged)
                {
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
                else
                {
                    return Disposer.Empty;
                }
            });

            async void Next(IObserver<Change<IMemberDescriptor>> observer, object item, System.Type componentType, Changes.Type changeType, int i)
            {

                var descriptor = new CollectionItemDescriptor(item, i, componentType);
                descriptors.Add(descriptor);
                var _guid = await GuidRepository.Instance.Find(memberDescriptor.Guid, componentType.Name, i);
                descriptor.Guid = _guid;
                observer.OnNext(new(descriptor, changeType));          
            }
        }
    }

}
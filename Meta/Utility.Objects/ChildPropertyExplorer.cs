using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Utility.Helpers;
using Utility.Helpers.Ex;
using Utility.Helpers.NonGeneric;
using Utility.Models;
using Utility.Observables.NonGeneric;
using Utility.PropertyDescriptors;

namespace Utility.Objects
{

    public record ChildrenResponse(PropertyDescriptor Descriptor, ChangeType ChangeType);


    public partial class ChildPropertyExplorer
    {
        public static IObservable<ChildrenResponse> Explore(object Instance, PropertyDescriptor descriptor)
        {

            var descriptors = ChildDescriptors(descriptor).ToArray();
            int count = descriptors.Length + Count(Instance);
            int i = 0;
            CompositeDisposable composite = new();

            return Observable.Create<ChildrenResponse>(observer =>
              {
                  if (count == 0)
                  {
                      return composite;
                      //observer.OnNext(new ChildrenResponse());
                  }
                  foreach (var descriptor in descriptors)
                  {
                      observer.OnNext(new ChildrenResponse(descriptor, ChangeType.Add));
                  }
                  SubscribeToCollectionItemDescriptors(observer)
                  .DisposeWith(composite);
                  return composite;
              });

            IDisposable SubscribeToCollectionItemDescriptors(IObserver<ChildrenResponse> obs)
            {
                return 
                    CollectionItemDescriptors(Instance)
                    .Subscribe(descriptor =>
                    {
                        if (descriptor.Value == null)
                        {
                            OnChange(descriptor, default);
                            return;
                        }
                        OnChange(descriptor, true);
                    },
                    () =>
                    {
                        //observer.OnCompleted();
                    });

                void OnChange(Change<CollectionItemDescriptor> change, bool include)
                {

                    if (change.Type == ChangeType.Add)
                    {
                        FromCollectionDescriptor(change.Value)
                        .Subscribe(p =>
                        {
                            i++;
                            obs.OnNext(p);
                            if (count == i)
                            {
                                if (Instance is not INotifyCollectionChanged)
                                    obs.OnCompleted();
                            }
                        })
                        .DisposeWith(composite);

                    }
                    else if (change.Type == ChangeType.Remove)
                        obs.OnNext(new ChildrenResponse(change.Value, ChangeType.Remove));

                    else if (change.Type == ChangeType.Reset)
                        obs.OnNext(new ChildrenResponse(default, ChangeType.Reset));
                }

                IObservable<ChildrenResponse> FromCollectionDescriptor(CollectionItemDescriptor descriptor)
                {
                    return Observable.Return(new ChildrenResponse(descriptor, ChangeType.Add));
                }

                IObservable<Change<CollectionItemDescriptor>> CollectionItemDescriptors(object data)
                {
                    return Observable.Create<Change<CollectionItemDescriptor>>(observer =>
                    {
                        int i = 0;
                        if (data is IEnumerable enumerable && data is not string s)
                            foreach (var item in enumerable)
                            {
                                Next(observer, item, data.GetType(), ChangeType.Add, i++);
                            }
                        if (data is INotifyCollectionChanged collectionChanged)
                        {
                            collectionChanged.SelectChanges()
                            .Subscribe(a =>
                            {
                                switch (a.Action)
                                {
                                    case NotifyCollectionChangedAction.Add:
                                        {
                                            foreach (var item in a.NewItems)
                                                Next(observer, item, data.GetType(), ChangeType.Add, i++);
                                            break;
                                        }
                                    case NotifyCollectionChangedAction.Remove:
                                        foreach (var item in a.OldItems)
                                        {
                                            --i;
                                            var descriptor = new CollectionItemDescriptor(item, a.OldStartingIndex, data.GetType());
                                            observer.OnNext(new(descriptor, ChangeType.Remove));
                                        }
                                        break;
                                    case NotifyCollectionChangedAction.Reset:
                                        i = 0;
                                        observer.OnNext(new(null, ChangeType.Reset));
                                        break;
                                }
                            });
                        }
                        else
                        {
                            //observer.OnCompleted();
                        }
                        return Disposer.Empty;
                    });
                    static void Next(IObserver<Change<CollectionItemDescriptor>> observer, object item, Type componentType, ChangeType changeType, int i)
                    {
                        var descriptor = new CollectionItemDescriptor(item, i, componentType);
                        //if (value.Filters?.Any(f => f.Invoke(descriptor) == false) == false)
                        observer.OnNext(new(descriptor, changeType));
                        i++;
                    }
                }
            }


            int Count(object data)
            {
                if (data is IEnumerable enumerable)
                {
                    return enumerable.Count();
                }
                return 0;
            }
        }
        public static IEnumerable<PropertyDescriptor> ChildDescriptors(PropertyDescriptor data) =>
                data
                .GetChildProperties()
                .Cast<PropertyDescriptor>()
                .OrderBy(d => d.Name);
    }
}
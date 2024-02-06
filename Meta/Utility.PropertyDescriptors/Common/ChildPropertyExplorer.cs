using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Utility.Changes;
using Utility.Helpers;
using Utility.Helpers.Ex;
using Utility.Helpers.NonGeneric;
using Utility.Models;
using Utility.Observables.NonGeneric;
using Descriptor = System.ComponentModel.PropertyDescriptor;

namespace Utility.PropertyDescriptors
{

    public record ChildrenResponse(IMemberDescriptor Descriptor);


    public partial class ChildPropertyExplorer
    {
        public static IObservable<Change<IMemberDescriptor>> Explore(object Instance, IMemberDescriptor descriptor)
        {

            var descriptors = TypeDescriptor.GetProperties(Instance);//  descriptor.GetChildProperties().Cast<Descriptor>().ToArray();
            int count = descriptors.Count + Count(Instance);
            int i = 0;
            CompositeDisposable composite = new();

            return Observable.Create<Change<IMemberDescriptor>>(observer =>
              {
                  if (count == 0)
                  {
                      return composite;
                      //observer.OnNext(new ChildrenResponse());
                  }
                  foreach (Descriptor descriptor in descriptors)
                  {
                      observer.OnNext(new Change<IMemberDescriptor>(ObjectConverter.ToValue(Instance, descriptor), Changes.Type.Add));
                  }
                  SubscribeToCollectionItemDescriptors(observer)
                  .DisposeWith(composite);
                  return composite;
              });

            IDisposable SubscribeToCollectionItemDescriptors(IObserver<Change<IMemberDescriptor>> obs)
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

                    if (change.Type == Changes.Type.Add)
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
                    else if (change.Type == Changes.Type.Remove)
                        obs.OnNext(new Change<IMemberDescriptor>(change.Value, Changes.Type.Remove));

                    else if (change.Type == Changes.Type.Reset)
                        obs.OnNext(new Change<IMemberDescriptor>(default, Changes.Type.Reset));
                }

                IObservable<Change<IMemberDescriptor>> FromCollectionDescriptor(CollectionItemDescriptor descriptor)
                {
                    return Observable.Return(new Change<IMemberDescriptor>(descriptor, Changes.Type.Add));
                }

                IObservable<Change<CollectionItemDescriptor>> CollectionItemDescriptors(object data)
                {
                    return Observable.Create<Change<CollectionItemDescriptor>>(observer =>
                    {
                        int i = 0;
                        if (data is IEnumerable enumerable && data is not string s)
                            foreach (var item in enumerable)
                            {
                                Next(observer, item, data.GetType(), Changes.Type.Add, i++);
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
                                                Next(observer, item, data.GetType(), Changes.Type.Add, i++);
                                            break;
                                        }
                                    case NotifyCollectionChangedAction.Remove:
                                        foreach (var item in a.OldItems)
                                        {
                                            --i;
                                            var descriptor = new CollectionItemDescriptor(item, a.OldStartingIndex, data.GetType());
                                            observer.OnNext(new(descriptor, Changes.Type.Remove));
                                        }
                                        break;
                                    case NotifyCollectionChangedAction.Reset:
                                        i = 0;
                                        observer.OnNext(new(null, Changes.Type.Reset));
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
                    static void Next(IObserver<Change<CollectionItemDescriptor>> observer, object item, System.Type componentType, Changes.Type changeType, int i)
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
    }
}
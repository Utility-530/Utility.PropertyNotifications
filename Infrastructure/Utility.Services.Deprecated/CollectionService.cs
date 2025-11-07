using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using Endless;
using Utility.Collections;
using Utility.Common.Contract;
using Utility.Common.Models;
using Utility.Extensions;
using Utility.Helpers.Ex;
using Utility.PropertyNotifications;

namespace Utility.Services
{
    public class CollectionService : ICollectionService, IDisposable
    {
        private IDisposable? disposable;
        private readonly ReplaySubject<RepositoryMessage> repositoryMessages = new(1);
        private readonly ReplaySubject<CollectionChangeMessage> collectionChangeMessages = new();
        private readonly ObservableRangeCollection<object> items = new();

        public CollectionService(bool initialise = true)
        {
            if (initialise)
                Init();
        }

        public ObservableCollection<object> Items => items;

        protected void Init()
        {
            var dis1 = AllItems()
              .Scan(default((CollectionChange, CollectionChange)), (a, b) => (a.Item2, b))
              .WithLatestFrom(repositoryMessages.Select(a => a.Service).WhereNotDefault())
              .Subscribe(cc =>
              {
                  var ((a, (action, enumerable)), repository) = cc;

                  if (a?.Action == NotifyCollectionChangedAction.Reset)
                      return;
                  var cachedEnumerable = enumerable.Cached();
                  switch (action)
                  {
                      case NotifyCollectionChangedAction.Reset:
                          {
                              return;
                          }
                      case NotifyCollectionChangedAction.Replace:
                          {
                              foreach (var item in cachedEnumerable)
                                  repository.Update(item);
                              break;
                          }

                      case NotifyCollectionChangedAction.Add:
                          {
                              foreach (var item in cachedEnumerable)
                                  repository.Add(item);
                              break;
                          }
                      case NotifyCollectionChangedAction.Remove:
                          {
                              foreach (var item in cachedEnumerable)
                                  repository.Remove(item);
                              break;
                          }
                      case NotifyCollectionChangedAction.Move:
                          break;

                      default:
                          throw new ArgumentOutOfRangeException();
                  }
                  collectionChangeMessages.OnNext(new CollectionChangeMessage(new CollectionChange(action, enumerable)));

                  var count = repository.Count();
                  if (count != Items.Count)
                  {
                      //TODO add logging
                  }
                  //else if (reason == NotifyCollectionChangedAction.)
                  //{
                  //    _docstore.Upsert(item);
                  //}
              });

            var changeSet = ObservableChangeSet.Create<object, string>(observer =>
            {
                return repositoryMessages
                        .Select(a => a.Service)
                        .WhereNotDefault()
                        .Select(a => a.All<object>())
                        .Subscribe(objects =>
                        {
                            var cachedObjects = objects.ToArray();

                            if (cachedObjects.Cached().Any() == false)
                                return;

                            var objectsArray = cachedObjects.ToArray();

                            foreach (var change in objectsArray.OfType<INotifyPropertyChanged>())
                            {
                                change
                                .WhenChanged()
                                .Subscribe(x =>
                                {
                                    observer.AddOrUpdate(x.Source);
                                });
                            }
                            observer.Edit(a =>
                            {
                                a.Clear();
                                foreach (var change in objectsArray.OfType<INotifyPropertyChanged>())
                                {
                                    a.AddOrUpdate(change);
                                }
                            });

                            items.ReplaceWithRange(objectsArray);
                        });
            }, a => a?.ToString() ?? string.Empty);

            var dis2 = changeSet
                .OnItemUpdated((a, sd) =>
                {
                    if (Items.Count > 0)
                    {
                        Items.Replace(a, sd);
                    }
                })
                .Subscribe(a =>
                {
                });

            //   changeSet.Bind(out ReadOnlyObservableCollection<object>? aa);

            disposable = new CompositeDisposable(dis1, dis2);
        }

        private IObservable<CollectionChange> AllItems()
        {
            return NewItems().Merge(OldItems());

            IObservable<CollectionChange> OldItems()
            {
                return items
                    .SelectChanges()
                    .Select(a => new CollectionChange(a.Action, a.OldItems?.Cast<object>()?.ToArray() ?? Array.Empty<object>()));
            }

            IObservable<CollectionChange> NewItems()
            {
                return Items
                    .SelectChanges()
                    .Select(a => new CollectionChange(a.Action, a.NewItems?.Cast<object>()?.ToArray() ?? Array.Empty<object>()));
            }
        }

        public void Dispose()
        {
            disposable?.Dispose();
        }

        public IDisposable Subscribe(IObserver<CollectionChangeMessage> observer)
        {
            observer.OnNext(new(new(NotifyCollectionChangedAction.Add, Items)));
            return collectionChangeMessages.Subscribe(observer);
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(RepositoryMessage value)
        {
            repositoryMessages.OnNext(value);
        }
    }
}
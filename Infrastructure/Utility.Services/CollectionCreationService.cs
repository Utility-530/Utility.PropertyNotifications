using System.Collections;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;
using Utility.Changes;
using Utility.Entities;
using Utility.Interfaces.Generic.Data;

namespace Utility.Services
{
    public class CollectionCreationService : IObserver<TypeValue>, IObservable<InList>, IObserver<Changes.Change>
    {
        ReplaySubject<TypeValue> _replaySubject = new();
        ReplaySubject<InList> inListSubject = new();
        ReplaySubject<Changes.Change> changeSubject = new();
        private IList list;

        public static object Transform(object x)
        {
            return x;
        }

        public CollectionCreationService()
        {
            _replaySubject.Subscribe(a =>
            {
                list = instance(a.Type);
                inListSubject.OnNext(new InList(list));
            });

            changeSubject.Subscribe(a =>
            {
                switch(a.Type)
                {
                    case Changes.Type.Add:
                        list.Add(a.Value);
                        break;
                    case Changes.Type.Remove:
                        list.Remove(a.Value);
                        break;
                }
            });

            IList instance(System.Type type)

            {
                var instance = createCollectionInstance(type);
                subscribe(instance);            
                return instance;

                static IList createCollectionInstance(System.Type type)
                {
                    var constructedListType = typeof(ObservableCollection<>).MakeGenericType(type);
                    var instance = (IList)Activator.CreateInstance(constructedListType);

                    return instance;
                }

                static void subscribe(object? instance)
                {
                    typeof(Utility.Persists.DatabaseHelper)
                        .GetMethod(nameof(Utility.Persists.DatabaseHelper.ToManager))
                        .MakeGenericMethod(instance.GetType())
                        .Invoke(null, parameters: [instance, new Func<object, Guid>(a => (a as IId<Guid>).Id), null]);
                }
            }
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(TypeValue value)
        {
            _replaySubject.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<InList> observer)
        {
            return inListSubject.Subscribe(observer);
        }

        public void OnNext(Change value)
        {
            changeSubject.OnNext(value);
        }
    }
}

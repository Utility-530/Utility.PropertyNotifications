using Utility.Infrastructure.Abstractions;
using Utility.Models;
using System.Reactive.Threading.Tasks;
using System.Reactive.Linq;
using Utility.Interfaces.NonGeneric;
using static Utility.Observables.NonGeneric.ObservableExtensions;
using Utility.Observables.NonGeneric;

namespace Utility.Infrastructure
{
    public class PropertyStore : BaseObject
    {
        private readonly IRepository repository;

        public override Key Key => new(Guids.PropertyStore, nameof(PropertyStore), typeof(PropertyStore));

        public PropertyStore(IRepository repository)
        {
            this.repository = repository;
        }

        public override object? Model => repository;

        public IObservable OnNext(FindPropertyRequest request)
        {
            if (request.Key.Name == "Size")
                return Create(observer => Disposer.Empty);
 
            return Create(observer =>
            {
                return repository
                .FindKeyByParent(request.Key)
                .ToObservable()
                .Select(childKey => new FindPropertyResponse(childKey))
                .ObserveOn(Context)
                .Subscribe(a =>
                {
                    observer.OnNext(a);
                    observer.OnCompleted();
                });
            });
        }

        public IObservable OnNext(SetPropertyRequest order)
        {
            return Create(observer =>
            {
                CompositeDisposable composite = new();

                observer.OnProgress(1, 3);
                return repository
                .FindValue(order.Key)
                .ToObservable()
                .ObserveOn(Context)
                .Subscribe(find =>
                {
                    observer.OnProgress(2, 3);

                    repository
                    .UpdateValue(order.Key, order.Value)
                    .ToObservable()
                    .Subscribe(a =>
                    {
                        observer.OnProgress(3, 3);
                        observer.OnNext(new SetPropertyResponse(order.Value));
                        observer.OnCompleted();
                    }).DisposeWith(composite);
                }).DisposeWith(composite);

            });
        }

        public IObservable OnNext(GetPropertyRequest order)
        {
            return Create(observer =>
            {
                CompositeDisposable composite = new();

                observer.OnProgress(0, 2);
                //observer.OnNext(new GetPropertyResponse(order.Key));
                observer.OnProgress(1, 2);
                return repository
                    .FindValue(order.Key)
                    .ToObservable()
                    .ObserveOn(Context)
                    .Subscribe(find =>
                    {
                        if (find != null)
                        {
                            observer.OnNext(new GetPropertyResponse(find));
                            observer.OnProgress(2, 2);
                        }
                        else
                        {
                            //observer.OnNext(new GetPropertyResponse(find));
                            observer.OnProgress(2, 2);
                        }
                        observer.OnCompleted();
                    }).DisposeWith(composite);
            });
        }
    }
}
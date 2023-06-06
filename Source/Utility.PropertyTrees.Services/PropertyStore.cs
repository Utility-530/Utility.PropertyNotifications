using Utility.Infrastructure.Abstractions;
using Utility.Models;
using System.Reactive.Threading.Tasks;
using System.Reactive.Linq;
using Utility.Interfaces.NonGeneric;
using static Utility.Observables.Generic.ObservableExtensions;
using Utility.Observables.NonGeneric;
using System.Reactive.Disposables;
using Utility.Infrastructure;

namespace Utility.PropertyTrees.Services
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

        public Utility.Interfaces.Generic.IObservable<FindPropertyResponse> OnNext(FindPropertyRequest request)
        {
            if (request.Key.Name == "Size")
                return Create<FindPropertyResponse>(observer => Disposer.Empty);

            return Create<FindPropertyResponse>(observer =>
            {
                observer.OnProgress(1, 2);
                return repository
                .FindKeyByParent(request.Key)
                .ToObservable()
                .Select(childKey => new FindPropertyResponse(childKey))
                .ObserveOn(Context)
                .Subscribe(a =>
                {
               
                    observer.OnNext(a);
                    observer.OnProgress(2, 2);
                    observer.OnCompleted();
                });
            });
        }

        public Utility.Interfaces.Generic.IObservable<SetPropertyResponse> OnNext(SetPropertyRequest order)
        {
            return Create<SetPropertyResponse>(observer =>
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
                        observer.OnNext(new SetPropertyResponse(order.Value));
                        observer.OnProgress(3, 3);
                        observer.OnCompleted();
                    }).DisposeWith(composite);
                }).DisposeWith(composite);

            });
        }

        public Utility.Interfaces.Generic.IObservable<GetPropertyResponse> OnNext(GetPropertyRequest order)
        {
            return Create<GetPropertyResponse>(observer =>
            {
                CompositeDisposable composite = new();
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
                        
                        }
                        else
                        {
                            //observer.OnNext(new GetPropertyResponse(find));                         
                        }
                        observer.OnProgress(2, 2);
                        observer.OnCompleted();
                    }).DisposeWith(composite);
            });
        }
    }
}
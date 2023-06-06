using Utility.Infrastructure.Abstractions;
using Utility.Models;
using System.Reactive.Threading.Tasks;
using System.Reactive.Linq;
using static Utility.Observables.Generic.ObservableExtensions;
using Utility.Observables.NonGeneric;
using System.Reactive.Disposables;
using Utility.Infrastructure;
using static Utility.PropertyTrees.Events;
using LiteDB;
using Utility.Enums;

namespace Utility.PropertyTrees.Services
{
    //public interface IViewModelRepository
    //{
    //    Task<IViewModel?> FindValue(IEquatable key);
    //}

    public class ViewModelStore : BaseObject
    {
        private readonly IRepository repository;

        public override Key Key => new(Guids.PropertyStore, nameof(PropertyStore), typeof(PropertyStore));

        public ViewModelStore(IRepository repository)
        {
            this.repository = repository;
        }

        public override object? Model => repository;

        //public Utility.Interfaces.Generic.IObservable<FindPropertyResponse> OnNext(FindPropertyRequest request)
        //{
        //    if (request.Key.Name == "Size")
        //        return Create<FindPropertyResponse>(observer => Disposer.Empty);

        //    return Create<FindPropertyResponse>(observer =>
        //    {
        //        return repository
        //        .FindKeyByParent(request.Key)
        //        .ToObservable()
        //        .Select(childKey => new FindPropertyResponse(childKey))
        //        .ObserveOn(Context)
        //        .Subscribe(a =>
        //        {
        //            observer.OnNext(a);
        //            observer.OnCompleted();
        //        });
        //    });
        //}

        //public Utility.Interfaces.Generic.IObservable<SetPropertyResponse> OnNext(SetPropertyRequest order)
        //{
        //    return Create<SetPropertyResponse>(observer =>
        //    {
        //        CompositeDisposable composite = new();

        //        observer.OnProgress(1, 3);
        //        return repository
        //        .FindValue(order.Key)
        //        .ToObservable()
        //        .ObserveOn(Context)
        //        .Subscribe(find =>
        //        {
        //            observer.OnProgress(2, 3);

        //            repository
        //            .UpdateValue(order.Key, order.Value)
        //            .ToObservable()
        //            .Subscribe(a =>
        //            {
        //                observer.OnProgress(3, 3);
        //                observer.OnNext(new SetPropertyResponse(order.Value));
        //                observer.OnCompleted();
        //            }).DisposeWith(composite);
        //        }).DisposeWith(composite);

        //    });
        //}

        public Utility.Interfaces.Generic.IObservable<GetViewModelResponse> OnNext(GetViewModelRequest order)
        {
            return Create<GetViewModelResponse>(observer =>
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
                    
                            observer.OnNext(new GetViewModelResponse((ViewModel)find));
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

    public class ViewModel: IViewModel
    {
        public bool IsExpanded { get; set; }
        public Position2D Dock { get; set; }
        public int GridRow { get; set; }
        public int GridColumn { get; set; }
        public int GridRowSpan { get; set; }
        public int GridColumnSpan { get; set; }
        public string Arrangement { get; set; } = string.Empty;
        public string DataTemplateKey { get; set; } = string.Empty;
    }
}
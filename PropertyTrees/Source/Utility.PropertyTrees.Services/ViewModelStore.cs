using Utility.Models;
using System.Reactive.Threading.Tasks;
using System.Reactive.Linq;
using static Utility.Observables.Generic.ObservableExtensions;
using Utility.Observables.NonGeneric;
using System.Reactive.Disposables;
using Utility.Infrastructure;
using LiteDB;
using Utility.Enums;
using System.Collections;
using Utility.Interfaces.NonGeneric;

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
        //        .Subscribe(a =>
        //        {
        //            observer.OnNext(a);
        //            observer.OnCompleted();
        //        });
        //    });
        //}

        public Utility.Interfaces.Generic.IObservable<SetViewModelResponse> OnNext(SetViewModelRequest order)
        {
            return Create<SetViewModelResponse>(observer =>
            {
                CompositeDisposable composite = new();

                observer.OnProgress(1, 3);
                return repository
                .FindValue(order.Key)
                .ToObservable()
                .Subscribe(find =>
                {
                    observer.OnProgress(2, 3);

                    repository
                    .Update(order.Key, order.ViewModel)
                    .ToObservable()
                    .Subscribe(a =>
                    {
                        observer.OnProgress(3, 3);
                        observer.OnNext(new SetViewModelResponse(order.ViewModel));
                        observer.OnCompleted();
                    }).DisposeWith(composite);
                }).DisposeWith(composite);

            });
        }

        public Utility.Interfaces.Generic.IObservable<GetViewModelResponse> OnNext(GetViewModelRequest order)
        {
            return Create<GetViewModelResponse>(observer =>
            {
                CompositeDisposable composite = new();

                observer.OnProgress(1, 2);
                return repository
                    .FindValue(order.Key)
                    .ToObservable()
                    .Subscribe(find =>
                    {
                        if (find is ViewModel viewModel)
                        {
                            observer.OnNext(new GetViewModelResponse(new[] { viewModel }));
                            observer.OnProgress(2, 2);
                        }
                        if (find is IEnumerable viewModels)
                        {
                            observer.OnNext(new GetViewModelResponse(viewModels.OfType<ViewModel>().ToArray()));
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

    public class ViewModel : IViewModel
    {
        public Guid Id { get; set; }
        public Guid ParentGuid { get; set; }
        public string Name { get; set; }
        public Type Type { get; set; }
        public bool IsExpanded { get; set; } = true;
        public Position2D Dock { get; set; }
        public int GridRow { get; set; }
        public int GridColumn { get; set; }
        public int GridRowSpan { get; set; } = 1;
        public int GridColumnSpan { get; set; } = 1;
        public string ItemsPanelTemplateKey { get; set; }
        public string DataTemplateKey { get; set; }
        public double Left { get; set; }
        public double Right { get; set; }
        public double Top { get; set; }
        public double Bottom { get; set; }
        public string Tooltip { get; set; }
    }
}
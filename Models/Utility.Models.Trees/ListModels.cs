using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utility.Enums;
using Utility.Interfaces.NonGeneric;
using Utility.Observables;
using Utility.Observables.Generic;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.Trees.Abstractions;

namespace Utility.Models.Trees
{
    public class DataFilesModel : ListModel<DataFileModel>, ISelectable
    {
        public DataFilesModel()
        {
        }

        public override IEnumerable Proliferation()
        {
            yield return new DataFileModel { Name = "datafile", FilePath = Path.Combine(Utility.Constants.DefaultDataPath, "Models"), FileName = "New", TableName = "Model" };
        }
    }

    public class SelectionsModel : ListModel<SelectionModel>
    {
        public SelectionsModel()
        {
        }

        public override IEnumerable Proliferation()
        {
            yield return new SelectionModel { Name = "selection" };
        }
    }

    public class NodePropertyRootsModel : ListModel<NodePropertyRootModel>
    {
        public NodePropertyRootsModel()
        {
            Limit = 1;
        }

        public override IEnumerable Proliferation()
        {
            yield return new NodePropertyRootModel { Name = "npr" };
        }
    }

    public class InputsModel : ListModel<InputsModel>
    {
        private ParameterInfo[] parameters;

        public ParameterInfo[] Parameters
        {
            get => parameters; set
            {
                parameters = value;
                Limit = value.Count();
                RaisePropertyChanged();
            }
        }

        public InputsModel()
        {
        }

        public override IEnumerable Proliferation()
        {
            var oc = new ObservableCollection<ThroughPutModel>();
            this.WithChangesTo(a => a.Parameters)
                .Subscribe(x =>
                {
                    oc.Clear();
                    foreach (var param in Parameters.Where(a => this.Children.Cast<ThroughPutModel>().All(ac => ac.Parameter != a)))
                    {
                        oc.Add(new ThroughPutModel() { Name = param.Name, Parameter = param });
                    }
                });
            return oc;
        }
    }

    public class TransformersModel : ListModel<TransformerModel>
    {
        public const string transformer = nameof(transformer);
        public TransformersModel()
        {
            this.IsEditable = false;
            this.IsRemovable = false;
        }

        public override IEnumerable Proliferation()
        {
            yield return new TransformerModel { Name = transformer };
        }
    }

    public class FiltersModel : ListModel<FilterModel>
    {
        public FiltersModel()
        {
        }
        public override IEnumerable Proliferation()
        {
            yield return new FilterModel { Name = "filter" };
        }
    }

    public class ExceptionsModel() : ListModel<ExceptionModel>
    {
    }

    public class AndOrModel : ListModel<AndOrModel>, /*IAndOr*/ System.IObservable<Unit>, IPredicate
    {
        protected AndOr value = 0;
        List<System.IObserver<Unit>> observers = [];
        Dictionary<IReadOnlyTree, IDisposable> dictionary = [];

        public AndOrModel() : base()
        {
            IsAugmentable = true;
        }

        public bool Evaluate(object data)
        {
            if (Get() is AndOrModel { Value: AndOr.And })
            {
                return (Children as IList<AndOrModel>).All(x => x.Evaluate(data));
            }
            else if (Get() is AndOrModel { Value: AndOr.Or })
            {
                return (Children as IList<AndOrModel>).Any(x => x.Evaluate(data));
            }
            else
                throw new ArgumentOutOfRangeException("sd 3433 33 x");
        }

        public override void Addition(IReadOnlyTree a)
        {
            if (a is AndOrModel aoModel)
            {
                //(Children as IList)?.Add(aoModel);
                {
                    var dis = aoModel.Subscribe(_ => onNext());
                    dictionary[aoModel] = dis;
                }
            }
            else if (a is FilterModel filterModel)
            {
                //(Children as IList)?.Add(filterModel);
                {
                    System.Reactive.Disposables.CompositeDisposable composite = new();
                    filterModel
                        .WithChangesTo(a => a.ResolvableModel)
                        .Subscribe(a =>
                        {
                            a.Types.Changes().Subscribe(a =>
                            {
                                onNext();
                            }).DisposeWith(composite);
                            a.Properties.Changes().Subscribe(a =>
                            {
                                onNext();
                            }).DisposeWith(composite);
                            //a.Subscribe(_a =>
                            //{
                            //    onNext();
                            //}).DisposeWith(composite);
                        }).DisposeWith(composite);
                    dictionary[filterModel] = composite;

                }
            }
            else
                throw new Exception("7 hhjkhj9099   ");

            base.Addition(a);

            void onNext()
            {
                foreach (var obs in observers)
                    obs.OnNext(Unit.Default);
            }
        }


        public override void Subtraction(IReadOnlyTree a)
        {
            if (a is AndOrModel aoModel)
            {
                dictionary[aoModel].Dispose();
                //(Children as IList)?.Remove(aoModel);
            }
            else if (a is FilterModel filterModel)
            {
                dictionary[filterModel]?.Dispose();
                //(Children as IList)?.Remove(filterModel);
            }
            else
                throw new Exception("877 hhj9099   ");
        }


        public IDisposable Subscribe(System.IObserver<Unit> observer)
        {
            observers.Add(observer);
            return new ActionDisposable(() => observers.Remove(observer));
        }

        public override IEnumerable Proliferation()
        {
            yield return new AndOrModel() { Name = "andor" };
            yield return new FilterModel() { Name = "filter" };
        }
    }


}

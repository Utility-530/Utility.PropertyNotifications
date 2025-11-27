using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Newtonsoft.Json;
using Utility.Helpers;
using Utility.Helpers.NonGeneric;
using Utility.Helpers.Reflection;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.ServiceLocation;
using Utility.Trees.Abstractions;
using Utility.Trees.Extensions;
using Type = System.Type;

namespace Utility.Models.Trees
{
    public class FilterModel : Model, IPredicate
    {
        private const string res = nameof(res);
        private const string b_ool = nameof(b_ool);
        private const string _value = nameof(_value);
        private ResolvableModel resolvableModel;
        private ComparisonModel comparisonModel;
        private ValueModel model;
        protected INodeSource source = Utility.Globals.Resolver.Resolve<INodeSource>();

        public FilterModel()
        {
            this.WithChangesTo(a => (a as IGetParent<IReadOnlyTree>).Parent).Subscribe(a =>
            {
                this.LocalIndex = a.Children.Count();
            });
        }

        [JsonIgnore]
        [Child(res)]
        public ResolvableModel ResolvableModel
        {
            get => resolvableModel;
            set
            {
                if (resolvableModel != value)
                {
                    var previous = resolvableModel;
                    resolvableModel = value;
                    this.RaisePropertyChanged(previous, value);
                }
            }
        }

        [JsonIgnore]
        [Child(b_ool)]
        public ComparisonModel ComparisonModel
        {
            get => comparisonModel;
            set
            {
                if (comparisonModel != value)
                {
                    var previous = comparisonModel;
                    comparisonModel = value;
                    this.RaisePropertyChanged(previous, value);
                }
            }
        }

        [JsonIgnore]
        [Child(_value)]
        public ValueModel Model
        {
            get => model;
            set
            {
                if (model != value)
                {
                    var previous = model;
                    model = value;
                    this.RaisePropertyChanged(previous, value);
                }
            }
        }

        public override void Addition(IReadOnlyTree a)
        {
            switch (a.ToString())
            {
                case res:
                    ResolvableModel = a as ResolvableModel;
                    ResolvableModel.Types.Changes()
                        .CombineLatest(ResolvableModel.Properties.Changes(), this.WithChangesTo(a => a.Model))
                        .Subscribe(a =>
                        {
                            var typesCount = ResolvableModel.Types.Count;
                            var propertiesCount = ResolvableModel.Properties.Count;
                            ObservableCollection<object> list = new();
                            source.Nodes.AndChanges<INodeViewModel>().Subscribe(a =>
                            {
                                foreach (var item in a)
                                    if (item.Type == Utility.Changes.Type.Add)
                                        if (ResolvableModel.TryGetValue(item.Value, out var x))
                                        {
                                            list.Add(x);
                                        }
                            });

                            if (typesCount > propertiesCount)
                            {
                                Model.Set(ResolvableModel.Types.Last());
                                ComparisonModel.Type = a.First == null ? ComparisonType.Default : ComparisonType.Type;
                            }
                            else if (typesCount == 0)
                            {
                                Model.Set(null);
                                ComparisonModel.Type = ComparisonType.Default;
                            }
                            else if (typesCount == propertiesCount)
                            {
                                var propertyType = ResolvableModel.Properties.Last().PropertyType;
                                Model.AutoList = list;
                                if (Model.Value?.GetType() != propertyType)
                                {
                                    Model.Set(_value ?? ActivateAnything.Activate.New(propertyType));
                                    ComparisonModel.Type = toComparisonType(propertyType);
                                }
                            }
                            else
                                throw new Exception("ee33 ffp[oe");
                        });
                    break;

                case b_ool:
                    ComparisonModel = a as ComparisonModel; break;
                case _value:
                    Model = a as ValueModel;
                    Model.WithChangesTo(a => a.IsListening)
                        .CombineLatest(source.Nodes.AndAdditions().SelectMany(a => a.WithChangesTo(ca => (ca as IGetIsSelected).IsSelected).Select(c => a)))
                        .Where(a => a.First)
                        .Select(a => a.Second)
                        .Subscribe(selected =>
                        {
                            if (ResolvableModel.TryGetValue(selected, out var _value))
                            {
                                //Model.Value = _value;
                                Model.Set(_value);
                            }
                        });
                    break;
            }
            base.Addition(a);

            static ComparisonType toComparisonType(Type type)
            {
                if (type == null)
                    return ComparisonType.Default;
                else if (type == typeof(string))
                    return ComparisonType.String;
                else if (TypeHelper.IsNumericType(type))
                    return ComparisonType.Number;
                else
                    return ComparisonType.Boolean;
            }
        }

        public bool Evaluate(object instance)
        {
            if (ResolvableModel.TryGetValue(instance, out var value))
            {
                return comparisonModel.Compare(value, Model.Value);
            }
            return false;
        }
    }
}
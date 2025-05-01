using System.Collections;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Utility.Nodes.Filters;
using Utility.Models;
using Utility.Interfaces.Exs;
using Utility.Trees.Extensions;
using Splat;
using System.Reactive.Subjects;
using Utility.Helpers;
using Utility.PropertyNotifications;
using Utility.Models.Trees;
using LanguageExt.Pipes;
using Utility.Trees.Abstractions;
using System.Reactive.Disposables;
using System.Linq.Expressions;

namespace Utility.Nodes.Demo.Filters.Services
{
    public enum ControlEventType
    {
        None, Save, Refresh
    }
    public readonly record struct ControlEvent(ControlEventType ControlEventType, int Count);

    public class ControlsService : Model, IObservable<ControlEvent>
    {
        Dictionary<Guid, IDisposable> disposables = new();
        ReplaySubject<ControlEvent> replaySubject = new(1);
        Dictionary<ControlEventType, int> dict = new();


        public ControlsService()
        {
            Locator.Current.GetService<INodeSource>()
                .Single(nameof(Factory.BuildControlRoot))
                .Subscribe(_n =>
                {
                    Utility.Trees.Extensions.Async.Match.Descendants(_n)
                    .Subscribe(node =>
                    {
                        if (node.NewItem is INode { Data: Model { Name: string name } model })
                        {
                            model.WhenChanged().Where(a => a.PropertyName == ".ctor").Subscribe(_ =>
                            {
                                Switch(name, model);
                            });
                        }
                    });
                });
        }

        private void Switch(string name, Model model)
        {
            switch (name)
            {
                case Factory.Run:
                    {
                        Locator.Current
                            .GetService<INodeSource>()
                            .Single(nameof(Factory.BuildTransformersRoot))
                            .Subscribe(c_node =>
                            {
                                if (c_node is { Data: TransformersModel ts })
                                {
                                    Origin.Content().Subscribe(c =>
                                    {
                                        foreach (var t in ts.Transformers)
                                        {
                                            NodesSource.nodes(c).Subscribe(x =>
                                            {
                                                InputsFilterModel.filterInputs(t, x)
                                                .Subscribe(a =>
                                                {
                                                    var inputs = InputsPrimerModel.setInputs(a);
                                                    if (inputs == null)
                                                        return;
                                                    var output = t.Converter.Method.Invoke(null, inputs);
                                                    OutputYielder.setOutput(t, output, x);
                                                });
                                            });
                                        }
                                    });
                                }
                            });
                    }
                    break;
            }
        }

        public IDisposable Subscribe(IObserver<ControlEvent> observer)
        {
            return replaySubject.Subscribe(observer);
        }

        //public static ControlsService Instance { get; } = new ControlsService();
    }


    public class TransformationModel : Model
    {
        private InputsFilterModel resolvableModel;
        private InputsPrimerModel inputsPrimer;
        private OutputYielder outputYielder;
        private NodesSource nodesModel;

        public ObservableCollection<DataConnector> DataConnectors = [
            new DataConnector<InputsFilterModel, InputsPrimerModel, Dictionary<ThroughPutModel, ArrayList>>([a=>a.Dictionary], [(a, v)=> a.Dictionary = v ]),
            ];


        //[JsonIgnore]
        [Child(nameof(InputsModel))]
        public InputsFilterModel InputsModel
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

        [Child(nameof(OutputYielder))]
        public OutputYielder OutputYielder
        {
            get => outputYielder;
            set
            {
                if (outputYielder != value)
                {
                    var previous = outputYielder;
                    outputYielder = value;
                    this.RaisePropertyChanged(previous, value);
                }

            }
        }

        [Child(nameof(InputsPrimer))]
        public InputsPrimerModel InputsPrimer
        {
            get => inputsPrimer;
            set
            {
                if (inputsPrimer != value)
                {
                    var previous = inputsPrimer;
                    inputsPrimer = value;
                    this.RaisePropertyChanged(previous, value);
                }
            }
        }


        [Child(nameof(NodesSource))]
        public NodesSource NodesSource
        {
            get => nodesModel;
            set
            {
                if (nodesModel != value)
                {
                    var previous = nodesModel;
                    nodesModel = value;
                    this.RaisePropertyChanged(previous, value);
                }
            }
        }

        public class DataConnector<T, R, S> : DataConnector
        {
            public DataConnector(
         List<Func<T, S>> inputs,
         List<Action<R, S>> output) :
                base(inputs.Select(xa => new Func<object, object>(x => xa.Invoke((T)x))).ToList(),
                    a => a is T,
                    output.Select(xa => new Action<object, object>((x, s) => xa.Invoke((R)x, (S)s))).ToList(),
                    a => a is R)
            {

            }
        }

        public class DataConnector
        {
            private readonly List<Func<object, object>> input;
            private readonly Predicate<object> inputFilter;
            private readonly List<Action<object, object>> output;
            private readonly Predicate<object> outputFilter;

            public Node Source { get; set; }
            public Node Destination { get; set; }

            public DataConnector(
                List<Func<object, object>> inputs,
                Predicate<object> inputFilter,
                List<Action<object, object>> output,
                Predicate<object> outputFilter)
            {
                this.input = input;
                this.inputFilter = inputFilter;
                this.output = output;
                this.outputFilter = outputFilter;
            }

            public void Connect()
            {
                Locator.Current.GetService<INodeSource>()
                    .Single(nameof(Factory.BuildDemoContentRoot))
                    .Subscribe(source =>
                    {
                        source
                        .Descendants()
                        .ForEach(n =>
                        {
                            //if (n.Data is Model m)
                            //    nodes.Add(n);
                            if (inputFilter.Invoke(n))
                            {
                                var _input = input.Select(@in => @in.Invoke(n));

                            }
                        });

                        source
                        .Descendants()
                        .ForEach(n =>
                        {
                            //if (n.Data is Model m)
                            //    nodes.Add(n);
                            if (outputFilter.Invoke(n))
                            {
                                output.ForEach(@out => @out.Invoke(n, output));

                            }
                        });
                    });
            }
        }
    }


    public class InputsFilterModel : Model
    {
        public TransformerModel Transformer { get; set; }
        public IEnumerable<IReadOnlyTree> Trees { get; set; }
        public Dictionary<ThroughPutModel, ArrayList> Dictionary { get; set; }

        public static IObservable<Dictionary<ThroughPutModel, ArrayList>> filterInputs(TransformerModel transformer, IEnumerable<IReadOnlyTree> trees)
        {
            return Observable.Create<Dictionary<ThroughPutModel, ArrayList>>(observer =>
            {
                var dict = transformer.Inputs.Collection.ToDictionary(a => a, a => new ArrayList());
                trees.ForEach(a =>
                {
                    foreach (var input in transformer.Inputs.Collection)
                    {
                        if (input.Filter == null)
                        {
                            //observer.OnNext(new TransformerException(transformer, "filter is null"));
                            return;
                        }
                        if (input.Filter.Get(a))
                        {
                            //if (transformer.Source.TryGetValue(a, out var value))
                            //    results.Add(value);

                            var list = new List<object>();
                            foreach (var source in input.Element.Collection)
                            {
                                if (source.TryGetValue(a, out var value))
                                    list.Add(value);
                            }
                            dict[input].Add(list);
                        }
                        else
                        {
                            //a.IsHighlighted = false;
                        }
                    }
                });

                observer.OnNext(dict);
                observer.OnCompleted();
                return Disposable.Empty;
            });
        }
    }


    public class InputsPrimerModel
    {

        public Dictionary<ThroughPutModel, ArrayList> Dictionary { get; set; }

        public object[] Output { get; set; }

        public static object[] setInputs(Dictionary<ThroughPutModel, ArrayList> dict)
        {
            object[] inputs = new object[dict.Count];
            int i = 0;

            foreach (var kevp in dict)
            {
                if (kevp.Value.Count == 0)
                {
                    //observer.OnNext(new TransformerException(transformer, $"no result for parameter {kevp.Key.Name}"));
                    return null;
                }


                var param = kevp.Key.Parameter;

                if (param.ParameterType.IsAssignableTo(typeof(IEnumerable)))
                {
                    if (param.ParameterType.GetGenericArguments().SingleOrDefault() is { } type)
                    {
                        if (kevp.Value.Select(a => a.GetType()).SingleOrDefault() is { } _type)
                        {
                            if (type.Equals(_type))
                            {
                                inputs[i] = kevp.Value.ToArray().Take(1).Single();
                            }
                        }
                    }
                }
                else if (dict.Select(a => a.Key.Parameter.ParameterType).ToHashSet().SingleOrDefault() is { } _type)
                {
                    if (param.ParameterType.Equals(_type))
                    {
                        //transformer.Converter.Inputs = results.ToArray();

                        var xx = (kevp.Value.ToArray().Take(1).Single() as IEnumerable).Cast<object>();
                        if (xx.Count() == 1)
                            inputs[i] = xx.Single();
                        else
                        {
                            //observer.OnNext(new TransformerException(transformer, "sdfsdsdf"));
                            return null;
                        }
                    }
                    else
                    {
                        //observer.OnNext(new TransformerException(transformer, $"input type, {_type.Name}, is not equal to Parameter-Type, {param.ParameterType.Name}"));
                        return null;
                    }
                }
                else
                {

                }
                i++;
            }
            return inputs;
        }
    }

    public class NodesSource
    {
        public static IObservable<IEnumerable<IReadOnlyTree>> nodes(IReadOnlyTree source)
        {
            return Observable.Create<IEnumerable<IReadOnlyTree>>(observer =>
            {
                List<IReadOnlyTree> nodes = new List<IReadOnlyTree>();
                source
                .Descendants()
                    .ForEach(n =>
                    {
                        if (n.Data is Model m)
                            nodes.Add(n);
                    });
                observer.OnNext(nodes);
                observer.OnCompleted();
                return Disposable.Empty;
            });
        }
    }

    public class Origin
    {
        public static IObservable<IReadOnlyTree> Content()
        {
            return Locator.Current.GetService<INodeSource>()
              .Single(nameof(Factory.BuildDemoContentRoot));
        }

    }

    public class OutputYielder
    {
        public static void setOutput(TransformerModel transformer, object output, IEnumerable<IReadOnlyTree> trees)
        {
            bool any = false;
            trees.ForEach(a =>
            {
                if (transformer.Output.Filter.Get(a))
                {
                    try
                    {
                        any = true;
                        //transformer.Target.TrySetValue(a, output);

                        if (output is IEnumerable enumerable)
                        {
                            var mr = enumerable.GetEnumerator();
                            var ee = transformer.Output.Element.Collection.GetEnumerator();
                            while (mr.MoveNext() && ee.MoveNext())
                            {
                                if (ee.Current.Types.Count != ee.Current.Properties.Count)
                                {
                                    //observer.OnNext(new TransformerException(transformer, $"types count {ee.Current.Types.Count} doesn't match properties count, {ee.Current.Properties.Count}"));
                                    return;
                                }
                                ee.Current.TrySetValue(a, mr.Current);
                            }



                        }
                        else
                        {
                            foreach (var target in transformer.Output.Element.Collection)
                            {
                                if (target.Types.Count != target.Properties.Count)
                                {
                                    //observer.OnNext(new TransformerException(transformer, $"types count {target.Types.Count} doesn't match properties count, {target.Properties.Count}"));
                                    return;
                                }
                                target.TrySetValue(a, output);
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        //observer.OnNext(new TransformerException(transformer, ex.Message));
                    }
                }
                else
                {
                }
            });

        }

    }


}

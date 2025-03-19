using DynamicData.Binding;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using Utility.Nodes.Filters;
using Utility.Reactives;
using Utility.Extensions;
using Utility.Trees.Extensions.Async;
using Utility.Helpers;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes.Demo.Filters.Services;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Interfaces.Exs;
using Splat;

namespace Utility.Nodes.Demo.Filters.Services
{
    public class TransformerException : Exception
    {
        public TransformerException(TransformerModel transformerModel, string message) : base(message)
        {
            TransformerModel = transformerModel;
        }

        public TransformerModel TransformerModel { get; }
    }

    public class TransformerService
    {
        public TransformerService()
        {
            int i = 0;
            Observable.Create<Exception>(observer =>
            {
                return Locator.Current.GetService<INodeSource>()
                .Single(nameof(Factory.BuildTransformersRoot))
                .Subscribe(c_node =>
                {
                    c_node
                    .Descendant(a => (a.Item1 as INode).Data.ToString() == Factory.content_root)
                    .Subscribe(a =>
                    {
                        var transformersModel = a.NewItem.Data as TransformersModel;
                        if (transformersModel == null)
                        {
                            observer.OnNext(new TransformerException(null, $"{a.NewItem.Data.ToString()}, is not {(nameof(TransformersModel))}"));
                            return;
                        }
                        observer.OnNext(null);
                        transformersModel.Transformers.SelfAndAdditions().Subscribe(transformer =>
                        {
                            changes(transformer)
                            .Subscribe(a =>
                            {
                                transformer
                                .WhenPropertyChanged(a => a.Exceptions)
                                .Where(a => a.Value != null)
                                .Select(a => a.Value)
                                .Take(1)
                                .Subscribe(_a =>
                                {
                                    Locator.Current.GetService<INodeSource>().Single(nameof(Factory.BuildContentRoot))
                                        .Subscribe(model =>
                                            {
                                                _a.Node.Clear();

                                                if (transformer.Inputs == null)
                                                {
                                                    observer.OnNext(new TransformerException(transformer, "input is null"));
                                                    return;
                                                }
                                                if (transformer.Output == null)
                                                {
                                                    observer.OnNext(new TransformerException(transformer, "output is null"));
                                                    return;
                                                }
                                                if (transformer.Converter == null)
                                                {
                                                    observer.OnNext(new TransformerException(transformer, "converter is null"));
                                                    return;
                                                }

                                                if (transformer.Converter.Method == null)
                                                {
                                                    observer.OnNext(new TransformerException(transformer, "method is null"));
                                                    return;
                                                }

                                                if (transformer.Output.Element == null)
                                                {
                                                    observer.OnNext(new TransformerException(transformer, "target is null"));
                                                    return;
                                                }

                                                if (transformer.Output.Element == null)
                                                {
                                                    observer.OnNext(new TransformerException(transformer, "target is null"));
                                                    return;
                                                }


                                                //ArrayList results = new();
                                                var dict = transformer.Inputs.Models.ToDictionary(a => a, a => new ArrayList());

                                                model
                                                .Descendants((a) =>
                                                {
                                                    if (a.Item1.Data is Model m)
                                                        return true;
                                                    return false;
                                                })
                                                .ForEach(a =>
                                                {
                                                    foreach (var input in transformer.Inputs.Models)
                                                    {
                                                        if (input.Filter == null)
                                                        {
                                                            observer.OnNext(new TransformerException(transformer, "filter is null"));
                                                            return;
                                                        }
                                                        if (input.Filter.Get(a))
                                                        {
                                                            //if (transformer.Source.TryGetValue(a, out var value))
                                                            //    results.Add(value);

                                                            var list = new List<object>();
                                                            foreach (var source in input.Element.Models)
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

                                                object[] inputs = new object[dict.Count];
                                                int i = 0;
                                                foreach (var kevp in dict)
                                                {
                                                    if (kevp.Value.Count == 0)
                                                    {
                                                        observer.OnNext(new TransformerException(transformer, $"no result for parameter {kevp.Key.Name}"));
                                                        return;
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
                                                                observer.OnNext(new TransformerException(transformer, "sdfsdsdf"));
                                                                return;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            observer.OnNext(new TransformerException(transformer, $"input type, {_type.Name}, is not equal to Parameter-Type, {param.ParameterType.Name}"));
                                                            return;
                                                        }
                                                    }
                                                    else
                                                    {

                                                    }
                                                    i++;
                                                }

                                                object? output = null;
                                                try
                                                {
                                                    output = transformer.Converter.Method.Invoke(null, inputs);
                                                }
                                                catch (Exception ex)
                                                {
                                                    observer.OnNext(new TransformerException(transformer, ex.Message));
                                                    return;
                                                }
                                                var returnType = transformer.Converter.Method.ReturnParameter.ParameterType;


                                                if (output.GetType().Equals(returnType))
                                                {
                                                    //transformer.Converter.Inputs = results.ToArray();
                                                }
                                                else
                                                {
                                                    observer.OnNext(new TransformerException(transformer, $"output type, {returnType.Name}, not equal to Parameter-Type, {output.GetType().Name}"));
                                                    //observer.OnCompleted();
                                                    return;
                                                }

                                                //transformer.Converter.Output = output;
                                                bool any = false;
                                                model.Descendants((a) =>
                                                {
                                                    if (a.Item1.Data is Model m)
                                                        return true;
                                                    return false;
                                                }).ForEach(a =>
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
                                                                var ee = transformer.Output.Element.Models.GetEnumerator();
                                                                while (mr.MoveNext() && ee.MoveNext())
                                                                {
                                                                    if (ee.Current.Types.Count != ee.Current.Properties.Count)
                                                                    {
                                                                        observer.OnNext(new TransformerException(transformer, $"types count {ee.Current.Types.Count} doesn't match properties count, {ee.Current.Properties.Count}"));
                                                                        return;
                                                                    }
                                                                    ee.Current.TrySetValue(a, mr.Current);
                                                                }



                                                            }
                                                            else
                                                            {
                                                                foreach (var target in transformer.Output.Element.Models)
                                                                {
                                                                    if (target.Types.Count != target.Properties.Count)
                                                                    {
                                                                        observer.OnNext(new TransformerException(transformer, $"types count {target.Types.Count} doesn't match properties count, {target.Properties.Count}"));
                                                                        return;
                                                                    }
                                                                    target.TrySetValue(a, output);
                                                                }

                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            observer.OnNext(new TransformerException(transformer, ex.Message));
                                                        }
                                                    }
                                                    else
                                                    {
                                                    }
                                                });
                                                if (any == false)
                                                {
                                                    observer.OnNext(new TransformerException(transformer, "no matches for target filter"));
                                                    return;
                                                }
                                            });
                                });
                            });
                        });
                    });
                });
            })
            .Subscribe(a =>
            {

                if (a is TransformerException { TransformerModel: { } transformer } ex)
                {
                    transformer.Exceptions.Node.Add(new Node(ExceptionModel.Create(ex)));
                }
            });
        }

        private static IObservable<Unit> changes(TransformerModel transformer)
        {
            return Observable.Create<Unit>(observer =>
            {
                CompositeDisposable composite = new();
                transformer.WhenPropertyChanged(a => a.Inputs)
                .Subscribe(a =>
                {
                    a.Value?.Models.Changes().Subscribe(change =>
                    {
                        if (change.Type == Changes.Type.Add)
                        {
                            change.Value
                            .WhenPropertyChanged(a => a.Element)
                            .Subscribe(a =>
                            {
                                a.Value?.Models.Changes()
                                .Subscribe(a =>
                                {
                                    a.Value.Types.Changes().Subscribe(a => observer.OnNext(Unit.Default));
                                    a.Value.Properties.Changes().Subscribe(a => observer.OnNext(Unit.Default));

                                }).DisposeWith(composite);
                            });
                            change.Value
                            .WhenPropertyChanged(a => a.Filter)
                            .Subscribe(a =>
                            {
                                a.Value?
                                .Subscribe(a =>
                                {
                                    observer.OnNext(Unit.Default);
                                }).DisposeWith(composite);
                            });
                        }
                    });
                });

                transformer.WhenPropertyChanged(a => a.Converter).Where(a => a.Value != null).SelectMany(a => a.Value.WhenPropertyChanged(a => a.Method)).Subscribe(a => observer.OnNext(Unit.Default)).DisposeWith(composite);
                //transformer.WhenPropertyChanged(a => a.Converter).Where(a => a.Value != null).SelectMany(a => a.Value.WhenPropertyChanged(a => a.Parameter)).Subscribe(a => observer.OnNext(Unit.Default)).DisposeWith(composite);

                transformer.WhenPropertyChanged(a => a.Output)
                .Subscribe(change =>
                {
                    change.Value?.WhenPropertyChanged(a => a.Element)
                    .Subscribe(a =>
                    {
                        a.Value?.Models.Changes().Subscribe(a =>
                        {
                            a.Value.Types.Changes().Subscribe(a => observer.OnNext(Unit.Default));
                            a.Value.Properties.Changes().Subscribe(a => observer.OnNext(Unit.Default));
                        }).DisposeWith(composite);
                    });

                    change.Value?.WhenPropertyChanged(a => a.Filter)
                    .Subscribe(a =>
                    {
                        a.Value?.Subscribe(a => observer.OnNext(Unit.Default)).DisposeWith(composite);
                    });
                });


                return composite;
            });
        }
    }

    public static class Helper
    {
        //public static IEnumerable<T> Select<T>(this ArrayList list, Func<object, T> map)
        //{
        //    foreach (var x in list)
        //    {
        //        yield return map(x);
        //    }
        //}

        public static HashSet<T> ToHashSet<T>(Collection<T> collection)
        {
            return new HashSet<T>(collection);
        }
    }
}
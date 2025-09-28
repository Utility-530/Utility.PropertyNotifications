using Fasterflect;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using Utility.Extensions;
using Utility.Helpers;
using Utility.Helpers.Reflection;
using Utility.Interfaces;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;
using Utility.Models;
using Utility.Models.Trees;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.ServiceLocation;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Meta
{
    internal class DataInitialiser : IDataInitialiser, IDisposable
    {

        Lazy<ITreeRepository> repository;
        Dictionary<string, Func<object, object>> getters = new();
        CompositeDisposable compositeDisposable = new();
        Lazy<NodeInterface> nodeInterface = new(() => new NodeInterface());
        private bool _disposed;

        public DataInitialiser()
        {
            var repo = Globals.Resolver.Resolve<ITreeRepository>();
            repository = new(() => repo);
        }

        public IObservable<INodeViewModel> Load(INodeViewModel node)
        {
            return Observable.Create<INodeViewModel>(observer =>
            {
                return repository.Value.Get(Guid.Parse(node.Key())).Subscribe(_d =>
                {
                    if (_d.Name == null)
                    {
                    }
                    else if (_d.Value != null)
                        nodeInterface.Value.Setter(_d.Name)?.Set(node, _d.Value);
                }, () =>
                {
                    observer.OnNext(node);
                    observer.OnCompleted();
                });
            });
        }

        public void Track(INodeViewModel node)
        {
            if (node.Key() == "5b672a24-2269-4c3b-861b-eb6d529ab41a")
            {

            }
            if (node is INotifyPropertyCalled called)
            {
                called.WhenCalled(true)
                .Subscribe(call =>
                {
                    if (call.Name == nameof(Model.Name))
                    {
                        return;
                    }
                    if (call.Name == nameof(ValueModel.Value))
                    {
                    }
                    repository.Value
                    .Get((GuidKey)node.Key(), nameof(NodeViewModel.Data) + "." + call.Name)
                    .Subscribe(a =>
                    {
                        if (a.Value != null)
                        {
                            object output = null;
                            FieldInfo fieldInfo = null;

                            if (call.Target is IGet get)
                            {
                                output = get.Get();
                            }
                            else if (call.Target.TryGetPrivateFieldValue(call.Name.ToLower(), out var _output, out var _fieldInfo) == false)
                            {
                                output = _output;
                                fieldInfo = _fieldInfo;
                            }
                            else
                            {
                                throw new Exception($"no field for property, {call.Name}");
                            }


                            if (output?.Equals(a.Value) == true)
                                return;
                            else
                            {
                                if (call.Target is ISet set)
                                {
                                    set.Set(a.Value);
                                }
                                else if (fieldInfo != null)
                                {
                                    fieldInfo.SetValue(call.Target, a.Value);
                                    if (call.Target is INotifyPropertyChanged changed)
                                        changed.RaisePropertyChanged(call.Name);
                                }
                                else
                                {
                                    throw new Exception($"no field for property, {call.Name}");
                                }
                            }
                        }
                    }).DisposeWith(compositeDisposable);
                }).DisposeWith(compositeDisposable);
            }
            if (node is INotifyPropertyReceived received)
            {
                received.WhenReceived()
                .Subscribe(reception =>
                {
                    if (reception.Name == nameof(Model.Name))
                    {
                        (reception.Target as INotifyPropertyChanged).RaisePropertyChanged(reception.Name);
                        repository.Value.UpdateName((GuidKey)(node as IGetParent<IReadOnlyTree>).Parent.Key(), (GuidKey)node.Key(), (string)reception.OldValue, (string)reception.Value);
                    }
                    else
                        repository.Value.Set((GuidKey)node.Key(), nameof(NodeViewModel.Data) + "." + reception.Name, reception.Value, DateTime.Now);
                }).DisposeWith(compositeDisposable);
            }
            if (node is INotifyPropertyChanged changed)
            {
                changed.WhenChanged()
                    .WhereIsNotNull()
                .Subscribe(reception =>
                {
                    object value;
                    if (reception.Name != nameof(ValueModel.Value))
                    {
                        return;
                    }
                    else if (reception is PropertyChange ex)
                    {
                        value = ex.Value;
                    }
                    else
                    {
                        value = getters
                                    .Get(reception.Name, a => node.Data.GetType().GetProperty(reception.Name).ToGetter<object>())
                                    .Invoke(node.Data);
                    }
                    repository.Value.Set((GuidKey)node.Key(), nameof(NodeViewModel.Data) + "." + reception.Name, value, DateTime.Now);

                }).DisposeWith(compositeDisposable);
            }

            if (node is ISetNode iSetNode)
            {
                iSetNode.SetNode(node);
            }
            else
            {
                //throw new Exception("R333 ");
            }




        }


        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
        //The Dispose method performs all object cleanup, so the garbage collector no longer needs to call the objects' Object.Finalize override. Therefore, the call to the SuppressFinalize method prevents the garbage collector from running the finalizer. If the type has no finalizer, the call to GC.SuppressFinalize has no effect. The actual cleanup is performed by the Dispose(bool) method overload.

        //In the overload, the disposing parameter is a Boolean that indicates whether the method call comes from a Dispose method(its value is true) or from a finalizer(its value is false).


        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose managed state (managed objects).
                // ...
                compositeDisposable.Dispose();
            }

            // Free unmanaged resources.
            // ...

            _disposed = true;
        }
        //    void configure(INodeViewModel node)
        //    {
        //        if (filter.Value?.Filter(node) == false)
        //        {
        //            node.IsVisible = false;
        //            return;
        //        }
        //        if (expander.Value?.Expand(node) == true)
        //        {
        //            node.IsExpanded = true;
        //        }

        //        node.WhenChanged().Subscribe(e =>
        //        {
        //            if (e is PropertyChange { Name: string name, Value: var value })
        //            {
        //                if (name == nameof(ViewModelTree.Order))
        //                {
        //                    ((node as IGetParent<IReadOnlyTree>).Parent as INodeViewModel).Sort(null);
        //                }
        //                if (name == nameof(ViewModelTree.IsSelected) && value is true)
        //                {
        //                    selections.OnNext(node);
        //                }
        //                if (names.Contains(name))
        //                {
        //                    if (nodeInterface.Value.Getter(name)?.Get(node) is { } _value)
        //                        repository?.Value.Set((GuidKey)node.Key(), name, _value, DateTime.Now);
        //                }
        //                else
        //                    Globals.UI.Post((a) =>
        //                    {
        //                        dirty.OnNext((DirtyModel)a);

        //                    }, new DirtyModel { Name = name + node.Children.Count(), SourceKey = node.Key(), PropertyName = name, NewValue = value });
        //            }
        //            else
        //                throw new Exception("ss FGre333333333");
        //        });
        //    }


    }
}

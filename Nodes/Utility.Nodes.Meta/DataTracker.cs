using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using Utility.Attributes;
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
using Utility.ServiceLocation;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Meta
{
    internal class DataTracker : IDataTracker, IDisposable
    {

        IValueRepository repository;
        CompositeDisposable compositeDisposable = new();
        NodeInterface nodeInterface;
        private bool _disposed;
        private static IEnumerable<PropertyInfo> yieldProperties;

        public DataTracker(IValueRepository valueRepository, NodeInterface nodeInterface)
        {
            repository = valueRepository;
            this.nodeInterface = nodeInterface;
        }

        public IObservable<INodeViewModel> Load(INodeViewModel node)
        {
            if (node is NodeViewModel tree)
                foreach (var change in tree.Changes())
                {
                    if (change.PropertyName != nameof(NodeViewModel.Name))
                    {
                        repository
                            .Get((GuidKey)node.Key(), change.PropertyName)
                            .Subscribe(a =>
                            {
                                if (a.Value == null)
                                {
                                    repository.Set((GuidKey)node.Key(), change.PropertyName, change.NewValue, DateTime.Now);
                                }
                            }).DisposeWith(compositeDisposable);
                    }
                }

            return Observable.Return(node);

            return Observable.Create<INodeViewModel>(observer =>
            {
                return repository
                .Get(Guid.Parse(node.Key()))
                .Subscribe(_d =>
                {
                    if (_d.Name == nameof(IGetValue.Value) && _d.Value != null)
                    {
                        if (nodeInterface.Setter(_d.Name) is not { } setter)
                        {
                            throw new Exception($"no field for property, {_d.Name}");
                        }
                        //if (node is ISet set)
                        //{
                        //    set.Set(_d.Value, _d.Name);
                        //    return;
                        //}
                        setter?.Set(node, _d.Value);
                    }
                }, () =>
                {
                    observer.OnNext(node);
                    observer.OnCompleted();
                });
            });
        }

        public void Track(INodeViewModel node)
        {
            if (node.Name() is "File")
            {

            }
            if (node is INotifyPropertyCalled called)
            {
                called
                    .WhenCalled(true)
                    .Subscribe(call =>
                    {
                        if (call.Name == nameof(Model.Name))
                        {
                            return;
                        }
                        if (call.Name == nameof(ValueModel.Value))
                        {
                        }
                        if (call.Name == nameof(ValueModel.Current))
                        {
                        }

                        repository
                        .Get((GuidKey)node.Key(), call.Name)
                        .Subscribe(a =>
                        {
                            if (a.Value == null)
                            {
                                return;
                            }
                            update(node, call, a.Value);
                        }).DisposeWith(compositeDisposable);
                    }).DisposeWith(compositeDisposable);
            }
            if (node is INotifyPropertyReceived received)
            {
                received
                    .WhenReceived()
                    .Subscribe(reception =>
                    {
                        if (reception.Name == nameof(Model.Name))
                        {
                            throw new Exception("ds2d cdd32");
                            //(reception.Target as INotifyPropertyChanged).RaisePropertyChanged(reception.Name);
                            //repository.Value.UpdateName((GuidKey)(node as IGetParent<IReadOnlyTree>).Parent.Key(), (GuidKey)node.Key(), (string)reception.OldValue, (string)reception.Value);
                        }
                        else
                        {
                            if ((yieldProperties ??=
                            AttributeHelper
                            .FilterPropertiesByAttribute<YieldAttribute>(node.GetType()))
                            .Any(a => a.Name == reception.Name))
                            {
                                return;
                            }
                            if (reception.Name == nameof(Model.Value))
                            {
                                if (node.ShouldValueBeTracked == false)
                                {
                                    return;
                                }
                            }
                            if (nodeInterface.Getter(reception.Name) is { } getter)
                            {
                                var value = getter.Get(node);
                                repository.Set((GuidKey)node.Key(), reception.Name, value, DateTime.Now);
                            }
                        }
                    }).DisposeWith(compositeDisposable);
            }

            if (node is IDoesValueRequireSaving { DoesValueRequireSaving: true } and IGetValue { Value: { } value })
            {
                repository.Set((GuidKey)node.Key(), nameof(IGetValue.Value), value, DateTime.Now);
            }
        }

        private void update(INodeViewModel node, PropertyCall call, object value)
        {
            //object output = null;
            FieldInfo fieldInfo = null;
            Getter? getter = nodeInterface.Getter(call.Name) ?? throw new Exception($"no getter for property, {call.Name}");

            var output = getter.Get(node);

            //else if (call.Target.TryGetPrivateFieldValue(call.Name.ToLower(), out var _output, out var _fieldInfo))
            //{
            //    output = _output;
            //    fieldInfo = _fieldInfo;
            //}

            if (getter.Equality(output, value) == true)
            {
                return;
            }
            else
            {
                if (nodeInterface.Setter(call.Name) is not { } setter)
                {
                    throw new Exception($"no field for property, {call.Name}");
                }
                setter.Set(node, value);

                //else if (fieldInfo != null)
                //{
                //    fieldInfo.SetValue(call.Target, value);
                //    if (call.Target is INotifyPropertyChanged changed)
                //        changed.RaisePropertyChanged(call.Name);
                //}
            }
        }

        #region dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

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
        #endregion dispose
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
        //                if (name == nameof(NodeViewModel.Order))
        //                {
        //                    ((node as IGetParent<IReadOnlyTree>).Parent as INodeViewModel).Sort(null);
        //                }
        //                if (name == nameof(NodeViewModel.IsSelected) && value is true)
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

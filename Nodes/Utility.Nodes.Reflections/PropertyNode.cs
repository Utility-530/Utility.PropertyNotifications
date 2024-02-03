using Utility.Observables.Generic;
using Utility.Objects;
using Utility.Reactive.Helpers;
using Utility.Helpers;
using Utility.PropertyDescriptors;
using Utility.Trees.Abstractions;
using System;
using Utility.Models;
using System.ComponentModel;
using Utility.Nodes.Reflections;
using Utility.PropertyNotifications;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Utility.Interfaces.NonGeneric;
using LanguageExt.ClassInstances;
using Microsoft.VisualBasic;
using Utility.Collections;

namespace Utility.Nodes
{
    public class RootPropertyNode : PropertyNode
    {
        public RootPropertyNode(object instance) : base(new PropertyData(new RootDescriptor(instance), instance))
        {

        }
    }

    public class PropertyNode : Node
    {
        protected PropertyData data;
        protected bool flag;
        private bool _isRefreshing;

        public PropertyNode(PropertyData propertyData)
        {
            if (propertyData.Descriptor == null)
            {
            }
            this.data = propertyData;
        }

        public PropertyNode()
        {
        }

        public override object Data
        {
            get
            {
                return data;
            }
            set => data = value as PropertyData;
        }



        public IObservable<Change<IReadOnlyTree>> Children()
        {
            flag = true;
            var inst = data.Descriptor.GetValue(data.Instance);
            if (inst == null)
            {
                return Observable.Empty<Change<IReadOnlyTree>>();
            }

            return Observable.Create<Change<IReadOnlyTree>>(observer =>
            {
                var disposable = ChildPropertyExplorer
                                    .Convert(inst, data.Descriptor)
                                    .Subscribe(async a =>
                                    {
                                        switch (a.ChangeType)
                                        {
                                            case ChangeType.Add:
                                                {
                                                    if (a.Descriptor?.PropertyType == data.Type)
                                                    {
                                                    }
                                                    else
                                                    {
                                                        var conversion = ObjectConverter.ToValue(inst, a.Descriptor);
                                                        var node = await ToNode(conversion);
                                                        observer.OnNext(new Change<IReadOnlyTree>(node, ChangeType.Add));
                                                    }
                                                    break;
                                                }
                                            case ChangeType.Remove:
                                                {
                                                    if (this.Key is Key { Guid: { } guid })
                                                    {
                                                        var _guid = await GuidRepository.Instance.Find(guid, a.Descriptor.Name);
                                                        observer.OnNext(new Change<IReadOnlyTree>(new EmptyNode { Key = new Key(_guid, a.Descriptor.Name, a.Descriptor.PropertyType), }, ChangeType.Remove));
                                                    }
                                                    break;
                                                }
                                            case ChangeType.Reset:
                                                {
                                                    observer.OnNext(new Change<IReadOnlyTree>(null, ChangeType.Reset));
                                                    break;
                                                }
                                        }
                                    });

                var x = MethodExplorer.MethodInfos(data.Descriptor.PropertyType).ToArray();

                if (data.Descriptor.IsValueOrStringProperty() == false && x.Any())
                {
                    ToNode(new MethodsData(data.Descriptor, inst)).
                    ToObservable()
                    .Subscribe(node =>
                    {
                        observer.OnNext(new Change<IReadOnlyTree>(node, ChangeType.Add));
                    });
                }
                return disposable;
            });
        }

        protected override async Task<bool> RefreshChildrenAsync()
        {
            if (_isRefreshing)
                return false;

            if (await HasMoreChildren() == false)
                return false;

            _isRefreshing = true;

            try
            {
                items.Clear();
                Children()
                        .Subscribe(item =>
                        {
                            switch (item.Type)
                            {
                                case ChangeType.Add:
                                    items.Add(item.Value);
                                    break;
                                case ChangeType.Remove:
                                    items.RemoveOne(a => (a as IReadOnlyTree)?.Key.Equals(item.Value.Key) == true);
                                    break;
                                case ChangeType.Reset:
                                    items.Clear();
                                    break;
                            }
                        },
                        e =>
                        {

                        },
                        () =>
                        {
                            _isRefreshing = false;
                            items.Complete();
                        });


                return true;
            }
            catch (Exception ex)
            {
                Error = ex;
                return false;
            }
            finally
            {
                _isRefreshing = false;
            }

        }


        public override async Task<IReadOnlyTree> ToNode(object value)
        {
            if (value is MethodsData { } methodsData)
            {
                if (this.Key is Key { Guid: { } guid })
                {
                    var _guid = await GuidRepository.Instance.Find(guid, "methods");
                    return new MethodsNode(methodsData) { Key = new Key(_guid, "methods", null), Parent = this };
                }
                else
                    throw new Exception("f 32676 443opppp");
            }
            else if (value is PropertyData { Descriptor.Name: { } name } propertyData)
            {
                if (this.Key is Key { Guid: { } guid })
                {
                    var _guid = await GuidRepository.Instance.Find(guid, name);
                    ValueRepository.Instance.Register(_guid, propertyData as INotifyPropertyCalled);
                    ValueRepository.Instance.Register(_guid, propertyData as INotifyPropertyReceived);
                    return new PropertyNode(propertyData) { Key = new Key(_guid, name, propertyData.Type), Parent = this };
                }
                else
                    throw new Exception("f 32443opppp");
            }
            else
                throw new Exception("34422 2!pod");
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(data != null && flag == false);
        }

        public override string ToString()
        {
            return data?.Descriptor.Name;
        }

        public override Task<object?> GetChildren()
        {
            throw new NotImplementedException();
        }
    }
}


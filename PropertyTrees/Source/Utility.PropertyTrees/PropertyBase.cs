using Utility.Helpers;
using Utility.Nodes;
using System.Collections.Specialized;
using System.Windows.Input;
using Utility.Commands;
using Utility.Interfaces.NonGeneric;
using Utility.Collections;
using Utility.Observables.Generic;
using Utility.Helpers.NonGeneric;
using Utility.Trees.Abstractions;
using Utility.Models;
using System.Reflection;
using Utility.Conversions;
using Utility.PropertyDescriptors;

namespace Utility.PropertyTrees
{
    using CType = Changes.Type;
    using PropertyDescriptor = System.ComponentModel.PropertyDescriptor;

    public abstract class PropertyBase : ValueNode, IProperty
    {
        private Command<object> command;
        private Collection _methods = new();
        protected Dictionary<string, MethodNode> nodes = new();
        bool isMethodsComplete = false, isRefreshMethodsRunnings = false;

        public PropertyBase(Guid guid) : base(guid)
        {
            command = new Command<object>(a =>
            {
                if (a is IGuid guid)
                    this.Send(guid);
                else
                    throw new Exception("sd sss");
            });
        }

        public override Key Key => new(Guid, Name, PropertyType);
        public bool IsCollection => PropertyType != null && PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(PropertyType);
        public bool IsObservableCollection => PropertyType != null && typeof(INotifyCollectionChanged).IsAssignableFrom(PropertyType);
        public bool IsFlagsEnum => PropertyType.IsFlagsEnum();
        public bool IsValueType => PropertyType.IsValueType;
        public abstract bool IsReadOnly { get; }
        public override object Content => Name;

        public ICommand Command => command;

        public string? DataTemplateKey { get; set; }

        public virtual PropertyDescriptor Descriptor { get; set; }

        protected override async Task<bool> RefreshAsync()
        {
            flag = true;

            disposable = Observe<ChildrenResponse, ChildrenRequest>(new ChildrenRequest(Guid, Data, Descriptor))
                .Subscribe(response =>
                {
                    if (response.NodeChange.Type == CType.Add)
                    {
                        if (response.NodeChange == null)
                        {
                            throw new Exception("dsv2s331hj f");
                        }
                        if (_children.Any(ass => response.NodeChange.Value.Key.Equals((ass as ITree)?.Key)) == false)
                        {
                            (response.NodeChange.Value as IReadOnlyTree).Parent = this;
                            _children.Add(response.NodeChange.Value);
                        }
                    }

                    if (_children.Count == (Data as IEnumerable)?.Count())
                    {
                        _children.Complete();
                    }
                }, 
                _children.Complete);
            //else
            //    _children.Complete();

            return await Task.FromResult(true);
        }

        public virtual Type PropertyType => Data.GetType();

        protected override object ChangeType(object value)
        {
            if (PropertyType == null)
            {
                throw new ArgumentNullException("type");
            }

            if (ConversionHelper.TryChangeType(value, PropertyType, CultureInfo.CurrentCulture, out var changedValue))
            {
                return changedValue;
            }

            throw new ArgumentException("Cannot convert value {" + value + "} to type '" + PropertyType.FullName + "'.");
        }



        //public bool IsString => PropertyType == typeof(string);


        public virtual IObservable Methods
        {
            get
            {
                if (isMethodsComplete == false && isRefreshMethodsRunnings == false)
                    _ = RefreshMethodsAsync();
                return _methods;
            }
        }


        protected virtual async Task<bool> RefreshMethodsAsync()
        {
            isRefreshMethodsRunnings = true;
            if (Descriptor.IsValueOrStringProperty() == false)
                disposable = Observe<MethodsResponse, MethodsRequest>(new MethodsRequest(Guid, Data, (Parent as PropertyBase)?.Data, Descriptor))
                    .Subscribe(response =>
                    {
                        if (response is { Source: MethodInfo info, Node: var node, ChangeType: CType changeType })
                        {
                            AddMethodNode(info, node, changeType);
                            return;
                        }

                    }, () => { isRefreshMethodsRunnings = false; isMethodsComplete = true; _children.Complete(); });

            return await Task.FromResult(true);        }


        protected void AddMethodNode(MethodInfo info, IReadOnlyTree? node, Changes.Type changeType)
        {
            if (changeType == CType.Add)
            {
                if (node is not MethodNode methodNode)
                {
                    throw new Exception("VS s333 fsdfds");
                }
                else
                    _methods.Add(methodNode);
            }
        }


        public override IEnumerator<ITree> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
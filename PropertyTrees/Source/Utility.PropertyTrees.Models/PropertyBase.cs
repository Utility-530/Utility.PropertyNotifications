using Utility.PropertyTrees.Abstractions;
using Utility.Helpers;
using System.Collections.Specialized;
using System.Windows.Input;
using Utility.Commands;
using System.ComponentModel;
using System.Collections;
using System.Globalization;
using Utility.Conversions;

namespace Utility.PropertyTrees
{

    public abstract class PropertyBase :  IProperty
    {
        private Command<object> command;
        //private Collection _methods = new();
        protected Dictionary<string, MethodNode> nodes = new();
        //bool isMethodsComplete = false, isRefreshMethodsRunnings = false;
        private object data;

        public PropertyBase(Guid guid) 
        {
            command = new Command<object>(a =>
            {
                //if (a is IGuid guid)
                //    this.Send(guid);
                //else
                //    throw new Exception("sd sss");
            });
        }

        //public override Key Key => new(Guid, Name, PropertyType);
        public virtual string Name { get; set; }

        public bool IsException => PropertyType == typeof(Exception);
        public bool IsCollection => PropertyType != null && PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(PropertyType);
        public bool IsObservableCollection => PropertyType != null && typeof(INotifyCollectionChanged).IsAssignableFrom(PropertyType);
        public bool IsFlagsEnum => PropertyType.IsFlagsEnum();
        public bool IsValueType => PropertyType.IsValueType;
        public virtual bool IsError { get; set; }
        public abstract bool IsReadOnly { get; }

        //public override object Content => Name;

        public ICommand Command => command;

        public string? DataTemplateKey { get; set; }

        public object Value
        {
            get => data; set
            {
                data = value;
                //if (data is INotifyPropertyChanged propertyChanged)
                //{
                //    propertyChanged.PropertyChanged += PropertyChanged_PropertyChanged;
                //}
            }
        }

        public virtual bool HasChildren { get; }

        public virtual PropertyDescriptor Descriptor { get; set; }

        //protected override async Task<bool> RefreshAsync()
        //{
        //    flag = true;

        //    disposable = Observe<ChildrenResponse, ChildrenRequest>(new ChildrenRequest(Guid, Data, Descriptor))
        //        .Subscribe(response =>
        //        {
        //            if (response.NodeChange.Type == CType.Add)
        //            {
        //                if (response.NodeChange == null)
        //                {
        //                    throw new Exception("dsv2s331hj f");
        //                }
        //                if (_children.Any(ass => response.NodeChange.Value.Key.Equals((ass as INode)?.Key)) == false)
        //                {
        //                    response.NodeChange.Value.Parent = this;
        //                    _children.Add(response.NodeChange.Value);
        //                }
        //            }

        //            if (_children.Count == (Data as IEnumerable)?.Count())
        //            {
        //                _children.Complete();
        //            }
        //        }, 
        //        _children.Complete);
        //    //else
        //    //    _children.Complete();

        //    return await Task.FromResult(true);
        //}

        public virtual Type PropertyType => data.GetType();

        protected virtual object ChangeType(object value)
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



        public bool IsString => PropertyType == typeof(string);


        //public virtual IObservable Methods
        //{
        //    get
        //    {
        //        if (isMethodsComplete == false && isRefreshMethodsRunnings == false)
        //            _ = RefreshMethodsAsync();
        //        return _methods;
        //    }
        //}

        public bool IsValid => throw new NotImplementedException();


        //protected virtual async Task<bool> RefreshMethodsAsync()
        //{
        //    isRefreshMethodsRunnings = true;
        //    if (Descriptor.IsValueOrStringProperty() == false)
        //        disposable = Observe<MethodsResponse, MethodsRequest>(new MethodsRequest(Guid, Data, (Parent as PropertyBase)?.Data, Descriptor))
        //            .Subscribe(response =>
        //            {
        //                if (response is { Source: MethodInfo info, Node: var node, ChangeType: CType changeType })
        //                {
        //                    AddMethodNode(info, node, changeType);
        //                    return;
        //                }

        //            }, () => { isRefreshMethodsRunnings = false; isMethodsComplete = true; _children.Complete(); });

        //    return await Task.FromResult(true);        }


        //protected void AddMethodNode(MethodInfo info, INode? node, ChangeType changeType)
        //{
        //    if (changeType == CType.Add)
        //    {
        //        if (node is not MethodNode methodNode)
        //        {
        //            throw new Exception("VS s333 fsdfds");
        //        }
        //        else
        //            _methods.Add(methodNode);
        //    }
        //}

        public override string ToString()
        {
            return Name;
        }
    }
}
using Utility.PropertyTrees.Abstractions;
using Utility.Helpers;
using Utility.PropertyTrees.Infrastructure;
using Utility.Nodes;
using System.Collections.Specialized;
using System.Windows.Input;
using Utility.Commands;
using Utility.Interfaces.NonGeneric;

namespace Utility.PropertyTrees
{
    public abstract class PropertyBase : ValueNode, IProperty
    {
        Command<object> command;
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
        public bool IsException => PropertyType == typeof(Exception);
        public bool IsCollection => PropertyType != null && PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(PropertyType);
        public bool IsObservableCollection => PropertyType != null && typeof(INotifyCollectionChanged).IsAssignableFrom(PropertyType);
        public bool IsFlagsEnum => PropertyType.IsFlagsEnum();
        public bool IsValueType => PropertyType.IsValueType;
        public virtual bool IsError { get => this.GetProperty<bool>(); set => this.SetProperty(value); }
        public abstract bool IsReadOnly { get; }
        public override object Content => Name;

        public ICommand Command => command;

        public string? DataTemplateKey { get; set; }

        public virtual PropertyDescriptor Descriptor { get; set; }

        protected override async Task<bool> RefreshAsync()
        {
            if ((PropertyType.IsValueType || PropertyType == typeof(string)) != true)
                return await base.RefreshAsync();

            _children.Complete();
            return await Task.FromResult(false);
        }

        public bool IsString => PropertyType == typeof(string);

        public override string ToString()
        {
            return Name;
        }
    }
}
using System.Xml.Linq;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;

namespace Utility.Models
{
    public class ValueModel<T> : ValueModel<T, Model>
    {
        public ValueModel(T? value = default):base(value) 
        {
        }

    }

    public class ValueModel<T, TModel> : CollectionModel<TModel>, IValue<T?>, ISetValue, IGet, ISet, IGet<T>, Utility.Interfaces.Generic.ISet<T>
    {
        private T value;

        public ValueModel(T? value = default)
        {
            this.value = value;
        }

        public override void Initialise()
        {
        }

        public virtual T? Value
        {
            get { RaisePropertyCalled(value); return value; }
            set => this.RaisePropertyReceived(ref this.value, value);
        }

        public virtual T Get() => value;
        public virtual void Set(T value)
        {
            if (value?.Equals(this.value) == false || this.value?.Equals(value) == false)
            {
                var previous = this.value;
                this.value = value;
                this.RaisePropertyChanged(previous, value, nameof(Value));
            }
            else
            {
            }
        }

        public override void SetNode(INode node)
        {
            Node = node;
        }

        object? IGet.Get() => this.value;

        void ISet.Set(object value) => Set((T)value);

        object IValue.Value => value;

        object ISetValue.Value { set => Value = (T)value; }
    }
}

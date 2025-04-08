using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;

namespace Utility.Models
{
    public class ValueModel<T> : Model, IValue<T?>, ISetValue, IGet, ISet, IGet<T>, Utility.Interfaces.Generic.ISet<T>
    {
        private T value;

        public ValueModel(T? value = default)
        {
            this.value = value;
        }


        public virtual T? Value
        {
            get => RaisePropertyCalled(value);
            set => this.RaisePropertyReceived(ref this.value, value);
        }

        public virtual T Get() => value;
        public virtual void Set(T value) => this.RaisePropertyChanged(ref this.value, value, nameof(Value));

        object? IGet.Get() => this.value;

        void ISet.Set(object value) => Set((T)value);

        object IValue.Value => Value;

        object ISetValue.Value { set => Value = (T)value; }
    }
}

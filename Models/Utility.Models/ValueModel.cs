using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;

namespace Utility.Models
{
    public class ValueModel<T> : Model, IValue<T?>, ISetValue
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

        public T Get() => value;
        public void Set(T value) => this.RaisePropertyChanged(ref this.value, value, nameof(Value));


        object IValue.Value => Value;

        object ISetValue.Value { set => Value = (T)value; }
    }
}

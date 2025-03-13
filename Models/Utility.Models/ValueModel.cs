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
            get
            {
                RaisePropertyCalled(value);
                return value;
            }
            set
            {
                this.value = value;
                RaisePropertyReceived(value);
            }
        }

        object IValue.Value => Value;

        object ISetValue.Value { set => Value = (T)value; }
    }
}

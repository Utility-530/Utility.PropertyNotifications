using Utility.Interfaces;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyNotifications;

namespace Utility.Models
{
    public class ValueModel<T> : NotifyPropertyClass, IValue<T?>, IValueModel, IGet, ISet, IGet<T>, Utility.Interfaces.Generic.ISet<T>, IModel
    {
        private T? value;
        protected string m_name = "unknown";

        public ValueModel(T? value = default, bool raisePropertyCalled = true, bool raisePropertyReceived = true) : base(raisePropertyCalled, raisePropertyReceived)
        {
            this.value = value;
        }

        public virtual required string Name
        {
            get { RaisePropertyCalled(m_name); return m_name; }
            set => this.RaisePropertyReceived(ref this.m_name, value);
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
                //this.RaisePropertyChanged(nameof(Value));
            }
        }

        object? IGet.Get() => this.value;

        void ISet.Set(object value) => Set((T)value);

        object IValue.Value => value;

        object ISetValue.Value { set => Value = (T)value; }
    }
}

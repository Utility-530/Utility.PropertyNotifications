using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyNotifications;

namespace Utility.Models
{
    public class ValueModel<T> : NotifyPropertyClass, IValue<T?>, IValueModel, IGet, IGet<T>, Utility.Interfaces.Generic.ISet<T>, IModel, IIsActive
    {
        private T? value;
        protected string m_name = "unknown";
        private bool isActive;

        public ValueModel(T? value = default, bool raisePropertyCalled = true, bool raisePropertyReceived = true) : base(raisePropertyCalled, raisePropertyReceived)
        {
            this.value = value;
        }
        public IModel Parent { get; set; }

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

        public bool IsActive { get => isActive; set => this.RaisePropertyChanged(ref isActive, value); }

        object? IGet.Get() => this.value;

        void ISet.Set(object value) => Set((T)value);

        object IValue.Value => value;

        object ISetValue.Value { set => Value = (T)value; }

        public override bool Equals(object? obj)
        {
            if (obj is ValueModel<T> mNode)
                return this.Name.Equals(mNode.Name);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.Name?.GetHashCode() ?? throw new Exception("Sd33 ds");
        }
    }
}

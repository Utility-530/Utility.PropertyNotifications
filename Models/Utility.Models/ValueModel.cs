using System.Reactive.Linq;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyNotifications;

namespace Utility.Models
{
    public class ValueModel<T> : Model, IValue<T?>, ISetValue
    {
        private object dateValue;
        private T value;

        public virtual T? Value
        {
            get
            {
                var value = Get();
                RaisePropertyCalled(value);
                return value;
            }
            set
            {
                Set(value);
                RaisePropertyReceived(value);
            }
        }

        object IValue.Value => Value;

        object ISetValue.Value { set => Value = (T)value; }

        public T? Get()
        {
            if (dateValue == null)
            {
                var previous = value;
                this.WithChangesTo(a => a.Node)
                    .Subscribe(x =>
                    {
                        Node.WithChangesTo(a => a.Key).Take(1).Subscribe(key =>
                        {
                            dateValue = source.Get(Guid.Parse(key), nameof(Node.Data))
                                .Subscribe(a =>
                                {
                                    if (a is { Value: ValueModel<T> { Value:{ } value } } x)
                                    {
                                        this.value = value;
                                    }
                                    else
                                        return;

                                    //changes.OnNext(new(Name, value));
                                    RaisePropertyChanged(ref previous, value, nameof(Value));
                                });
                        });
                    });
            }
            return value;
        }

        public void Set(T? value)
        {
            this.value = value;
            this.WithChangesTo(a => a.Node)
                .Subscribe(a =>
                {
                    if ((Node as IIsReadOnly).IsReadOnly == false)
                    {
                        Node.WithChangesTo(a => a.Key).Take(1).Subscribe(a =>
                        {
                            source.Set(Guid.Parse(Node.Key), nameof(Node.Data), this, DateTime.Now);
                            //Descriptor.SetValue(Instance, value);

                            //changes.OnNext(new(Name, value));
                        });
                    }
                });    
        }

    }
}

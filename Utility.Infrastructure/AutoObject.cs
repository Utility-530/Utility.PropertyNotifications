using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Enums;
using Utility.Infrastructure;
using Utility.Infrastructure.Common;

namespace Utility.PropertyTrees.Infrastructure
{
    /// <summary>
    /// Defines a utility class to implement objects with typed properties without private fields.
    /// This class supports automatically property change notifications and error validations.
    /// </summary>
    public abstract class AutoObject : BaseObject, IDataErrorInfo, INotifyPropertyChanged, IGuid
    {
        private readonly Guid guid;

        private IDisposable disposable;

        private Dictionary<IEquatable, PropertyChange> store = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoObject"/> class.
        /// </summary>
        protected AutoObject(Guid guid)
        {
            this.guid = guid;
        }

        public override Key Key => new(guid, nameof(AutoObject), typeof(AutoObject));

        public Guid Guid => guid;

        string IDataErrorInfo.Error => Validate(null);

        [XmlIgnore]
        [Browsable(false)]
        public virtual bool HasChanged { get; set; }

        [Browsable(false)]
        public virtual bool IsValid => Validate(null) == null;

        public virtual object? Value { get; set; }

        string IDataErrorInfo.this[string columnName] => Validate(columnName);


        [MethodImpl(MethodImplOptions.NoInlining)]
        public virtual object GetProperty(Type type, [CallerMemberName] string? name = null)
        {
            //disposable ??= PropertyStore.Subscribe(this);
            var key = new Key(guid, name, type);

            if (store.TryGetValue(key, out var value))
            {
                return (value as IValueChange)?.NewValue;
            }
            var order = new PropertyOrder { Key = key, Access = Access.Get };
            this.Broadcast(order);
            return default;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool SetProperty(object value, Type type, [CallerMemberName] string name = null)
        {
            //disposable ??= PropertyStore.Subscribe(this);
            var key = new Key(guid, name, type);
            var order = new PropertyOrder { Key = key, Access = Access.Set, Value = value };
            store[key] = new PropertyChange(key, value, default);
            this.Broadcast(order);
            return true;
        }


        protected virtual string Validate(string? memberName)
        {
            //return PropertyStore.Validate(memberName);
            return default;
        }

        public override bool OnNext(object obj)
        {
            if (obj is not PropertyChange { Key: Key { Guid: var guid } } valueChange)
            {
                return base.OnNext(obj);
            }
            if (this.guid != guid)
            {
                return false;
            }

            if (store.ContainsKey(valueChange.Key))
            {
                if (store[valueChange.Key].NewValue == valueChange.NewValue)
                {
                    return true;
                }
                store.Remove(valueChange.Key);
            }
            store.Add(valueChange.Key, valueChange);

            if (valueChange is IName { Name: var name } && valueChange.NewValue != null)
            {
                OnPropertyChanged(name);
                OnPropertyChanged(nameof(Value));
                return true;
            }
            throw new Exception("zg 34422111");
        }
    }
}
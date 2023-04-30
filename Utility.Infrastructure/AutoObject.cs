using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Enums;
using Utility.Infrastructure;

namespace Utility.PropertyTrees.Infrastructure
{
    /// <summary>
    /// Defines a utility class to implement objects with typed properties without private fields.
    /// This class supports automatically property change notifications and error validations.
    /// </summary>
    public abstract class AutoObject : BaseObject, IObserver, IDataErrorInfo, INotifyPropertyChanged, IGuid
    {
        private readonly Guid guid;

        private IDisposable disposable;

        private HashSet<IEquatable> store = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoObject"/> class.
        /// </summary>
        protected AutoObject(Guid guid)
        {
            this.guid = guid;
        }

        public override Key Key => new (guid, nameof(AutoObject), typeof(AutoObject));

        public Guid Guid => guid;

        string IDataErrorInfo.Error => Validate(null);

        [XmlIgnore]
        [Browsable(false)]
        public virtual bool HasChanged { get; set; }

        [Browsable(false)]
        public virtual bool IsValid => Validate(null) == null;

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
            this.Broadcast(order);
            return true;
        }


        protected virtual string Validate(string? memberName)
        {
            //return PropertyStore.Validate(memberName);
            return default;
        }

        public override void OnNext(object obj)
        {
            if (obj is not IValueChange valueChange)
            {
                base.OnNext(obj);
                return;
            }

            if (store.Contains(valueChange))
                store.Remove(valueChange);
            store.Add(valueChange);

            //store[propertyResult] = propertyResult.Value;
            if (valueChange is IName { Name: var name })
            {
                OnPropertyChanged(name);
                return;
            }
            throw new Exception("zg 34422111");
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }
    }
}
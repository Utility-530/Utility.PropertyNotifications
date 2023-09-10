using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Observables.Generic;

namespace Utility.Infrastructure
{

    public class Store
    {
        private Dictionary<IEquatable, object> store = new();

        public object? this[IEquatable key]
        {
            get
            {
                if (store.TryGetValue(key, out var value))
                {
                    return value;
                }
                return default;
            }
            set
            {
                store[key] = value;
            }
        }

        //public void Set(IEquatable key, object value) => store[key] = value;

        public static Store Instance { get; } = new Store();
    }

    /// <summary>
    /// Defines a utility class to implement objects with typed properties without private fields.
    /// This class supports automatically property change notifications and error validations.
    /// </summary>
    public abstract class AutoObject : BaseObject, IDataErrorInfo, INotifyPropertyChanged, IGuid
    {
        private readonly Guid guid;
        private IDisposable? disposable;


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

        string IDataErrorInfo.this[string columnName] => Validate(columnName);

        public DateTime? LastUpdate { get; set; }

        protected virtual object? GetValue(IEquatable name)
        {
            return Store.Instance[name];
        }

        protected virtual void SetValue(IEquatable name, object value)
        {
            Store.Instance[name] = value;
            OnPropertyChanged((name as Key)?.Name);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public object? GetProperty(IEquatable key)
        {
            if (LastUpdate is not DateTime value)
            {
                //TODO: show progress
                Observe<GetPropertyResponse, GetPropertyRequest>(new(key))
                    .Select(a => a.Value)
                    .Subscribe(a =>
                    {
                        LastUpdate = DateTime.Now;
                        SetValue(key, a);
                    });
            }

            return GetValue(key);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void SetProperty(IEquatable key, object value)
        {
            disposable?.Dispose();
            disposable = Observe<SetPropertyResponse, SetPropertyRequest>(new(key, value))
                .Subscribe(a =>
                {
                    LastUpdate = DateTime.Now;
                    SetValue(key, a.Value);
                    // Validation response
                });
        }


        protected virtual string Validate(string? memberName)
        {
            //return PropertyStore.Validate(memberName);
            return default;
        }
    }
}
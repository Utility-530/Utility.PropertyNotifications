using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Enums;
using Utility.Infrastructure;
using System.Globalization;
using Utility.Conversions;
using Utility.Observables;
using Utility.Observables.Generic;

namespace Utility.PropertyTrees.Infrastructure
{

    public class Store
    {
        private Dictionary<IEquatable, object> store = new();

        public object this[IEquatable key]
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
        private IDisposable disposable;


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

        //public abstract  Type PropertyType { get; }
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
        public object? GetProperty(Key key)
        {
            if (LastUpdate is DateTime value)
            {
                return GetValue(key);
            }

            //var key = new Key(guid, name, type);

            // show progress
            this.Observe<GetPropertyResponse, GetPropertyRequest>(new(key))
                .Select(a => a.Value)
                .Subscribe(a =>
                {
                    SetValue(key, a);
                    LastUpdate = DateTime.Now;
                    //OnPropertyChanged(nameof(Value));
                });
            return default;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void SetProperty(Key key, object value)
        {
            //SetValue(name, value);

            //var key = new Key(guid, name, type);
            var order = new SetPropertyRequest(key, value);
            this.Observe<SetPropertyResponse, SetPropertyRequest>(new(key, value))
                .Subscribe(a =>
                {
                    SetValue(key, a.Value);
                    LastUpdate = DateTime.Now;
                    // Validation response
                });
        }


        protected virtual string Validate(string? memberName)
        {
            //return PropertyStore.Validate(memberName);
            return default;
        }


        #region PropertyChange
        /// <inheritdoc />
        /// <summary>
        ///     The event on property changed
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        ///     Raise the <see cref="PropertyChanged" /> event
        /// </summary>
        /// <param name="propertyName">The caller member name of the property (auto-set)</param>
        //[NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = default)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Set a property and raise the <see cref="PropertyChanged" /> event
        /// </summary>
        /// <typeparam name="T">The type of the Property</typeparam>
        /// <param name="field">A reference to the backing field from the property</param>
        /// <param name="value">The new value being set</param>
        /// <param name="callerName">The caller member name of the property (auto-set)</param>
        protected void Set<T>(ref T field, T value, [CallerMemberName] string? callerName = default)
        {
            if (field?.Equals(value) == true)
            {
                return;
            }

            field = value;
            OnPropertyChanged(callerName);
        }
        #endregion PropertyChange
    }
}
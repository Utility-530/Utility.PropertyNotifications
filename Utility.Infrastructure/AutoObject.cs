using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Conversions;
using Utility.Enums;
using Utility.Infrastructure;
using System.Collections;
using Utility.Infrastructure.Abstractions;
using Utility.Observables.NonGeneric;

namespace Utility.PropertyTrees.Infrastructure
{
    /// <summary>
    /// Defines a utility class to implement objects with typed properties without private fields.
    /// This class supports automatically property change notifications and error validations.
    /// </summary>
    public abstract class AutoObject : BaseObject, IObserver, IDataErrorInfo, INotifyPropertyChanged, IGuid, IObservable
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
            Initialise();
        }

        public override Key Key => new (guid, nameof(AutoObject), typeof(AutoObject));

        private List<Order> orders = new();

        //public static IPropertyStore PropertyStore { get; set; }
        private async void Initialise()
        {
            //lock (PropertyStore)
            //    PropertyStore?.Subscribe(this);
        }

        public Guid Guid => guid;

        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An error message indicating what is wrong with this object. The default is an empty string ("").
        /// </returns>
        string IDataErrorInfo.Error => Validate(null);

        /// <summary>
        /// Gets a value indicating whether at least a property has changed since the last time this property has been set to false.
        /// </summary>
        /// <value>
        /// <c>true</c> if at least a property has changed; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        [Browsable(false)]
        public virtual bool HasChanged { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        [Browsable(false)]
        public virtual bool IsValid => Validate(null) == null;


        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets the <see cref="string"/> with the specified column name.
        /// </summary>
        /// <value></value>
        string IDataErrorInfo.this[string columnName] => Validate(columnName);

        /// <summary>
        /// Gets the default value for a given property.
        /// </summary>
        /// <param name="propertyName">The property name. May not be null.</param>
        /// <returns>The default value. May be null.</returns>
        protected virtual object GetDefaultValue(PropertyDescriptor descriptor)
        {
            if (descriptor.Name == null)
            {
                throw new ArgumentNullException("propertyName");
            }

            PropertyInfo propertyInfo = descriptor.ComponentType.GetProperty(descriptor.Name);
            if (propertyInfo == null)
            {
                //if (ThrowOnInvalidProperty)
                //{
                //throw new InvalidOperationException(SR.GetString("invalidPropertyName", GetType().FullName, propertyName));
                throw new InvalidOperationException("invalidPropertyName");
                // }

                //  return null;
            }

            object defaultValue = propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
            DefaultValueAttribute att = Attribute.GetCustomAttribute(propertyInfo, typeof(DefaultValueAttribute), true) as DefaultValueAttribute;
            if (att != null)
            {
                return ConversionHelper.ChangeType(att.Value, defaultValue);
            }

            return defaultValue;
        }

        /// <summary>
        /// Called when a property changed.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="setChanged">if set to <c>true</c> set the HasChanged property to true.</param>
        /// <param name="forceRaise">if set to <c>true</c> force the raise, even if RaisePropertyChanged is set to false.</param>
        /// <returns>
        /// true if the event has been raised; otherwise false.
        /// </returns>
        protected virtual bool OnPropertyChanged(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            return true;
        }

        /// <summary>
        /// Gets a property value.
        /// </summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <returns>The value automatically converted into the requested type.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        protected virtual T GetProperty<T>([CallerMemberName] string? name = null)
        {
            return (T)GetProperty(typeof(T), name);
        }

        /// <summary>
        /// Gets a property value.
        /// </summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <returns>The value automatically converted into the requested type.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        protected virtual object GetProperty(Type type, [CallerMemberName] string? name = null)
        {
            //disposable ??= PropertyStore.Subscribe(this);
            var key = new Key(guid, name, type);

            if (store.TryGetValue(key, out var value))
            {
                return (value as IValueChange).NewValue;
            }
            var order = new Order { Key = key, Access = Access.Get };
            orders.Add(order);
            this.Broadcast(order);


            //order.Subscribe(this);
            //PropertyStore.OnNext(order);

            return default;
        }

        /// <summary>
        /// Sets a property value.
        /// </summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>true if the value has changed; otherwise false.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool SetProperty<T>(T value, [CallerMemberName] string name = null)
        {
            SetProperty(value, typeof(T), name);
            return true;
        }

        /// <summary>
        /// Sets a property value.
        /// </summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>true if the value has changed; otherwise false.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool SetProperty(object value, [CallerMemberName] string name = null)
        {
            SetProperty(value, value.GetType(), name);
            return true;
        }

        /// <summary>
        /// Sets a property value.
        /// </summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>true if the value has changed; otherwise false.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool SetProperty(object value, Type type, [CallerMemberName] string name = null)
        {
            //disposable ??= PropertyStore.Subscribe(this);
            var key = new Key(guid, name, type);
            var order = new Order { Key = key, Access = Access.Set, Value = value };
            //order.Subscribe(this);
            orders.Add(order);
            //PropertyStore.SetValue(order.Key, value);
            this.Broadcast(order);
            return true;
        }

        /// <summary>
        /// Validates the specified member name.
        /// </summary>
        /// <param name="memberName">The member to validate or null to validate all members.</param>
        /// <returns>A string if an error occured; null otherwise.</returns>
        protected virtual string Validate(string? memberName)
        {
            //return PropertyStore.Validate(memberName);
            return default;
        }

        public bool Equals(IEquatable? other)
        {
            return Guid == ((other as AutoObject)?.Guid ?? (other as Key)?.Guid);
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }

        public void OnNext(object obj)
        {
            if (obj is not IValueChange valueChange)
            {
                throw new Exception("sdf 33 a");
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


        //public IDisposable Subscribe(IObserver value)
        //{
        //    throw new NotImplementedException();
        //}

        public override IEnumerator GetEnumerator()
        {
            return orders.GetEnumerator();
        }


        private void Broadcast(object obj)
        {
            foreach (var observer in Observers)
                observer.OnNext(obj);
        }
    }
}
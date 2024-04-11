using System.ComponentModel;
using System.Runtime.CompilerServices;
using Utility.Interfaces;

namespace Utility.PropertyNotifications
{

    //public abstract class ViewModelBase<T> : ViewModelBase
    //{
    //    private T value;

    //    public T Value
    //    {
    //        get
    //        {
    //            RaisePropertyCalled();
    //            return value;
    //        }
    //        set
    //        {
    //            this.value = value;
    //            RaisePropertyReceived();
    //        }
    //    }
    //}

    /// <summary>
    /// Base class for all ViewModel classes in the application.
    /// It provides support for property change notifications
    /// and has a DisplayName property.  This class is abstract.
    /// </summary>

    public abstract record NotifyProperty : INotifyPropertyCalled, INotifyPropertyReceived, INotifyPropertyChanged
    {
        //private Lazy<Dictionary<string, PropertyInfo>> properties;
        //private Lazy<Dictionary<string, FieldInfo>> fields;

        #region Constructor

        protected NotifyProperty()
            : base()
        {
            // _dispatcher = Dispatcher.CurrentDispatcher;
            //properties = new Lazy<Dictionary<string, PropertyInfo>>(() => this.GetType().GetProperties().ToDictionary(a => a.Name, a => a));
            //fields = new Lazy<Dictionary<string, FieldInfo>>(() =>
            //{
            //    var type = this.GetType();
            //    var fields = type.GetFields().Where(f => f.GetCustomAttribute<CompilerGeneratedAttribute>() == null).ToArray();
            //    return fields.ToDictionary(a => a.Name, a => a, StringComparer.InvariantCultureIgnoreCase);
            //});

            //this.PropertyChanging += ViewModelBase_PropertyChanging;        
        }

        //private void ViewModelBase_PropertyChanging(object sender, PropertyChangingEventArgs e)
        //{
        //    PropertyChangedName = e.PropertyName;
        //}

        #endregion Constructor

        #region events

        //public event Action<Initialised> Initialised;

        //public virtual Task Initialise()
        //{
        //    Initialised?.Invoke(new Initialised(this));
        //    return Task.CompletedTask;
        //}

        #endregion



        #region Debugging Aides

        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This
        /// method does not exist in a Release build.
        /// </summary>
        //[Conditional("DEBUG")]
        //[DebuggerStepThrough]
        //public void VerifyPropertyName(string propertyName)
        //{
        //    if (propertyName == null || propertyName == string.Empty)
        //    {
        //        return;
        //    }
        //    // Verify that the property name matches a real,
        //    // public, instance property on this object.
        //    if (TypeDescriptor.GetProperties(this)[propertyName] == null)
        //    {
        //        string msg = "Invalid property name: " + propertyName;

        //        //if (this.ThrowOnInvalidPropertyName)
        //        //    throw new Exception(msg);
        //        //else
        //        Debug.Print(msg);
        //    }
        //}

        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might
        /// override this property's getter to return true.
        /// </summary>



        #endregion Debugging Aides

        public event PropertyChangingEventHandler PropertyChanging;

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        public virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }


        ///// <summary>
        /////     Set a property and raise the <see cref="PropertyChanged" /> event
        ///// </summary>
        ///// <typeparam name="T">The type of the Property</typeparam>
        ///// <param name="field">A reference to the backing field from the property</param>
        ///// <param name="value">The new value being set</param>
        ///// <param name="callerName">The caller member name of the property (auto-set)</param>
        //protected T Get<T>(T field, [CallerMemberName] string callerMemberName = null)
        //{
        //    this.RaisePropertyCalled(callerMemberName);
        //    return field;
        //}

        ///// <summary>
        /////     Set a property and raise the <see cref="PropertyChanged" /> event
        ///// </summary>
        ///// <typeparam name="T">The type of the Property</typeparam>
        ///// <param name="field">A reference to the backing field from the property</param>
        ///// <param name="value">The new value being set</param>
        ///// <param name="callerName">The caller member name of the property (auto-set)</param>
        //protected void _Set<T>(ref T field, T value, [CallerMemberName] string callerMemberName = null)
        //{
        //    field = value;
        //    this.RaisePropertyReceived(callerMemberName);
        //}

        ///// <summary>
        /////     Set a property and raise the <see cref="PropertyChanged" /> event
        ///// </summary>
        ///// <typeparam name="T">The type of the Property</typeparam>
        ///// <param name="field">A reference to the backing field from the property</param>
        ///// <param name="value">The new value being set</param>
        ///// <param name="callerName">The caller member name of the property (auto-set)</param>
        //protected bool Set<T>(ref T field, T value, [CallerMemberName] string callerName = default)
        //{
        //    if (field?.Equals(value) != true && ((field == null && value == null) == false))
        //    {
        //        field = value;
        //        RaisePropertyChanged(callerName);
        //        return true;
        //    }
        //    return false;
        //}

        //public virtual bool Set<T>(string fieldName, T value)
        //{
        //    var currentValue = (T)fields.Value[fieldName].GetValue(this);
        //    if (currentValue?.Equals(value) != true && ((currentValue == null && value == null) == false))
        //    {
        //        fields.Value[fieldName].SetValue(this, value);
        //        RaisePropertyChanged(fieldName);
        //        return true;
        //    }
        //    return false;
        //}

        #endregion INotifyPropertyChanged Members

        #region INotifyPropertyCalled Members


        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyCalledEventHandler PropertyCalled;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void RaisePropertyCalled(object value, [CallerMemberName] string propertyName = null)
        {
            //if (PropertyChangedName != propertyName)
            //{

            PropertyCalled?.Invoke(this, PropertyCalledArgs(propertyName, value));

            //}
            //else
            //    PropertyChangedName = null;
        }

        private PropertyCalledEventArgs PropertyCalledArgs(string propertyName, object value)
        {
            //var info = properties.Value[propertyName];
            //if (fields.Value.TryGetValue(propertyName, out var fieldInfo) == false)
            //{
            //    if (fields.Value.TryGetValue("_" + propertyName, out fieldInfo) == false)
            //    {
            //        throw new InvalidOperationException(propertyName);
            //    }
            //}
            var e = new PropertyCalledEventArgs(propertyName, value);
            return e;
        }

        #endregion INotifyPropertyCalled Members

        #region INotifyPropertyReceived Members

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyReceivedEventHandler PropertyReceived;



        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void RaisePropertyReceived(object value, [CallerMemberName] string propertyName = null)
        {
            var handler = PropertyReceived;
            if (handler != null)
            {

                //if (fields.Value.TryGetValue(propertyName, out var fieldInfo) == false)
                //{
                //    //if (fields.Value.TryGetValue("_" + propertyName, out fieldInfo) == false)
                //    //{
                //    //    throw new InvalidOperationException(propertyName);
                //    //}
                //}
                var e = new PropertyReceivedEventArgs(propertyName, value);
                handler(this, e);
            }

        }



        #endregion INotifyPropertyReceived Members



    }
}
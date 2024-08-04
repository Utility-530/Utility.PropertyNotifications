using System.ComponentModel;
using System.Runtime.CompilerServices;
using Utility.Interfaces;

namespace Utility.PropertyNotifications
{


    /// <summary>
    /// Base class for all ViewModel classes in the application.
    /// It provides support for property change notifications
    /// and has a DisplayName property.  This class is abstract.
    /// </summary>

    public abstract class NotifyPropertyClass : INotifyPropertyCalled, INotifyPropertyReceived, INotifyPropertyChanged, IRaiseChanges
    {
        bool flag;


        #region Constructor

        protected NotifyPropertyClass()
            : base()
        {   
        }



        #endregion Constructor

        #region events


        #endregion



        #region Debugging Aides

       


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


     
        #endregion INotifyPropertyChanged Members

        #region INotifyPropertyCalled Members


        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyCalledEventHandler? PropertyCalled;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        public virtual void RaisePropertyCalled(object? value, [CallerMemberName] string propertyName = null)
        {
        
            if (flag == false)
                PropertyCalled?.Invoke(this, PropertyCalledArgs(propertyName, value));
        }

        private PropertyCalledEventArgs PropertyCalledArgs(string propertyName, object? value)
        {         
            var e = new PropertyCalledEventArgs(propertyName, value);
            return e;
        }

        #endregion INotifyPropertyCalled Members

        #region INotifyPropertyReceived Members

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyReceivedEventHandler? PropertyReceived;



        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        public virtual void RaisePropertyReceived(object value, [CallerMemberName] string propertyName = null)
        {
            var handler = PropertyReceived;
            if (handler != null)
            {
            
                flag = true;
                var e = new PropertyReceivedEventArgs(propertyName, value);
                handler(this, e);
                flag = false;
            }
        }

        #endregion INotifyPropertyReceived Members
    }
}
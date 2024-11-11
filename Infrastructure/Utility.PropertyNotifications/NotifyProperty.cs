using System.ComponentModel;
using System.Runtime.CompilerServices;
using Utility.Interfaces;

namespace Utility.PropertyNotifications
{

    /// <summary>
    /// Base class for all ViewModel records.
    /// It provides support for property change notifications.
    /// </summary>
    public abstract record NotifyProperty : INotifyPropertyCalled, INotifyPropertyReceived, INotifyPropertyChanged, IRaiseChanges
    {
        bool flag;

        protected NotifyProperty() : base()
        {        
        }

        public event PropertyChangingEventHandler? PropertyChanging;

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
        public virtual void RaisePropertyCalled(object? value, [CallerMemberName] string? propertyName = null)
        {
            if (flag == false)
                PropertyCalled?.Invoke(this, PropertyCalledArgs(propertyName, value));
        }

        private PropertyCalledEventArgs PropertyCalledArgs(string? propertyName, object? value)
        {
            return new PropertyCalledEventArgs(propertyName, value);
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
        public virtual void RaisePropertyReceived(object value, [CallerMemberName] string? propertyName = null)
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
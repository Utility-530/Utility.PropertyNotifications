using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Utility.Interfaces;

namespace Utility.PropertyNotifications
{
    /// <summary>
    /// Base class for all ViewModel classes in the application.
    /// It provides support for property change notifications.
    /// </summary>
    public abstract class NotifyPropertyClass : INotifyPropertyCalled, INotifyPropertyReceived, INotifyPropertyChanged, IRaiseChanges
    {
        bool flag;

        protected NotifyPropertyClass() : base()
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
        public virtual void RaiseValuePropertyChanged<T>(ref T previousValue, T value, [CallerMemberName] string? propertyName = null) where T : struct
        {
            if (value.Equals(default) && previousValue.Equals(default))
                return;
            if (value.Equals(previousValue) == true)
                return;

            var _previousValue = previousValue;
            previousValue = value;
            RaisePropertyChanged(_previousValue, value, propertyName);
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        public virtual void RaisePropertyChanged<T>(ref T previousValue, T value, [CallerMemberName] string? propertyName = null)
        {
            if (value?.Equals(default) == true && previousValue?.Equals(default) == true)
                return;
            if (value?.Equals(previousValue) == true)
                return;

            var _previousValue = previousValue;
            previousValue = value;
            RaisePropertyChanged(_previousValue, value, propertyName);
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        public virtual void RaisePropertyChanged<T>(ref T? previousValue, T? value, [CallerMemberName] string? propertyName = null) where T : struct
        {
            if (value.Equals(default) && previousValue.Equals(default))
                return;
            if (value.Equals(previousValue) == true)
                return;

            var _previousValue = previousValue;
            previousValue = value;
            RaisePropertyChanged(_previousValue, value, propertyName);
        }

        public virtual void RaisePropertyChanged<T>(T previousValue, T value, [CallerMemberName] string? propertyName = null)
        {
            if (PropertyChanged != null)
            {
                var e = new PropertyChangedExEventArgs(propertyName, value, previousValue);
                PropertyChanged(this, e);
            }
        }

        public virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (PropertyChanged != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                PropertyChanged(this, e);
            }
        }

        #endregion INotifyPropertyChanged Members

        #region INotifyPropertyCalled Members

        private ObservableCollection<PropertyCalledEventArgs> missedCalls = [];

        public IEnumerable<PropertyCalledEventArgs> MissedCalls => missedCalls;

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyCalledEventHandler? PropertyCalled;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        //public virtual T? RaisePropertyCalled<T>(T? value, [CallerMemberName] string? propertyName = null)
        //{
        //    RaisePropertyCalled((object?)value, propertyName);
        //    return value;
        //}

        public virtual void RaisePropertyCalled(object? value, [CallerMemberName] string? propertyName = null)
        {
            if (flag == false)
            {
                var args = PropertyCalledArgs(propertyName, value);
                if (PropertyCalled is PropertyCalledEventHandler handler)
                    handler.Invoke(this, args);
                else
                    missedCalls.Add(args);
            }
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
        public virtual void RaisePropertyReceived(object oldValue, object value, [CallerMemberName] string? propertyName = null)
        {
            var handler = PropertyReceived;
            if (handler != null)
            {

                flag = true;
                var e = new PropertyReceivedEventArgs(propertyName, value, oldValue);
                handler(this, e);
                flag = false;
            }
        }

        public virtual void RaisePropertyReceived<T>(ref T previousValue, T value, [CallerMemberName] string? propertyName = null)
        {
            if (value == null && previousValue == null)
                return;
            if (value?.Equals(previousValue) == true)
                return;

            var _previousValue = previousValue;
            previousValue = value;
            RaisePropertyReceived(_previousValue, value, propertyName);
        }

        #endregion INotifyPropertyReceived Members
    }
}
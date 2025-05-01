using System.Collections.ObjectModel;
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
        public virtual void RaisePropertyChanged<T>(ref T previousValue, T value, [CallerMemberName] string? propertyName = null) where T : struct
        {
            if (value.Equals(previousValue) == true)
                return;

            var handler = PropertyChanged;
            var _previousValue = previousValue;
            previousValue = value;

            if (handler != null)
            {
                var e = new PropertyChangedExEventArgs(propertyName, value, _previousValue);
                handler(this, e);
            }
        }

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
        public virtual void RaisePropertyCalled(object? value, [CallerMemberName] string? propertyName = null)
        {
            if (flag == false)
                if (PropertyCalled is { } called)
                    called.Invoke(this, PropertyCalledArgs(propertyName, value));
                else
                    missedCalls.Add(PropertyCalledArgs(propertyName, value));
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
        public virtual void RaisePropertyReceived(object? value, object? oldValue, [CallerMemberName] string? propertyName = null)
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
        #endregion INotifyPropertyReceived Members



    }
}
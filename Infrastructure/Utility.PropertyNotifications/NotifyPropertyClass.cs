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

        protected NotifyPropertyClass(bool raisePropertyCalled = true, bool raisePropertyReceived = true, bool raisePropertyChanged = true) : base()
        {
            this.raisePropertyCalled = raisePropertyCalled;
            this.raisePropertyReceived = raisePropertyReceived;
            this.raisePropertyChanged = raisePropertyChanged;
        }

        public event PropertyChangingEventHandler? PropertyChanging;

        #region INotifyPropertyChanged Members

        bool raisePropertyChanged;
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        public virtual bool RaiseValuePropertyChanged<T>(ref T previousValue, T value, [CallerMemberName] string? propertyName = null) where T : struct
        {
            if (value.Equals(default) && previousValue.Equals(default))
                return false;
            if (value.Equals(previousValue) == true)
                return false;

            var _previousValue = previousValue;
            previousValue = value;
            RaisePropertyChanged(_previousValue, value, propertyName);
            return true;
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        public virtual bool RaisePropertyChanged<T>(ref T previousValue, T value, [CallerMemberName] string? propertyName = null)
        {
            if (value?.Equals(default) == true && previousValue?.Equals(default) == true)
                return false;
            if (value?.Equals(previousValue) == true)
                return false;

            var _previousValue = previousValue;
            previousValue = value;
            RaisePropertyChanged(_previousValue, value, propertyName);
            return true;
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        public virtual bool RaisePropertyChanged<T>(ref T? previousValue, T? value, [CallerMemberName] string? propertyName = null) where T : struct
        {
            if (value.Equals(default) && previousValue.Equals(default))
                return false;
            if (value.Equals(previousValue) == true)
                return false;

            var _previousValue = previousValue;
            previousValue = value;
            RaisePropertyChanged(_previousValue, value, propertyName);
            return true;
        }

        public virtual bool RaisePropertyChanged<T>(T previousValue, T value, [CallerMemberName] string? propertyName = null)
        {
            if (PropertyChanged != null && raisePropertyChanged)
            {
                var e = new PropertyChangedExEventArgs(propertyName, value, previousValue);
                PropertyChanged(this, e);
                return true;
            }
            return false;
        }

        public virtual bool RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (PropertyChanged != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                PropertyChanged(this, e);
                return true;
            }
            return false;
        }

        #endregion INotifyPropertyChanged Members

        #region INotifyPropertyCalled Members

        private ObservableCollection<PropertyCalledEventArgs> missedCalls = [];
        private SynchronizationContext? context;
        private readonly bool raisePropertyCalled;


        public IEnumerable<PropertyCalledEventArgs> MissedCalls => missedCalls;

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyCalledEventHandler? PropertyCalled;

        /// <summary>
        /// Raises this object's PropertyChanged event
        /// !Dangerous 'value' can change before method finished executing!
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        //public virtual T? RaisePropertyCalled<T>(T? value, [CallerMemberName] string? propertyName = null)
        //{
        //    RaisePropertyCalled((object?)value, propertyName);
        //    return value;
        //}

        public virtual bool RaisePropertyCalled(object? value, [CallerMemberName] string? propertyName = null)
        {
            context ??= SynchronizationContext.Current;
            if (flag == false)
            {
                var args = PropertyCalledArgs(propertyName, value);
                if (raisePropertyCalled && PropertyCalled is PropertyCalledEventHandler handler)
                {
                    handler.Invoke(this, args);
                    return true;
                }
                else if (context != null)
                    context.Post(a => missedCalls.Add(args), args);
                    
            }
            return false;
        }

        private PropertyCalledEventArgs PropertyCalledArgs(string? propertyName, object? value)
        {
            return new PropertyCalledEventArgs(propertyName, value);
        }

        #endregion INotifyPropertyCalled Members

        #region INotifyPropertyReceived Members

        private readonly bool raisePropertyReceived;
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyReceivedEventHandler? PropertyReceived;



        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        public virtual bool RaisePropertyReceived(object oldValue, object value, [CallerMemberName] string? propertyName = null)
        {
            var handler = PropertyReceived;
            if (handler != null)
            {

                flag = true;
                var e = new PropertyReceivedEventArgs(propertyName, value, oldValue);
                handler(this, e);
                flag = false;
                return true;
            }
            return false;
        }

        public virtual bool RaisePropertyReceived<T>(ref T previousValue, T value, [CallerMemberName] string? propertyName = null)
        {
            if (value == null && previousValue == null)
                return false;
            if (value?.Equals(previousValue) == true)
                return false;

            var _previousValue = previousValue;
            previousValue = value;
            if (raisePropertyReceived)
            {
                RaisePropertyReceived(_previousValue, value, propertyName);
                return true;
            }
            return false;
        }

        #endregion INotifyPropertyReceived Members
    }
}
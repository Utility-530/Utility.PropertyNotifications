//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.ComponentModel.DataAnnotations;
//using System.Diagnostics;
//using System.Linq;
//using System.Reflection;
//using System.Runtime.CompilerServices;
//using System.Text.Json.Serialization;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using System.Windows;
//using Utility.PropertyNotifications;


//namespace Puttshack.ViewModels.Base
//{

//    //public abstract class ViewModelBase<T> : ViewModelBase
//    //{
//    //    private T value;

//    //    public T Value
//    //    {
//    //        get
//    //        {
//    //            RaisePropertyCalled();
//    //            return value;
//    //        }
//    //        set
//    //        {
//    //            this.value = value;
//    //            RaisePropertyReceived();
//    //        }
//    //    }
//    //}

//    /// <summary>
//    /// Base class for all ViewModel classes in the application.
//    /// It provides support for property change notifications
//    /// and has a DisplayName property.  This class is abstract.
//    /// </summary>
 
//    public static class NotifyPropertyExtensions
//    {



  



//        #region INotifyPropertyChanged Members


//        /// <summary>
//        /// Raises this object's PropertyChanged event.
//        /// </summary>
//        /// <param name="propertyName">The property that has a new value.</param>
//        public static void RaisePropertyChanged(this INotifyPropertyChanged notifyPropertyChanged [CallerMemberName] string? propertyName = null)
//        {
     

//            var handler = PropertyChanged;
//            if (handler != null)
//            {
//                var e = new PropertyChangedEventArgs(propertyName);
//                handler(this, e);
//            }
//        }


//        ///// <summary>
//        /////     Set a property and raise the <see cref="PropertyChanged" /> event
//        ///// </summary>
//        ///// <typeparam name="T">The type of the Property</typeparam>
//        ///// <param name="field">A reference to the backing field from the property</param>
//        ///// <param name="value">The new value being set</param>
//        ///// <param name="callerName">The caller member name of the property (auto-set)</param>
//        //protected T Get<T>(T field, [CallerMemberName] string callerMemberName = null)
//        //{
//        //    this.RaisePropertyCalled(callerMemberName);
//        //    return field;
//        //}

//        ///// <summary>
//        /////     Set a property and raise the <see cref="PropertyChanged" /> event
//        ///// </summary>
//        ///// <typeparam name="T">The type of the Property</typeparam>
//        ///// <param name="field">A reference to the backing field from the property</param>
//        ///// <param name="value">The new value being set</param>
//        ///// <param name="callerName">The caller member name of the property (auto-set)</param>
//        //protected void _Set<T>(ref T field, T value, [CallerMemberName] string callerMemberName = null)
//        //{
//        //    field = value;
//        //    this.RaisePropertyReceived(callerMemberName);
//        //}

//        ///// <summary>
//        /////     Set a property and raise the <see cref="PropertyChanged" /> event
//        ///// </summary>
//        ///// <typeparam name="T">The type of the Property</typeparam>
//        ///// <param name="field">A reference to the backing field from the property</param>
//        ///// <param name="value">The new value being set</param>
//        ///// <param name="callerName">The caller member name of the property (auto-set)</param>
//        //protected bool Set<T>(ref T field, T value, [CallerMemberName] string callerName = default)
//        //{
//        //    if (field?.Equals(value) != true && ((field == null && value == null) == false))
//        //    {
//        //        field = value;
//        //        RaisePropertyChanged(callerName);
//        //        return true;
//        //    }
//        //    return false;
//        //}

//        //public virtual bool Set<T>(string fieldName, T value)
//        //{
//        //    var currentValue = (T)fields.Value[fieldName].GetValue(this);
//        //    if (currentValue?.Equals(value) != true && ((currentValue == null && value == null) == false))
//        //    {
//        //        fields.Value[fieldName].SetValue(this, value);
//        //        RaisePropertyChanged(fieldName);
//        //        return true;
//        //    }
//        //    return false;
//        //}

//        #endregion INotifyPropertyChanged Members

//        #region INotifyPropertyCalled Members

//        private PropertyCalledEventHandler propertyCalled;
//        private readonly HashSet<string> propertiesCalled = new();
//        private readonly HashSet<string> propertiesNotCalled = new();

//        /// <summary>
//        /// Raised when a property on this object has a new value.
//        /// </summary>
//        public event PropertyCalledEventHandler PropertyCalled
//        {
//            add
//            {
//                propertyCalled = (PropertyCalledEventHandler)Delegate.Combine(propertyCalled, value);
//                // waits for subscription before firing propertycalled event instead of ignoring
//                foreach (var propertyName in propertiesNotCalled)
//                {
//                    propertyCalled.Invoke(this, PropertyCalledArgs(propertyName, propertiesCalled.Add(propertyName)));
//                }
//                propertiesNotCalled.Clear();
//            }

//            remove
//            {
//                propertyCalled = (PropertyCalledEventHandler)Delegate.Remove(propertyCalled, value); ;
//            }
//        }

//        /// <summary>
//        /// Raises this object's PropertyChanged event.
//        /// </summary>
//        /// <param name="propertyName">The property that has a new value.</param>
//        protected virtual void RaisePropertyCalled([CallerMemberName] string propertyName = null)
//        {
//            if (PropertyChangedName != propertyName)
//            {
//                if (propertyCalled != null)
//                {
//                    propertyCalled(this, PropertyCalledArgs(propertyName, propertiesCalled.Add(propertyName)));
//                }
//                else
//                {
//                    propertiesNotCalled.Add(propertyName);
//                }
//            }
//            else
//                PropertyChangedName = null;
//        }

//        private PropertyCalledEventArgs PropertyCalledArgs(string propertyName, bool isFirstCall)
//        {
//            var info = properties.Value[propertyName];
//            //if (fields.Value.TryGetValue(propertyName, out var fieldInfo) == false)
//            //{
//            //    if (fields.Value.TryGetValue("_" + propertyName, out fieldInfo) == false)
//            //    {
//            //        throw new InvalidOperationException(propertyName);
//            //    }
//            //}
//            var e = new PropertyCalledEventArgs(propertyName, info, isFirstCall);
//            return e;
//        }

//        #endregion INotifyPropertyCalled Members

//        #region INotifyPropertyReceived Members

//        /// <summary>
//        /// Raised when a property on this object has a new value.
//        /// </summary>
//        public event PropertyReceivedEventHandler PropertyReceived;


//        private readonly HashSet<string> propertiesReceived = new HashSet<string>();

//        /// <summary>
//        /// Raises this object's PropertyChanged event.
//        /// </summary>
//        /// <param name="propertyName">The property that has a new value.</param>
//        protected virtual void RaisePropertyReceived([CallerMemberName] string propertyName = null)
//        {
//            var handler = PropertyReceived;
//            if (handler != null)
//            {
//                var info = properties.Value[propertyName];
//                //if (fields.Value.TryGetValue(propertyName, out var fieldInfo) == false)
//                //{
//                //    //if (fields.Value.TryGetValue("_" + propertyName, out fieldInfo) == false)
//                //    //{
//                //    //    throw new InvalidOperationException(propertyName);
//                //    //}
//                //}
//                var e = new PropertyReceivedEventArgs(propertyName, info, this.GetValue(propertyName), propertiesReceived.Add(propertyName));
//                handler(this, e);
//            }

//        }

//        protected abstract object GetValue(string propertyName);

//        #endregion INotifyPropertyReceived Members



//    }
//}
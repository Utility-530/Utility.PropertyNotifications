using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CoffeeFlow.Demo
{
    /// <summary>
    /// <a href="https://www.codeproject.com/Tips/109979/Binding-to-a-singleton-the-elegant-way-using-a-cus"></a>
    /// Represents a DataSourceProvider that gets the data from a static property
    /// </summary>
    public class PropertyDataProvider : DataSourceProvider
    {
        private Type _objectType;
        private string _propertyName;

        /// <summary>
        /// Gets or sets the object type used to get data
        /// </summary>
        public Type ObjectType
        {
            get { return _objectType; }
            set
            {
                if (value == _objectType) return;
                _objectType = value;
                Notify();
            }
        }

        /// <summary>
        /// Gets or sets the name of a static property of ObjectType type
        /// </summary>
        public string PropertyName
        {
            get { return _propertyName; }
            set
            {
                if (value == _propertyName) return;
                _propertyName = value;
                Notify();
            }
        }

        private void Notify([CallerMemberName] string? memberName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(memberName));
            if (!base.IsRefreshDeferred) base.Refresh();
        }

        protected override void BeginQuery()
        {
            Exception error = null;
            object result = null;

            if (_objectType == null)
            {
                error = new InvalidOperationException("ObjectType is not set.");
            }
            else if (String.IsNullOrEmpty(_propertyName))
            {
                error = new InvalidOperationException("PropertyName is not set.");
            }
            else
            {
                PropertyInfo prop = _objectType.GetProperty(_propertyName, BindingFlags.Static | BindingFlags.Public);
                if (prop == null)
                {
                    error = new MissingMemberException(_objectType.FullName, _propertyName);
                }
                else
                {
                    try
                    {
                        result = prop.GetValue(null, null);
                    }
                    catch (MethodAccessException e)
                    {
                        error = e;
                    }
                    catch (TargetInvocationException e)
                    {
                        error = e;
                    }
                }
            }

            base.OnQueryFinished(result, error, null, null);
        }
    }
}

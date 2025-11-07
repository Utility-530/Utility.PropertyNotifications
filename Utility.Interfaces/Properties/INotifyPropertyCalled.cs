using System.Collections.Generic;
using System.ComponentModel;

namespace Utility.Interfaces
{
    public interface INotifyPropertyCalled
    {
        event PropertyCalledEventHandler PropertyCalled;

        IEnumerable<PropertyCalledEventArgs> MissedCalls { get; }
    }

    public delegate void PropertyCalledEventHandler(object sender, PropertyCalledEventArgs e);

    public class PropertyCalledEventArgs : PropertyChangedEventArgs
    {
        public PropertyCalledEventArgs(string? propertyName, object? value) : base(propertyName)
        {
            Value = value;
        }

        public object? Value { get; }
    }
}
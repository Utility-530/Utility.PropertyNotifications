using System.ComponentModel;
using System.Reflection;

namespace Utility.Interfaces
{
    public interface INotifyPropertyCalled
    {
        event PropertyCalledEventHandler PropertyCalled;
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
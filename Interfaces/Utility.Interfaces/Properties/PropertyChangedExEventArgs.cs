using System.ComponentModel;

namespace Utility.Interfaces
{
    public class PropertyChangedExEventArgs : PropertyChangedEventArgs
    {
        public PropertyChangedExEventArgs(string? propertyName, object? value, object? previousValue) : base(propertyName)
        {
            Value = value;
            PreviousValue = previousValue;
        }

        public object? Value { get; }
        public object? PreviousValue { get; }
    }

}
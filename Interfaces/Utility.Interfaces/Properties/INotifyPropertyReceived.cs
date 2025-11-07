using System.ComponentModel;

namespace Utility.Interfaces
{
    public interface INotifyPropertyReceived
    {
        event PropertyReceivedEventHandler PropertyReceived;
    }

    public delegate void PropertyReceivedEventHandler(object sender, PropertyReceivedEventArgs e);

    public class PropertyReceivedEventArgs : PropertyChangedEventArgs
    {
        public PropertyReceivedEventArgs(string? propertyName, object? value, object? oldValue, object? source = null) : base(propertyName)
        {
            Value = value;
            OldValue = oldValue;
            Source = source;
        }

        public object? Value { get; }
        public object? OldValue { get; }
        public object? Source { get; }
    }
}
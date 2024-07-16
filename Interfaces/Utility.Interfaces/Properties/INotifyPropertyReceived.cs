using System.ComponentModel;
using System.Reflection;

namespace Utility.Interfaces
{
    public interface INotifyPropertyReceived
    {
        event PropertyReceivedEventHandler PropertyReceived;
    }

    public delegate void PropertyReceivedEventHandler(object sender, PropertyReceivedEventArgs e);

    public class PropertyReceivedEventArgs : PropertyChangedEventArgs
    {
        public PropertyReceivedEventArgs(string propertyName, object value, object? source = null) : base(propertyName)
        {

            Value = value;
            Source = source;
        }



        public object Value { get; }
        public object? Source { get; }
    }


}
using System.Windows;

namespace Utility.WPF.Abstract
{

    public delegate void OutputChangedEventHandler(object sender, RoutedEventArgs args);
    public delegate void OutputChangedEventHandler<T>(object sender, T args) where T : RoutedEventArgs;

    public interface IOutput
    {
        event OutputChangedEventHandler OutputChange;

        object Output { get;  }
    }

    public interface IOutput<T> where T : RoutedEventArgs
    {
        event OutputChangedEventHandler<T> OutputChange;

        object Output { get; }
    }
}

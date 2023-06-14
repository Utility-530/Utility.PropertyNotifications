using System.Windows;

namespace Utility.WPF.Controls.FileSystem.Infrastructure
{
    public class TextChangedRoutedEventArgs : RoutedEventArgs
    {
        public TextChangedRoutedEventArgs(RoutedEvent routedEvent, string text) : base(routedEvent)
        {
            Text = text;
        }

        public string Text { get; set; }
    }

    public delegate void TextChangedRoutedEventHandler(object sender, TextChangedRoutedEventArgs e);
}
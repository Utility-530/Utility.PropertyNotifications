using System.Windows;

namespace Utility.WPF.Controls.Html
{
    internal class TextEdit
    {
        private static string? text;

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached(
          "Text",
          typeof(string),
          typeof(TextEdit),
          new FrameworkPropertyMetadata(null, Changed)
        );

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is string str && str != text && d is AvalonEditB.TextEditor editor)
            {
                editor.Text = str;
                //editor.Document.WithChangesTo(a => a.Text)
                //    .Subscribe(a => SetText(d as UIElement, text = a));
            }
        }

        public static string GetText(UIElement target) => (string)target.GetValue(TextProperty);

        public static void SetText(UIElement target, string value)
        {
            target.SetValue(TextProperty, value);
        }
    }
}
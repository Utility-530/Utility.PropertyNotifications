using System.Windows;
using AvalonEditB;
using Microsoft.Xaml.Behaviors;

namespace Utility.WPF.Controls.Html
{
    public class TextEditorTextBehavior : Behavior<TextEditor>
    {
        public string Html
        {
            get { return (string)GetValue(HtmlProperty); }
            set { SetValue(HtmlProperty, value); }
        }

        public static readonly DependencyProperty HtmlProperty =
            DependencyProperty.Register("Html", typeof(string), typeof(TextEditorTextBehavior), new PropertyMetadata(changed));

        protected override void OnAttached()
        {
            if (Html is { } html)
            {
                AssociatedObject.Text = html;
            }
            base.OnAttached();
        }

        private static void changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextEditorTextBehavior { AssociatedObject: { } associatedObject } && e.NewValue is string html)
            {
                associatedObject.Text = html;
            }
        }
    }
}
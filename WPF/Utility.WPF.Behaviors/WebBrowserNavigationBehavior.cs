using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Behaviors
{
    public class WebBrowserNavigationBehavior : Behavior<WebBrowser>
    {
        public WebBrowserNavigationBehavior()
        {

        }

        protected override void OnAttached()
        {
            if (Html is { } html)
            {
                AssociatedObject.NavigateToString(html);
            }
            base.OnAttached();
        }

        public string Html
        {
            get { return (string)GetValue(HtmlProperty); }
            set { SetValue(HtmlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Html.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HtmlProperty =
            DependencyProperty.Register("Html", typeof(string), typeof(WebBrowserNavigationBehavior), new PropertyMetadata(changed));

        private static void changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WebBrowserNavigationBehavior { AssociatedObject: { } obj } && e.NewValue is string newValue)
            {
                obj.NavigateToString(newValue);
            }
        }
    }
}

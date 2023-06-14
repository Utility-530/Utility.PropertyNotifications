using HandyControl.Controls;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Utility.WPF.Adorners;

namespace Utility.WPF.Demo.Adorners.Infrastructure
{

    public class CustomFrameworkElementAdorner : FrameworkElementAdorner
    {
        public CustomFrameworkElementAdorner(FrameworkElement adornedElement) : base(adornedElement)
        {
        }

        public override void SetAdornedElement(DependencyObject adorner, FrameworkElement? adornedElement)
        {
            if (adorner is Button button)
            {
                if (adornedElement == null)
                {
                    button.Click -= Button_Click;
                }
                else
                {
                    button.Click += Button_Click;
                }
            }

            void Button_Click(object sender, RoutedEventArgs e)
            {
                if (AdornedElement is Control control)
                {
                    control.Background = control.Background == Brushes.Red ? Brushes.PowderBlue : Brushes.Red;
                }

                if (AdornedElement is Panel panel)
                {
                    panel.Background = panel.Background == Brushes.Red ? Brushes.PowderBlue : Brushes.Red;
                }
            }
        }
    }

}

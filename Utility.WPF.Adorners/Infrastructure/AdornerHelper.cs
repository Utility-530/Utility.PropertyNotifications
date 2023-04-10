using System.Windows;
using System.Windows.Documents;

namespace Utility.WPF.Adorners.Infrastructure
{
    public static class AdornerHelper
    {
        public static void AddIfMissingAdorner(this DependencyObject adornedElement, DependencyObject adorner)
        {
            AdornerCollection? adorners = AdornerEx.GetAdorners(adornedElement);
            if (adorners.IndexOf(adorner) == -1)
            {
                adorners.Add(adorner);
            }
        }

        public static Adorner[]? Adorners(this UIElement adornedElement)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            return adornerLayer.GetAdorners(adornedElement);
        }
    }
}

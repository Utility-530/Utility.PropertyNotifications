using System.Windows;
using System.Windows.Documents;

namespace Utility.WPF.Adorners.Infrastructure
{
    public static class AdornerHelper
    {
        public static bool AddIfMissingAdorner(this DependencyObject adornedElement, DependencyObject adorner)
        {
            AdornerCollection? adorners = AdornerEx.GetAdorners(adornedElement);
            if (adorners.Count > 0)
            {
            }
            if (adorners.IndexOf(adorner) == -1)
            {
                adorners.Add(adorner);
                return true;
            }
            return false;
        }

        public static Adorner[]? Adorners(this UIElement adornedElement)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            return adornerLayer.GetAdorners(adornedElement);
        }
    }
}
using System.Collections;
using System.Windows;
using System.Windows.Documents;

namespace Utility.WPF.Helpers
{
    public static class AdornerHelper
    {
        public static void AddIfMissingAdorner(this AdornerLayer layer, Adorner adorner)
        {
            if (layer.GetAdorners(adorner.AdornedElement) is IList list)
            {
                if (list.IndexOf(adorner) != -1)
                    return;
            }
            layer.Add(adorner);
        }

        public static void RemoveAdorners(this UIElement adornedElement)
        {
            AdornerLayer? layer = AdornerLayer.GetAdornerLayer(adornedElement);
            Adorner[]? toRemoveArray = layer.GetAdorners(adornedElement);
            if (toRemoveArray != null)
            {
                foreach (Adorner a in toRemoveArray)
                {
                    layer.Remove(a);
                }
            }
        }
    }
}
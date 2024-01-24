using System.Collections;
using System.Linq;
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
            if (adornedElement is AdornerLayer adornerLayer)
                Clear(adornerLayer);
            Clear(AdornerLayer.GetAdornerLayer(adornedElement), adornedElement);
        }

        public static void Clear(this AdornerLayer adornedLayer, UIElement? adornedElement = null)
        {
            Adorner[]? toRemoveArray = adornedElement != null ? adornedLayer.GetAdorners(adornedElement) : adornedLayer.Children<Adorner>().ToArray();
            if (toRemoveArray != null)
            {
                foreach (Adorner a in toRemoveArray)
                {
                    adornedLayer.Remove(a);
                }
            }
        }
        public static void Clear<T>(this AdornerLayer adornedLayer, UIElement? adornedElement = null) where T : Adorner
        {
            Adorner[]? toRemoveArray = adornedElement != null ? adornedLayer.GetAdorners(adornedElement) : adornedLayer.Children<Adorner>().ToArray();
            if (toRemoveArray != null)
            {
                foreach (Adorner a in toRemoveArray)
                {
                    if (a is T t)
                        adornedLayer.Remove(a);
                }
            }
        }

    }
}
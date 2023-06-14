using System.Windows;
using Utility.WPF.Adorners.Infrastructure;
using Utility.WPF.Helpers;
using Utility.WPF.Controls.Adorners;

namespace Utility.WPF.Demo.Adorners.Infrastructure;

internal class AdornerController
{
    private readonly UIElement adornedElement;
    private readonly DependencyObject adorner;

    public AdornerController(UIElement adornedElement, DependencyObject? dependencyObject = null)
    {
        this.adornedElement = adornedElement;
        adorner = dependencyObject ?? new SettingsControl();
    }

    public void Apply()
    {
        adornedElement.AddIfMissingAdorner(adorner);
        adornedElement.SetValue(AdornerEx.IsEnabledProperty, true);
    }

    public void Hide()
    {
        adornedElement.SetValue(AdornerEx.IsEnabledProperty, false);
    }

    public void Remove()
    {
        adornedElement.RemoveAdorners();
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using RandomColorGenerator;
using Utility.WPF.Attached;
using Utility.WPF.Helpers;

namespace Utility.WPF.Demo.Adorners.Infrastructure
{
    internal class ControlColourer
    {
        private readonly DependencyObject dependencyObject;
        private readonly Dictionary<Guid, Brush> originalBrushes = new();
        private readonly Dictionary<Guid, Brush> tempBrushes = new();

        public ControlColourer(DependencyObject dependencyObject)
        {
            this.dependencyObject = dependencyObject;
        }

        public void Apply()
        {
            foreach (Control child in dependencyObject.FindChildren<Control>())
            {
                Brush background = child.Background;
                Guid guid = (Guid?)child.GetValue(Ex.KeyProperty) ?? Guid.NewGuid();
                child.SetValue(Ex.KeyProperty, guid);
                child.Background = tempBrushes.ContainsKey(guid) ?
                    tempBrushes[guid] :
                    RandomColor
                    .GetColor(ColorScheme.Random, Luminosity.Bright)
                    .ToMediaBrush();
                originalBrushes[guid] = background;
                tempBrushes[guid] = child.Background;
            }
        }

        public void Remove()
        {
            if (tempBrushes.Any())
            {
                foreach (Control child in dependencyObject.FindChildren<Control>())
                {
                    Guid? guid = (Guid?)child.GetValue(Ex.KeyProperty);
                    if (guid.HasValue && originalBrushes.ContainsKey(guid.Value))
                    {
                        child.Background = originalBrushes[guid.Value];
                    }
                    else
                    {
                        // child's been removed
                    }
                }
            }
        }
    }
}
using System.Collections;
using System.Linq;
using System.Windows;

namespace Utility.WPF.Attached
{
    /// <summary>
    /// <a href="https://stackoverflow.com/questions/31912603/how-do-i-reuse-visualstate-visualstategroup-and-vsualstatemanager-in-shared-res"></a>
    /// </summary>
    public class VisualStateExtensions : DependencyObject
    {
        public static void SetVisualStatefromTemplate(UIElement element, DataTemplate value)
        {
            element.SetValue(VisualStatefromTemplateProperty, value);
        }

        public static DataTemplate GetVisualStatefromTemplate(UIElement element)
        {
            return (DataTemplate)element.GetValue(VisualStatefromTemplateProperty);
        }

        public static readonly DependencyProperty VisualStatefromTemplateProperty = DependencyProperty.RegisterAttached("VisualStatefromTemplate", typeof(DataTemplate), typeof(VisualStateExtensions), new PropertyMetadata(null, VisualStatefromTemplateChanged));

        private static void VisualStatefromTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement frameworkElement)
            {
                var visualStateGroups = VisualStateManager.GetVisualStateGroups(frameworkElement);
                if (visualStateGroups != null)
                {
                    var template = (DataTemplate)e.NewValue;
                    var content = (FrameworkElement)template.LoadContent();
                    if (VisualStateManager.GetVisualStateGroups(content) is IList list)
                    {
                        var source = list.Cast<VisualStateGroup>().ToList();
                        var original = source.First();

                        source.RemoveAt(0);

                        visualStateGroups.Add(original);
                    }
                }
            }
        }
    }
}
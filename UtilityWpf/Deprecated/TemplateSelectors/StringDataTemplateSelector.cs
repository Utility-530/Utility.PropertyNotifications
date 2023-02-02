using System.Windows;

namespace UtilityWpf.DataTemplateSelectors
{
    //public class StringDataTemplateSelector : System.Windows.Controls.DataTemplateSelector
    //{
    //    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    //    {
    //        if (container is FrameworkElement && item is string)
    //        {
    //            var dataTemplate = TextTemplate;
    //            if (dataTemplate != null)
    //                return dataTemplate;
    //        }

    //        if (item != null)
    //        {
    //            var dataTemplateKey = new DataTemplateKey(item.GetType());
    //            var dataTemplate = ((FrameworkElement)container).TryFindResource(dataTemplateKey);
    //            if (dataTemplate != null)
    //                return (DataTemplate)dataTemplate;
    //        }

    //        return new DataTemplate(); //null does not work
    //    }

    //    public DataTemplate? TextTemplate { get; set; }

    //    public static StringDataTemplateSelector Instance => new();
    //}
}
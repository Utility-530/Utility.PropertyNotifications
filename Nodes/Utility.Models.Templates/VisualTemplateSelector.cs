namespace Utility.Models.Templates
{
    using System.Windows;
    using Utility.WPF.Templates;

    public class VisualTemplateSelector : CustomDataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            //bool isReadOnly = false;
            //if (item is IIsReadOnly { IsReadOnly: { } _isReadOnly })
            //    isReadOnly = _isReadOnly;

            //if(isReadOnly)
            //    return VisualReadOnlyTemplateSelector.VisualInstance.SelectTemplate(item, container);
            //else
                return VisualWriteTemplateSelector.VisualInstance.SelectTemplate(item, container);
        }


        public static VisualTemplateSelector VisualInstance { get; } = new();
    }
}

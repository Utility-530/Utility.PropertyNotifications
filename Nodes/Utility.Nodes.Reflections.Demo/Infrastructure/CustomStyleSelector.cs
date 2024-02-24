
namespace Utility.Nodes.Reflections.Demo.Infrastructure
{
    public class CustomStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is TreeViewItem { Header: IReadOnlyTree { Data: ICollectionItemDescriptor { } } })
            {
                var style = NewTemplates["CustomTreeViewItemStyle"] as Style;
                return style;
            }
            return TreeViewItemStyleSelector.Instance.SelectStyle(item, container);
        }

        public ResourceDictionary NewTemplates => new() 
        {
            Source = new Uri($"/{typeof(CustomStyleSelector).Assembly.GetName().Name};component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
        };

        public static CustomStyleSelector Instance { get; } = new();

    }
}

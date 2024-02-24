namespace Utility.Nodes.Demo
{
    public class CustomItemsPanelConverter : ItemsPanelConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is RootNode { })
            {
                return convert(new ItemsPanel
                {
                    Type = Arrangement.Stacked,
                    Orientation = Orientation.Horizontal,
                });
            }
            else if (value is IReadOnlyTree { Data: CollectionHeaderDescriptor { } _descriptor })
            {
                return convert(new ItemsPanel
                {
                    Type = Arrangement.Stacked,
                    Orientation = Orientation.Horizontal,
                });

            }
            else if (value is IReadOnlyTree { Data: IMemberDescriptor { Guid: { } guid }, Parent.Data: { } descriptor })
            {
                if (descriptor is CollectionItemDescriptor { Index: { } index })
                {
                    return convert(new ItemsPanel
                    {
                        Type = Arrangement.Stacked,
                        Orientation = Orientation.Horizontal,
                    });

                    if (ViewModelStore.Instance.Get(guid) is ViewModel viewModel)
                    {
                        var itemsPanel = viewModel.ToItemsPanel();
                        return convert(itemsPanel);
                    }
                    return convert(new ItemsPanel
                    {
                        Type = Arrangement.Stacked,
                        Orientation = Orientation.Vertical,
                    });
                }

                if (value is CustomMethodsNode { })
                {
                    return convert(new ItemsPanel
                    {
                        Type = Arrangement.Stacked,
                        Orientation = System.Windows.Controls.Orientation.Horizontal,
                    });
                }
            }

            return base.Convert(value, targetType, parameter, culture);

        }

        public static CustomItemsPanelConverter Instance { get; } = new();
    }
}

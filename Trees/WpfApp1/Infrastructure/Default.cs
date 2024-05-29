using System.Globalization;
using System.Windows.Controls;
using System.Windows;
using Utility.Commands;
using Utility.Interfaces.NonGeneric;
using Utility.Trees.WPF;
using Utility.WPF.Factorys;
using System.Windows.Shapes;
using System.Windows.Media;
using Utility.Trees.Abstractions;
using Utility.Interfaces.Generic;
using Utility.Trees.WPF.Abstractions;
using Utility.Reactives;
using Utility.WPF.Controls.Trees;

namespace Utility.Trees.Demo.FilterBuilder.Infrastructure
{
    public class Default
    {
        public class TreeViewItemFactory : ITreeViewItemFactory
        {
            Random random = new();
            public HeaderedItemsControl Make(object instance)
            {
                TreeViewItem item = null;
                if (instance is TypeNode {  })
                {
                    item = new ComboTreeViewItem
                    {
                        IsExpanded = true,
                        BorderThickness = new Thickness(2),
                        AddCommand = new Command(() => { if (instance is IExpand expand) expand.IsExpanded = true; }),
                        RemoveCommand = new Command(() => { if (instance is IExpand { } expand) expand.IsExpanded = false; }),
                        Header = instance,
                        DataContext = instance
                    };
                }
                else
                item = new CustomTreeViewItem
                {
                    IsExpanded = true,
                    BorderThickness = new Thickness(2),
                    AddCommand = new Command(() => { if (instance is IExpand expand) expand.IsExpanded = true; }),
                    RemoveCommand = new Command(() => { if (instance is IExpand {} expand) expand.IsExpanded = false; }),
                    Header = instance,
                    DataContext = instance
                };
                return item;

                //static int[] index(ModelTree tree)
                //{
                //    return (tree.Key as IEnumerable<int>).Append(tree.Items.Count()).ToArray();
                //}
            }

            public static TreeViewItemFactory Instance { get; } = new();
        }

        public class ItemsPanelConverter : System.Windows.Data.IValueConverter
        {
            public static ItemsPanelConverter Instance { get; } = new();

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return ItemsPanelFactory.Template(default, default, Orientation.Vertical, Enums.Arrangement.Stacked);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        public class DataTemplateSelector : System.Windows.Controls.DataTemplateSelector
        {
            public static DataTemplateSelector Instance { get; } = new();

            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                //if (item?.GetType() is Type type && new DataTemplateKey(type) is var key &&  App.Current?.TryFindResource(key) is DataTemplate dataTemplate)
                //    return dataTemplate;        

                if (Application.Current?.TryFindResource("Tree") is DataTemplate dataTemplate)
                    return dataTemplate;

                return TemplateGenerator.CreateDataTemplate(() => new Ellipse { Fill = Brushes.Red, Height = 20, Width = 20 });
            }
        }

        public class Filter : ITreeViewFilter
        {
            public static Filter Instance { get; } = new();

            public bool Convert(object item)
            {
                return true;
            }
        }

        public class EventListener : ReplayModel<IEvent>, IEventListener
        {
            public static EventListener Instance { get; } = new();

            public void Send(IEvent @event)
            {
                OnNext(@event);
            }
        }
    }
}

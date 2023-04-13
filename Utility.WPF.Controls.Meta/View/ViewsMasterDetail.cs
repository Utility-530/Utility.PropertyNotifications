using Evan.Wpf;
using NetFabric.Hyperlinq;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Common;
using Utility.WPF.Controls.Base;
using Utility.WPF.Meta;
using Utility.WPF.Controls.Master;
using Utility.WPF.Model;

namespace Utility.WPF.Controls.Meta
{
    internal class ViewTypeItem : ListBoxItem
    {
    }

    internal class ViewTypeItemListBox : ListBox<ViewTypeItem>
    {
    }

    public class ViewsMasterDetail : MasterDetail
    {
        public static readonly DependencyProperty AssemblyProperty = DependencyHelper.Register(new PropertyMetadata(Assembly.GetEntryAssembly()));
        //public static readonly DependencyProperty DemoTypeProperty = DependencyHelper.Register();

        public ViewsMasterDetail()
        {
            Orientation = Orientation.Horizontal;
            var listBox = new ViewTypeItemListBox();

            //listBox.GroupStyle.Add(new GroupStyle());
            //var resource = new ResourceDictionary
            //{
            //    Source = new Uri("/Utility.WPF.Controls.Meta;component/Themes/Generic.xaml",
            //         UriKind.RelativeOrAbsolute)
            //};
            //var dataTemplateKey = new DataTemplateKey(typeof(KeyValue));
            //listBox.ItemTemplate = (DataTemplate)resource[dataTemplateKey];

            Content = listBox;
            UseDataContext = true;
            _ = this.WhenAnyValue(a => a.Assembly)
                .WhereNotNull()
                .Select(a=> FrameworkElementKeyValue.Types(a))
                //.CombineLatest(this.WhenAnyValue(a => a.DemoType).Where(a => a != AssemblyType.Default))
                //.Select(a =>
                //{
                //    return a.Second switch
                //    {
                //        AssemblyType.UserControl => FrameworkElementKeyValue.ViewTypes(a.First),
                //        AssemblyType.ResourceDictionary => (IEnumerable<KeyValue>)ResourceDictionaryKeyValue.ResourceViewTypes(a.First),
                //        _ => throw new Exception("FDGS££Fff"),
                //    };
                //})
                .Subscribe(pairs =>
                {
                    listBox.ItemsSource = pairs.ToArray();
                    listBox.SelectedIndex = 0;
                });

            Header = CreateDetail();

            static Grid CreateDetail()
            {
                var grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.0, GridUnitType.Auto) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.0, GridUnitType.Star) });
                grid.Children.Add(AddTextBlock(grid));
                grid.Children.Add(AddContentControl(grid));
                return grid;

                static TextBlock AddTextBlock(Grid grid)
                {
                    var textBlock = new TextBlock
                    {
                        Margin = new Thickness(20),
                        FontSize = 20,
                        Text = "n o   k e y"
                    };
       
                    Binding binding = new()
                    {
                        Path = new PropertyPath(nameof(KeyValue.Key)),
                    };
                    textBlock.SetBinding(TextBlock.TextProperty, binding);
                    return textBlock;
                }

                static ContentControl AddContentControl(Grid grid)
                {
                    var contentControl = new ContentControl
                    {
                        Content = new TextBlock { Text = "n o   c o n t e n t", FontSize = 30, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center },
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalAlignment = HorizontalAlignment.Stretch
                    };
                    Grid.SetRow(contentControl, 1);
                    var binding = new Binding
                    {
                        Path = new PropertyPath(nameof(KeyValue.Value)),
                    };
                    contentControl.SetBinding(ContentProperty, binding);
                    return contentControl;
                }
            }
        }

        public Assembly Assembly
        {
            get => (Assembly)GetValue(AssemblyProperty);
            set => SetValue(AssemblyProperty, value);
        }
    }
}
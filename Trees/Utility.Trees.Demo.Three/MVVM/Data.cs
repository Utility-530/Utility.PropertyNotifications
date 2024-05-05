using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Descriptors;
using Utility.Enums;
using Utility.Interfaces;
using Utility.Interfaces.NonGeneric;
using Utility.Trees.Abstractions;
using Utility.WPF.Factorys;
using Utility.WPF.Templates;
using Views.Trees;
using O = System.Windows.Controls.Orientation;

namespace Utility.Trees.Demo.MVVM.MVVM
{
    public class Data
    {
        public class DataTemplateSelector : System.Windows.Controls.DataTemplateSelector
        {
            public static DataTemplateSelector Instance { get; } = new();




            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {

                if (item is IReadOnlyTree { Data: IPropertiesDescriptor { } })
                {
                    return MakeEmptyTemplate();
                }
                //if (item is CustomMethodsNode { Data: { } })
                //{
                //    return MakeTemplate(item, "None");
                //}
                if (item is IReadOnlyTree { Data: IHeaderDescriptor { } })
                {
                    return MakeHeaderTemplate(item, 1);
                }
                if (item is IReadOnlyTree { Data: ICollectionDescriptor { } })
                {
                    return MakeEmptyTemplate();
                }
                // method
                if (item is IReadOnlyTree { Data: IMethodDescriptor { } })
                {
                    return MakeButtonTemplate(item);
                }
                if (item is IReadOnlyTree { Parent.Data: ICollectionItemDescriptor { } _ })
                {
                    return MakeEmptyTemplate();
                }
                if (item is IReadOnlyTree { Parent.Parent.Data: ICollectionItemDescriptor { Index: { } _index } _ })
                {
                    return MakeTemplate(item);
                }
                // root
                //if (item is RootNode { })
                //{
                //    return MakeTemplate(item, "None");
                //}
                // root
                if (item is IReadOnlyTree { Data: IPropertyDescriptor { Descriptor: RootDescriptor { } } })
                {
                    //return MakeHeaderTemplate(item, depth);
                    return MakeEmptyTemplate();
                }
                // default
                if (item is ITree { Depth: { } depth, Data: IPropertyDescriptor { Descriptor: { } } })
                {
                    return MakeHeaderTemplate(item, depth);
                }
                // collection item
                if (item is ITree { Data: ICollectionItemDescriptor { } })
                {
                    return MakeEmptyTemplate();
                }
                // inner collection item descriptor

                // parameter
                //if (item is ParameterNode { Data: PropertyData { Descriptor: { } } })
                //{
                //    return MakeTemplate(item);
                //}
                // methods
                if (item is IReadOnlyTree { Data: IMethodsDescriptor { } })
                {
                    return MakeEmptyTemplate();
                }
                if (item is IReadOnlyTree { Data: IPropertiesDescriptor { } })
                {
                    //return MakeHeaderTemplate(item, depth);
                }
                // method
                if (item is IReadOnlyTree { Data: IMethodDescriptor { } })
                {
                    return MakeEmptyTemplate();
                }

                return MakeTemplate(item);

            }

            private DataTemplate MakeTemplate(object item)
            {
                return TemplateGenerator.CreateDataTemplate(() =>
                {
                    var binding = new Binding { Mode = BindingMode.OneWay, Path = new PropertyPath(nameof(IReadOnlyTree.Data)), Source = item };
                    var contentControl = new ContentPresenter
                    {
                        ContentTemplateSelector = CustomDataTemplateSelector.Instance
                    };
                    //if (name is string _name)
                    //    contentControl.ContentTemplate = ResourceHelper.FindResource<DataTemplate>(_name);
                    contentControl.SetBinding(ContentPresenter.ContentProperty, binding);

                    return contentControl;
                });
            }

            private DataTemplate MakeEmptyTemplate()
            {
                return TemplateGenerator.CreateDataTemplate(() =>
                {

                    var contentControl = new ContentPresenter { };
                    return contentControl;
                });
            }


            //<DataTemplate x:Key="Header"  >

            //        <TextBlock Width="120" Text="{Binding Name}" Style="{StaticResource TextBlockStyle}"></TextBlock>
            //        <!--<ContentControl Style="{StaticResource ContentControlStyle}" Content="{Binding }" ContentTemplateSelector="{x:Static templates:CustomDataTemplateSelector.Instance }"/>-->

            //</DataTemplate>


            private DataTemplate MakeHeaderTemplate(object item, int count)
            {
                return TemplateGenerator.CreateDataTemplate(() =>
                {

                    var textBlock = new TextBlock
                    {
                        FontWeight = FontWeight.FromOpenTypeWeight(600 - count * 10),
                        FontSize = 18 - count * 0.5
                    };
                    textBlock.SetBinding(TextBlock.TextProperty, Binding(item));

                    return textBlock;
                });

                static Binding Binding(object item)
                {
                    return new()
                    {
                        Converter = Utility.WPF.Converters.LambdaConverter.HumanizerConverter,
                        Path = new PropertyPath($"{nameof(IReadOnlyTree.Data)}.{nameof(IDescriptor.Name)}"),
                        Source = item
                    };
                }
            }

            private DataTemplate MakeButtonTemplate(object item)
            {
                return TemplateGenerator.CreateDataTemplate(() =>
                {

                    var button = new Button
                    {

                    };
                    button.SetBinding(Button.ContentProperty, Binding(item));
                    button.SetBinding(Button.CommandProperty, Binding2(item));

                    return button;
                });

                static Binding Binding(object item)
                {
                    return new()
                    {
                        Path = new PropertyPath($"{nameof(IReadOnlyTree.Data)}.{nameof(IMethodDescriptor.Name)}"),
                        Converter = Utility.WPF.Converters.LambdaConverter.HumanizerConverter,
                        Source = item
                    };
                }
                static Binding Binding2(object item)
                {
                    return new()
                    {

                        Path = new PropertyPath($"{nameof(IReadOnlyTree.Data)}.{nameof(IMethodDescriptor.Command)}"),
                        Source = item
                    };
                }
            }


            //var style = App.Current.Resources["CustomTree"] as DataTemplate;
            //    return style;
            //}
        }

        public class StyleSelector : System.Windows.Controls.StyleSelector
        {
            public override Style SelectStyle(object item, DependencyObject container)
            {
                if (item is TreeViewItem { })
                {
                    var style = App.Current.Resources["GenericTreeViewItem"] as Style;
                    return style;
                }
                return base.SelectStyle(item, container);
            }
            public static StyleSelector Instance { get; } = new();
        }

        public class TreeViewItemFactory : ITreeViewItemFactory
        {
            public TreeViewItem Make(object instance)
            {
                //if (instance is not IReadOnlyTree { } tree)
                //{
                //    return null; 
                //}
                //var item = new TreeViewItem
                //{
                //    IsExpanded = true,
                //    BorderThickness = new Thickness(2),
                //    Header = tree.Data,
                //    DataContext = tree.Data,
                //};

                var item = new TreeViewItem
                {
                    IsExpanded = true,
                    BorderThickness = new Thickness(2),
                    Header = instance,
                    DataContext = instance,
                };
                return item;
            }

            public static TreeViewItemFactory Instance { get; } = new();
        }


        public class ItemsPanelConverter : System.Windows.Data.IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is not IReadOnlyTree tree)
                {
                    return ItemsPanelFactory.Template(default, default, O.Vertical, Arrangement.Stacked);
                }

                //{
                //    if (tree.Data is ICollectionHeadersDescriptor { } _descriptor)
                //        return ItemsPanelFactory.Template(default, default, O.Horizontal, Arrangement.Stacked);

                //}
                {
                    if (tree.Data is ICollectionHeadersDescriptor { } _descriptor)
                        return ItemsPanelFactory.Template(default, default, O.Horizontal, Arrangement.Stacked);
                }
                {
                    if (tree.Parent?.Data is ICollectionItemDescriptor { } _descriptor)
                        return ItemsPanelFactory.Template(default, default, O.Horizontal, Arrangement.Stacked);
                }

                return ItemsPanelFactory.Template(default, default, O.Vertical, Arrangement.Stacked);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }

            public static ItemsPanelConverter Instance { get; } = new();

        }


    }

    public class Value : IValue, IType
    {
        public Type Type => typeof(object);

        object IValue.Value => null;
    }
}

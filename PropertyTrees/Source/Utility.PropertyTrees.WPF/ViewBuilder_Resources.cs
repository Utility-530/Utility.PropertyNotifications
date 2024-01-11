using System.Windows.Controls;
using System.Windows;
using Utility.WPF.Panels;
using System.Collections.Generic;
using Utility.Nodes;
using Utility.Enums;
using System.Windows.Media;
using Orientation = System.Windows.Controls.Orientation;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using Utility.WPF.Factorys;

namespace Utility.PropertyTrees.WPF;

public partial class ViewBuilder
{
    public class HorizontalPanel : UniformGrid
    {
        public HorizontalPanel()
        {
            Rows = 1;
        }
    }   

    readonly Dictionary<PanelKey, ItemsPanelTemplate> panelsDictionary = new() {
        { new(0, Orientation.Vertical, Arrangement.Stacked), defaultTemplate } ,
        { new(1, Orientation.Horizontal, Arrangement.Stacked), defaultTemplate }
    };

    static readonly DataTemplate emptyTemplate = TemplateGenerator.CreateDataTemplate(() =>
    {
        return new Control();
    });


    static readonly ItemsPanelTemplate defaultTemplate = TemplateGenerator.CreateItemsPanelTemplate<StackPanel>(factory =>
    factory.SetValue(Control.BackgroundProperty, new SolidColorBrush(Colors.LightGray) { Opacity = 0.1 }));
    static readonly ItemsPanelTemplate uniformStackTemplate = TemplateGenerator.CreateItemsPanelTemplate<UniformStackPanel>(factory =>
    factory.SetValue(Panel.IsItemsHostProperty, true));
    static readonly ItemsPanelTemplate horizontalTemplate = TemplateGenerator.CreateItemsPanelTemplate<HorizontalPanel>(factory =>
    {
        factory.SetValue(Control.BackgroundProperty, new SolidColorBrush(Colors.LightGray) { Opacity = 0.1 });
        factory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
    });

    private class CustomGrid : Grid
    {
        readonly ColumnDefinition
            column1 = new() { Width = new GridLength(160) },
            column2 = new()
            {
                Width = new GridLength(2, GridUnitType.Star)
                //column3 = new() { Width = new GridLength(1, GridUnitType.Star)
            };

        public CustomGrid()
        {
            ColumnDefinitions.Add(column1);
            ColumnDefinitions.Add(column2);
        }
    }

    public DataTemplate HeaderedContentTemplate
    {
        get
        {
            return headeredContentTemplate ??= TemplateGenerator.CreateDataTemplate(Create);

            HeaderedContentControl Create()
            {
                var headeredContentControl = new HeaderedContentControl
                {
                    Style = HorizontalStyle,
                    ContentTemplateSelector = dataTemplateSelector,
                };

                Binding binding = new(nameof(ValueNode.Name))
                {
                    Mode = BindingMode.OneWay,
                    Converter = Utility.WPF.Converters.LambdaConverter.HumanizerConverter
                };
                headeredContentControl.SetBinding(HeaderedContentControl.HeaderProperty, binding);

                Binding binding2 = new() { Mode = BindingMode.OneWay, };
                headeredContentControl.SetBinding(HeaderedContentControl.ContentProperty, binding2);

                return headeredContentControl;
            }
        }
    }
    private static Style HorizontalStyle
    {
        get
        {
            horizontalStyle = new Style(typeof(HeaderedContentControl));
            horizontalStyle.Setters.Add(new Setter(Control.TemplateProperty,
                new ControlTemplate(typeof(HeaderedContentControl))
                {
                    VisualTree = BuildHeaderedContentControlFactory()
                }));
            return horizontalStyle;
        }
    }

    static FrameworkElementFactory BuildHeaderedContentControlFactory()
    {
        FrameworkElementFactory gridFactory = new(typeof(CustomGrid))
        {
            Name = "PART_StackPanel"
        };
        FrameworkElementFactory headerPresenterFactory = new(typeof(ContentPresenter))
        {
            Name = "PART_HeaderPresenter"
        };
        headerPresenterFactory.SetValue(Grid.ColumnProperty, 0);
        headerPresenterFactory.SetValue(ContentPresenter.ContentSourceProperty, "Header");
        FrameworkElementFactory contentPresenterFactory = new(typeof(ContentPresenter))
        {
            Name = "PART_ContentPresenter"
        };
        contentPresenterFactory.SetValue(Grid.ColumnProperty, 1);
        gridFactory.AppendChild(headerPresenterFactory);
        gridFactory.AppendChild(contentPresenterFactory);
        return gridFactory;
    }


    // Only shows the item presenter
   

  

}

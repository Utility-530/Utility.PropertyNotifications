using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Utility.Factorys;
using Utility.Helpers;

namespace Utility.WPF.Factorys
{
    public static class FrameworkElementConverter
    {
        public static FrameworkElement GetFrameworkElement(object value)
        {
            switch (value)
            {
                case DataTemplate dataTemplate:
                    {
                        object? content = null;
                        if (dataTemplate.DataType is Type datatype)
                        {
                            try
                            {
                                throw new Exception("ds ss");
                                //content = new Filler(datatype).Create();
                            }
                            catch (Exception ex)
                            {
                                content = new ContentPresenter { ContentTemplate = dataTemplate, Content = new AutoMoqer(datatype).Build().Service };
                            }
                        }
                        else
                        {
                            content = new ContentPresenter { ContentTemplate = dataTemplate };
                        }
                        return new ContentControl
                        {
                            ContentTemplate = dataTemplate
                        };
                    }
                case FrameworkElement frameworkElement:
                    return frameworkElement;
                case Brush solidColorBrush:
                    {
                        Viewbox viewBox = new();
                        var rect = new Rectangle { Fill = solidColorBrush, Height = 1, Width = 1 };
                        viewBox.Child = rect;
                        return viewBox;
                    }
                case Geometry geometry:
                    return new Path
                    {
                        Stretch = Stretch.Fill,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1,
                        Data = geometry
                    };
                case Style { TargetType: var type } style:
                    {
                        var instance = Activator.CreateInstance(type) as Control;
                        instance.Style = style;
                        return instance;
                    }
                case IValueConverter converter:
                    {
                        var mb = converter.GetType().GetMethods().First();
                        return new TextBlock { Text = mb.AsString() };
                    }
                default:
                    throw new Exception($"Unexpected type {value.GetType().Name} in {nameof(FrameworkElementConverter)}");
            }
        }
    }

}
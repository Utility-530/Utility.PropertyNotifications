using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
                                if (datatype.IsValueType)
                                    content = new ContentPresenter { ContentTemplate = dataTemplate, Content = Activator.CreateInstance(datatype) };
                                else if (datatype.Equals(typeof(string)))
                                    content = new ContentPresenter { ContentTemplate = dataTemplate, Content = string.Empty };
                                else if (datatype.IsAbstract)
                                    content = new ContentPresenter { ContentTemplate = dataTemplate };
                                else
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
                        if (type.GetConstructor(Type.EmptyTypes) is not null)
                        {
                            var instance = Activator.CreateInstance(type) as FrameworkElement;
                            instance.Style = style;
                            return instance;
                        }
                        return null;
                    }
                case IValueConverter converter:
                    {
                        var mb = converter.GetType().GetMethods().First();
                        return new TextBlock { Text = mb.AsString() };
                    }

                case DataTemplateSelector selector:
                    {
                        var mb = selector.GetType().Name;
                        return new TextBlock { Text = mb };
                    }
                case ControlTemplate ct:
                    {
                        var control = Activator.CreateInstance(ct.TargetType) as Control;
                        control.Template = ct;
                        return control;
                    }
                case Storyboard { Name: var name } storyboard:
                    {
                        //storyboard.DependencyObjectType;
                        return new TextBlock { Text = name };
                    }

                case IMultiValueConverter converter:
                    {
                        //storyboard.DependencyObjectType;
                        return new TextBlock { Text = converter.GetType().Name };
                    }


                case double d:
                    return new TextBlock { Text = d.ToString() };
                case ItemsPanelTemplate itemsPanelTemplate:
                    return new ListBox { ItemsSource = Array.CreateInstance(typeof(string), 5), ItemsPanel = itemsPanelTemplate };
                default:
                    throw new Exception($"Unexpected type {value.GetType().Name} in {nameof(FrameworkElementConverter)}");
            }
        }
    }

}
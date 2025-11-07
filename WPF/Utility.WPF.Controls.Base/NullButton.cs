using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Utility.WPF.Controls.Base
{
    public class NullButton : ToggleButton
    {
        private ResourceDictionary? resourceDictionary;

        static NullButton()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(NullButton), new FrameworkPropertyMetadata(typeof(NullButton)));
        }

        public override void OnApplyTemplate()
        {
            OnPropertyChanged(new DependencyPropertyChangedEventArgs(IsCheckedProperty, null, IsChecked));
            base.OnApplyTemplate();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == IsCheckedProperty)
            {
                if (e.NewValue is null)
                    return;
                if ((bool)e.NewValue == true)
                {
                    this.Content = new Path()
                    {
                        Stroke = Brushes.Gray,
                        StrokeThickness = 2,
                        Stretch = Stretch.UniformToFill,
                        Data = Utility.WPF.Helpers.ResourceHelper.FindResource<Geometry>("NotNull") ?? (Geometry)resource()["NotNull"]
                    };
                }
                else if ((bool)e.NewValue == false)
                {
                    this.Content = new Path()
                    {
                        Stroke = Brushes.Gray,
                        StrokeThickness = 2,
                        Stretch = Stretch.UniformToFill,
                        Data = Utility.WPF.Helpers.ResourceHelper.FindResource<Geometry>("Null") ?? (Geometry)resource()["Null"]
                    };
                }
            }
            base.OnPropertyChanged(e);
        }

        private ResourceDictionary resource()
        {
            if (resourceDictionary != null)
                return resourceDictionary;
            resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/Utility.WPF.Geometries;component/Resources.xaml", UriKind.Relative);
            return resourceDictionary;
        }
    }
}
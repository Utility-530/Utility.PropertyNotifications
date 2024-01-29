using Jellyfish;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using Utility.WPF.Adorners;
using Utility.WPF.Adorners.Infrastructure;
using Utility.WPF.Helpers;

namespace Utility.WPF.Demo.SandBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {


            if (AdornerLayer.GetAdornerLayer(Button3) is { } _adornerLayer)
            {

                var adorner2 = new NotificationAdorner(Button3);
                _adornerLayer.Add(adorner2);

            }


        }

        private void Click4(object sender, RoutedEventArgs e)
        {
            //if (AdornerLayer.GetAdornerLayer(Button4) is { } adornerLayer)
            //{
            //    var adorner = new IconAdorner(Button4);
            //    adornerLayer.Add(adorner);

            //}

            var adorners = Button4.GetAdorners();
            if (adorners.Count == 1)
            {
                return;
            }
            var packIcon = new PackIcon
            {
                Width = 24,
                Height = 24,
                Kind = PackIconKind.Gear,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Foreground = Brushes.DarkGray,
            };

            var path = CreatePath2();
    

            Grid grid = new Grid() { Height=12, Width=12  };
          
            grid.Children.Add(path);
            Button4.Content = grid;
            //AdornerEx.SetOffsetX(packIcon, 100);
            AdornerEx.SetOffsetX(grid, 150);
            AdornerEx.SetOffsetY(grid, 150);
            //packIcon.Effect = new DropShadowEffect { BlurRadius = 10, ShadowDepth = 0.1, Color = Colors.LightGray };
            //packIcon.MouseDown += PackIcon_MouseDown;
            //adorners.Add(packIcon);
            //adorners.Add(grid);
            path.MouseDown += Path_MouseDown;
        }

        private Path CreatePath2()
        {
            double CapDiameter = 4;
            Point ellipseCenter1 = new Point(0, 0);
            Point ellipseCenter2 = new Point(30, 0);
            Point ellipseCenter3 = new Point(60, 0);
            var ellipse1 = new EllipseGeometry(ellipseCenter1, CapDiameter, CapDiameter);
            var ellipse2 = new EllipseGeometry(ellipseCenter2, CapDiameter, CapDiameter);
            var ellipse3 = new EllipseGeometry(ellipseCenter3, CapDiameter, CapDiameter);
            GeometryGroup combined = new GeometryGroup();
            combined.Children.Add(ellipse1);
            combined.Children.Add(ellipse2);
            combined.Children.Add(ellipse3);

            //string sData = "M19.375 36.7818V100.625C19.375 102.834 21.1659 104.625 23.375 104.625H87.2181C90.7818 104.625 92.5664 100.316 90.0466 97.7966L26.2034 33.9534C23.6836 31.4336 19.375 33.2182 19.375 36.7818Z";
            Path path = new Path() { Stroke = new SolidColorBrush(Colors.Black), Fill = new SolidColorBrush(Colors.Black), StrokeThickness = 2, Stretch = Stretch.Uniform };
            //string sData = "M 250,40 L200,20 L200,60 Z";
            //var converter = TypeDescriptor.GetConverter(typeof(Geometry));
            //path.Data = (Geometry)converter.ConvertFrom(sData);
            path.Data = combined;
            return path;
        }
        private Path CreatePath()
        {
            string sData = "M19.375 36.7818V100.625C19.375 102.834 21.1659 104.625 23.375 104.625H87.2181C90.7818 104.625 92.5664 100.316 90.0466 97.7966L26.2034 33.9534C23.6836 31.4336 19.375 33.2182 19.375 36.7818Z";
            Path path = new Path() { Stroke = new SolidColorBrush(Colors.Black), Fill = new SolidColorBrush(Colors.Black), StrokeThickness = 2, Stretch = Stretch.Fill };
            //string sData = "M 250,40 L200,20 L200,60 Z";
            var converter = TypeDescriptor.GetConverter(typeof(Geometry));
            path.Data = (Geometry)converter.ConvertFrom(sData);
            return path;
        }

        private void Path_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Button4.ContextMenu.IsOpen = true;
        }

        private void PackIcon_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Button4.ContextMenu.IsOpen = true;
        }

        private void Click3(object sender, RoutedEventArgs e)
        {
            if (AdornerLayer.GetAdornerLayer(Button3) is { } _adornerLayer)
            {

                var adorner2 = new NotificationAdorner(Button3);
                _adornerLayer.Add(adorner2);

            }
        }
    }


    //private void Button_Click(object sender, RoutedEventArgs e)
    //{
    //    //VisualStateManager.GoToElementState(this.Button3, "Pressed", false);

    //    if (AdornerLayer.GetAdornerLayer(Button3) is { } adornerLayer)
    //    {
    //        adornerLayer.Clear(TextBox1);
    //        RelayCommand relayCommand = new RelayCommand((e) => {
    //            Button2_Click(default, default);

    //        });
    //        var adorner = new CancelAdorner(TextBox1, relayCommand) { Opacity = 0 };


    //        adornerLayer.Add(adorner);

    //        DoubleAnimation doubleAnimation = new (1, new Duration(TimeSpan.FromSeconds(1)));
    //        doubleAnimation.Completed += fadeOutAnimation_Completed;
    //        doubleAnimation.Freeze();
    //        adorner.BeginAnimation(UIElement.OpacityProperty, doubleAnimation);

    //    }

    //}

    //private void fadeOutAnimation_Completed(object sender, EventArgs e)
    //{
    //    ;
    //}

    //private void Button2_Click(object sender, RoutedEventArgs e)
    //{
    //    if (AdornerLayer.GetAdornerLayer(Button3) is { } adornerLayer)
    //    {
    //        adornerLayer.Clear(TextBox1);

    //        RelayCommand relayCommand = new RelayCommand((e) => {
    //            Button_Click(default, default);
    //        });
    //        var adorner = new MaskAdorner(TextBox1, relayCommand) { Opacity=0};
    //        adornerLayer.Add(adorner);

    //        DoubleAnimation doubleAnimation = new DoubleAnimation(1, new Duration(TimeSpan.FromSeconds(1)));
    //        doubleAnimation.Completed += fadeOutAnimation_Completed;
    //        doubleAnimation.Freeze();
    //        adorner.BeginAnimation(UIElement.OpacityProperty, doubleAnimation);

    //    }
    //}


    public class ViewModel
    {
        public IEnumerable Collection { get; } = new[] { new MeasurementViewModel { Header = "Height" }, new MeasurementViewModel { Header = "Width" } };
    }

    public class MeasurementViewModel
    {
        public string Header { get; init; }
        public double Value { get; init; }
    }
}
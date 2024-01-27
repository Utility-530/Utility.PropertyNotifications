using Jellyfish;
using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using Utility.WPF.Adorners;
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
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    //VisualStateManager.GoToElementState(this.Button3, "Pressed", false);

        //    if (AdornerLayer.GetAdornerLayer(Button3) is { } adornerLayer)
        //    {
        //        adornerLayer.Clear(TextBox1);
        //        AdornerLayer? adLayer = AdornerLayer.GetAdornerLayer(Button3);
        //        RelayCommand relayCommand = new RelayCommand((e) => {
        //            Button2_Click(default, default);

        //        });
        //        var adorner = new CancelAdorner(TextBox1, relayCommand) { Opacity = 0 };
    

        //        adLayer.Add(adorner);

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
        //        AdornerLayer? adLayer = AdornerLayer.GetAdornerLayer(Button3);
        //        RelayCommand relayCommand = new RelayCommand((e) => {
        //            Button_Click(default, default);
        //        });
        //        var adorner = new MaskAdorner(TextBox1, relayCommand) { Opacity=0};
        //        adLayer.Add(adorner);

        //        DoubleAnimation doubleAnimation = new DoubleAnimation(1, new Duration(TimeSpan.FromSeconds(1)));
        //        doubleAnimation.Completed += fadeOutAnimation_Completed;
        //        doubleAnimation.Freeze();
        //        adorner.BeginAnimation(UIElement.OpacityProperty, doubleAnimation);

        //    }
        //}
    }

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
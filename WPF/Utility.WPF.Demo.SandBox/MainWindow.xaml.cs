using System.Collections;
using System.Windows;

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
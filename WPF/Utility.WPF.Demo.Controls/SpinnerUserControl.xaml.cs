using System;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Demo.View
{
    /// <summary>
    /// Interaction logic for SpinnerUserControl.xaml
    /// </summary>
    public partial class SpinnerUserControl : UserControl
    {
        public SpinnerUserControl()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            shortTimeSpanControl1.Value = TimeSpan.Zero;
        }
    }

    public class SpinnerViewModel
    {
        private int value = 10;

        public int Value { get => value; set => this.value = value; }
    }
}
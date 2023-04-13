using System;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Demo.Panels
{
    /// <summary>
    /// Interaction logic for TransitionPanelView.xaml
    /// </summary>
    public partial class TransitionPanelView : UserControl
    {
        public TransitionPanelView()
        {
            InitializeComponent();
            TransitionPanel.CurrentValue = false;
        }

        private void Click(object sender, RoutedEventArgs e)
        {
            TransitionPanel.CurrentValue = !(bool)TransitionPanel.CurrentValue;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ListBox1.SelectedIndex = MainTransitionPanel.Children.IndexOf(sender as UIElement);
        }
    }
}

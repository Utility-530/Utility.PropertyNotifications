using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utility.Helpers.NonGeneric;

namespace Utility.WPF.Demo.Extrinsic
{
    /// <summary>
    /// Interaction logic for AnimatedListBoxUserControl.xaml
    /// </summary>
    public partial class AnimatedListBoxUserControl : UserControl
    {
        private Random random = new Random();

        public AnimatedListBoxUserControl()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            AnimatedListBox.ScrollToSelectedItem = true;
            var index = random.Next(0, AnimatedListBox.ItemsSource.Count() - 1);
            AnimatedListBox.SelectedItem = AnimatedListBox.ItemsSource.Cast<object>().ElementAt(index);
        }
    }
}
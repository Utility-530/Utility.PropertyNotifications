using System;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Demo.Animations
{
    /// <summary>
    /// Interaction logic for GlowEffectUserControl.xaml
    /// </summary>
    public partial class GlowEffectUserControl
    {
        public GlowEffectUserControl()
        {
            InitializeComponent();
        }

        private void BtnDemo_Click(object sender, RoutedEventArgs e)
        {
            HighlightControl(sender as Control);
        }

        private void TxtDemo_GotFocus(object sender, RoutedEventArgs e)
        {
            HighlightControl(sender as Control);
        }

        private void TxtDemo_LostFocus(object sender, RoutedEventArgs e)
        {
            StopHighlight(sender as Control);
        }

        private void BorderDemo_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is Border border)
                border.Tag = "Pulse";
        }

        private void BorderDemo_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is Border border)
                border.Tag = null;
        }

        private void CmbDemo_DropDownOpened(object sender, EventArgs e)
        {
            HighlightControl(sender as Control);
        }

        private void HighlightAll_Click(object sender, RoutedEventArgs e)
        {
            HighlightControl(btnDemo);
            HighlightControl(txtDemo);
            HighlightControl(cmbDemo);
            borderDemo.Tag = "Pulse";
        }

        private void StopHighlights_Click(object sender, RoutedEventArgs e)
        {
            StopHighlight(btnDemo);
            StopHighlight(txtDemo);
            StopHighlight(cmbDemo);
            borderDemo.Tag = null;
        }

        private void HighlightControl(Control control)
        {
            if (control != null)
                control.Tag = "Highlight";
        }

        private void StopHighlight(Control control)
        {
            if (control != null)
                control.Tag = null;
        }
    }
}
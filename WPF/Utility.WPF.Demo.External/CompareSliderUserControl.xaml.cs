using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Utility.WPF.Demo.View
{
    /// <summary>
    /// Interaction logic for CompareSliderUserControl.xaml
    /// </summary>
    public partial class CompareSliderUserControl : UserControl
    {
        public CompareSliderUserControl()
        {
            InitializeComponent();
        }

        private void btnSlideUp_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.BeginStoryboard((Storyboard)this.FindResource("SlideUpAnimation"));
        }

        private void btnSlideDown_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.BeginStoryboard((Storyboard)this.FindResource("SlideDownAnimation"));
        }
    }
}
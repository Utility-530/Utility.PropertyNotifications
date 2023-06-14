using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Utility.WPF.Animation.Infrastructure;

namespace Utility.WPF.Demo.Animation
{
    /// <summary>
    /// Interaction logic for ExplosionUserControl.xaml
    /// </summary>
    public partial class ExplosionUserControl : UserControl
    {
        public ExplosionUserControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Storyboard storyboard = new();
            foreach (var anim in ExplosionAnimationHelper.CreateTargetAnimation(MainEllipse, 1))
                storyboard.Children.Add(anim);
            storyboard.Begin();

            Storyboard storyboard2 = new();
            storyboard2.Children.Add(ExplosionAnimationHelper.SetBlinkAnimation(TwoEllipse, 1));
            ExplosionAnimationHelper.ApplyOpacityMask(TwoEllipse);
            storyboard2.Begin();

            Storyboard storyboard3 = new();
            storyboard3.Children.Add(ExplosionAnimationHelper.SetColorAnimation(ThreeEllipse, 1));
            ExplosionAnimationHelper.ApplyOpacityMask(ThreeEllipse);
            storyboard3.Begin();
        }
    }
}
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using LookOrFeel.Animation;

namespace Utility.WPF.Demo.Animations
{
    /// <summary>
    /// Interaction logic for PennerDoubleAnimationView.xaml
    /// </summary>
    public partial class PennerDoubleAnimationView : UserControl
    {
        public PennerDoubleAnimationView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Window1_Loaded);
        }

        private void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            // populate ComboBox with Equation types
            foreach (string equationName in Enum.GetNames(typeof(PennerDoubleAnimation.Equations)))
                _Equations.Items.Add(equationName);

            _Equations.SelectedIndex = 0;
        }

        private void Animate()
        {
            PennerDoubleAnimation.Equations equation =
                (PennerDoubleAnimation.Equations)Enum.Parse(typeof(PennerDoubleAnimation.Equations), _Equations.SelectedItem.ToString());
            double from = double.Parse(_FromTB.Text);
            double to = double.Parse(_ToTB.Text);
            int durationMS = int.Parse(_DurationTB.Text);

            Animator.AnimatePenner(_Rect, Canvas.LeftProperty, equation, from, to, durationMS, OnAnimationComplete);

            StatusTB.Text = "Animating";
        }

        private void OnAnimateClicked(object sender, RoutedEventArgs e)
        {
            Animate();
        }

        /// <summary>
        /// Sets a random value for "To" and calls animate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAnimateRandomClicked(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
            _FromTB.Text = Canvas.GetLeft(_Rect).ToString();
            _ToTB.Text = Math.Round(rand.NextDouble() * 750).ToString();

            Animate();
        }

        /// <summary>
        /// Sample animation callback.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAnimationComplete(object sender, EventArgs e)
        {
            AnimationTimeline at = sender as AnimationTimeline;
            if (at != null)
                at.Completed -= OnAnimationComplete;

            StatusTB.Text = "Animation Complete!";
        }
    }
}
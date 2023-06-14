using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Utility.WPF.Demo.Animation
{
    /// <summary>
    /// Interaction logic for TextBoxUserControl.xaml
    /// </summary>
    public partial class TextBoxUserControl : UserControl
    {
        private DispatcherTimer timer;
        private bool _testRunning;
        private int i = 0;

        // This is some sample text to fill the AnimatedTextBox
        private string lorem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas iaculis ex scelerisque mauris blandit sollicitudin. Integer ac dolor metus. Nam eget elit vitae urna mattis egestas sed sit amet turpis. Morbi ultrices, nulla ac blandit ornare, dui diam bibendum nulla, at molestie arcu magna id metus. Vestibulum blandit, lectus nec gravida porta, ipsum turpis posuere est, in imperdiet sem lacus et neque. Phasellus varius massa non odio posuere iaculis.";

        public TextBoxUserControl()
        {
            InitializeComponent();

            // Initialize the timer
            timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(20) };
            timer.Tick += Timer_Tick;
        }

        /// <summary>
        /// Indicates whether the test is running.
        /// </summary>
        public bool TestRunning
        {
            get { return _testRunning; }
            set
            {
                _testRunning = value;

                // Enable the timer
                timer.IsEnabled = value;

                // Fill the indicator with color
                testIndicator.Fill = new SolidColorBrush(value ? Colors.Lime : Colors.Red);
            }
        }

        private void btnWrite_Click(object sender, RoutedEventArgs e)
        {
            // Fill or empty the AnimatedTextBox
            TxtAnimated.Text = (TxtAnimated.Text == "") ? lorem : "";
        }

        private void sliderDuration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Set the animation duration (if the window is loaded)
            if (this.IsLoaded) TxtAnimated.AnimationDuration = sliderDuration.Value;
        }

        private void btnWriteTimer_Click(object sender, RoutedEventArgs e)
        {
            // Toggle the test
            TestRunning = !TestRunning;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Write the sample text with a timer
            if (i < lorem.Length)
            {
                i++;
                TxtAnimated.Text = lorem.Substring(0, i);
            }
            else
            {
                TestRunning = false;
                i = 0;
            }
        }

        private void btnToggleWrap_Click(object sender, RoutedEventArgs e)
        {
            // Toggle the AnimatedTextBox's TextWrapping property
            TxtAnimated.TextWrapping = (TxtAnimated.TextWrapping == TextWrapping.Wrap) ? TextWrapping.NoWrap : TextWrapping.Wrap;
        }

        private void boxIsAnimated_Click(object sender, RoutedEventArgs e)
        {
            // Toggle the AnimatedTextBox's IsAnimated property
            TxtAnimated.IsAnimated = boxIsAnimated.IsChecked.Value;
        }
    }
}
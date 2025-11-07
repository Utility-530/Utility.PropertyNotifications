using System.Windows;
using System.Windows.Controls.Primitives;
using Utility.Enums;

namespace Utility.WPF.SandBox
{
    /// <summary>
    /// Interaction logic for ToggleWindow.xaml
    /// </summary>
    public partial class ToggleWindow : Window
    {
        public ToggleWindow()
        {
            InitializeComponent();
        }

        public XYTraversal Movement
        {
            get { return (XYTraversal)GetValue(MovementProperty); }
            set { SetValue(MovementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Movement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MovementProperty =
            DependencyProperty.Register("Movement", typeof(XYTraversal), typeof(ToggleWindow), new PropertyMetadata(XYTraversal.LeftToRight));
    }

    public class Ex
    {
        public static readonly DependencyProperty TimedTextProperty = DependencyProperty.RegisterAttached(
        "TimedText",
        typeof(int),
        typeof(Ex),
        new FrameworkPropertyMetadata(int.MinValue, TimedTextPropertyChanged));

        private static bool changed;
        private static RepeatButton repeatButton;

        private static void TimedTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RepeatButton r && repeatButton == null)
            {
                repeatButton = r;
                r.Click += (s, _) => TimedTextPropertyChanged(d, e);
            }
            if (changed == true)
                return;
            changed = true;
            SetTimedText(d, (GetTimedText(d) + 1) % 4);
            changed = false;
        }

        public static void SetTimedText(DependencyObject textBlock, int value)
        {
            textBlock.SetValue(TimedTextProperty, value);
        }

        public static int GetTimedText(DependencyObject textBlock)
        {
            return (int)textBlock.GetValue(TimedTextProperty);
        }
    }
}
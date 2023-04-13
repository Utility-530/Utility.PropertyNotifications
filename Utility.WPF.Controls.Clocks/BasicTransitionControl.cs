using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Controls.Clocks
{
    public class BasicTransitionControl : Control
    {
        public static readonly DependencyProperty UnCheckedContentProperty =
            DependencyProperty.Register("UnCheckedContent", typeof(object), typeof(BasicTransitionControl), new PropertyMetadata(null));

        public static readonly DependencyProperty CheckedContentProperty =
            DependencyProperty.Register("CheckedContent", typeof(object), typeof(BasicTransitionControl), new PropertyMetadata(null));

        static BasicTransitionControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BasicTransitionControl), new FrameworkPropertyMetadata(typeof(BasicTransitionControl)));
        }

        public object CheckedContent
        {
            get { return (object)GetValue(CheckedContentProperty); }
            set { SetValue(CheckedContentProperty, value); }
        }

        public object UnCheckedContent
        {
            get { return (object)GetValue(UnCheckedContentProperty); }
            set { SetValue(UnCheckedContentProperty, value); }
        }
    }
}
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Controls
{
    public class TransitionControl : Control
    {
        public static readonly DependencyProperty UnCheckedButtonContentProperty =
            DependencyProperty.Register("UnCheckedButtonContent", typeof(object), typeof(TransitionControl), new PropertyMetadata(null));

        public static readonly DependencyProperty UnCheckedContentProperty =
            DependencyProperty.Register("UnCheckedContent", typeof(object), typeof(TransitionControl), new PropertyMetadata(null));

        public static readonly DependencyProperty CheckedContentProperty =
            DependencyProperty.Register("CheckedContent", typeof(object), typeof(TransitionControl), new PropertyMetadata(null));

        public static readonly DependencyProperty CheckedButtonContentProperty =
            DependencyProperty.Register("CheckedButtonContent", typeof(object), typeof(TransitionControl), new PropertyMetadata(null));

        static TransitionControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TransitionControl), new FrameworkPropertyMetadata(typeof(TransitionControl)));
        }

        public object CheckedButtonContent
        {
            get { return (object)GetValue(CheckedButtonContentProperty); }
            set { SetValue(CheckedButtonContentProperty, value); }
        }

        public object UnCheckedButtonContent
        {
            get { return (object)GetValue(UnCheckedButtonContentProperty); }
            set { SetValue(UnCheckedButtonContentProperty, value); }
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
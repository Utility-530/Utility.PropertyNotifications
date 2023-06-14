using ReactiveUI;
using System.Windows;
using Utility.WPF.Behavior;

namespace Utility.WPF.Controls.Buttons
{
    public class ToggleButton : System.Windows.Controls.Primitives.ToggleButton
    {
        public static readonly DependencyProperty UnCheckedContentProperty = DependencyProperty.Register("UnCheckedContent", typeof(object), typeof(ToggleButton), new FrameworkPropertyMetadata(null));

        public ToggleButton()
        {
            object? checkedContent = null;
            ToggleButtonContentBehavior.ConfigureContent(this, this.WhenAnyValue(a => a.UnCheckedContent), checkedContent);
        }

        public object UnCheckedContent
        {
            get { return GetValue(UnCheckedContentProperty); }
            set { SetValue(UnCheckedContentProperty, value); }
        }
    }

    public class RadioButton : System.Windows.Controls.RadioButton
    {
        public static readonly DependencyProperty UnCheckedContentProperty = DependencyProperty.Register("UnCheckedContent", typeof(object), typeof(RadioButton), new FrameworkPropertyMetadata(null));

        public RadioButton()
        {
            object? checkedContent = null;
            ToggleButtonContentBehavior.ConfigureContent(this, this.WhenAnyValue(a => a.UnCheckedContent), checkedContent);
        }

        public object UnCheckedContent
        {
            get { return GetValue(UnCheckedContentProperty); }
            set { SetValue(UnCheckedContentProperty, value); }
        }
    }
}
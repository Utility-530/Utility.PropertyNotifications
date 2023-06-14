using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Utility.WPF.Controls
{
    public class LabelledTextBox : LabelledContent
    {
        static LabelledTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LabelledTextBox), new FrameworkPropertyMetadata(typeof(LabelledTextBox)));
        }
    }

    public class LabelledTextBlock : LabelledContent
    {
        static LabelledTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LabelledTextBlock), new FrameworkPropertyMetadata(typeof(LabelledTextBlock)));
        }
    }

    public class LabelledContent : HeaderedContentControl
    {
        private const double OpacityLabel = 0.5;

        private ContentPresenter? label;
        private TextBlock? hintBox;

        //public static readonly DependencyProperty LabelTextProperty = DependencyProperty.Register("LabelText", typeof(string), typeof(LabelledContent));
        public static readonly DependencyProperty HintProperty = DependencyProperty.Register("Hint", typeof(string), typeof(LabelledContent));

        public static readonly DependencyProperty HintColorProperty = DependencyProperty.Register("HintColor", typeof(Color), typeof(LabelledContent), new PropertyMetadata(Colors.Gray));

        static LabelledContent()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LabelledContent), new FrameworkPropertyMetadata(typeof(LabelledContent)));
        }

        public override void OnApplyTemplate()
        {
            //label = GetTemplateChild("PART_Label") as Label ?? throw new NullReferenceException("PART_Label object is null");
            hintBox = GetTemplateChild("HintTextBox") as TextBlock ?? throw new NullReferenceException("HintTextBox object is null");
            label = GetTemplateChild("PART_HeaderPresenter") as ContentPresenter;
            //this.WhenAnyValue(a => a.Header)
            //    .BindTo(label, a => a.Content);

            //Binding parentBinding = bindingExpression.ParentBinding;
            //textBox2.SetBinding(TextBox.TextProperty, parentBinding);

            this.WhenAnyValue(a => a.Hint)
                .BindTo(hintBox, a => a.Text);

            this.WhenAnyValue(a => a.HintColor)
                .Select(a => new SolidColorBrush { Color = a })
                .BindTo(hintBox, a => a.Foreground);

            //if (Header != null)
            //    label.Content = Header;
            SetupAnimation();
            base.OnApplyTemplate();
        }

        private void SetupAnimation()
        {
            if (label is not null)
            {
                label.Opacity = OpacityLabel;
                GotFocus += LabelledContent_GotFocus;
                LostFocus += LabelledContent_LostFocus;
            }

            void LabelledContent_LostFocus(object sender, RoutedEventArgs e)
            {
                DoubleAnimation animation = new(label!.Opacity, OpacityLabel, new Duration(TimeSpan.FromSeconds(1)));
                label.BeginAnimation(OpacityProperty, animation);
            }

            void LabelledContent_GotFocus(object sender, RoutedEventArgs e)
            {
                DoubleAnimation animation = new(label!.Opacity, 1, new Duration(TimeSpan.FromSeconds(0.1)));
                label.BeginAnimation(OpacityProperty, animation);
            }
        }

        //public string LabelText
        //{
        //    get => (string)GetValue(LabelTextProperty);
        //    set => SetValue(LabelTextProperty, value);
        //}

        public string Hint
        {
            get => (string)GetValue(HintProperty);
            set => SetValue(HintProperty, value);
        }

        public Color HintColor
        {
            get => (Color)GetValue(HintColorProperty);
            set => SetValue(HintColorProperty, value);
        }
    }
}
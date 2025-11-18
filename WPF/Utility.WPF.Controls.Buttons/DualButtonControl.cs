using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Utility.WPF.Reactives;

namespace Utility.WPF.Controls.Buttons
{
    public class DualButtonControl : SwitchControl
    {
        static DualButtonControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DualButtonControl), new FrameworkPropertyMetadata(typeof(DualButtonControl)));
            BackgroundProperty.OverrideMetadata(typeof(DualButtonControl), new FrameworkPropertyMetadata(SystemColors.ControlLightBrush));
            ForegroundProperty.OverrideMetadata(typeof(DualButtonControl), new FrameworkPropertyMetadata(SystemColors.ControlDarkBrush));
        }

        public DualButtonControl()
        {
            this.Observe(a => a.Value).CombineLatest(this.Observe(a => a.Main), this.Observe(a => a.Alternate))
                .Where(a => a.Second != null && a.Third != null)
                .Subscribe(b =>
                {
                    Change(b.First);
                });
        }

        public override void OnApplyTemplate()
        {
            var edit2Button = GetTemplateChild("Edit2Button") as ButtonBase ?? throw new NullReferenceException("Deserialized object is null");
            edit2Button.Click += EditButton_OnClick;

            var editButton = GetTemplateChild("EditButton") as ButtonBase ?? throw new NullReferenceException("Deserialized object is null");
            editButton.Click += Edit_Button_OnClick;
            base.OnApplyTemplate();
        }

        protected override void EditButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (Value != false)
                Change(Value = false);
        }

        protected void Edit_Button_OnClick(object sender, RoutedEventArgs e)
        {
            if (Value != true)
                Change(Value = true);
        }

        public static readonly DependencyProperty OrientationProperty = WrapPanel.OrientationProperty.AddOwner(typeof(DualButtonControl));

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }
    }
}
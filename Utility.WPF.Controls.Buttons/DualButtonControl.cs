using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Utility.WPF.Controls.Buttons
{
    public class DualButtonControl : SwitchControl
    {
        static DualButtonControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DualButtonControl), new FrameworkPropertyMetadata(typeof(DualButtonControl)));
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
            if (Value)
                SetValueCommand.Execute(Alternate);
        }

        protected void Edit_Button_OnClick(object sender, RoutedEventArgs e)
        {
            if (!Value)
                SetValueCommand.Execute(Main);
        }

        public static readonly DependencyProperty OrientationProperty = WrapPanel.OrientationProperty.AddOwner(typeof(DualButtonControl));


        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Leepfrog.WpfFramework.Actions
{
    /// <summary>
    /// Trigger action to set focus to another control
    /// </summary>
    public class SetFocusAction : TriggerAction<UIElement>
    {

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(UIElement), typeof(SetFocusAction), new UIPropertyMetadata(null));

        public UIElement Target
        {
            get { return (UIElement)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        protected override void Invoke(object parameter)
        {
            if (Target == null)
            {
                return;
            }
            Target.Focus();
            //Keyboard.Focus(Target);
            if (Target is TextBox)
            {
                ((TextBox)Target).SelectAll();
            }
            if (!Target.IsVisible)
            {
                Target.IsVisibleChanged += setFocus;
            }
        }

        private void setFocus(object sender, DependencyPropertyChangedEventArgs e)
        {
            Target.IsVisibleChanged -= setFocus;
            Invoke(null);
        }
    }
}

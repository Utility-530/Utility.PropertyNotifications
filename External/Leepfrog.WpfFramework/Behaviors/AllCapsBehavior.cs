using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Media;
using System.Globalization;

namespace Leepfrog.WpfFramework.Behaviors
{
    /// <summary>
    /// Behaviour to be applied to Label controls
    /// When label's target is bound to a required field, IsTargetRequired = true
    /// </summary>
    public static class AllCapsBehavior
    {
        public enum AllCapsMode
        {
            Upper,
            Lower
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached(
            "Text",
            typeof(string),
            typeof(AllCapsBehavior),
            new UIPropertyMetadata(string.Empty, OnTextChanged));

        public static string GetText(DependencyObject d)
        {
            return (string)d.GetValue(TextProperty);
        }

        public static void SetText(DependencyObject d, string value)
        {
            d.SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.RegisterAttached(
            "Mode",
            typeof(AllCapsMode),
            typeof(AllCapsBehavior),
            new UIPropertyMetadata(AllCapsMode.Upper, OnModeChanged));

        public static string GetMode(DependencyObject d)
        {
            return (string)d.GetValue(ModeProperty);
        }

        public static void SetMode(DependencyObject d, string value)
        {
            d.SetValue(ModeProperty, value);
        }
        private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            updateText(sender);
        }
        private static void OnModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            updateText(sender);
        }
        private static void updateText(DependencyObject sender)
        {
            var text = sender.GetValue(TextProperty) as string;
            switch (sender.GetValue(ModeProperty))
            {
                case AllCapsMode.Lower:
                    text = text?.ToLower();
                    break;
                default:
                    text = text?.ToUpper();
                    break;
            }
            if (sender is TextBlock tb)
            {
                tb.Text = text;
            }
            else if (sender is Run r)
            {
                r.Text = text;
            }
            else if (sender is ContentControl c)
            {
                c.Content = text;
            }
        }
    }
}

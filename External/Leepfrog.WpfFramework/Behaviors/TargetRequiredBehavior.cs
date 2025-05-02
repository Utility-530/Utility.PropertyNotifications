using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;

namespace Leepfrog.WpfFramework.Behaviors
{
	/// <summary>
	/// Behaviour to be applied to Label controls
    /// When label's target is bound to a required field, IsTargetRequired = true
	/// </summary>
    public static class TargetRequiredBehavior
    {
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(TargetRequiredBehavior),
            new UIPropertyMetadata(false, OnIsEnabledChanged));

        public static bool GetIsEnabled(DependencyObject d)
        {
            return (bool)d.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject d, bool value)
        {
            d.SetValue(IsEnabledProperty, value);
        }
        private static void OnIsEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var label = sender as Label;
            // IF SENDER IS NOT A LABEL, JUST EXIT
            if (label == null)
            {
                return;
            }

            bool isEnabled = (bool)(e.NewValue);

            // GET DESCRIPTOR OF LABEL'S TARGET PROPERTY
            DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(Label.TargetProperty, typeof(Label));
            if (isEnabled)
            {
                // REGISTER FOR UPDATES WHENEVER THE TARGET PROPERTY CHANGES...
                dpd.AddValueChanged(label, label_TargetChanged);
                hookLabelTarget(label);
            }
            else
            {
                // UNREGISTER
                dpd.RemoveValueChanged(label, label_TargetChanged);
                unhookLabelTarget(label);
            }
        }

        private static Dictionary<Label, Control> _labelTargets = new Dictionary<Label, Control>();

        private static void label_TargetChanged(object sender, EventArgs e)
        {
            var label = sender as Label;
            hookLabelTarget(label);
        }

        private static void hookLabelTarget(Label label)
        {
            unhookLabelTarget(label);

            var targetControl = ((label.Target) as Control);
            // IF TARGET IS NOT VALID, JUST EXIT
            if (targetControl == null)
            {
                return;
            }

            var prop = getAssociatedProperty(targetControl);
            // IF THIS TYPE OF CONTROL DOESN'T HAVE A PROPERTY ASSOCIATED...
            if (prop == null)
            {
                // JUST QUIT
                return;
            }
            //var dpd = DependencyPropertyDescriptor.FromProperty(prop, targetControl.GetType());
            //dpd.AddValueChanged(targetControl, targetControl_TargetPropertyChanged);
            targetControl.DataContextChanged += targetControl_DataContextChanged;
            _labelTargets.Add(label, targetControl);

            if (targetControl.IsLoaded)
            {
                checkIsRequired(label, targetControl, prop);
            }
            else
            {
                // TARGET CONTROL IS NOT LOADED YET
                // WE'LL HOOK UP ONLOADED
                // AND RECHECK THERE
                targetControl.Loaded += targetControl_Loaded;
            }

        }

        static void targetControl_Loaded(object sender, RoutedEventArgs e)
        {
            var targetControl = (sender as Control);
            targetControl.Loaded -= targetControl_Loaded;
            checkIsRequired(targetControl);
        }

        static void targetControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            checkIsRequired(sender as Control);
        }

        private static void checkIsRequired(Control targetControl)
        {
            var label = _labelTargets.FirstOrDefault(t => t.Value == targetControl).Key;
            if (label == null)
            {
                return;
            }
            checkIsRequired(label, targetControl);
        }
        private static void checkIsRequired(Label label, Control targetControl)
        {
            var prop = getAssociatedProperty(targetControl);
            checkIsRequired(label, targetControl, prop);
        }
        private static void checkIsRequired(Label label, Control targetControl, DependencyProperty prop)
        {
            // GET THE BINDING FROM THAT PROPERTY
            var binding = targetControl.GetBindingExpression(prop);
            if ((binding == null) || (binding.ResolvedSource == null))
            {
                return;
            }

            // SET OR CLEAR VALUE DEPENDING ON WHETHER OR NOT THAT PROPERTY HAS ANY REQUIRED FIELD ATTRIBUTES
            label.SetValue(IsTargetRequiredProperty, (binding.ResolvedSource.GetType().GetProperty(binding.ResolvedSourcePropertyName).GetCustomAttributes(typeof(RequiredAttribute), true).Any()));
        }
        private static void targetControl_TargetPropertyChanged(object sender, EventArgs e)
        {
            checkIsRequired(sender as Control );
        }

        private static void unhookLabelTarget(Label label)
        {
            if (!_labelTargets.ContainsKey(label))
            {
                return;
            }
            var targetControl = _labelTargets[label];
            var prop = getAssociatedProperty(targetControl);
            //var dpd = DependencyPropertyDescriptor.FromProperty(prop, targetControl.GetType());
            //dpd.RemoveValueChanged(targetControl, targetControl_TargetPropertyChanged);
            _labelTargets.Remove(label);
        }

        private static DependencyProperty getAssociatedProperty(Control targetControl)
        {
            // DEPENDING ON CONTROL TYPE, GET THE CORRECT PROPERTY TO LOOK UP
            if (targetControl is TextBox)
            {
                return TextBox.TextProperty;
            }
            else if (targetControl is ComboBox)
            {
                return ComboBox.SelectedValueProperty;
            }
            else if (targetControl is ListBox)
            {
                return ListBox.SelectedValueProperty;
            }
            else if (targetControl is CheckBox)
            {
                return CheckBox.IsCheckedProperty;
            }
            else 
            {
                return null;
            }

        }



        public static readonly DependencyProperty IsTargetRequiredProperty =
            DependencyProperty.RegisterAttached(
            "IsTargetRequired",
            typeof(bool),
            typeof(TargetRequiredBehavior),
            new UIPropertyMetadata(false));

        public static bool GetIsTargetRequired(DependencyObject d)
        {
            return (bool)d.GetValue(IsTargetRequiredProperty);
        }

        public static void SetIsTargetRequired(DependencyObject d, bool value)
        {
            d.SetValue(IsTargetRequiredProperty, value);
        }

    }
}

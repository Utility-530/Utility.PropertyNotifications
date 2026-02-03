using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Utility.WPF.ComboBoxes
{

    public partial class FilterBehavior : Behavior<ComboBox>
    {
        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.RegisterAttached(
                "IsEditing",
                typeof(bool),
                typeof(FilterBehavior),
                new PropertyMetadata(false, _changed));


        public static bool GetIsEditing(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEditingProperty);
        }

        private static void SetIsEditing(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEditingProperty, value);
        }
        private static void _changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ComboBox comboBox)
            {
                if (e.NewValue is true)
                {
                    var textBox = findTextBox(comboBox);
                    textBox.Focus();
                    Keyboard.Focus(textBox);
                }
                else
                {
                    findPopup(comboBox).IsOpen = false;
                }
            }
        }


        protected override void OnAttached()
        {
            if (AssociatedObject.IsLoaded)
                ComboBox_Loaded(AssociatedObject, null);
            else
                AssociatedObject.Loaded += ComboBox_Loaded;

            AssociatedObject.DropDownOpened += ComboBox_DropDownOpened;
            AssociatedObject.DropDownClosed += ComboBox_DropDownClosed;
            AssociatedObject.LostKeyboardFocus += ComboBox_LostKeyboardFocus;
            AssociatedObject.IsKeyboardFocusWithinChanged += ComboBox_IsKeyboardFocusWithinChanged;
            AssociatedObject.SelectionChanged += ComboBox_SelectionChanged;

            // Handle keyboard shortcuts
            AssociatedObject.PreviewKeyDown += ComboBox_PreviewKeyDown;

            // Open dropdown on focus
            AssociatedObject.GotFocus += (s, args) => { 
            };
            AssociatedObject.DropDownOpened += (s, args) =>
            {
                findPopup(AssociatedObject).IsOpen = true;
            };
            AssociatedObject.DropDownClosed += (s, args) =>
            {

            };

            var textBox = findTextBox(AssociatedObject);
            setupFiltering(AssociatedObject, findTextBox(AssociatedObject));
            setupHoverTracking(AssociatedObject);
        }

        private static void ComboBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                // Delay to check if focus went to the popup
                //comboBox.Dispatcher.BeginInvoke(new Action(() =>
                //{
                if (!comboBox.IsKeyboardFocusWithin && !comboBox.IsDropDownOpen)
                {
                    SetIsEditing(comboBox, false);
                }
                //}), System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        private static void ComboBox_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && e.NewValue is bool hasKeyboardFocus && !hasKeyboardFocus)
            {
                // Small delay to ensure it's not just moving to popup
                //comboBox.Dispatcher.BeginInvoke(new Action(() =>
                //{
                if (!comboBox.IsKeyboardFocusWithin && !comboBox.IsDropDownOpen)
                {
                    SetIsEditing(comboBox, false);
                }
                //}), System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        private static void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                var textBox = findTextBox(comboBox);
                textBox.Focus();
                Keyboard.Focus(textBox);
                textBox.SelectAll();

    
                if (textBox != null)
                {
                    textBox.GotKeyboardFocus += (s, args) => SetIsEditing(comboBox, true);
                    textBox.LostKeyboardFocus += (s, args) =>
                    {
                        // Small delay to check if dropdown is still open
                        //comboBox.Dispatcher.BeginInvoke(new System.Action(() =>
                        //{
                        if (!comboBox.IsDropDownOpen && !textBox.IsKeyboardFocused)
                        {
                            SetIsEditing(comboBox, false);
                        }
                        //}), System.Windows.Threading.DispatcherPriority.Background);
                    };

                }
            }
        }

        private static void ComboBox_DropDownOpened(object sender, System.EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                if (GetIsEditing(comboBox) == false)
                    comboBox.Dispatcher.BeginInvoke(new System.Action(() =>
                    {
                        SetIsEditing(comboBox, true);
                    }), System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        private static void ComboBox_DropDownClosed(object sender, System.EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                //comboBox.Dispatcher.BeginInvoke(new System.Action(() =>
                //{
                var textBox = comboBox.Template?.FindName("PART_Custom_EditableTextBox", comboBox) as TextBox;
                if (textBox != null /*&& !textBox.IsKeyboardFocused*/)
                {
                    SetIsEditing(comboBox, false);
                }
                //}), System.Windows.Threading.DispatcherPriority.Background);
            }
        }
    }
}

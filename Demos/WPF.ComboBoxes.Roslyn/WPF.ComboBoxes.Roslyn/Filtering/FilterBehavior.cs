using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace WPF.ComboBoxes.Roslyn
{
    /// <summary>
    /// Attached behavior for ComboBox that enables filtering with IntelliSense-style sorting
    /// </summary>
    public partial class FilterBehavior
    {
        private static readonly DependencyProperty IsUpdatingTextProperty =
            DependencyProperty.RegisterAttached(
                "IsUpdatingText",
                typeof(bool),
                typeof(FilterBehavior),
                new PropertyMetadata(false));

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.RegisterAttached(
                "SelectedItem",
                typeof(object),
                typeof(FilterBehavior),
                new FrameworkPropertyMetadata(changed));

        public static object GetSelectedItem(DependencyObject obj)
        {
            return (object)obj.GetValue(SelectedItemProperty);
        }

        public static void SetSelectedItem(DependencyObject obj, int value)
        {
            obj.SetValue(SelectedItemProperty, value);
        }


        private static readonly DependencyProperty FilterTimerProperty =
            DependencyProperty.RegisterAttached(
                "FilterTimer",
                typeof(DispatcherTimer),
                typeof(FilterBehavior),
                new PropertyMetadata(null));

        public static readonly DependencyProperty SearchTextProperty =
     DependencyProperty.RegisterAttached(
         "SearchText",
         typeof(string),
         typeof(FilterBehavior),
         new PropertyMetadata(string.Empty));

        public static string GetSearchText(DependencyObject obj)
        {
            return (string)obj.GetValue(SearchTextProperty);
        }

        public static void SetSearchText(DependencyObject obj, string value)
        {
            obj.SetValue(SearchTextProperty, value);
        }

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.RegisterAttached(
                "Index",
                typeof(int),
                typeof(FilterBehavior),
                new FrameworkPropertyMetadata(
                    -1,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange,
                    changed));

        public static int GetIndex(DependencyObject obj)
        {
            return (int)obj.GetValue(IndexProperty);
        }

        public static void SetIndex(DependencyObject obj, int value)
        {
            obj.SetValue(IndexProperty, value);
        }

        public static readonly DependencyProperty ConverterProperty =
            DependencyProperty.RegisterAttached(
                "Converter",
                typeof(IValueConverter),
                typeof(FilterBehavior),
                new FrameworkPropertyMetadata());

        public static IValueConverter GetConverter(DependencyObject obj)
        {
            return (IValueConverter)obj.GetValue(ConverterProperty);
        }

        public static void SetConverter(DependencyObject obj, IValueConverter value)
        {
            obj.SetValue(ConverterProperty, value);
        }

        public static readonly DependencyProperty IsDebuggingProperty =
            DependencyProperty.RegisterAttached(
        "IsDebugging",
        typeof(bool),
        typeof(FilterBehavior),
         new FrameworkPropertyMetadata(
            false,
            changed));

        public static bool GetIsDebugging(DependencyObject obj)
        {
            return (bool)obj.GetValue(IndexProperty);
        }

        public static void SetIsDebugging(DependencyObject obj, bool value)
        {
            obj.SetValue(IndexProperty, value);
        }

        public static readonly DependencyProperty IsSelectionSecondOrderProperty =
            DependencyProperty.RegisterAttached(
        "IsSelectionSecondOrder",
        typeof(bool),
        typeof(FilterBehavior),
         new FrameworkPropertyMetadata(
            false,
            changed));

        public static bool GetIsSelectionSecondOrder(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSelectionSecondOrderProperty);
        }

        public static void SetIsSelectionSecondOrder(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSelectionSecondOrderProperty, value);
        }

        private static void changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public static readonly DependencyProperty ProgressProperty =
    DependencyProperty.RegisterAttached(
        "Progress",
        typeof(double),
        typeof(FilterBehavior),
        new PropertyMetadata(0d));

        public static double GetProgress(DependencyObject obj)
        {
            return (double)obj.GetValue(ProgressProperty);
        }

        public static void SetProgress(DependencyObject obj, double value)
        {
            obj.SetValue(ProgressProperty, value);
        }

        #region Event Handlers


        private static void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                SetIndex(comboBox, 0);
                comboBox.SetValue(IsUpdatingTextProperty, true);
                if (e.AddedItems.Count > 0)
                {         
                    SetIndex(comboBox, comboBox.SelectedIndex);
                    if (GetConverter(comboBox)?.Convert(e.AddedItems[0], typeof(string), null, null) is string conversion)
                        findTextBox(comboBox).Text = conversion;
                    comboBox.SetValue(FilterBehavior.SelectedItemProperty, e.AddedItems[0]);

                    // Set flag to ignore the TextChanged event that happens when selection updates the text    
                    //if (e.AddedItems[0] is IntelliSenseResult { Symbol.Item: ISymbol symbol } result)
                    //{
                    //    findTextBox(comboBox).Text = symbol?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                    //    //SetIndex(comboBox, comboBox.SelectedIndex);
                    //    var paraphernalia = pairs[comboBox];
                    //    paraphernalia.telemetry.MarkUsed(symbol);
                    //    paraphernalia.mru.MarkUsed(symbol);

                    //    if (GetIsSelectionSecondOrder(comboBox))
                    //    {
                    //        if (symbol is IMethodSymbol methodSymbol)
                    //        {
                    //            comboBox.ItemsSource = methodSymbol.Parameters;
                    //            comboBox.IsDropDownOpen = true;
                    //        }
                    //        if (symbol is ITypeSymbol typeSymbol)
                    //        {
                    //            comboBox.ItemsSource = typeSymbol.AllInterfaces;
                    //            comboBox.IsDropDownOpen = true;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        findPopup(comboBox).IsOpen = false;
                    //    }
                    //}
                    //else if (e.AddedItems[0] is ISymbol _symbol)
                    //{
                    //    comboBox.SetValue(FilterBehavior.SelectedItemProperty, _symbol);
                    //}
                    //else
                    //    throw new Exception("FilterBehavior only supports items of type ITypeSpecifier.");
                }
                else
                {

                }
                comboBox.SetValue(IsUpdatingTextProperty, false);
                if (comboBox.StaysOpenOnEdit)
                {
                    findPopup(comboBox).IsOpen = false;
                }
            }
            else
                throw new Exception("ds 3 s");
        }

        //private static void ComboBox_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    if (sender is ComboBox comboBox)
        //    {
        //        // Open dropdown on focus
        //        if (!comboBox.IsDropDownOpen)
        //        {
        //            //comboBox.IsDropDownOpen = true;
        //        }

        //        // Select all text for easy replacement
        //        var textBox = findTextBox(comboBox);
        //        setupFiltering(comboBox, findTextBox(comboBox));
        //        setupHoverTracking(comboBox);
        //        //if (textBox != null)
        //        //{

        //        //    //textBox.SelectAll();
        //        //}
        //    }
        //}

        private static void ComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                // Handle Enter key to confirm selection
                //if (e.Key == Key.Enter && comboBox.IsDropDownOpen)
                //{
                //    if (comboBox.Items.Count > 0 && comboBox.SelectedIndex >= 0)
                //    {
                //        comboBox.IsDropDownOpen = false;
                //        e.Handled = true;
                //    }
                //}
                // Handle Escape to close dropdown
                if (e.Key == Key.Escape)
                {
                    comboBox.IsDropDownOpen = false;
                    e.Handled = true;
                }
                // Handle Enter key to confirm selection
                if (e.Key == Key.Enter)
                {
                    if (comboBox.IsDropDownOpen && comboBox.Items.Count > 0)
                    {
                        // If nothing is selected, select the first item
                        if (GetIndex(comboBox) < 0)
                        {
                            SetIndex(comboBox, 0);
                        }
                        comboBox.SelectedIndex = GetIndex(comboBox);
                        //ScrollIntoView(comboBox);
                        //comboBox.IsDropDownOpen = false;

                        // Move focus away from textbox to trigger display mode
                        var textBox = findTextBox(comboBox);
                        if (textBox != null)
                        {
                            //Keyboard.ClearFocus();
                        }

                        e.Handled = true;
                    }
                }
                // Handle Down arrow - move to next item
                else if (e.Key == Key.Down && comboBox.IsDropDownOpen)
                {
                    if (comboBox.Items.Count > 0)
                    {
                        int newIndex = GetIndex(comboBox) + 1;
                        if (newIndex >= comboBox.Items.Count)
                            newIndex = 0;

                        SetIndex(comboBox, newIndex);
                        //ScrollIntoView(comboBox);
                        e.Handled = true;
                    }
                }
                // Handle Up arrow - move to previous item
                else if (e.Key == Key.Up && comboBox.IsDropDownOpen)
                {
                    if (comboBox.Items.Count > 0)
                    {
                        int newIndex = GetIndex(comboBox) - 1;
                        if (newIndex < 0)
                            newIndex = comboBox.Items.Count - 1;

                        SetIndex(comboBox, newIndex);
                        //ScrollIntoView(comboBox);
                        e.Handled = true;
                    }
                }
            }
        }



        #endregion
        #region Helper Methods

        private static void scrollIntoView(ComboBox comboBox)
        {
            if (comboBox.SelectedItem != null)
            {
                // Scroll the selected item into view
                comboBox.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var container = comboBox.ItemContainerGenerator.ContainerFromItem(comboBox.SelectedItem) as FrameworkElement;
                    container?.BringIntoView();
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }
        private static void setupHoverTracking(ComboBox comboBox)
        {
            if (comboBox.IsDropDownOpen)
            {
                onDropDownOpened();
            }
            comboBox.DropDownOpened += (s, e) =>
            {
                onDropDownOpened();
            };
            void onDropDownOpened()
            {
                MouseEventHandler enterHandler = (sender, args) =>
                {
                    // Find the ComboBoxItem that raised this event
                    var item = (args.OriginalSource as DependencyObject).FindParent<ComboBoxItem>();

                    if (item != null)
                    {
                        int index = comboBox.ItemContainerGenerator.IndexFromContainer(item);
                        if (index >= 0)
                        {
                            SetIndex(comboBox, index);
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        foreach (var x in comboBox.Items)
                        {
                            var _item = (x as ComboBoxItem) ?? (ComboBoxItem)comboBox.ItemContainerGenerator.ContainerFromItem(x);
                            if (_item is { IsHighlighted: true })
                            {
                                SetIndex(comboBox, comboBox.ItemContainerGenerator.IndexFromContainer(_item));
                            }
                        }
                    }
                };


                var popup = findPopup(comboBox);
                // Attach to ComboBox - events from containers will bubble up
                popup.AddHandler(UIElement.MouseMoveEvent, enterHandler);
                popup.AddHandler(UIElement.MouseEnterEvent, enterHandler);
                popup.AddHandler(UIElement.PreviewMouseMoveEvent, enterHandler);

                // Clean up when dropdown closes
                EventHandler closedHandler = null;
                closedHandler = (sender, args) =>
                {
                    popup.RemoveHandler(UIElement.MouseMoveEvent, enterHandler);
                    popup.RemoveHandler(UIElement.MouseEnterEvent, enterHandler);
                    popup.RemoveHandler(UIElement.PreviewMouseMoveEvent, enterHandler);
                    comboBox.DropDownClosed -= closedHandler;
                };
                comboBox.DropDownClosed += closedHandler;


            }


        }
        //private static int GetContainerIndexAtPointUsingHitTest(ComboBox comboBox, System.Windows.Point point)
        //{
        //    // Find the visual element at the mouse position
        //    HitTestResult hitTestResult = VisualTreeHelper.HitTest(comboBox, point);

        //    if (hitTestResult?.VisualHit != null)
        //    {
        //        // Walk up the visual tree to find ComboBoxItem
        //        DependencyObject current = hitTestResult.VisualHit;

        //        while (current != null && current != comboBox)
        //        {
        //            if (current is ComboBoxItem item)
        //            {
        //                // Get the index of this container
        //                int index = comboBox.ItemContainerGenerator.IndexFromContainer(item);
        //                return index;
        //            }

        //            current = VisualTreeHelper.GetParent(current);
        //        }
        //    }

        //    return -1;
        //}

        private static void setupFiltering(ComboBox comboBox, TextBox textBox)
        {

            // Create debounce timer
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(150) // Adjust delay as needed
            };

            timer.Tick += (s, e) =>
            {
                var tb = textBox;
                timer.Stop();
                applyFilter(comboBox);
            };

            comboBox.SetValue(FilterTimerProperty, timer);
            attachTextBoxHandlers(comboBox, textBox);
            timer.Start();

        }

        private static TextBox findTextBox(ComboBox comboBox)
        {

            comboBox.ApplyTemplate();
            var textBox = /*comboBox.Template?.FindName("PART_Custom_EditableTextBox", comboBox) as TextBox ??*/ comboBox.FindChild<TextBox>();
            if (textBox == null)
                throw new Exception("SF d");
            return textBox;
        }
        private static Popup findPopup(ComboBox comboBox)
        {
            comboBox.ApplyTemplate();
            return comboBox.Template?.FindName("Popup", comboBox) as Popup;
        }

        #endregion

        private static void attachTextBoxHandlers(ComboBox comboBox, TextBox textBox)
        {
            textBox.TextChanged += (sender, args) =>
            {
                // Debounce the filter
                if (comboBox.GetValue(IsUpdatingTextProperty) is false)
                {
                    var timer = comboBox.GetValue(FilterTimerProperty) as DispatcherTimer;
                    if (timer != null)
                    {
                        timer.Stop();
                        timer.Start();
                    }
                }
            };
        }

        private static async void applyFilter(ComboBox comboBox)
        {
            if ((bool)comboBox.GetValue(IsUpdatingTextProperty))
                return;

            if (findTextBox(comboBox) is not TextBox textBox)
                return;

            bool flag = false;

            if (comboBox.ItemsSource != null)
                foreach (var item in comboBox.ItemsSource)
                {
                    if (item == comboBox.SelectedItem)
                        flag = true;
                }
            if (flag == false)
                SetIndex(comboBox, 0);

            string searchText = textBox.Text ?? string.Empty;
            //string filterProperty = GetFilterProperty(comboBox);
            if (!string.IsNullOrEmpty(searchText))
            {
                findPopup(comboBox).IsOpen = true;
            }
            SetSearchText(comboBox, searchText);

            //var results = await pairs[comboBox].asyncEngine.UpdateAsync(searchText, fast =>
            //{
            //    comboBox.Dispatcher.Invoke(() =>
            //    {
            //        var array = fast.Take(10).ToArray();
            //        comboBox.ItemsSource = array;

            //    });
            //});
            //comboBox.ItemsSource = results.Take(10).ToArray();


        }
    }
}
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
using Microsoft.CodeAnalysis;


namespace WPF.ComboBoxes.Roslyn
{

    class Paraphernalia
    {
        public AsyncIntelliSenseEngine asyncEngine;
        public TelemetryTracker telemetry;
        public MruTracker mru;
    }
    /// <summary>
    /// Attached behavior for ComboBox that enables filtering with IntelliSense-style sorting
    /// </summary>
    public partial class FilteringBehavior
    {
        static Dictionary<ComboBox, Paraphernalia> pairs = new();

        private static readonly DependencyProperty IsUpdatingTextProperty =
            DependencyProperty.RegisterAttached(
                "IsUpdatingText",
                typeof(bool),
                typeof(FilteringBehavior),
                new PropertyMetadata(false));

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.RegisterAttached(
                "SelectedItem",
                typeof(object),
                typeof(FilteringBehavior),
                new FrameworkPropertyMetadata(changed));

        public static object GetSelectedItem(DependencyObject obj)
        {
            return (object)obj.GetValue(SelectedItemProperty);
        }

        public static void SetSelectedItem(DependencyObject obj, int value)
        {
            obj.SetValue(SelectedItemProperty, value);
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached(
                "Source",
                typeof(IEnumerable),
                typeof(FilteringBehavior),
                new PropertyMetadata(null, sourceChanged));

        public static IEnumerable GetSource(DependencyObject obj)
        {
            return (IEnumerable)obj.GetValue(SourceProperty);
        }

        public static void SetSource(DependencyObject obj, IEnumerable value)
        {
            obj.SetValue(SourceProperty, value);
        }

        private static readonly DependencyProperty FilterTimerProperty =
            DependencyProperty.RegisterAttached(
                "FilterTimer",
                typeof(DispatcherTimer),
                typeof(FilteringBehavior),
                new PropertyMetadata(null));

        public static readonly DependencyProperty SearchTextProperty =
     DependencyProperty.RegisterAttached(
         "SearchText",
         typeof(string),
         typeof(FilteringBehavior),
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
                typeof(FilteringBehavior),
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

        public static readonly DependencyProperty IsDebuggingProperty =
            DependencyProperty.RegisterAttached(
        "IsDebugging",
        typeof(bool),
        typeof(FilteringBehavior),
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
        typeof(FilteringBehavior),
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
        typeof(FilteringBehavior),
        new PropertyMetadata(0d));

        public static double GetProgress(DependencyObject obj)
        {
            return (double)obj.GetValue(ProgressProperty);
        }

        public static void SetProgress(DependencyObject obj, double value)
        {
            obj.SetValue(ProgressProperty, value);
        }

        private static async void sourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ComboBox comboBox && e.NewValue is IEnumerable objects)
            {
                List<IndexedSymbol> symbols = new List<IndexedSymbol>();
                var count = objects.Count();
                var paraphernalia = new Paraphernalia();
                pairs.Add(comboBox, paraphernalia);
                IProgress<double> progress = new Progress<double>(p =>
                {

                    paraphernalia.telemetry = new TelemetryTracker();
                    paraphernalia.mru = new MruTracker();
                    var session = new IntelliSenseSession(symbols.ToArray(), paraphernalia.mru, paraphernalia.telemetry);
                    paraphernalia.asyncEngine = new AsyncIntelliSenseEngine(session, new AsyncRankingController());
                    SetProgress(comboBox, p);
                });

                await Task.Run(() =>
                {
                    foreach (ISymbol item in objects)
                    {
                        var text = item?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                        var token = new PatternToken(text);
                        var kind = IntelliSenseSymbolKind.Type;
                        var fullname = text;
                        symbols.Add(new IndexedSymbol(item, token, kind, fullname));

                        if (symbols.Count % 100 == 0 || symbols.Count == count)
                        {
                            progress.Report(100.0 * symbols.Count / count);
                        }
                    }
                });

                setupFiltering(comboBox, findTextBox(comboBox));
                setupHoverTracking(comboBox);

            }
        }


        #region Event Handlers


        private static void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                SetIndex(comboBox, 0);
                comboBox.SetValue(IsUpdatingTextProperty, true);
                if (e.AddedItems.Count > 0)
                    // Set flag to ignore the TextChanged event that happens when selection updates the text    
                    if (e.AddedItems[0] is IntelliSenseResult { Symbol.Item: ISymbol symbol } result)
                    {
                        comboBox.SetValue(FilteringBehavior.SelectedItemProperty, result);
                        findTextBox(comboBox).Text = symbol?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                        SetIndex(comboBox, comboBox.SelectedIndex);
                        var paraphernalia = pairs[comboBox];
                        paraphernalia.telemetry.MarkUsed(symbol);
                        paraphernalia.mru.MarkUsed(symbol);

                        if (GetIsSelectionSecondOrder(comboBox))
                        {
                            if (symbol is IMethodSymbol methodSymbol)
                            {
                                comboBox.ItemsSource = methodSymbol.Parameters;
                                comboBox.IsDropDownOpen = true;
                            }
                            if (symbol is ITypeSymbol typeSymbol)
                            {
                                comboBox.ItemsSource = typeSymbol.AllInterfaces;
                                comboBox.IsDropDownOpen = true;
                            }
                        }
                        else
                        {
                            findPopup(comboBox).IsOpen = false;
                        }
                        comboBox.SetValue(IsUpdatingTextProperty, false);
                    }
                    else if (e.AddedItems[0] is ISymbol _symbol)
                    {
                        comboBox.SetValue(FilteringBehavior.SelectedItemProperty, _symbol);
                        findPopup(comboBox).IsOpen = false;
                        comboBox.SetValue(IsUpdatingTextProperty, false);
                    }
                    else
                        throw new Exception("FilteringBehavior only supports items of type ITypeSpecifier.");
                else
                {
                    comboBox.SetValue(IsUpdatingTextProperty, false);
                }
            }
            else
                throw new Exception("ds 3 s");
        }

        private static void ComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                // Open dropdown on focus
                if (!comboBox.IsDropDownOpen)
                {
                    //comboBox.IsDropDownOpen = true;
                }

                // Select all text for easy replacement
                var textBox = findTextBox(comboBox);
                if (textBox != null)
                {

                    //textBox.SelectAll();
                }
            }
        }

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
                //comboBox.DropDownClosed += closedHandler;


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

            SetIndex(comboBox, 0);

            string searchText = textBox.Text ?? string.Empty;
            //string filterProperty = GetFilterProperty(comboBox);

            SetSearchText(comboBox, searchText);

            var results = await pairs[comboBox].asyncEngine.UpdateAsync(searchText, fast =>
            {
                comboBox.Dispatcher.Invoke(() =>
                {
                    var array = fast.Take(10).ToArray();
                    comboBox.ItemsSource = array;
               
                });
            });
            comboBox.ItemsSource = results.Take(10).ToArray();

            if (!string.IsNullOrEmpty(searchText))
            {
                comboBox.IsDropDownOpen = true;
            }
        }
    }
}
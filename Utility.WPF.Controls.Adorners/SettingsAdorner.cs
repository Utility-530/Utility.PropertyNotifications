using RandomColorGenerator;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Utility.WPF.Adorners;
using Utility.WPF.Adorners.Infrastructure;
using Utility.WPF.Helper;
using static Utility.WPF.Controls.Adorners.SettingsControl;
using Type = Utility.WPF.Adorners.Type;
using System.Reactive.Linq;
using System.Reactive.Disposables;

namespace Utility.WPF.Controls.Adorners
{
    public class SettingsAdorner : FrameworkElementAdorner<SettingsControl>
    {
        private ControlColourer? controlColourer;


        /// <summary>
        /// The internal encapsulating adorner
        /// </summary>
        //public static readonly DependencyProperty AdornerProperty =
        //    DependencyProperty.RegisterAttached("Adorner", typeof(object), typeof(SettingsAdorner), new PropertyMetadata(Changed));

        //private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (d is FrameworkElement element)
        //    {
        //        AddTo(element);
        //    }
        //    //if (e.NewValue is SettingsAdorner)
        //    //{
        //    //    d.SetValue(AdornerEx.IsEnabledProperty, true);
        //    //}
        //}


        private SettingsAdorner(FrameworkElement adornedElement) : base(adornedElement)
        {
        }

        public static SettingsAdorner AddTo(FrameworkElement adornedElement)
        {
            if (adornedElement.Adorners()?.OfType<SettingsAdorner>().Any() == true)
            {
                throw new Exception($"{adornedElement.GetType().Name} ({adornedElement.Name}) has a {nameof(SettingsAdorner)} already");
            }
            var settingsAdorner = new SettingsAdorner(adornedElement);
            adornedElement.SetValue(AdornerEx.AdornerProperty, settingsAdorner);
            adornedElement.SetValue(AdornerEx.IsEnabledProperty, true);
            adornedElement.AddIfMissingAdorner(new SettingsControl());
            return settingsAdorner;
        }

        //public static object GetAdorner(DependencyObject d)
        //{
        //    return d.GetValue(AdornerProperty);
        //}

        //public static void SetAdorner(DependencyObject d, object value)
        //{
        //    d.SetValue(AdornerProperty, value);
        //}


        protected override IDisposable SetAdornedElement(SettingsControl settingsControl, FrameworkElement? adornedElement)
        {
            adornedElement.SetValue(Type.ShowDataContextProperty, true);

            var disposable = settingsControl
                           .WhenAnyValue(a => a.SelectedDock)
                           .Subscribe(dock =>
                           {
                               adornedElement.SetValue(Text.PositionProperty, dock);
                           });

            var dis2 = Observable
                .FromEventPattern<CheckedRoutedEventHandler, CheckedEventArgs>(a => settingsControl.Checked += a, a => settingsControl.Checked -= a)
                .Select(a => a.EventArgs)
                .Subscribe(SettingsControl_Checked);

            return new CompositeDisposable(disposable, dis2);

            void SettingsControl_Checked(CheckedEventArgs e)
            {
                //AdornedElement.SetValue(Type.IsRecursiveProperty, e.IsChecked);

                switch (e.CheckedType)
                {
                    case CheckedType.DataContext:
                        AdornedElement.SetValue(Type.ShowDataContextProperty, e.IsChecked);

                        break;

                    case CheckedType.Dimensions:
                        AdornedElement.SetValue(Type.ShowDimensionsProperty, e.IsChecked);
                        break;

                    case CheckedType.HighlightArea:
                        controlColourer ??= new(adornedElement);
                        //AdornedElement.SetValue(Type.HighlightColourProperty, e.IsChecked);
                        if (e.IsChecked)
                            controlColourer.Apply();
                        else
                            controlColourer.Remove();
                        break;
                }
            }
        }

        internal class ControlColourer
        {
            public static readonly DependencyProperty KeyProperty = DependencyProperty.RegisterAttached("Key", typeof(Guid), typeof(ControlColourer));

            public static Guid GetKey(DependencyObject d)
            {
                return (Guid)d.GetValue(KeyProperty);
            }

            public static void SetKey(DependencyObject d, Guid value)
            {
                d.SetValue(KeyProperty, value);
            }

            private readonly DependencyObject dependencyObject;
            private readonly Dictionary<Guid, Brush> originalBrushes = new();
            private readonly Dictionary<Guid, Brush> tempBrushes = new();

            public ControlColourer(DependencyObject dependencyObject)
            {
                this.dependencyObject = dependencyObject;
            }

            public void Apply()
            {
                foreach (FrameworkElement child in dependencyObject.FindChildren<FrameworkElement>().Prepend(dependencyObject))
                {
                    Guid guid = GetKey(child) == default ? Guid.NewGuid() : GetKey(child);
                    Brush? background = default;
                    if (child is Control control)
                    {
                        background = ApplyBrush(guid, () => control.Background, b => control.Background = b);
                    }
                    else if (child is Panel panel)
                    {
                        background = ApplyBrush(guid, () => panel.Background, b => panel.Background = b);
                    }
                    else if (child is Shape shape)
                    {
                        background = ApplyBrush(guid, () => shape.Fill, b => shape.Fill = b);
                    }
                    else
                    {
                        return;
                    }

                    child.SetValue(KeyProperty, guid);
                    originalBrushes[guid] = background;
                }

                Brush ApplyBrush(Guid guid, Func<Brush> func, Action<Brush> action)
                {
                    Brush background = func();
                    action(tempBrushes[guid] = Brush(guid));
                    return background;

                    Brush Brush(Guid guid)
                    {
                        return tempBrushes.ContainsKey(guid) ?
                            tempBrushes[guid] :
                            RandomColor.GetColor(ColorScheme.Random, Luminosity.Light)
                                       .ToMediaBrush()
                                       .WithOpacity(0.5);
                    }
                }

            }

            public void Remove()
            {
                if (tempBrushes.Any())
                {
                    foreach (FrameworkElement child in dependencyObject.FindChildren<FrameworkElement>().Prepend(dependencyObject))
                    {
                        Guid guid = GetKey(child) == default ? Guid.NewGuid() : GetKey(child);
                        if (originalBrushes.ContainsKey(guid))
                        {
                            if (child is Control control)
                            {
                                control.Background = originalBrushes[guid];
                            }
                            else if (child is Panel panel)
                            {
                                panel.Background = originalBrushes[guid];
                            }
                            else if (child is Shape shape)
                            {
                                shape.Fill = originalBrushes[guid];
                            }
                            else
                            {
                                throw new Exception("REG34 hdfgghfd");
                            }
                        }
                        else
                        {
                            // child's probably been removed
                        }
                    }
                }
            }
        }
    }
}
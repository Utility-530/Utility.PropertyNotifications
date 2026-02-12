using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.CodeAnalysis;
using Microsoft.Xaml.Behaviors;
using Utility.PatternMatchings;
using Utility.Roslyn;

namespace Utility.WPF.ComboBoxes.Roslyn.Infrastructure
{
    public class CheckedArgs:EventArgs
    {
        public readonly Dictionary<string, bool> isChecked;

        public CheckedArgs(Dictionary<string, bool> isChecked)
        {
            this.isChecked = isChecked;
        }
    }

    public delegate void CheckedChangesEventHandler(
    object sender,
    CheckedChangesEventArgs e);

    public class CheckedChangesEventArgs : RoutedEventArgs
    {
        public CheckedChangesEventArgs(RoutedEvent routedEvent, string name, bool isChecked, IDictionary<string, bool> dictionary) : base(routedEvent)
        {
            Name = name;
            IsChecked = isChecked;
            Dictionary = dictionary;
        }
        public IReadOnlyDictionary<string, bool> Changes { get; }
        public string Name { get; }
        public bool IsChecked { get; }
        public IDictionary<string, bool> Dictionary { get; }
    }

    public static class CheckedEvents
    {
        public static readonly RoutedEvent CheckedChangesEvent =
            EventManager.RegisterRoutedEvent(
                "CheckedChanges",
                RoutingStrategy.Bubble,
                typeof(CheckedChangesEventHandler),
                typeof(CheckedEvents));

        public static void AddCheckedChangesHandler(
            DependencyObject d,
            CheckedChangesEventHandler handler)
        {
            if (d is UIElement u)
                u.AddHandler(CheckedChangesEvent, handler);
        }

        public static void RemoveCheckedChangesHandler(
            DependencyObject d,
            CheckedChangesEventHandler handler)
        {
            if (d is UIElement u)
                u.RemoveHandler(CheckedChangesEvent, handler);
        }
        // Helper to raise the event
        public static void RaiseEvent(UIElement source, string fullName, bool isChecked, IDictionary<string, bool> dictionary)
        {
            source.RaiseEvent(new CheckedChangesEventArgs(CheckedChangesEvent, fullName, isChecked, dictionary));
        }
    }

    public class ReflectionBehavior : Behavior<ComboBox>
    {
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register(nameof(Type), typeof(Type), typeof(ReflectionBehavior), new PropertyMetadata(null));

        public static readonly DependencyProperty AccessibilityProperty =
            DependencyProperty.Register(nameof(Microsoft.CodeAnalysis.Accessibility), typeof(Microsoft.CodeAnalysis.Accessibility), typeof(ReflectionBehavior), new PropertyMetadata(Microsoft.CodeAnalysis.Accessibility.Public));

        public static readonly DependencyProperty FilterTypeProperty =
     DependencyProperty.Register(nameof(FilterType), typeof(Type), typeof(ReflectionBehavior), new PropertyMetadata());

        private readonly TelemetryTracker telemetry = new();
        private Input[] collection;
        private MruTracker mru = new();
        private Session session;
        public readonly Dictionary<string, bool> @checked = [];

        public Type FilterType
        {
            get { return (Type)GetValue(FilterTypeProperty); }
            set { SetValue(FilterTypeProperty, value); }
        }

        public Type Type
        {
            get { return (Type)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public Microsoft.CodeAnalysis.Accessibility Accessibility
        {
            get { return (Microsoft.CodeAnalysis.Accessibility)GetValue(AccessibilityProperty); }
            set { SetValue(AccessibilityProperty, value); }
        }

        protected override void OnAttached()
        {
            FilterBehavior.SetConverter(AssociatedObject, new SymbolStringConverter());

            AssociatedObject.DropDownOpened += (s, e) =>
            {
                if (AssociatedObject.ItemsSource is null)
                {
                    initialise();
                }
            };

            AssociatedObject.DropDownClosed += (s, e) =>
            {
            };

            AssociatedObject.SelectionChanged += (s, e) =>
            {
            };

            var dpd = DependencyPropertyDescriptor.FromProperty(FilterBehavior.SearchTextProperty, typeof(UIElement));
            dpd.AddValueChanged(AssociatedObject, OnAttachedPropertyChanged);

            base.OnAttached();
        }


        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == TypeProperty || e.Property == AccessibilityProperty || e.Property == FilterTypeProperty)
            {
                if (AssociatedObject != null)
                    initialise();
            }

            base.OnPropertyChanged(e);
        }

        void initialise()
        {
            collection = Type
                .GetProperties(Accessibility.ToBindingFlags() | System.Reflection.BindingFlags.Instance)
                .Where(a => FilterType != null ? a.PropertyType.IsAssignableTo(FilterType) : true)
                .Select(item => new Input(item, new PatternToken(item.Name), (int)0, item.Name))
                .ToArray();

            session = new Session(collection, mru, telemetry, x => 1);
            updateSession();
        }

        List<PatternMatchings.Result> results = new();
        private async void OnAttachedPropertyChanged(object? sender, EventArgs e)
        {
            updateSession();
        }

        void updateSession()
        {
            foreach (var result in results)
            {
                result.PropertyChanged -= update;
            }
            results = session.Update(FilterBehavior.GetSearchText(AssociatedObject));
            foreach (var result in results)
            {
                if(@checked.TryGetValue(result.Symbol.FullName, out var isChecked))
                {
                    result.IsChecked = isChecked;
                }
                result.PropertyChanged += update;
            }
            AssociatedObject.ItemsSource = results;
        }

        void update(object obj, PropertyChangedEventArgs e)
        {
            if (obj is PatternMatchings.Result result && 
                e.PropertyName == nameof(PatternMatchings.Result.IsChecked))
            {
                @checked[result.Symbol.FullName] = result.IsChecked;
                CheckedEvents.RaiseEvent(AssociatedObject, result.Symbol.FullName, result.IsChecked, @checked);
            }

        }
    }
}

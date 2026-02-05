using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.CodeAnalysis;
using Microsoft.Xaml.Behaviors;
using Utility.PatternMatchings;
using Utility.Roslyn;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Utility.WPF.ComboBoxes.Roslyn.Infrastructure
{
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
                    update();
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
                    update();
            }

            base.OnPropertyChanged(e);
        }

        void update()
        {
            collection = Type
                .GetProperties(Accessibility.ToBindingFlags() | System.Reflection.BindingFlags.Instance)
                .Where(a => FilterType != null ? a.PropertyType.IsAssignableTo(FilterType) : true)
                .Select(item => new Input(item, new PatternToken(item.Name), (int)0, item.Name))
                .ToArray();

            session = new Session(collection, mru, telemetry, x => 1);
            AssociatedObject.ItemsSource = session.Update(FilterBehavior.GetSearchText(AssociatedObject));
        }

        private async void OnAttachedPropertyChanged(object? sender, EventArgs e)
        {
            AssociatedObject.ItemsSource = session.Update(FilterBehavior.GetSearchText(AssociatedObject));
        }
    }
}

using System;
using System.Collections;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using Evan.Wpf;
using Utility.WPF.Reactives;
using Utility.WPF.Behaviors;
using Utility.WPF.Controls.Base;

namespace Utility.WPF.Controls.Hybrid
{
    public class MeasurementsControl : SelectorAndContentControl
    {
        public static readonly DependencyProperty UnitProperty = DependencyProperty.Register("Unit", typeof(Type), typeof(MeasurementsControl));
        public static readonly DependencyProperty DisplayKeyPathProperty = NumbersControl.DisplayKeyPathProperty.AddOwner(typeof(MeasurementsControl));
        public static readonly DependencyProperty DisplayMemberPathProperty = ItemsControl.DisplayMemberPathProperty.AddOwner(typeof(MeasurementsControl));
        public static readonly DependencyProperty EnumFilterCollectionProperty = EnumSelectorBehavior.EnumFilterCollectionProperty.AddOwner(typeof(MeasurementsControl));
        public static readonly DependencyProperty SelectedUnitProperty = DependencyHelper.Register<Enum>();

        static MeasurementsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MeasurementsControl), new FrameworkPropertyMetadata(typeof(MeasurementsControl)));
        }

        public MeasurementsControl()
        {
            this.Observe(a => a.DisplayMemberPath).CombineLatest(this.Observe(a => a.DisplayKeyPath))
                 .Subscribe(a =>
                 {
                     var xx = new NumbersControl
                     {
                         DisplayMemberPath = a.First,
                         DisplayKeyPath = a.Second
                     };
                     this.Content = xx;
                 });
        }

        #region properties

        public string DisplayKeyPath
        {
            get => (string)GetValue(DisplayKeyPathProperty);
            set => SetValue(DisplayKeyPathProperty, value);
        }

        public string DisplayMemberPath
        {
            get => (string)GetValue(DisplayMemberPathProperty);
            set => SetValue(DisplayMemberPathProperty, value);
        }

        public Type Unit
        {
            get => (Type)GetValue(UnitProperty);
            set => SetValue(UnitProperty, value);
        }

        public IEnumerable EnumFilterCollection
        {
            get => (IEnumerable)GetValue(EnumFilterCollectionProperty);
            set => SetValue(EnumFilterCollectionProperty, value);
        }

        public Enum SelectedUnit
        {
            get => (Enum)GetValue(SelectedUnitProperty);
            set => SetValue(SelectedUnitProperty, value);
        }

        #endregion properties
    }
}
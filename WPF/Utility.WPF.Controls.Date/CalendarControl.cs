using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.WPF.Controls.Buttons;
using Utility.WPF.Controls.Date.Model;

namespace Utility.WPF.Controls.Date
{
    public class CalendarControl : Control
    {
        public static readonly DependencyProperty ItemsSourceProperty = ItemsControl.ItemsSourceProperty.AddOwner(typeof(CalendarControl));
        public static readonly DependencyProperty SelectedItemProperty = ListBox.SelectedItemProperty.AddOwner(typeof(CalendarControl));
        public static readonly RoutedEvent SelectionChangedEvent = ListBox.SelectionChangedEvent.AddOwner(typeof(CalendarControl));
        public static readonly DependencyProperty ValueConverterProperty = MonthControl.ValueConverterProperty.AddOwner(typeof(CalendarControl), new FrameworkPropertyMetadata(null, Changed));

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CalendarControl { monthControl: { } md } control)
            {
                md.ValueConverter = (IValueConverter)e.NewValue;
            }
        }

        static CalendarControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarControl), new FrameworkPropertyMetadata(typeof(CalendarControl)));
        }

        private DateModel dateModel;
        private MonthControl? monthControl;
        private Grid monthGrid;
        private DualButtonControl dualButtonControl;
        private DatePickerControl datePickerControl;

        public override void OnApplyTemplate()
        {
            monthControl = this.GetTemplateChild("MonthControl") as MonthControl;
            dateModel = monthControl is MonthGridControl ? new DateMonthModel() : new DateRangeModel();
            datePickerControl = this.GetTemplateChild("DatePickerControl") as DatePickerControl;
            dualButtonControl = this.GetTemplateChild("DualButtonControl") as DualButtonControl;
            monthGrid = this.GetTemplateChild("MonthGrid") as Grid;

            SetGridType((GridType)dualButtonControl.ValueToKey());
            datePickerControl.Month = dateModel.Month;
            datePickerControl.Year = dateModel.Year;

            //if (dualButtonControl.IsLoaded)
            //{
            //sdg();
            //}
            //else
            //{
            //    dualButtonControl.Loaded += (_, _) => sdg();
            //}

            dualButtonControl.ButtonToggle += DualButtonControl_ButtonToggle;

            this.SelectedItem = monthControl.SelectedItem;
            datePickerControl.DateChange += DatePickerControl_DateChange;

            void sdg()
            {
                SetGridType((GridType)dualButtonControl.ValueToKey());
                datePickerControl.Month = dateModel.Month;
                datePickerControl.Year = dateModel.Year;

                //SetMonthControl(monthControl);
                //monthControl.ItemsSource = dateModel.Days;
                //monthControl.SelectedItem = dateModel.Current;
                // dateModel.PropertyChanged += DateModel_PropertyChanged;
            }

            base.OnApplyTemplate();

            void DatePickerControl_DateChange(object sender, DateChangeEventArgs e)
            {
                dateModel.Month = e.Month;
                dateModel.Year = e.Year;
                monthControl.ItemsSource = dateModel.Days;
                monthControl.SelectedItem = dateModel.Current;
            }
        }

        private void DualButtonControl_ButtonToggle(object sender, SwitchControl.ToggleEventArgs size)
        {
            SetGridType((GridType)size.Key);
        }

        private void SetGridType(GridType size)
        {
            switch (size)
            {
                case GridType.Grid:
                    {
                        if (monthControl is not MonthGridControl)
                        {
                            dateModel = new DateMonthModel();
                            SetMonthControl(new MonthGridControl());
                            SetUIContent();
                        }

                        break;
                    }
                case GridType.List:
                    {
                        if (monthControl is not MonthListControl)
                        {
                            dateModel = new DateRangeModel();
                            SetMonthControl(new MonthListControl());
                            SetUIContent();
                        }

                        break;
                    }
                default:
                    throw new Exception("DF££ GGGDc");
            }

            void SetUIContent()
            {
                monthGrid.Children.Clear();
                monthGrid.Children.Add(monthControl);
            }
        }

        private void SetMonthControl(MonthControl _monthControl)
        {
            if (monthControl != null)
                monthControl.SelectionChanged -= MonthGridControl_SelectionChanged;
            monthControl = _monthControl;
            monthControl.SelectionChanged += MonthGridControl_SelectionChanged;
            monthControl.ValueConverter = this.ValueConverter;
            monthControl.ItemsSource = dateModel.Days;
            monthControl.SelectedItem = dateModel.Current;

            void MonthGridControl_SelectionChanged(object sender, RoutedEventArgs e)
            {
                if (sender is MonthControl listBox)
                {
                    this.SelectedItem = listBox.SelectedItem;
                    var date = listBox.SelectedDate;
                    if (date.HasValue)
                        dateModel.Current = date.Value;
                    datePickerControl.Month = dateModel.Month;
                    datePickerControl.Year = dateModel.Year;
                }
            }
        }

        #region properties

        public IValueConverter ValueConverter
        {
            get => (IValueConverter)GetValue(ValueConverterProperty);
            set => SetValue(ValueConverterProperty, value);
        }

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public event RoutedEventHandler SelectionChanged
        {
            add => AddHandler(SelectionChangedEvent, value);
            remove => RemoveHandler(SelectionChangedEvent, value);
        }

        #endregion properties
    }

    public enum GridType
    {
        Grid, List
    }
}
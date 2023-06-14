using DateWork.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Controls.Date
{
    public class DayControl : ContentControl
    {
        public static readonly DependencyProperty DayProperty =
            DependencyProperty.Register("Day", typeof(DateTime), typeof(DayControl),
                new PropertyMetadata(DateTime.Now, new PropertyChangedCallback(OnDayPropertyChanged)));
        public static readonly DependencyProperty DayNameProperty =
            DependencyProperty.Register("DayName", typeof(string), typeof(DayControl),
                new PropertyMetadata(null, new PropertyChangedCallback(OnDayNamePropertyChanged)));
        public static readonly DependencyProperty MonthDayNameProperty =
            DependencyProperty.Register("MonthDayName", typeof(string), typeof(DayControl),
                new PropertyMetadata(null, new PropertyChangedCallback(OnMonthDayNamePropertyChanged)));
        public static readonly DependencyProperty NoteTextProperty =
            DependencyProperty.Register("NoteText", typeof(string), typeof(DayControl),
                new PropertyMetadata(null, new PropertyChangedCallback(OnNoteTextPropertyChanged)));
        public static readonly DependencyProperty CurrentProperty =
            DependencyProperty.Register("Current", typeof(DateTime), typeof(DayControl),
                new PropertyMetadata(DateTime.Now, new PropertyChangedCallback(OnCurrentPropertyChanged)));
        public static readonly DependencyProperty DayTypeProperty =
            DependencyProperty.Register("DayType", typeof(DayType), typeof(DayControl),
                new PropertyMetadata(DayType.CurrentMonth, new PropertyChangedCallback(OnDayTypePropertyChanged)));

        static DayControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DayControl), new FrameworkPropertyMetadata(typeof(DayControl)));
        }

        #region properties
        public DateTime Current
        {
            get { return (DateTime)GetValue(CurrentProperty); }
            set { SetValue(CurrentProperty, value); }
        }

        private static void OnCurrentPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is DayControl)
            {
                (obj as DayControl).OnCurrentValueChanged();
            }
        }

        public DateTime Day
        {
            get { return (DateTime)GetValue(DayProperty); }
            set { SetValue(DayProperty, value); }
        }

        private static void OnDayPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is DayControl)
            {
                (obj as DayControl).OnDayValueChanged();
            }
        }

        public string DayName
        {
            get { return (string)GetValue(DayNameProperty); }
            set { SetValue(DayNameProperty, value); }
        }

        private static void OnDayNamePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is DayControl dayControl)
            {
                dayControl.OnDayNameValueChanged();
            }
        }

        public string MonthDayName
        {
            get { return (string)GetValue(MonthDayNameProperty); }
            set { SetValue(MonthDayNameProperty, value); }
        }

        private static void OnMonthDayNamePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is DayControl)
            {
                (obj as DayControl).OnMonthDayNameValueChanged();
            }
        }

        public string NoteText
        {
            get { return (string)GetValue(NoteTextProperty); }
            set { SetValue(NoteTextProperty, value); }
        }



        public DayType DayType
        {
            get { return (DayType)GetValue(DayTypeProperty); }
            set { SetValue(DayTypeProperty, value); }
        }

        private static void OnDayTypePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is DayControl)
            {
                (obj as DayControl).OnDayTypeValueChanged();
            }
        }
        private static void OnNoteTextPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is DayControl)
            {
                (obj as DayControl).OnNoteTextValueChanged();
            }
        }

        #endregion

        private void RefreshMonthDayName()
        {
            MonthDayName = Day.ToString("MMMM");
        }

        protected void OnDayTypeValueChanged()
        {

        }

        private void RefreshBackground()
        {
            DayType = Day.ToDayType();
        }

        protected void OnNoteTextValueChanged()
        {

        }

        protected void OnCurrentValueChanged()
        {
            RefreshBackground();
        }

        protected void OnMonthDayNameValueChanged()
        {

        }

        protected void OnDayValueChanged()
        {
            RefreshDayName();
            RefreshMonthDayName();
            RefreshBackground();
        }

        protected void OnDayNameValueChanged()
        {

        }

        private void RefreshDayName()
        {
            DayName = Day.Day.ToString();
        }
    }
}
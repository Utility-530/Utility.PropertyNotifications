using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

using System.ComponentModel;
using Leepfrog.WpfFramework.Commands;

namespace Leepfrog.WpfFramework.Controls
{
    /// <summary>
    /// Interaction logic for Calendar.xaml
    /// </summary>

    public partial class Calendar : UserControl, INotifyPropertyChanged
    {

        public RelayCommand ChangeMonthCommand { get; private set; } 

        public Calendar()
        {
            ChangeMonthCommand = new RelayCommand((param) => changeMonth(Convert.ToInt16(param)));
            _weekCommencingDates = new ObservableCollection<DateTime>();
            // TODO: calculate number of rows, depending on size of control
            _weekCommencingDatesReadOnly = new ReadOnlyObservableCollection<DateTime>(_weekCommencingDates);
            InitializeComponent();
            ctlWeeks.LayoutUpdated += ctlWeeks_LayoutUpdated;
            DisplayMonth= DateTime.Now.Date;
        }
        private double _widthOfDay = 0.1;

        private void changeMonth(Int16 delta)
        {
            DisplayMonth = DisplayMonth.AddMonths(delta);
        }

        private void ctlWeeks_LayoutUpdated(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(checkLayout), System.Windows.Threading.DispatcherPriority.Normal, new object[] { });
        }


        public DateTime SelectedDateOnly
        {
            get { return SelectedDate.Date; }
            set
            {
                // preserve time
                var diff = value.Date - SelectedDate.Date;
                if (diff != TimeSpan.Zero)
                {
                    SelectedDate = SelectedDate.AddDays(diff.Days);
                }
            }
        }

        public DateTimeOffset SelectedDate
        {
            get { return (DateTimeOffset)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedDate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register("SelectedDate", typeof(DateTimeOffset), typeof(Calendar), new FrameworkPropertyMetadata(DateTimeOffset.Now, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedDate_Changed));

        private static void SelectedDate_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as Calendar).PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(nameof(SelectedDateOnly)));
        }

        public DateTime DisplayMonth
        {
            get
            {
                var endOfFirstWeek = DateTime.Today;
                var startOfFirstWeek = _weekCommencingDates.Skip(2);
                if (startOfFirstWeek.Any())
                {
                    endOfFirstWeek = startOfFirstWeek.First().AddDays(6);
                }
                return new DateTime(endOfFirstWeek.Year, endOfFirstWeek.Month, 1);
            }
            set
            {
                // get current top row
                var startOfNewMonth = new DateTime(value.Year, value.Month, 1);
                if (_widthOfDay <1)
                {
                    StartDate = startOfNewMonth;
                }
                else
                {
                    var startOfFirstWeek = _weekCommencingDates.Skip(2).FirstOrDefault();
                    var offsetWeeks = Math.Floor((startOfNewMonth - startOfFirstWeek).TotalDays / 7);
                    animateTo(-offsetWeeks * _widthOfDay);
                }
            }
        }
        public DateTime StartDate
        {
            get { return (DateTime)GetValue(StartDateProperty); }
            set { SetValue(StartDateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartDate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartDateProperty =
            DependencyProperty.Register("StartDate", typeof(DateTime), typeof(Calendar), new PropertyMetadata(DateTime.Now.AddDays(-7), startDate_Changed, startDate_Coerce));

        private static void startDate_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            sender.AddLog("CALENDAR", $"Start Date - {e.OldValue} => {e.NewValue}");
            //sender.CoerceValue(StartDateProperty);
            (sender as Calendar)._weekCommencingDates.Clear();
            (sender as Calendar)._weekCommencingDates.Add((DateTime)e.NewValue);
        }

        private static object startDate_Coerce(DependencyObject sender, object baseValue)
        {
            var date = (DateTime)baseValue;
            date = date.Date;
            if (date.DayOfWeek == DayOfWeek.Monday)
            {
                sender.AddLog("CALENDAR", $@"Start Date - coerce {date:dd/MM/yyyy} => {date:dd/MM/yyyy}");
                return date;
            }
            else
            {
                sender.AddLog("CALENDAR", $@"Start Date - coerce {date:dd/MM/yyyy} => {date.AddDays((int)DayOfWeek.Monday - (int)date.DayOfWeek):dd/MM/yyyy}");
                return date.AddDays((int)DayOfWeek.Monday - (int)date.DayOfWeek);
            }
        }

        private ObservableCollection<DateTime> _weekCommencingDates;
        private ReadOnlyObservableCollection<DateTime> _weekCommencingDatesReadOnly;
        public ReadOnlyObservableCollection<DateTime> WeekCommencingDates => _weekCommencingDatesReadOnly;


        private static string[] _daysOfWeek = { "MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN" };
        public static string[] DaysOfWeek => _daysOfWeek;
        private static int[] _dayOffsets = { 0, 1, 2, 3, 4, 5, 6 };
        public static int[] DayOffsets => _dayOffsets;

        private bool _isMouseDown = false;
        private bool _isDragging = false;
        private bool _isAnimating = false;
        private Point _startPoint;
        private List<Tuple<int, Point>> _points = new List<Tuple<int, Point>>();

        public event PropertyChangedEventHandler PropertyChanged;

        private void ItemsControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            stopAnimation();
            checkLayout();
            _isMouseDown = true;
            _startPoint = e.GetPosition(null);
            //Mouse.Capture(this);
        }

        private void stopAnimation()
        {
            _isAnimating = false;
            transOffset.Y += transAnimation.Y;
            transAnimation.BeginAnimation(TranslateTransform.YProperty, null);
            transAnimation.Y = 0;
        }

        private void animateTo(double finalPos)
        {
            stopAnimation();

            CompositionTarget.Rendering += compositionTarget_Rendering;

            // ANIMATE TO FINAL POS
            var ani = new DoubleAnimationUsingKeyFrames();
            ani.Duration = new Duration(TimeSpan.FromSeconds(1));
            ani.FillBehavior = FillBehavior.HoldEnd;
            ani.Completed += (s2, e2) => { stopAnimation(); checkLayout(); };
            ani.KeyFrames.Add(new EasingDoubleKeyFrame(finalPos, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.5)), new PowerEase() { Power = 2, EasingMode = EasingMode.EaseOut }));
            finalPos = Math.Round((transOffset.Y + finalPos) / _widthOfDay) * (_widthOfDay) - transOffset.Y;
            ani.KeyFrames.Add(new EasingDoubleKeyFrame(finalPos, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1)), new BackEase() { Amplitude = 0.5, EasingMode = EasingMode.EaseOut }));
            _isAnimating = true;
            transAnimation.BeginAnimation(TranslateTransform.YProperty, ani);

        }

        private void ItemsControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                (sender as UIElement).ReleaseMouseCapture();
                var distance = (_points.Last().Item2 - _points.First().Item2);
                var duration = (_points.Last().Item1 - _points.First().Item1);
                Vector speed = new Vector(0, 0);
                if (duration > 0)
                {
                    speed = distance / duration;
                }
                var finalPos = new Point(0, transAnimation.Y) + (speed * 200);

                animateTo(finalPos.Y);
            }
            else
            {
                // WE WEREN'T DRAGGING, SO LET'S TRY TO HIGHLIGHT A DATE!
                FrameworkElement hitTestResult = null;
                VisualTreeHelper.HitTest
                    (
                        ctlWeeks,
                        (target) =>
                        {
                            var element = target as FrameworkElement;
                            if (element.Name == "grdDay")
                            {
                                hitTestResult = element;
                                return HitTestFilterBehavior.Stop;
                            }
                            return HitTestFilterBehavior.Continue;
                        },
                        (target) =>
                        {
                            var element = target.VisualHit as FrameworkElement;
                            if (element.Name == "grdDay")
                            {
                                //hitTestResult = target;
                                return HitTestResultBehavior.Stop;
                            }
                            return HitTestResultBehavior.Continue;
                        },
                        new PointHitTestParameters
                            (
                                e.GetPosition(ctlWeeks)
                            )
                    );
                //-------------------------------------------------------------
                if ((hitTestResult != null))
                {
                    //-------------------------------------------------------------
                    SelectedDateOnly = (DateTime)(hitTestResult).DataContext;
                    //-------------------------------------------------------------
                }
                //-------------------------------------------------------------
            }
            //-------------------------------------------------------------
            _isDragging = false;
            _isMouseDown = false;
            //-------------------------------------------------------------
        }

        private void compositionTarget_Rendering(object sender, EventArgs e)
        {
            //-------------------------------------------------------------
            checkLayout();
            if (!_isAnimating)
            {
                CompositionTarget.Rendering -= compositionTarget_Rendering;
            }
            //-------------------------------------------------------------
        }

        private void ItemsControl_MouseMove(object sender, MouseEventArgs e)
        {
            var newPos = e.GetPosition(null);
            if ((_isMouseDown) && (!_isDragging))
            {
                if ((_startPoint - newPos).Length > 3)
                {
                    _isDragging = true;
                    _points = new List<Tuple<int, Point>>();
                    (sender as UIElement).CaptureMouse();
                }
            }
            if (_isDragging)
            {
                _points.Add(new Tuple<int, Point>(e.Timestamp, newPos));
                if (_points.Count > 5)
                {
                    _points.RemoveAt(0);
                }
                transAnimation.Y += (newPos.Y - _startPoint.Y);
                _startPoint = newPos;
                checkLayout();
            }
        }

        private void checkLayout()
        {
            var monthChanged = false;
            try
            {
                //if ( !_isAnimating )
                {
                    // LOOKING AT THIS PROC, IT CAUSES TIME PICKER TO STOP SCROLLING... WHY?!?!?!
                    // if bottom of first row is below top of control, we need to add new rows to the top
                    if (_weekCommencingDatesReadOnly.Count >= 1)
                    {
                        var element = (FrameworkElement)ctlWeeks.ItemContainerGenerator.ContainerFromIndex(0);
                        if (element == null)
                        {
                            return;
                        }
                        _widthOfDay = element.ActualHeight;
                        if (element.TranslatePoint(new Point(0, element.RenderSize.Height), grdContainer).Y >= 0)
                        {
                            var h = element.RenderSize.Height;
                            _weekCommencingDates.Insert(0, _weekCommencingDates[0].AddDays(-7));
                            transOffset.Y -= h;
                            monthChanged = true;
                            return;
                        }
                    }
                    // if bottom of 2nd row is above top of control, we need to remove rows from the top
                    if (_weekCommencingDatesReadOnly.Count > 1)
                    {
                        var element = (FrameworkElement)ctlWeeks.ItemContainerGenerator.ContainerFromIndex(1);
                        if (element.TranslatePoint(new Point(0, element.RenderSize.Height), grdContainer).Y < -10)
                        {
                            _weekCommencingDates.RemoveAt(0);
                            transOffset.Y += element.RenderSize.Height;
                            monthChanged = true;
                            return;
                        }
                    }
                }
                // if top of last row is above bottom of control, we need to add new rows to the bottom
                if (_weekCommencingDatesReadOnly.Count >= 1)
                {
                    var element = (UIElement)ctlWeeks.ItemContainerGenerator.ContainerFromIndex(_weekCommencingDatesReadOnly.Count - 1);
                    if (element.TranslatePoint(new Point(0, 0), grdContainer).Y <= grdContainer.RenderSize.Height)
                    {
                        _weekCommencingDates.Add(_weekCommencingDates.Last().AddDays(7));
                        return;
                    }
                }
                // if top of 2nd last row is below bottom of control, we need to remove rows from the bottom  
                if (_weekCommencingDatesReadOnly.Count > 1)
                {
                    var element = (UIElement)ctlWeeks.ItemContainerGenerator.ContainerFromIndex(_weekCommencingDatesReadOnly.Count - 2);
                    if (element.TranslatePoint(new Point(0, 0), grdContainer).Y > grdContainer.RenderSize.Height)
                    {
                        _weekCommencingDates.RemoveAt(_weekCommencingDates.Count - 1);
                        return;
                    }
                }
            }
            finally
            {
                if (monthChanged)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayMonth)));
                }
            }
        }
    }

    public class CalendarData
    {
        public CalendarData()
        {
            DisplayMonth = DateTime.Today;
            //StartDate = new DateTime(StartDate.Year, StartDate.Month, 1);
            SelectedDate = DateTime.Today.AddDays(1);
            _weekCommencingDates = new ObservableCollection<DateTime>();
            // TODO: calculate number of rows, depending on size of control
            _weekCommencingDates.Add(StartDate.AddDays(-7)); // hidden row off top of screen, but preloaded for when we scroll!
            _weekCommencingDates.Add(StartDate.AddDays(0));
            _weekCommencingDates.Add(StartDate.AddDays(7));
            _weekCommencingDates.Add(StartDate.AddDays(14));
            _weekCommencingDates.Add(StartDate.AddDays(21));
            _weekCommencingDates.Add(StartDate.AddDays(28));
            _weekCommencingDates.Add(StartDate.AddDays(35)); // hidden row off bottom of screen, but preloaded for when we scroll!
            _weekCommencingDatesReadOnly = new ReadOnlyObservableCollection<DateTime>(_weekCommencingDates);
        }
        private DateTime _startDate;
        public DateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }
        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set { _selectedDate = value; }
        }
        private ObservableCollection<DateTime> _weekCommencingDates;
        private ReadOnlyObservableCollection<DateTime> _weekCommencingDatesReadOnly;
        public ReadOnlyObservableCollection<DateTime> WeekCommencingDates => _weekCommencingDatesReadOnly;
        private static string[] _daysOfWeek = { "MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN" };
        public static string[] DaysOfWeek => _daysOfWeek;
        private static int[] _dayOffsets = { 0, 1, 2, 3, 4, 5, 6 };
        public static int[] DayOffsets => _dayOffsets;
        public DateTime DisplayMonth
        {
            get
            {
                var endOfFirstWeek = StartDate.AddDays(6);
                return new DateTime(endOfFirstWeek.Year, endOfFirstWeek.Month, 1);
            }
            set
            {
                var startDate = value;
                if (startDate.DayOfWeek != DayOfWeek.Monday)
                {
                    startDate.AddDays((int)DayOfWeek.Monday - (int)startDate.DayOfWeek);
                }
                StartDate = startDate;
            }
        }

    }
}

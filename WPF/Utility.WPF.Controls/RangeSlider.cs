using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Utility.WPF.Controls
{
    public class DayRangeSlider : RangeSlider
    {
        static DayRangeSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DayRangeSlider), new FrameworkPropertyMetadata(typeof(DayRangeSlider)));
        }
    }

    public class RangeSlider : Control
    {
        private FrameworkElement SliderContainer;
        private Thumb StartThumb, EndThumb;
        private FrameworkElement StartArea;
        private FrameworkElement EndArea;

        private enum SliderThumb
        {
            None,
            Start,
            End
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(RangeSlider),
            new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(RangeSlider),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty StartProperty = DependencyProperty.Register("Start", typeof(double), typeof(RangeSlider),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty EndProperty = DependencyProperty.Register("End", typeof(double), typeof(RangeSlider),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(RangeSlider),
            new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty IsMoveToPointEnabledProperty = DependencyProperty.Register("IsMoveToPointEnabled", typeof(bool), typeof(RangeSlider), new FrameworkPropertyMetadata(true));
        public static readonly DependencyProperty StartThumbToolTipProperty = DependencyProperty.Register("StartThumbToolTip", typeof(object), typeof(RangeSlider));
        public static readonly DependencyProperty EndThumbToolTipProperty = DependencyProperty.Register("EndThumbToolTip", typeof(object), typeof(RangeSlider));
        public static readonly DependencyProperty StartThumbStyleProperty = DependencyProperty.Register("StartThumbStyle", typeof(Style), typeof(RangeSlider));
        public static readonly DependencyProperty EndThumbStyleProperty = DependencyProperty.Register("EndThumbStyle", typeof(Style), typeof(RangeSlider));
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(RangeSlider));

        static RangeSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RangeSlider), new FrameworkPropertyMetadata(typeof(RangeSlider)));

            EventManager.RegisterClassHandler(typeof(RangeSlider), Thumb.DragStartedEvent, new DragStartedEventHandler(OnDragStartedEvent));
            EventManager.RegisterClassHandler(typeof(RangeSlider), Thumb.DragDeltaEvent, new DragDeltaEventHandler(OnThumbDragDelta));
            EventManager.RegisterClassHandler(typeof(RangeSlider), Thumb.DragCompletedEvent, new DragCompletedEventHandler(OnDragCompletedEvent));

            //Ticks
            GeneratedTicksProperty = DependencyProperty.Register("GeneratedTicks", typeof(DoubleCollection), typeof(RangeSlider), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
            TickLabelTemplateProperty = DependencyProperty.Register("TickLabelTemplate", typeof(DataTemplate), typeof(RangeSlider), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        }

        public RangeSlider()
        {
            CalculateTicks();
        }

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public double Start
        {
            get => (double)GetValue(StartProperty);
            set => SetValue(StartProperty, value);
        }

        public double End
        {
            get => (double)GetValue(EndProperty);
            set => SetValue(EndProperty, value);
        }

        public bool IsMoveToPointEnabled
        {
            get => (bool)GetValue(IsMoveToPointEnabledProperty);
            set => SetValue(IsMoveToPointEnabledProperty, value);
        }

        public object StartThumbToolTip
        {
            get => GetValue(StartThumbToolTipProperty);
            set => SetValue(StartThumbToolTipProperty, value);
        }

        public object EndThumbToolTip
        {
            get => GetValue(EndThumbToolTipProperty);
            set => SetValue(EndThumbToolTipProperty, value);
        }

        public Style StartThumbStyle
        {
            get => (Style)GetValue(StartThumbStyleProperty);
            set => SetValue(StartThumbStyleProperty, value);
        }

        public Style EndThumbStyle
        {
            get => (Style)GetValue(EndThumbStyleProperty);
            set => SetValue(EndThumbStyleProperty, value);
        }

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            SliderContainer = GetTemplateChild("PART_SliderContainer") as FrameworkElement;
            if (SliderContainer != null)
            {
                SliderContainer.PreviewMouseDown += ViewBox_PreviewMouseDown;
            }

            StartArea = GetTemplateChild("PART_StartArea") as FrameworkElement;
            EndArea = GetTemplateChild("PART_EndArea") as FrameworkElement;
            StartThumb = GetTemplateChild("PART_StartThumb") as Thumb;
            EndThumb = GetTemplateChild("PART_EndThumb") as Thumb;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var arrangeSize = base.ArrangeOverride(arrangeBounds);

            if (Maximum > Minimum && StartArea != null && EndArea != null)
            {
                var start = Math.Max(Minimum, Math.Min(Maximum, Start));
                var end = Math.Max(Minimum, Math.Min(Maximum, End));
                Rect rectStart, rectEnd;
                if (Orientation == Orientation.Horizontal)
                {
                    var viewportSize = SliderContainer != null ? SliderContainer.ActualWidth : arrangeBounds.Width;
                    var startPosition = (start - Minimum) / (Maximum - Minimum) * viewportSize;
                    var endPosition = (end - Minimum) / (Maximum - Minimum) * viewportSize;
                    rectStart = new Rect(0, 0, startPosition, arrangeBounds.Height);
                    rectEnd = new Rect(endPosition, 0, viewportSize - endPosition, arrangeBounds.Height);
                }
                else
                {
                    var viewportSize = SliderContainer != null ? SliderContainer.ActualHeight : arrangeBounds.Height;
                    var startPosition = (start - Minimum) / (Maximum - Minimum) * viewportSize;
                    var endPosition = (end - Minimum) / (Maximum - Minimum) * viewportSize;
                    rectStart = new Rect(0, 0, arrangeBounds.Width, startPosition);
                    rectEnd = new Rect(0, endPosition, arrangeBounds.Width, viewportSize - endPosition);
                }

                if (StartArea != null) StartArea.Arrange(rectStart);
                if (EndArea != null) EndArea.Arrange(rectEnd);
                if (StartThumb != null) StartThumb.Arrange(rectStart);
                if (EndThumb != null) EndThumb.Arrange(rectEnd);
            }

            return arrangeSize;
        }

        private void ViewBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsReadOnly && IsMoveToPointEnabled)
            {
                if (StartThumb != null && StartThumb.IsMouseOver || EndThumb != null && EndThumb.IsMouseOver)
                    return;

                var point = e.GetPosition(SliderContainer);
                if (e.ChangedButton == MouseButton.Left)
                {
                    MoveBlockTo(point, SliderThumb.Start);
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    MoveBlockTo(point, SliderThumb.End);
                }

                e.Handled = true;
            }
        }

        private void MoveBlockTo(Point point, SliderThumb block)
        {
            double position;
            if (Orientation == Orientation.Horizontal)
            {
                position = point.X;
            }
            else
            {
                position = point.Y;
            }

            double viewportSize = Orientation == Orientation.Horizontal ? SliderContainer.ActualWidth : SliderContainer.ActualHeight;
            if (!double.IsNaN(viewportSize) && viewportSize > 0)
            {
                var value = Math.Min(Maximum, Minimum + position / viewportSize * (Maximum - Minimum));
                if (block == SliderThumb.Start)
                {
                    Start = Math.Min(End, value);
                }
                else if (block == SliderThumb.End)
                {
                    End = Math.Max(Start, value);
                }
            }
        }

        private static void OnDragStartedEvent(object sender, DragStartedEventArgs e)
        {
            if (sender is RangeSlider rs)
            {
                rs.OnDragStartedEvent(e);
            }
        }

        private void OnDragStartedEvent(DragStartedEventArgs e)
        {
        }

        private static void OnThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is RangeSlider rs)
            {
                rs.OnThumbDragDelta(e);
            }
        }

        private void OnThumbDragDelta(DragDeltaEventArgs e)
        {
            if (!IsReadOnly && e.OriginalSource is Thumb thumb && SliderContainer != null)
            {
                double change;
                if (Orientation == Orientation.Horizontal)
                {
                    change = e.HorizontalChange / SliderContainer.ActualWidth * (Maximum - Minimum);
                }
                else
                {
                    change = e.VerticalChange / SliderContainer.ActualHeight * (Maximum - Minimum);
                }

                if (thumb == StartThumb)
                {
                    Start = Math.Max(Minimum, Math.Min(End, Start + change));
                }
                else if (thumb == EndThumb)
                {
                    End = Math.Min(Maximum, Math.Max(Start, End + change));
                }
            }
        }

        private static void OnDragCompletedEvent(object sender, DragCompletedEventArgs e)
        {
            if (sender is RangeSlider rs)
            {
                rs.OnDragCompletedEvent(e);
            }
        }

        private void OnDragCompletedEvent(DragCompletedEventArgs e)
        {
        }

        // Summary:
        //     Gets or sets the interval between tick marks.
        //
        // Returns:
        //     The distance between tick marks. The default is (1.0).
        [Bindable(true)]
        [Category("Appearance")]
        public double TickFrequency
        {
            get { return (double)GetValue(TickFrequencyProperty); }
            set { SetValue(TickFrequencyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TickFrequency.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TickFrequencyProperty =
            DependencyProperty.Register("TickFrequency", typeof(double), typeof(RangeSlider), new PropertyMetadata(1d));

        //public double TickFrequency { get; set; }
        //
        // Summary:
        //     Gets or sets the positions of the tick marks to display for a System.Windows.Controls.Slider.
        //
        // Returns:
        //     A set of tick marks to display for a System.Windows.Controls.Slider. The default
        //     is null.
        [Bindable(true)]
        [Category("Appearance")]
        public DoubleCollection Ticks
        {
            get { return (DoubleCollection)GetValue(TicksProperty); }
            set { SetValue(TicksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Ticks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TicksProperty =
            DependencyProperty.Register("Ticks", typeof(DoubleCollection), typeof(RangeSlider), new PropertyMetadata(null));

        // Ticks

        private string[] propertyNames = new string[] { "Minimum", "Maximum", "TickFrequency", "Ticks", "IsDirectionReversed" };
        public static readonly DependencyProperty GeneratedTicksProperty;
        public static readonly DependencyProperty TickLabelTemplateProperty;
        private object sync = new object();

        [Bindable(true)]
        public DoubleCollection GeneratedTicks
        {
            get => (DoubleCollection)GetValue(GeneratedTicksProperty);
            set => SetValue(GeneratedTicksProperty, value);
        }

        [Bindable(true)]
        public DataTemplate TickLabelTemplate
        {
            get => (DataTemplate)GetValue(TickLabelTemplateProperty);
            set => SetValue(TickLabelTemplateProperty, value);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (IsInitialized && propertyNames.Contains(e.Property.Name))
            {
                CalculateTicks();
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            CalculateTicks();
        }

        protected override Geometry? GetLayoutClip(Size layoutSlotSize)
        {
            return ClipToBounds ? base.GetLayoutClip(layoutSlotSize) : null;
        }

        private async void CalculateTicks()
        {
            double[]? ticks = null;
            double min;
            double max;
            double tickFrequency;
            lock (sync)
            {
                ticks = Ticks?.ToArray();
                min = Minimum;
                max = Maximum;
                tickFrequency = TickFrequency;
            }

            await Task.Run(async () =>
            {
                DoubleCollection c = null;
                try
                {
                    if (ticks != null && ticks.Any())
                    {
                        await Dispatcher.InvokeAsync(() =>
                        {
                            SetValue(GeneratedTicksProperty, new DoubleCollection(ticks.Union(new double[] { min, max }).Where(t => min <= t && t <= max)));
                        }, System.Windows.Threading.DispatcherPriority.Background);
                    }
                    else if (tickFrequency > 0.0)
                    {
                        long l = (long)Math.Ceiling((max - min) / tickFrequency);
                        double[] range = Enumerable.Range(0, (int)l + 1).Select(t => Math.Min(t * tickFrequency, max)).ToArray();
                        await Dispatcher.InvokeAsync(() =>
                        {
                            if (l <= int.MaxValue)
                            {
                                SetValue(GeneratedTicksProperty, new DoubleCollection(range));
                            }
                            //else
                            //{
                            //    this.GeneratedTicks = new DoubleCollection(Enumerable.Range(0, int.MaxValue).Select(t => Math.Min(t * tickFrequency + min, max)));
                            //}
                        }, System.Windows.Threading.DispatcherPriority.Background);
                    }
                }
                catch (Exception ex)
                {
                    await Dispatcher.InvokeAsync(() => MessageBox.Show(ex.Message));
                }
            });
        }
    }
}
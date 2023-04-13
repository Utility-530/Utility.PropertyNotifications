using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace Utility.WPF.Controls
{
    /// <summary>
    ///<a href="https://github.com/tdcosta100/SliderWithTickLabels">originally by tdcosta</a>
    /// </summary>
    public class SliderWithTickLabels : Slider
    {
        private string[] propertyNames = new string[] { "Minimum", "Maximum", "TickFrequency", "Ticks", "IsDirectionReversed" };
        public static readonly DependencyProperty GeneratedTicksProperty;
        public static readonly DependencyProperty TickLabelTemplateProperty;
        private object sync = new object();

        //[Bindable(true)]
        //public DoubleCollection GeneratedTicks
        //{
        //    get => base.GetValue(SliderWithTickLabels.GeneratedTicksProperty) as DoubleCollection;
        //    set => base.SetValue(SliderWithTickLabels.GeneratedTicksProperty, value);
        //}

        [Bindable(true)]
        public DataTemplate TickLabelTemplate
        {
            get => (DataTemplate)GetValue(TickLabelTemplateProperty);
            set => SetValue(TickLabelTemplateProperty, value);
        }

        static SliderWithTickLabels()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SliderWithTickLabels), new FrameworkPropertyMetadata(typeof(SliderWithTickLabels)));

            GeneratedTicksProperty = DependencyProperty.Register("GeneratedTicks", typeof(DoubleCollection), typeof(SliderWithTickLabels), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
            TickLabelTemplateProperty = DependencyProperty.Register("TickLabelTemplate", typeof(DataTemplate), typeof(SliderWithTickLabels), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        }

        public SliderWithTickLabels()
        {
            CalculateTicks();
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
                ticks = Ticks.Select(a => a).ToArray();
                min = Minimum;
                max = Maximum;
                tickFrequency = TickFrequency;
            }

            await Task.Run(async () =>
        {
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
                    long l = (long)Math.Ceiling((max - min) / tickFrequency) + 1;
                    double[] range = Enumerable.Range(0, (int)l).Select(t => Math.Min(t * tickFrequency + min, max)).ToArray();
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

    internal class TickBarLabelMarginConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var slider = values[0] as SliderWithTickLabels;
                var tickBar = slider.Template.FindName("TopTick", slider) as TickBar;

                var positionMinimum = tickBar.ReservedSpace / 2;

                double scalingValue = 0.0;
                double left = 0.0;
                double top = 0.0;

                switch (slider.Orientation)
                {
                    case Orientation.Horizontal:
                        scalingValue = (tickBar.ActualWidth - tickBar.ReservedSpace) / (tickBar.Maximum - tickBar.Minimum);
                        left = -(tickBar.ActualWidth / 2 - positionMinimum - scalingValue * (System.Convert.ToDouble(values[1]) - tickBar.Minimum)) * 2;
                        top = System.Convert.ToDouble(values[2]);
                        break;

                    case Orientation.Vertical:
                        scalingValue = (tickBar.ActualHeight - tickBar.ReservedSpace) / (tickBar.Maximum - tickBar.Minimum);
                        left = System.Convert.ToDouble(values[1]);
                        top = (tickBar.ActualHeight / 2 - positionMinimum - scalingValue * (System.Convert.ToDouble(values[2]) - tickBar.Minimum)) * 2;
                        break;

                    default:
                        break;
                }

                if (slider.IsDirectionReversed)
                {
                    left *= -1.0;
                    top *= -1.0;
                }

                var thickness = new Thickness
                {
                    Left = (left <= 0.0) ? left : 0,
                    Top = (top <= 0.0) ? top : 0,
                    Right = (left > 0.0) ? -left : System.Convert.ToDouble(values[3]),
                    Bottom = (top > 0.0) ? -top : System.Convert.ToDouble(values[4])
                };

                return thickness;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[]
            {
                null,
                ((Thickness)value).Left,
                ((Thickness)value).Top,
                ((Thickness)value).Right,
                ((Thickness)value).Bottom
            };
        }
    }
}
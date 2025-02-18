using System;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Utility.WPF.Controls.Handy
{
    /// <summary>
    /// Handy Control
    /// </summary>
    public class CompareSlider : Slider
    {
        private readonly ISubject<double> subject = new Subject<double>();

        public static readonly DependencyProperty TargetContentProperty = DependencyProperty.Register(
            "TargetContent", typeof(object), typeof(CompareSlider), new PropertyMetadata(default(object)));

        public static readonly DependencyProperty SourceContentProperty = DependencyProperty.Register(
            "SourceContent", typeof(object), typeof(CompareSlider), new PropertyMetadata(default(object)));

        public static readonly DependencyProperty DimensionProperty =
            DependencyProperty.Register("Dimension", typeof(double), typeof(CompareSlider), new PropertyMetadata(0d, propertyChangedCallback));

        static CompareSlider()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(CompareSlider), new FrameworkPropertyMetadata(typeof(CompareSlider)));
        }

        public CompareSlider()
        {
        }

        public object TargetContent
        {
            get => GetValue(TargetContentProperty);
            set => SetValue(TargetContentProperty, value);
        }

        public object SourceContent
        {
            get => GetValue(SourceContentProperty);
            set => SetValue(SourceContentProperty, value);
        }

        public double Dimension
        {
            get { return (double)GetValue(DimensionProperty); }
            set { SetValue(DimensionProperty, value); }
        }

        private static void propertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CompareSlider).subject.OnNext((double)(e.NewValue));
        }

        private void move()
        {
            //DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            //myDoubleAnimation.From = 50;
            //myDoubleAnimation.To = 300;
            //myDoubleAnimation.Duration =
            //    new Duration(TimeSpan.FromSeconds(10));
            //myDoubleAnimation.AutoReverse = true;
            //myDoubleAnimation.RepeatBehavior = RepeatBehavior.Forever;

            //Storyboard.SetTargetName(myDoubleAnimation, "Ln");
            //Storyboard.SetTargetProperty(myDoubleAnimation,
            //    new PropertyPath(Canvas.TopProperty));
            //Storyboard myStoryboard = new Storyboard();
            //myStoryboard.Children.Add(myDoubleAnimation);
            //myStoryboard.Begin(Ln);
        }
    }

    public class CompareTrack : Track
    {
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            base.ArrangeOverride(arrangeSize);

            var isVertical = Orientation == Orientation.Vertical;
            ComputeSliderLengths(arrangeSize, isVertical, out var decreaseButtonLength, out var thumbLength,
                out var increaseButtonLength);

            var offset = new Point();
            var pieceSize = arrangeSize;
            var isDirectionReversed = IsDirectionReversed;

            if (isVertical)
            {
                CoerceLength(ref decreaseButtonLength, arrangeSize.Height);
                CoerceLength(ref increaseButtonLength, arrangeSize.Height);
                CoerceLength(ref thumbLength, arrangeSize.Height);

                offset.Y = isDirectionReversed ? decreaseButtonLength + thumbLength : 0.0;
                pieceSize.Height = increaseButtonLength;

                IncreaseRepeatButton?.Arrange(new Rect(offset, pieceSize));

                offset.Y = isDirectionReversed ? 0.0 : increaseButtonLength + thumbLength;
                pieceSize.Height = decreaseButtonLength;

                if (DecreaseRepeatButton != null)
                {
                    DecreaseRepeatButton.Arrange(new Rect(offset, pieceSize));
                    DecreaseRepeatButton.Height = pieceSize.Height;
                }

                offset.Y = isDirectionReversed ? decreaseButtonLength : increaseButtonLength;
                pieceSize.Height = thumbLength;

                Thumb?.Arrange(new Rect(offset, pieceSize));
            }
            else
            {
                CoerceLength(ref decreaseButtonLength, arrangeSize.Width);
                CoerceLength(ref increaseButtonLength, arrangeSize.Width);
                CoerceLength(ref thumbLength, arrangeSize.Width);

                offset.X = isDirectionReversed ? increaseButtonLength + thumbLength : 0.0;
                pieceSize.Width = decreaseButtonLength;

                DecreaseRepeatButton?.Arrange(new Rect(offset, pieceSize));

                offset.X = isDirectionReversed ? 0.0 : decreaseButtonLength + thumbLength;
                pieceSize.Width = increaseButtonLength;

                if (IncreaseRepeatButton != null)
                {
                    IncreaseRepeatButton.Arrange(new Rect(offset, pieceSize));
                    IncreaseRepeatButton.Width = pieceSize.Width;
                }

                offset.X = isDirectionReversed ? increaseButtonLength : decreaseButtonLength;
                pieceSize.Width = thumbLength;

                Thumb?.Arrange(new Rect(offset, pieceSize));
            }

            return arrangeSize;
        }

        private void ComputeSliderLengths(Size arrangeSize, bool isVertical, out double decreaseButtonLength,
            out double thumbLength, out double increaseButtonLength)
        {
            var min = Minimum;
            var range = Math.Max(0.0, Maximum - min);
            var offset = Math.Min(range, Value - min);

            double trackLength;

            // Compute thumb size
            if (isVertical)
            {
                trackLength = arrangeSize.Height;
                thumbLength = Thumb?.DesiredSize.Height ?? 0;
            }
            else
            {
                trackLength = arrangeSize.Width;
                thumbLength = Thumb?.DesiredSize.Width ?? 0;
            }

            CoerceLength(ref thumbLength, trackLength);

            var remainingTrackLength = trackLength - thumbLength;

            decreaseButtonLength = remainingTrackLength * offset / range;
            CoerceLength(ref decreaseButtonLength, remainingTrackLength);

            increaseButtonLength = remainingTrackLength - decreaseButtonLength;
            CoerceLength(ref increaseButtonLength, remainingTrackLength);
        }

        private static void CoerceLength(ref double componentLength, double trackLength)
        {
            if (componentLength < 0)
                componentLength = 0.0;
            else if (componentLength > trackLength || double.IsNaN(componentLength))
                componentLength = trackLength;
        }
    }
}
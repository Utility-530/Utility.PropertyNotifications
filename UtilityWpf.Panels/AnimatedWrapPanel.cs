using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace UtilityWpf.Panels
{
    /// <summary>
    /// <a href="http://tech.pro/tutorial/736/wpf-tutorial-creating-a-custom-panel-control"/>
    /// </summary>
    public class AnimatedWrapPanel : Panel
    {
        private static readonly TimeSpan _animationLength = TimeSpan.FromMilliseconds(250);
        public static readonly DependencyProperty AnimationTimeProperty = DependencyProperty.Register("AnimationTime", typeof(TimeSpan), typeof(AnimatedWrapPanel), new PropertyMetadata(_animationLength));

        public TimeSpan AnimationTime
        {
            get { return (TimeSpan)GetValue(AnimationTimeProperty); }
            set { SetValue(AnimationTimeProperty, value); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var infiniteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            double curX = 0, curY = 0, curLineHeight = 0;
            foreach (UIElement child in Children)
            {
                child.Measure(infiniteSize);

                if (curX + child.DesiredSize.Width > availableSize.Width)
                { //Wrap to next line
                    curY += curLineHeight;
                    curX = 0;
                    curLineHeight = 0;
                }

                curX += child.DesiredSize.Width;
                if (child.DesiredSize.Height > curLineHeight)
                    curLineHeight = child.DesiredSize.Height;
            }

            curY += curLineHeight;

            var resultSize = new Size
            {
                Width = double.IsPositiveInfinity(availableSize.Width)
                    ? curX
                    : availableSize.Width,
                Height = double.IsPositiveInfinity(availableSize.Height)
                    ? curY
                    : availableSize.Height
            };

            return resultSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.Children == null || this.Children.Count == 0)
                return finalSize;

            double curX = 0, curY = 0, curLineHeight = 0;

            foreach (UIElement child in Children)
            {
                if (curX + child.DesiredSize.Width > finalSize.Width)
                { //Wrap to next line
                    curY += curLineHeight;
                    curX = 0;
                    curLineHeight = 0;
                }

                child.Arrange(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));

                AnimationHelper.AnimatePosition(child, curX, curY, AnimationTime);

                                curX += child.DesiredSize.Width;
                if (child.DesiredSize.Height > curLineHeight)
                    curLineHeight = child.DesiredSize.Height;
            }

            return finalSize;
        }
    }
}
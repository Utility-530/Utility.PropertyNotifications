using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Linq;

namespace Leepfrog.WpfFramework.Controls
{
    public class AlignableUniformGrid : UniformGrid
    {
        #region ORIENTATION PROPERTY

        public static readonly DependencyProperty OrientationProperty =
                DependencyProperty.Register("Orientation", typeof(System.Windows.Controls.Orientation), typeof(AlignableUniformGrid),
                    new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure)
                    );

        public System.Windows.Controls.Orientation Orientation
        {
            get { return (System.Windows.Controls.Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
        #endregion


        protected override Size MeasureOverride(Size constraint)
        {
            this.UpdateComputedValues();
            Size availableSize = new Size(constraint.Width / ((double)_columns), constraint.Height / ((double)_rows));
            double width = 0.0;
            double height = 0.0;
            foreach (UIElement element in base.InternalChildren)
            { 
                element.Measure(availableSize);
                Size desiredSize = element.DesiredSize;
                if (width < desiredSize.Width)
                {
                    width = desiredSize.Width;
                }
                if (height < desiredSize.Height)
                {
                    height = desiredSize.Height;
                }
            }
            return new Size(width * _columns, height * _rows);
        }



        private int _columns;
        private int _rows;

        private void UpdateComputedValues()
        {
            _columns = Columns;
            _rows = Rows;

            if ((_rows == 0) || (_columns == 0))
            {
                int visibleChildren = 0;                
                foreach (UIElement element in base.InternalChildren)
                {
                    if (element.Visibility != Visibility.Collapsed)
                    {
                        visibleChildren++;
                    }
                }

                if (visibleChildren == 0)
                {
                    visibleChildren = 1;
                }
                if ((_rows == 0) && (_columns == 0))
                {
                    _rows = (int)Math.Sqrt((double)visibleChildren);
                    if ((_rows * _rows) < visibleChildren)
                    {
                        _rows++;
                    }
                    _columns = _rows;
                }
                else if (_rows == 0)
                {
                    _rows = (visibleChildren  + _columns - 1) / _columns;
                }
                else if (_columns == 0)
                {
                    _columns = (visibleChildren + _rows - 1) / _rows;
                }
            }
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            Rect finalRect = new Rect(0.0, 0.0, arrangeSize.Width / ((double)_columns), arrangeSize.Height / ((double)_rows));
            double height = finalRect.Height;
            double numX = arrangeSize.Height - 1.0;
            foreach (UIElement element in base.InternalChildren)
            {
                element.Arrange(finalRect);
                if (element.Visibility != Visibility.Collapsed)
                {
                    finalRect.Y += height;
                    if (finalRect.Y >= numX)
                    {
                        finalRect.X += finalRect.Width;
                        finalRect.Y = 0.0;
                    }
                }
            }
            return arrangeSize;
        }
    }
}



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Leepfrog.WpfFramework.Controls
{
    public class SpinnerAdorner : Adorner
    {
        private FrameworkElement _child;

        public SpinnerAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            Child = new Spinner()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index != 0) throw new ArgumentOutOfRangeException();
            return _child;
        }

        public FrameworkElement Child
        {
            get { return _child; }
            set
            {
                if (_child != null)
                {
                    RemoveVisualChild(_child);
                    base.RemoveLogicalChild(_child);
                }
                _child = value;
                if (_child != null)
                {
                    AddVisualChild(_child);
                    base.AddLogicalChild(_child);
                }
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _child.Measure(constraint);
            return _child.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _child.Arrange(new Rect(new Point(0, 0), finalSize));
            return new Size(_child.ActualWidth, _child.ActualHeight);
        }
    }
}
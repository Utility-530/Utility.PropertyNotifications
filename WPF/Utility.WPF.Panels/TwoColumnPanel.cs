using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Panels
{
    public class TwoColumnPanel : Panel
    {
        private static FrameworkPropertyMetadata secondColumnMetadata = new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsParentArrange);

        public static readonly DependencyProperty SecondColumnProperty = DependencyProperty.RegisterAttached("SecondColumn", typeof(bool), typeof(TwoColumnPanel), secondColumnMetadata);

        public static void SetSecondColumn(DependencyObject depObj, bool value)
        {
            depObj.SetValue(SecondColumnProperty, value);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement elem in InternalChildren)
                elem.Measure(availableSize);

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double topColumn1 = 0.0;
            double topColumn2 = 0.0;

            for (int i = 0; i < InternalChildren.Count; i++)
            {
                bool col2 = (bool)InternalChildren[i].GetValue(SecondColumnProperty);

                double left = col2 ? finalSize.Width / 2.0 : 0.0;
                double top = col2 ? topColumn2 : topColumn1;

                Rect r = new Rect(new Point(left, top),
                                  InternalChildren[i].DesiredSize);

                InternalChildren[i].Arrange(r);

                if (col2)
                    topColumn2 += InternalChildren[i].DesiredSize.Height;
                else
                    topColumn1 += InternalChildren[i].DesiredSize.Height;
            }

            return finalSize;
        }
    }
}
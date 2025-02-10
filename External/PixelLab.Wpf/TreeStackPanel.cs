using System;
using System.Windows;
using System.Windows.Controls;
using PixelLab.Contracts;

#if CONTRACTS_FULL
using System.Diagnostics.Contracts;
#else
using PixelLab.Wpf;
#endif

namespace PixelLab.Wpf
{
    public class TreeStackPanel : AnimatingPanel
    {

        public static readonly DependencyProperty OrientationProperty =
            WrapPanel.OrientationProperty.AddOwner(typeof(TreeStackPanel), new PropertyMetadata(null));

        public static readonly DependencyProperty AreaProperty =
            DependencyProperty.RegisterAttached(
                "Area",
                typeof(double),
                typeof(TreeStackPanel),
                new FrameworkPropertyMetadata(
                    1.0,
                    FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public static double GetArea(DependencyObject element)
        {
            Contract.Requires<ArgumentException>(element != null);

            return (double)element.GetValue(AreaProperty);
        }

        public static void SetArea(DependencyObject element, double value)
        {
            Contract.Requires<ArgumentException>(element != null);

            element.SetValue(AreaProperty, value);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (finalSize.Width < c_tolerance || finalSize.Height < c_tolerance)
            {
                return finalSize;
            }

            UIElementCollection children = InternalChildren;
            ComputeWeightMap(children);

            Rect strip = new Rect(finalSize);
            double remainingWeight = m_totalWeight;

            int arranged = 0;
            //while (arranged < children.Count)
            //{
            double bestStripWeight = 0;

            int i;

            if (finalSize.Width < c_tolerance || finalSize.Height < c_tolerance)
            {
                return finalSize;
            }
            double sum = 0;
            foreach (var item in m_weights)
            {
                sum += item;
            }
            if (strip.Width > strip.Height || (Orientation?)this.GetValue(OrientationProperty)== Orientation.Horizontal)
            {
                double y = 0;
                // Arrange Vertically
                for (i = arranged; i < children.Count; i++)
                {
                    double height = strip.Height * m_weights[i] /  sum;
                    ArrangeChild(children[i], new Rect(strip.X, y, strip.Width, height));
                    y += height;
                }
            }
            else 
            {
                double x = 0;
                // Arrange Vertically
                for (i = arranged; i < children.Count; i++)
                {
                    double width = strip.Width * m_weights[i] / sum;
                    ArrangeChild(children[i], new Rect(x,strip.Y,width, strip.Height));
                    x += width;
                }
            }
            remainingWeight -= bestStripWeight;
            // }

            return finalSize;
        }

        private UIElement GetChild(UIElementCollection children, int index)
        {
            return children[m_weightMap[index]];
        }

        private double GetWeight(int index)
        {
            return m_weights[m_weightMap[index]];
        }

        private void ComputeWeightMap(UIElementCollection children)
        {
            m_totalWeight = 0;

            if (m_weightMap == null || m_weightMap.Length != InternalChildren.Count)
            {
                m_weightMap = new int[InternalChildren.Count];
            }

            if (m_weights == null || m_weights.Length != InternalChildren.Count)
            {
                m_weights = new double[InternalChildren.Count];
            }

            for (int i = 0; i < m_weightMap.Length; i++)
            {
                m_weightMap[i] = i;
                m_weights[i] = GetArea(children[i]);
                m_totalWeight += m_weights[i];
            }

            Array.Sort<int>(m_weightMap, compareWeights);
        }

        private int compareWeights(int index1, int index2)
        {
            return m_weights[index2].CompareTo(m_weights[index1]);
        }

        private double m_totalWeight;
        private int[] m_weightMap;
        private double[] m_weights;

        private const double c_tolerance = 1e-2;
    }
}


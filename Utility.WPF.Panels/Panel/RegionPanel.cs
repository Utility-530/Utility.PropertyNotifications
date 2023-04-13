using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Utility.WPF.Panels;

namespace Utility.WPF.Demo.Panels
{
    public class RegionPanel : Panel
    {
        #region static properties

        public static readonly DependencyProperty WidthRatioProperty = DependencyProperty.Register("WidthRatio", typeof(double), typeof(RegionPanel),
            new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsArrange));

        public static readonly DependencyProperty HeightRatioProperty = DependencyProperty.Register("HeightRatio", typeof(double), typeof(RegionPanel),
            new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsArrange));

        public static readonly DependencyProperty UseDesiredSizeProperty = DependencyProperty.Register("UseDesiredSize", typeof(bool), typeof(RegionPanel),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty UseAnimationProperty = DependencyProperty.Register("UseAnimation", typeof(bool), typeof(RegionPanel),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty RegionProperty = DependencyProperty.RegisterAttached("Region", typeof(Region), typeof(RegionPanel),
                new PropertyMetadata(Region.Top, new PropertyChangedCallback(OnRegionChanged)), new ValidateValueCallback(IsValidRegion));

        private static bool IsValidRegion(object value)
        {
            return true;
        }

        /// <summary>
        /// Reads the attached property Dock from the given element.
        /// </summary>
        /// <param name="element">UIElement from which to read the attached property.</param>
        /// <returns>The property's value.</returns>
        /// <seealso cref="RegionProperty" />
        [AttachedPropertyBrowsableForChildren()]
        public static Region GetRegion(UIElement element)
        {
            return element != null ? (Region)element.GetValue(RegionProperty) : throw new ArgumentNullException("element");
        }

        /// <summary>
        /// Writes the attached property Dock to the given element.
        /// </summary>
        /// <param name="element">UIElement to which to write the attached property.</param>
        /// <param name="dock">The property value to set</param>
        /// <seealso cref="RegionProperty" />
        public static void SetRegion(UIElement element, Region dock)
        {
            if (element == null) { throw new ArgumentNullException("element"); }

            element.SetValue(RegionProperty, dock);
        }

        private static void OnRegionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement uie && VisualTreeHelper.GetParent(uie) is RegionPanel p)
            {
                p.InvalidateMeasure();
            }
        }

        #endregion static properties

        public RegionPanel()
        {
        }

        public double WidthRatio
        {
            get { return (double)GetValue(WidthRatioProperty); }
            set { SetValue(WidthRatioProperty, value); }
        }

        public double HeightRatio
        {
            get { return (double)GetValue(HeightRatioProperty); }
            set { SetValue(HeightRatioProperty, value); }
        }

        public bool UseDesiredSize
        {
            get { return (bool)GetValue(UseDesiredSizeProperty); }
            set { SetValue(UseDesiredSizeProperty, value); }
        }

        public bool UseAnimation
        {
            get { return (bool)GetValue(UseAnimationProperty); }
            set { SetValue(UseAnimationProperty, value); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var arr = InternalChildren.Cast<UIElement>().ToArray();

            foreach (var child in arr)
            {
                child.Measure(availableSize);
            }
            double width = availableSize.Width == double.PositiveInfinity ? arr.Sum(a => a.DesiredSize.Width) : availableSize.Width;
            double height = availableSize.Height == double.PositiveInfinity ? arr.Sum(a => a.DesiredSize.Height) : availableSize.Height;

            return new Size(width, height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            List<List<Coordinate>> coordinates = new List<List<Coordinate>>();

            var elementsBag = new ElementsBag();
            var elems = InternalChildren.Cast<FrameworkElement>().ToArray();
            elementsBag.CountPositions(elems);
            var (x, y) = elementsBag.GetSize();
            var existingCoordinates = SelectExistingCoordinates(elems, x, y);

            var arrange = Arrange(this, elementsBag, existingCoordinates, finalSize, x, y, WidthRatio, HeightRatio, UseDesiredSize, UseAnimation);

            static Size Arrange(FrameworkElement parent, ElementsBag elementsBag, IEnumerable<Coordinate> existingCoordinates, Size finalSize,
                int x, int y, double widthRatio, double heightRatio, bool useDesiredSize, bool useAnimation)
            {
                var size = FinalArrange(parent, elementsBag, finalSize, x, y,
                     SelectionCoordinatesForRegions(existingCoordinates), useAnimation, widthRatio, heightRatio, useDesiredSize).ToArray();

                return new Size(size.Sum(s => s.Height), size.Sum(a => a.Width));

                static IEnumerable<Size> FinalArrange(FrameworkElement parent, ElementsBag elementsBag, Size finalSize, int x, int y,
                    (Region Key, List<Coordinate> ac)[][] combinations, bool useAnimation,
                    double widthRatio = 1, double heightRatio = 1, bool useDesiredSize = false)
                {
                    foreach (var combination in combinations)
                        foreach (var combo in combination)
                            if (RegionPanelHelper.ConvertToRect(combo.Key, combo.ac, finalSize, x, y, widthRatio, heightRatio) is { } ar)
                            {
                                var es = RegionPanelHelper.SelectInnerElementRects(combo.Key, elementsBag.SelectElements(combo.Key), ar, useDesiredSize).ToArray();
                                foreach (var (rect, child) in es)
                                {
                                    if (useAnimation)
                                    {
                                        if (!(child.RenderTransform is TranslateTransform translateTransform))
                                        {
                                            child.RenderTransform = translateTransform = new TranslateTransform();
                                        }
                                        var translationPoint = child.TranslatePoint(new Point(), parent);
                                        child.RenderTransformOrigin = translationPoint;

                                        child.Arrange(new Rect(new Point(translationPoint.X, translationPoint.Y), rect.Size));

                                        AnimationHelper.Animate(translateTransform, translationPoint, rect.Location);
                                    }
                                    else
                                    {
                                        child.Arrange(rect);
                                    }
                                }

                                yield return new Size(es.Sum(a => a.Item1.Size.Height), es.Sum(a => a.Item1.Size.Width));
                            }
                }

                (Region region, List<Coordinate> ac)[][] SelectionCoordinatesForRegions(IEnumerable<Coordinate> existingCoordinates)
                {
                    var sides = RegionPanelHelper.SelectPotentialCoordinatesForRegions(elementsBag, existingCoordinates, x, y).Select(a => a.Value.Select(ac => (a.Key, ac)));

                    var combined = x * y;

                    var combos = CombinationHelper.SelectSetCombinations(sides).ToArray();

                    var combinations = combos
                                             .Select(a => (a: a.ToArray(), arr: a.Select(v => v.ac).ToArray()))
                                             .Select(a => (a.a, a.arr, count: a.arr.Sum(a => a.Count)))
                                             .Where(a => a.count <= combined && a.arr.SelectMany(a => a).ToArray().AllDistinct())
                                             .OrderByDescending(a => a.a.Select(a => a.Key).Distinct().Count())
                                             .OrderByDescending(a => a.count)
                                             .OrderByDescending(a => a.arr.Sum(c => Coordinate.Size(Coordinate.Size(c))))
                                             .Take(1)
                                             .Select(a => a.a)
                                             .ToArray();
                    return combinations;
                }
            }

            static List<Coordinate> SelectExistingCoordinates(UIElement[] elems, int x, int y)
            {
                List<Coordinate> existingCoordinates = new List<Coordinate>();
                foreach (UIElement child in elems)
                {
                    var region = RegionPanel.GetRegion(child);
                    existingCoordinates.Add(RegionPanelHelper.GetCoordinate(region, Math.Max(x - 1, 0), Math.Max(y - 1, 0)));
                }
                return existingCoordinates;
            }

            //static Point GetPoint(Size finalSize, Size childSize, Region region, Point lastPoint)
            //{
            //    Point center = new Point(finalSize.Width / 2, finalSize.Height / 2);

            //    var distanceFromCenter = GetDistance(region, new Vector(finalSize.Width - childSize.Width, finalSize.Height - childSize.Height));
            //    var offset = new Point(
            //        lastPoint.X + distanceFromCenter.X + center.X - childSize.Width / 2d,
            //        lastPoint.Y + distanceFromCenter.Y + center.Y - childSize.Height / 2d);

            //    return offset;

            //    static Point GetDistance(Region Region, Vector size)
            //    {
            //        return Region switch
            //        {
            //            Region.TopLeft => new Point(-size.X / 2, -size.Y / 2),
            //            Region.TopRight => new Point(size.X / 2, -size.Y / 2),
            //            Region.BottomRight => new Point(size.X / 2, size.Y / 2),
            //            Region.BottomLeft => new Point(-size.X / 2, size.Y / 2),

            //            Region.Top => new Point(0, -size.Y / 2),
            //            Region.Bottom => new Point(0, size.Y / 2),

            //            Region.Right => new Point(size.X / 2, 0),
            //            Region.Left => new Point(-size.X / 2, 0),

            //            Region.Middle => new Point(0, 0),
            //            _ => throw new NotImplementedException(),
            //        };
            //    }
            //}
            if (UseDesiredSize)
                return arrange;
            return finalSize;
        }
    }

    internal class ElementsBag
    {
        private int totalCount = 0;

        private int leftCount = 0, centerCount = 0, rightCount = 0,
         topCount = 0, middleCount = 0, bottomCount = 0,
         center2Count = 0, middle2Count = 0;

        private int bottomleftWidthCount = 0, bottomrightWidthCount = 0, topleftWidthCount = 0, toprightWidthCount = 0;

        private int bottomleftHeightCount = 0, bottomrightHeightCount = 0,
        topleftHeightCount = 0, toprightHeightCount = 0;

        public readonly List<FrameworkElement>
      bottomleft = new List<FrameworkElement>(),
      bottomright = new List<FrameworkElement>(),
      topright = new List<FrameworkElement>(),
      topleft = new List<FrameworkElement>(),
      left = new List<FrameworkElement>(),
      right = new List<FrameworkElement>(),
      middle = new List<FrameworkElement>(),
      top = new List<FrameworkElement>(),
      bottom = new List<FrameworkElement>();

        public (int x, int y) GetSize()
        {
            return totalCount switch
            {
                1 => (1, 1),
                2 => (1, 2),
                _ => (Math.Min(leftCount + bottomleftWidthCount + bottomrightWidthCount, 1) +
                Math.Min(centerCount + center2Count, 1) +
                Math.Min(rightCount + bottomrightWidthCount + toprightWidthCount, 1),
                Math.Min(bottomCount + bottomleftHeightCount + bottomrightHeightCount, 1) +
                Math.Min(topCount + toprightHeightCount + topleftHeightCount, 1) +
                Math.Min(middleCount + middle2Count, 1))
            };
        }

        public void CountPositions(IEnumerable<FrameworkElement> elements)
        {
            foreach (FrameworkElement child in elements)
            {
                totalCount++;
                var region = RegionPanel.GetRegion(child);
                //var heightSizing = EdgeLegacyPanel.GetHeightSizing(child);
                //var widthSizing = EdgeLegacyPanel.GetWidthSizing(child);

                int widthCount = 1;
                int heightCount = 1;

                switch (region)
                {
                    case Region.TopLeft:
                        topleft.Add(child);
                        topleftWidthCount += widthCount;
                        topleftHeightCount += heightCount;
                        break;

                    case Region.BottomLeft:
                        bottomleft.Add(child);
                        bottomleftWidthCount += widthCount;
                        bottomleftHeightCount += heightCount;
                        break;

                    case Region.BottomRight:
                        bottomright.Add(child);
                        bottomrightWidthCount += widthCount;
                        bottomrightHeightCount += heightCount;
                        break;

                    case Region.TopRight:
                        topright.Add(child);
                        toprightWidthCount += widthCount;
                        toprightHeightCount += heightCount;
                        break;

                    case Region.Left:
                        left.Add(child);
                        leftCount += widthCount;
                        middle2Count = 1;
                        break;

                    case Region.Right:
                        right.Add(child);
                        rightCount += widthCount;
                        middle2Count = 1;
                        break;

                    case Region.Top:
                        top.Add(child);
                        center2Count = 1;
                        topCount += heightCount;
                        break;

                    case Region.Bottom:
                        bottom.Add(child);
                        center2Count = 1;
                        bottomCount += heightCount;
                        break;

                    case Region.Middle:
                        middle.Add(child);
                        centerCount += widthCount;
                        middleCount += heightCount;
                        break;
                };
            }
        }

        public IList<FrameworkElement> SelectElements(Region region)
        {
            return SelectElements(region, this);

            static IList<FrameworkElement> SelectElements(Region region, ElementsBag bag)
            {
                return region switch
                {
                    Region.Right => bag.right,
                    Region.Left => bag.left,
                    Region.Bottom => bag.bottom,
                    Region.Top => bag.top,
                    Region.TopRight => bag.topright,
                    Region.BottomLeft => bag.bottomleft,
                    Region.BottomRight => bag.bottomright,
                    Region.TopLeft => bag.topleft,
                    Region.Middle => bag.middle,
                    _ => throw new ArgumentOutOfRangeException(nameof(region), region, null)
                };
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UtilityWpf.Panels;

namespace UtilityWpf.Demo.Panels
{
    public class RegionTwoPanel : Panel
    {
        #region static properties

        public static readonly DependencyProperty WidthRatioProperty = DependencyProperty.Register("WidthRatio", typeof(double), typeof(RegionTwoPanel),
            new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsArrange));

        public static readonly DependencyProperty HeightRatioProperty = DependencyProperty.Register("HeightRatio", typeof(double), typeof(RegionTwoPanel),
            new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsArrange));

        public static readonly DependencyProperty UseDesiredSizeProperty = DependencyProperty.Register("UseDesiredSize", typeof(bool), typeof(RegionTwoPanel),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty UseAnimationProperty = DependencyProperty.Register("UseAnimation", typeof(bool), typeof(RegionTwoPanel),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty RegionProperty = DependencyProperty.RegisterAttached("Region", typeof(Region), typeof(RegionTwoPanel),
                new PropertyMetadata(Region.Top, new PropertyChangedCallback(OnRegionChanged)), new ValidateValueCallback(IsValidRegion));

        private KeyValuePair<Region, (Rect, FrameworkElement)[]>[] arrangeMent;

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
            if (d is UIElement uie && VisualTreeHelper.GetParent(uie) is RegionTwoPanel p)
            {
                p.InvalidateMeasure();
            }
        }

        #endregion static properties

        public RegionTwoPanel()
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
            var arr = InternalChildren.Cast<FrameworkElement>().ToArray();

            foreach (var child in arr)
            {
                child.Measure(availableSize);
            }
            //double width = availableSize.Width == double.PositiveInfinity ? arr.DefaultIfEmpty().Sum(a => a?.DesiredSize.Width ?? 0) : availableSize.Width;
            //double height = availableSize.Height == double.PositiveInfinity ? arr.DefaultIfEmpty().Sum(a => a?.DesiredSize.Height ?? 0) : availableSize.Height;
            //double aheight = availableSize.Height == double.PositiveInfinity ? arr.DefaultIfEmpty().Max(a => a?.DesiredSize.Height ?? 0) : availableSize.Height;
            //if (height != aheight)
            //{
            //}
            //return new System.Windows.Size(width, height);

            double width = 0, height = 0;
            var parent = this.Parent;
            if (arr.Length > 0)
            {
                arrangeMent = SizerHelper.Arrange(this, arr, availableSize, WidthRatio, HeightRatio, true);
                //width = arrangeMent.DefaultIfEmpty().Max(a => a.Value.Where(a => double.IsInfinity(a.Item1.Right) == false).DefaultIfEmpty().Max(a => a.Item1.Right));
                //height = arrangeMent.DefaultIfEmpty().Max(a => a.Value.Where(a => double.IsInfinity(a.Item1.Bottom) == false).DefaultIfEmpty().Max(a => a.Item1.Bottom));
                width = arrangeMent.DefaultIfEmpty().Max(a => a.Value.Where(a => double.IsInfinity(a.Item1.Right) == false).DefaultIfEmpty().Max(a => a.Item1.Right));
                var width2 = arrangeMent.DefaultIfEmpty().Sum(a => a.Value.Where(a => double.IsInfinity(a.Item1.Width) == false).DefaultIfEmpty().Sum(a => a.Item1.Width));
                if (width <= availableSize.Width)
                {
                    height = arrangeMent.DefaultIfEmpty().Sum(a => a.Value.Where(a => double.IsInfinity(a.Item1.Size.Height) == false).DefaultIfEmpty().Sum(a => a.Item1.Size.Height));
                }
                else
                {
                    width = arrangeMent.DefaultIfEmpty().Sum(a => a.Value.Where(a => double.IsInfinity(a.Item1.Width) == false).DefaultIfEmpty().Sum(a => a.Item1.Width));
                    height = arrangeMent.DefaultIfEmpty().Max(a => a.Value.Where(a => double.IsInfinity(a.Item1.Bottom) == false).DefaultIfEmpty().Max(a => a.Item1.Bottom));
                }

                // var height2 = arrangeMent.DefaultIfEmpty().Max(a => a.Value.Where(a => double.IsInfinity(a.Item1.Size.Height) == false).DefaultIfEmpty().Max(a => a.Item1.Size.Height));
            }

            //width = !double.IsNaN(availableSize.Width) && double.IsInfinity(availableSize.Width) && width != 0 ? width : double.IsNaN(availableSize.Width) || double.IsInfinity(availableSize.Width) ? arr.Length > 0 ? arr.Sum(a => a.DesiredSize.Width) : 0 : availableSize.Width;
            //height = !double.IsNaN(availableSize.Height) && !double.IsInfinity(availableSize.Height) && height != 0 ? height : double.IsNaN(availableSize.Height) || double.IsInfinity(availableSize.Height) ? arr.Length > 0 ? arr.Sum(a => a.DesiredSize.Height) : 0 : availableSize.Height;
            if (double.IsInfinity(width) || double.IsInfinity(height) || double.IsNaN(width) || double.IsNaN(height))
            {
            }
            return new Size(width, height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            arrangeMent = SizerHelper.Arrange(this, this.InternalChildren.Cast<FrameworkElement>().ToArray(), finalSize, WidthRatio, HeightRatio, UseDesiredSize);

            foreach (var regionElements in arrangeMent)
            {
                foreach (var (rect, child) in regionElements.Value)
                {
                    if (UseAnimation)
                    {
                        AnimationHelper.Animate(this, child, rect);
                    }
                    else
                    {
                        child.Arrange(rect);
                    }
                }
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
            //if (UseDesiredSize)
            //    return arrange;
            return finalSize;
        }
    }

    internal static class SizerHelper

    {
        public static KeyValuePair<Region, (Rect, FrameworkElement)[]>[] Arrange(FrameworkElement parent, FrameworkElement[] children, Size finalSize, double widthRatio, double heightRatio, bool useDesiredSize)
        {
            //List<List<Coordinate>> coordinates = new List<List<Coordinate>>();

            var elementsBag = new ElementsBag();
            //var elems = InternalChildren.Cast<FrameworkElement>().ToArray();
            elementsBag.CountPositions(children);
            var (x, y) = elementsBag.GetSize();
            var existingCoordinates = SelectExistingCoordinates(children, x, y);

            //var arrange = Arrange(parent, elementsBag, existingCoordinates, finalSize, x, y, widthRatio, heightRatio, useDesiredSize, useAnimation);

            //static Size Arrange(FrameworkElement parent, ElementsBag elementsBag, IEnumerable<Coordinate> existingCoordinates, Size finalSize,
            //    int x, int y, double widthRatio, double heightRatio, bool useDesiredSize, bool useAnimation)
            //{
            var size = FinalArrange(parent, elementsBag, finalSize, x, y, SelectionCoordinatesForRegions(existingCoordinates), widthRatio, heightRatio, useDesiredSize)
            .ToArray();

            return size;
            //return new Size(size.Sum(s => s.Height), size.Sum(a => a.Width));

            static IEnumerable<KeyValuePair<Region, (Rect, FrameworkElement)[]>> FinalArrange(FrameworkElement parent, ElementsBag elementsBag, Size finalSize, int x, int y,
                (Region Key, List<Coordinate> ac)[][] combinations,
                double widthRatio = 1, double heightRatio = 1, bool useDesiredSize = false)
            {
                foreach (var combination in combinations)
                    foreach (var (Key, ac) in combination)
                        if (RegionPanelHelper.ConvertToRect(Key, ac, finalSize, x, y, widthRatio, heightRatio) is { } ar)
                        {
                            yield return KeyValuePair.Create(Key, RegionPanelHelper.SelectInnerElementRects(Key, elementsBag.SelectElements(Key), ar, useDesiredSize).ToArray());

                            //yield return new Size(es.Sum(a => a.Item1.Size.Height), es.Sum(a => a.Item1.Size.Width));
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
            // }

            static List<Coordinate> SelectExistingCoordinates(UIElement[] elems, int x, int y)
            {
                List<Coordinate> existingCoordinates = new List<Coordinate>();
                foreach (UIElement child in elems)
                {
                    var region = RegionTwoPanel.GetRegion(child);
                    existingCoordinates.Add(RegionPanelHelper.GetCoordinate(region, Math.Max(x - 1, 0), Math.Max(y - 1, 0)));
                }
                return existingCoordinates;
            }
        }
    }
}
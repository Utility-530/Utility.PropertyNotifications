//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Media;
//using Utility.UI.WPF.Panels;
//using Utility.WPF.Abstract;

//namespace Utility.WPF.Panels
//{
//    public class RegionPanel : Panel
//    {
//        private KeyValuePair<Region, (Rect, FrameworkElement)[]>[]? arrangement = null;

//        #region static properties
//        public static readonly DependencyProperty WidthRatioProperty = DependencyProperty.Register("WidthRatio", typeof(double), typeof(RegionPanel),
//            new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsArrange));

//        public static readonly DependencyProperty HeightRatioProperty = DependencyProperty.Register("HeightRatio", typeof(double), typeof(RegionPanel),
//            new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsArrange));

//        public static readonly DependencyProperty UseDesiredSizeProperty = DependencyProperty.Register("UseDesiredSize", typeof(bool), typeof(RegionPanel),
//                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

//        public static readonly DependencyProperty UseAnimationProperty = DependencyProperty.Register("UseAnimation", typeof(bool), typeof(RegionPanel),
//                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

//        public static readonly DependencyProperty RegionProperty = DependencyProperty.RegisterAttached("Region", typeof(Region), typeof(RegionPanel),
//                new PropertyMetadata(Region.Top, new PropertyChangedCallback(OnRegionChanged)), new ValidateValueCallback(IsValidRegion));

//        private static bool IsValidRegion(object value)
//        {
//            return true;
//        }

//        /// <summary>
//        /// Reads the attached property Dock from the given element.
//        /// </summary>
//        /// <param name="element">UIElement from which to read the attached property.</param>
//        /// <returns>The property's value.</returns>
//        /// <seealso cref="RegionProperty" />
//        [AttachedPropertyBrowsableForChildren()]
//        public static Region GetRegion(UIElement element)
//        {
//            return element != null ? (Region)element.GetValue(RegionProperty) : throw new ArgumentNullException("element");
//        }

//        /// <summary>
//        /// Writes the attached property Dock to the given element.
//        /// </summary>
//        /// <param name="element">UIElement to which to write the attached property.</param>
//        /// <param name="dock">The property value to set</param>
//        /// <seealso cref="RegionProperty" />
//        public static void SetRegion(UIElement element, Region dock)
//        {
//            if (element == null) { throw new ArgumentNullException("element"); }

//            element.SetValue(RegionProperty, dock);
//        }

//        private static void OnRegionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            if (d is UIElement uie && VisualTreeHelper.GetParent(uie) is RegionPanel p)
//            {
//                p.InvalidateMeasure();
//            }
//        }

//        #endregion

//        public RegionPanel()
//        {
//        }

//        public double WidthRatio
//        {
//            get { return (double)GetValue(WidthRatioProperty); }
//            set { SetValue(WidthRatioProperty, value); }
//        }

//        public double HeightRatio
//        {
//            get { return (double)GetValue(HeightRatioProperty); }
//            set { SetValue(HeightRatioProperty, value); }
//        }

//        public bool UseDesiredSize
//        {
//            get { return (bool)GetValue(UseDesiredSizeProperty); }
//            set { SetValue(UseDesiredSizeProperty, value); }
//        }

//        public bool UseAnimation
//        {
//            get { return (bool)GetValue(UseAnimationProperty); }
//            set { SetValue(UseAnimationProperty, value); }
//        }

//        protected override Size MeasureOverride(Size availableSize)
//        {
//            var arr = InternalChildren.Cast<FrameworkElement>().ToArray();

//            foreach (var child in arr)
//            {
//                child.Measure(availableSize);
//            }
//            //double width = availableSize.Width == double.PositiveInfinity ? arr.DefaultIfEmpty().Sum(a => a?.DesiredSize.Width ?? 0) : availableSize.Width;
//            //double height = availableSize.Height == double.PositiveInfinity ? arr.DefaultIfEmpty().Sum(a => a?.DesiredSize.Height ?? 0) : availableSize.Height;
//            //double aheight = availableSize.Height == double.PositiveInfinity ? arr.DefaultIfEmpty().Max(a => a?.DesiredSize.Height ?? 0) : availableSize.Height;
//            //if (height != aheight)
//            //{
//            //}
//            //return new System.Windows.Size(width, height);

//            double width = 0, height = 0;
//            var parent = Parent;
//            //if (arr.Length > 0)
//            //{
//            //    arrangeMent = Sizer.Arrange(this, arr, availableSize, WidthRatio, HeightRatio, true);
//            //    //width = arrangeMent.DefaultIfEmpty().Max(a => a.Value.Where(a => double.IsInfinity(a.Item1.Right) == false).DefaultIfEmpty().Max(a => a.Item1.Right));
//            //    //height = arrangeMent.DefaultIfEmpty().Max(a => a.Value.Where(a => double.IsInfinity(a.Item1.Bottom) == false).DefaultIfEmpty().Max(a => a.Item1.Bottom));
//            //    width = arrangeMent.DefaultIfEmpty().Max(a => a.Value.Where(a => double.IsInfinity(a.Item1.Right) == false).DefaultIfEmpty().Max(a => a.Item1.Right));
//            //    var width2 = arrangeMent.DefaultIfEmpty().Sum(a => a.Value.Where(a => double.IsInfinity(a.Item1.Width) == false).DefaultIfEmpty().Sum(a => a.Item1.Width));
//            //    if (width<=availableSize.Width)
//            //    {
//            //        height = arrangeMent.DefaultIfEmpty().Sum(a => a.Value.Where(a => double.IsInfinity(a.Item1.Size.Height) == false).DefaultIfEmpty().Sum(a => a.Item1.Size.Height));
//            //    }
//            //    else
//            //    {
//            //        width = arrangeMent.DefaultIfEmpty().Sum(a => a.Value.Where(a => double.IsInfinity(a.Item1.Width) == false).DefaultIfEmpty().Sum(a => a.Item1.Width));
//            //        height = arrangeMent.DefaultIfEmpty().Max(a => a.Value.Where(a => double.IsInfinity(a.Item1.Bottom) == false).DefaultIfEmpty().Max(a => a.Item1.Bottom));
//            //    }

//            //   // var height2 = arrangeMent.DefaultIfEmpty().Max(a => a.Value.Where(a => double.IsInfinity(a.Item1.Size.Height) == false).DefaultIfEmpty().Max(a => a.Item1.Size.Height));
//            //}

//            width = !double.IsNaN(availableSize.Width) && double.IsInfinity(availableSize.Width) && width != 0 ? width : double.IsNaN(availableSize.Width) || double.IsInfinity(availableSize.Width) ? arr.Length > 0 ? arr.Sum(a => a.DesiredSize.Width) : 0 : availableSize.Width;
//            height = !double.IsNaN(availableSize.Height) && !double.IsInfinity(availableSize.Height) && height != 0 ? height : double.IsNaN(availableSize.Height) || double.IsInfinity(availableSize.Height) ? arr.Length > 0 ? arr.Sum(a => a.DesiredSize.Height) : 0 : availableSize.Height;
//            if (double.IsInfinity(width) || double.IsInfinity(height) || double.IsNaN(width) || double.IsNaN(height))
//            {
//            }
//            return new Size(width, height);
//        }

//        protected override Size ArrangeOverride(Size finalSize)
//        {
//            arrangement = Sizer.Arrange(this, InternalChildren.Cast<FrameworkElement>().ToArray(), finalSize, WidthRatio, HeightRatio, UseDesiredSize);

//            foreach (var regionElements in arrangement)
//            {
//                foreach (var (rect, child) in regionElements.Value)
//                {
//                    if (UseAnimation)
//                    {
//                        AnimationHelper.Animate(this, child, rect);
//                    }
//                    else
//                    {
//                        child.Arrange(rect);
//                    }
//                }
//            }

//            //static Point GetPoint(Size finalSize, Size childSize, Region region, Point lastPoint)
//            //{
//            //    Point center = new Point(finalSize.Width / 2, finalSize.Height / 2);

//            //    var distanceFromCenter = GetDistance(region, new Vector(finalSize.Width - childSize.Width, finalSize.Height - childSize.Height));
//            //    var offset = new Point(
//            //        lastPoint.X + distanceFromCenter.X + center.X - childSize.Width / 2d,
//            //        lastPoint.Y + distanceFromCenter.Y + center.Y - childSize.Height / 2d);

//            //    return offset;

//            //    static Point GetDistance(Region Region, Vector size)
//            //    {
//            //        return Region switch
//            //        {
//            //            Region.TopLeft => new Point(-size.X / 2, -size.Y / 2),
//            //            Region.TopRight => new Point(size.X / 2, -size.Y / 2),
//            //            Region.BottomRight => new Point(size.X / 2, size.Y / 2),
//            //            Region.BottomLeft => new Point(-size.X / 2, size.Y / 2),

//            //            Region.Top => new Point(0, -size.Y / 2),
//            //            Region.Bottom => new Point(0, size.Y / 2),

//            //            Region.Right => new Point(size.X / 2, 0),
//            //            Region.Left => new Point(-size.X / 2, 0),

//            //            Region.Middle => new Point(0, 0),
//            //            _ => throw new NotImplementedException(),
//            //        };
//            //    }
//            //}
//            //if (UseDesiredSize)
//            //    return arrange;
//            return finalSize;
//        }

//    }

//    static class Sizer

//    {
//        public static KeyValuePair<Region, (Rect, FrameworkElement)[]>[] Arrange(FrameworkElement parent, FrameworkElement[] children, Size finalSize, double widthRatio, double heightRatio, bool useDesiredSize)
//        {
//            //List<List<Coordinate>> coordinates = new List<List<Coordinate>>();

//            var elementsBag = new ElementsBag();
//            //var elems = InternalChildren.Cast<FrameworkElement>().ToArray();
//            elementsBag.CountPositions(children);
//            var (x, y) = elementsBag.GetSize();
//            var existingCoordinates = SelectExistingCoordinates(children, x, y);

//            //var arrange = Arrange(parent, elementsBag, existingCoordinates, finalSize, x, y, widthRatio, heightRatio, useDesiredSize, useAnimation);

//            //static Size Arrange(FrameworkElement parent, ElementsBag elementsBag, IEnumerable<Coordinate> existingCoordinates, Size finalSize,
//            //    int x, int y, double widthRatio, double heightRatio, bool useDesiredSize, bool useAnimation)
//            //{
//            var size = FinalArrange(parent, elementsBag, finalSize, x, y, SelectionCoordinatesForRegions(existingCoordinates), widthRatio, heightRatio, useDesiredSize)
//            .ToArray();

//            return size;
//            //return new Size(size.Sum(s => s.Height), size.Sum(a => a.Width));

//            static IEnumerable<KeyValuePair<Region, (Rect, FrameworkElement)[]>> FinalArrange(FrameworkElement parent, ElementsBag elementsBag, Size finalSize, int x, int y,
//                (Region Key, List<Coordinate> ac)[][] combinations,
//                double widthRatio = 1, double heightRatio = 1, bool useDesiredSize = false)
//            {
//                foreach (var combination in combinations)
//                    foreach (var (Key, ac) in combination)
//                        if (RegionPanelHelper.ConvertToRect(Key, ac, finalSize, x, y, widthRatio, heightRatio) is { } ar)
//                        {
//                            yield return KeyValuePair.Create(Key, RegionPanelHelper.SelectInnerElementRects(Key, elementsBag.SelectElements(Key), ar, useDesiredSize).ToArray());

//                            //yield return new Size(es.Sum(a => a.Item1.Size.Height), es.Sum(a => a.Item1.Size.Width));
//                        }
//            }

//            (Region region, List<Coordinate> ac)[][] SelectionCoordinatesForRegions(IEnumerable<Coordinate> existingCoordinates)
//            {
//                var sides = RegionPanelHelper.SelectPotentialCoordinatesForRegions(elementsBag, existingCoordinates, x, y).Select(a => a.Value.Select(ac => (a.Key, ac)));

//                var combined = x * y;

//                var combos = sides.SelectSetCombinations().ToArray();

//                var combinations = combos
//                                         .Select(a => (a: a.ToArray(), arr: a.Select(v => v.ac).ToArray()))
//                                         .Select(a => (a.a, a.arr, count: a.arr.Sum(a => a.Count)))
//                                         .Where(a => a.count <= combined && a.arr.SelectMany(a => a).ToArray().AllDistinct())
//                                         .OrderByDescending(a => a.a.Select(a => a.Key).Distinct().Count())
//                                         .OrderByDescending(a => a.count)
//                                         .OrderByDescending(a => a.arr.Sum(c => Coordinate.Size(Coordinate.Size(c))))
//                                         .Take(1)
//                                         .Select(a => a.a)
//                                         .ToArray();
//                return combinations;

//            }
//            // }

//            static List<Coordinate> SelectExistingCoordinates(UIElement[] elems, int x, int y)
//            {
//                List<Coordinate> existingCoordinates = new List<Coordinate>();
//                foreach (UIElement child in elems)
//                {
//                    var region = RegionPanel.GetRegion(child);
//                    existingCoordinates.Add(RegionPanelHelper.GetCoordinate(region, Math.Max(x - 1, 0), Math.Max(y - 1, 0)));
//                }
//                return existingCoordinates;
//            }
//        }

//    }
//    //class ElementsBag
//    //{
//    //    int totalCount = 0;
//    //    int leftCount = 0, centerCount = 0, rightCount = 0,
//    //     topCount = 0, middleCount = 0, bottomCount = 0,
//    //     center2Count = 0, middle2Count = 0;

//    //    int bottomleftWidthCount = 0, bottomrightWidthCount = 0, topleftWidthCount = 0, toprightWidthCount = 0;

//    //    int bottomleftHeightCount = 0, bottomrightHeightCount = 0,
//    //    topleftHeightCount = 0, toprightHeightCount = 0;

//    //    public readonly List<FrameworkElement>
//    //  bottomleft = new List<FrameworkElement>(),
//    //  bottomright = new List<FrameworkElement>(),
//    //  topright = new List<FrameworkElement>(),
//    //  topleft = new List<FrameworkElement>(),
//    //  left = new List<FrameworkElement>(),
//    //  right = new List<FrameworkElement>(),
//    //  middle = new List<FrameworkElement>(),
//    //  top = new List<FrameworkElement>(),
//    //  bottom = new List<FrameworkElement>();

//    //    public (int x, int y) GetSize()
//    //    {
//    //        return totalCount switch
//    //        {
//    //            1 => (1, 1),
//    //            2 => (1, 2),
//    //            _ => (Math.Min(leftCount + bottomleftWidthCount + bottomrightWidthCount, 1) +
//    //            Math.Min(centerCount + center2Count, 1) +
//    //            Math.Min(rightCount + bottomrightWidthCount + toprightWidthCount, 1),
//    //            Math.Min(bottomCount + bottomleftHeightCount + bottomrightHeightCount, 1) +
//    //            Math.Min(topCount + toprightHeightCount + topleftHeightCount, 1) +
//    //            Math.Min(middleCount + middle2Count, 1))
//    //        };
//    //    }

//    //    public void CountPositions(IEnumerable<FrameworkElement> elements)
//    //    {
//    //        foreach (FrameworkElement child in elements)
//    //        {
//    //            totalCount++;
//    //            var region = RegionPanel.GetRegion(child);
//    //            //var heightSizing = EdgeLegacyPanel.GetHeightSizing(child);
//    //            //var widthSizing = EdgeLegacyPanel.GetWidthSizing(child);

//    //            int widthCount = 1;
//    //            int heightCount = 1;

//    //            switch (region)
//    //            {
//    //                case Region.TopLeft:
//    //                    topleft.Add(child);
//    //                    topleftWidthCount += widthCount;
//    //                    topleftHeightCount += heightCount;
//    //                    break;
//    //                case Region.BottomLeft:
//    //                    bottomleft.Add(child);
//    //                    bottomleftWidthCount += widthCount;
//    //                    bottomleftHeightCount += heightCount;
//    //                    break;
//    //                case Region.BottomRight:
//    //                    bottomright.Add(child);
//    //                    bottomrightWidthCount += widthCount;
//    //                    bottomrightHeightCount += heightCount;
//    //                    break;
//    //                case Region.TopRight:
//    //                    topright.Add(child);
//    //                    toprightWidthCount += widthCount;
//    //                    toprightHeightCount += heightCount;
//    //                    break;

//    //                case Region.Left:
//    //                    left.Add(child);
//    //                    leftCount += widthCount;
//    //                    middle2Count = 1;
//    //                    break;

//    //                case Region.Right:
//    //                    right.Add(child);
//    //                    rightCount += widthCount;
//    //                    middle2Count = 1;
//    //                    break;

//    //                case Region.Top:
//    //                    top.Add(child);
//    //                    center2Count = 1;
//    //                    topCount += heightCount;
//    //                    break;

//    //                case Region.Bottom:
//    //                    bottom.Add(child);
//    //                    center2Count = 1;
//    //                    bottomCount += heightCount;
//    //                    break;

//    //                case Region.Middle:
//    //                    middle.Add(child);
//    //                    centerCount += widthCount;
//    //                    middleCount += heightCount;
//    //                    break;

//    //            };
//    //        }
//    //    }

//    //    public IList<FrameworkElement> SelectElements(Region region)
//    //    {
//    //        return SelectElements(region, this);

//    //        static IList<FrameworkElement> SelectElements(Region region, ElementsBag bag)
//    //        {
//    //            return region switch
//    //            {
//    //                Region.Right => bag.right,
//    //                Region.Left => bag.left,
//    //                Region.Bottom => bag.bottom,
//    //                Region.Top => bag.top,
//    //                Region.TopRight => bag.topright,
//    //                Region.BottomLeft => bag.bottomleft,
//    //                Region.BottomRight => bag.bottomright,
//    //                Region.TopLeft => bag.topleft,
//    //                Region.Middle => bag.middle,
//    //            };
//    //        }
//    //    }

//    //}

//    using System;
//using System.Collections.Generic;
//using System.Diagnostics.CodeAnalysis;
//using System.Linq;
//using System.Windows;
//using Utility.Abstract.Enums;

//namespace Utility.UI.WPF.Panels
//    {
//        class RegionPanelHelper
//        {
//            public static IEnumerable<KeyValuePair<Region, IOrderedEnumerable<List<Coordinate>>>>
//               SelectPotentialCoordinatesForRegions(ElementsBag bag, IEnumerable<Coordinate> existingCoordinates, int x, int y)
//            {
//                var combos = GetCoordinateCombinations(x, y).ToArray();
//                //var test = combos.Select(a => a.Min).Select(x, y).ToArray();
//                var remainingCoordinates = combos.Select(a => a.Min).Except(existingCoordinates).ToList();

//                if (bag.right.Count > 0)
//                {
//                    yield return GetCoordinateBoxesByRegion(Region.Right, x, y, remainingCoordinates);
//                }
//                if (bag.left.Count > 0)
//                {
//                    yield return GetCoordinateBoxesByRegion(Region.Left, x, y, remainingCoordinates);
//                }
//                if (bag.top.Count > 0)
//                {
//                    yield return GetCoordinateBoxesByRegion(Region.Top, x, y, remainingCoordinates);
//                }
//                if (bag.bottom.Count > 0)
//                {
//                    yield return GetCoordinateBoxesByRegion(Region.Bottom, x, y, remainingCoordinates);
//                }
//                if (bag.topleft.Count > 0)
//                {
//                    yield return GetCoordinateBoxesByRegion(Region.TopLeft, x, y, remainingCoordinates);
//                }
//                if (bag.topright.Count > 0)
//                {
//                    yield return GetCoordinateBoxesByRegion(Region.TopRight, x, y, remainingCoordinates);
//                }
//                if (bag.bottomleft.Count > 0)
//                {
//                    yield return GetCoordinateBoxesByRegion(Region.BottomLeft, x, y, remainingCoordinates);
//                }
//                if (bag.bottomright.Count > 0)
//                {
//                    yield return GetCoordinateBoxesByRegion(Region.BottomRight, x, y, remainingCoordinates);
//                }
//                if (bag.middle.Count > 0)
//                {
//                    yield return GetCoordinateBoxesByRegion(Region.Middle, x, y, remainingCoordinates);
//                }

//                static KeyValuePair<Region, IOrderedEnumerable<List<Coordinate>>> GetCoordinateBoxesByRegion(Region region, int x, int y, List<Coordinate> remainingCoordinates)
//                {
//                    var coord = GetCoordinate(region, x - 1, y - 1);
//                    var sidesRemaining = remainingCoordinates.Select(a => (side: a.IsSide(coord), touching: a.IsTouching(coord), coord: a)).ToArray();

//                    if (remainingCoordinates.Count == 0 || !sidesRemaining.Any(a => a.side))
//                    {
//                        var ae = new[] { new List<Coordinate> { coord } }.OrderBy(a => a);
//                        return KeyValuePair.Create(region, ae);
//                    }
//                    if (sidesRemaining.Count(a => a.side) == 1)
//                    {
//                        var ae = new[] {
//                        new List<Coordinate> { coord, sidesRemaining.Single(ac => ac.side).coord },
//                        new List<Coordinate> { coord }
//                    }.OrderByDescending(a => a.Count);
//                        return KeyValuePair.Create(region, ae);
//                    }
//                    if (sidesRemaining.Count(a => a.side) > 1)
//                    {
//                        if (NewMethod1(region, coord, sidesRemaining) is { } aa)
//                            return aa;

//                        if (NewMethod2(region, coord, sidesRemaining) is { } ae)
//                            return ae;
//                    }

//                    return AlternateMethod();

//                    static KeyValuePair<Region, IOrderedEnumerable<List<Coordinate>>>? NewMethod1(Region region, Coordinate coord, (bool side, bool touching, Coordinate coord)[] sidesRemaining)
//                    {
//                        var ae = sidesRemaining.Where(a => a.side).GroupBy(a => a.coord.Y).ToArray();
//                        if (ae.Length == 1)
//                        {
//                            var es = ae.Single().Select(a => a.coord.X).Concat(new[] { coord.X }).OrderBy(a => a).ToArray();
//                            if ((es.Last() - es.First()) == es.Length - 1)
//                            {
//                                var aed = new[] {
//                                    ae.Single().Select(c => c.coord).Concat(new[] { coord }).ToList() ,
//                                    new List<Coordinate>{ae.Single().First().coord, coord },
//                                    new List<Coordinate>{ae.Single().Last().coord, coord },
//                                    new List<Coordinate>{coord}
//                                }.OrderByDescending(a => a.Count);
//                                return KeyValuePair.Create(region, aed);
//                            }
//                        }
//                        return null;
//                    }

//                    static KeyValuePair<Region, IOrderedEnumerable<List<Coordinate>>>? NewMethod2(Region region, Coordinate coord, (bool side, bool touching, Coordinate coord)[] sidesRemaining)
//                    {
//                        var ae = sidesRemaining.Where(a => a.side).GroupBy(a => a.coord.X).ToArray();
//                        if (ae.Length == 1)
//                        {
//                            var es = ae.Single().Select(a => a.coord.Y).Concat(new[] { coord.Y }).OrderBy(a => a).ToArray();
//                            if ((es.Last() - es.First()) == es.Length - 1)
//                            {
//                                var aed = new[] {
//                                    ae.Single().Select(c => c.coord).Concat(new[] { coord }).ToList() ,
//                                    new List<Coordinate>{ae.Single().First().coord, coord },
//                                    new List<Coordinate>{ae.Single().Last().coord, coord },
//                                    new List<Coordinate>{coord}
//                                }.OrderByDescending(a => a.Count);
//                                return KeyValuePair.Create(region, aed);
//                            }
//                        }
//                        return null;
//                    }

//                    KeyValuePair<Region, IOrderedEnumerable<List<Coordinate>>> AlternateMethod()
//                    {
//                        var remainingCoordinatesList = remainingCoordinates.Concat(new[] { coord }).ToList();

//                        var remainingCoordinateBoxes = RegionPanelHelper.SelectCoordinates(remainingCoordinatesList).ToList()
//                            .OrderByDescending(a => a.Count);

//                        var boxes = SelectcoordinateBoxes(GetLimitFunc(region, x, y), x, y).OrderByDescending(a => a.Count);

//                        return KeyValuePair.Create(region, boxes);

//                        static Func<MinMax<Coordinate>, bool> GetLimitFunc(Region region, int x, int y)
//                        {
//                            return region switch
//                            {
//                                Region.TopLeft => new Func<MinMax<Coordinate>, bool>(a => a.Min.X <= 0 && a.Max.X >= 0 && a.Min.Y <= 0 && a.Max.Y >= 0),
//                                Region.Top => a => a.Min.Y <= 0 && a.Max.Y >= 0,
//                                Region.TopRight => (a => a.Min.X <= x - 1 && a.Max.X >= x - 1 && a.Min.Y <= 0 && a.Max.Y >= 0),

//                                Region.Left => a => a.Min.X <= 0 && a.Max.X >= 0,
//                                Region.Middle => (a => a.Min.X <= (x - 1) / 2 && a.Max.X >= (x - 1) / 2 && a.Min.Y <= (y - 1) / 2 && a.Max.Y >= (y - 1) / 2),
//                                Region.Right => a => a.Min.X <= x - 1 && a.Max.X >= x - 1,

//                                Region.BottomLeft => a => a.Min.X <= x - 1 && a.Max.X >= x - 1 && a.Min.Y <= y - 1 && a.Max.Y >= y - 1,
//                                Region.Bottom => a => a.Min.Y <= y - 1 && a.Max.Y >= y - 1,
//                                Region.BottomRight => a => a.Min.X <= x - 1 && a.Max.X >= x - 1 && a.Min.Y <= y - 1 && a.Max.Y >= y - 1,
//                                _ => throw new NotImplementedException(),
//                            };
//                        }

//                        static List<List<Coordinate>> SelectcoordinateBoxes(Func<MinMax<Coordinate>, bool> func, int x, int y)
//                        {
//                            var initialCombos = GetCoordinateCombinations(x, y);
//                            var combos = initialCombos.Where(func).ToList();

//                            var vls = combos
//                                .Select(a =>
//                                {
//                                    try
//                                    {
//                                        var da =
//                                        from x in Enumerable.Range(a.Min.X, 1 + a.Max.X - a.Min.X)
//                                        join y in Enumerable.Range(a.Min.Y, 1 + a.Max.Y - a.Min.Y)
//                                        on true equals true
//                                        select new Coordinate(x, y);
//                                        return da.ToList();
//                                    }
//                                    catch (Exception)
//                                    {
//                                        throw;
//                                    }
//                                })
//                                .Where(a => a.Count > 0)
//                                .ToList();

//                            return vls;
//                        }
//                    }
//                }
//            }

//            static List<MinMax<Coordinate>> GetCoordinateCombinations(int x, int y)
//            {
//                var cs = CombinationHelper.SelectSetCombinations(new[] { Enumerable.Range(0, x).ToList(), Enumerable.Range(0, y).ToList() })
//                    .Select(a => a.ToArray())
//                    .Select(a => new Coordinate(a[0], a[1]))
//                    .OrderBy(a => a.Y).ThenBy(a => a.X).ToList()
//                    .ToList();

//                var cbs = CombinationHelper.SelectCombinations(cs);

//                var minMaxes = cbs
//                    // Remove items where
//                    .Where(a => !(a.Item1.CompareTo(a.Item2) == 0 && !a.Item1.Equals(a.Item2)))
//                    .Select(a => new[] { a.Item1, a.Item2 }.OrderBy(a => a).ToArray()).Select(a => new MinMax<Coordinate>(a[0], a[1])).ToList();

//                return minMaxes;
//            }

//            static IEnumerable<List<Coordinate>> SelectCoordinates(List<Coordinate> coordinates)
//            {
//                var initial = SelectCoordinateBoxes(coordinates).ToList();
//                var sede = initial.ToList();

//                return sede.Select(a =>
//                {
//                    var ad = (from x in Enumerable.Range(Math.Min(a.Item1.X, a.Item2.X), 1 + Math.Abs(a.Item1.X - a.Item2.X))
//                              join y in Enumerable.Range(Math.Min(a.Item1.Y, a.Item2.Y), 1 + Math.Abs(a.Item1.Y - a.Item2.Y))
//                              on true equals true
//                              orderby x
//                              orderby y
//                              select new Coordinate(x, y)
//                            ).ToList();
//                    return ad;
//                });

//                static IEnumerable<(Coordinate, Coordinate)> SelectCoordinateBoxes(List<Coordinate> coordinates)
//                {
//                    var ordered = coordinates.OrderBy(a => a.X).OrderBy(a => a.Y).ToList();
//                    int i = 0;
//                    foreach (var coord in ordered)
//                    {
//                        int j = 0;
//                        Coordinate? last = default;
//                        int? maxX = default;
//                        while (i + j < ordered.Count)
//                        {
//                            var next = ordered[(i + j)];
//                            if (last.HasValue && last.Value.Y == next.Y && (next.X - 1) != last.Value.X ||
//                                (maxX.HasValue && last.HasValue && last.Value.Y < next.Y && (next.X > maxX.Value || next.X < coord.X)))
//                            {
//                                break;
//                            }

//                            if (last.HasValue == false || (last.HasValue && next.X >= coord.X && next.Y >= coord.Y))
//                            {
//                                maxX = next.X;
//                                last = ordered[(i + j)];
//                            }
//                            j++;
//                        }

//                        if (last.HasValue)
//                            yield return (coord, last.Value);

//                        i++;
//                    }
//                }
//            }

//            public static Coordinate GetCoordinate(Region region, int x, int y)
//            {
//                return region switch
//                {
//                    Region.TopLeft => new Coordinate(0, 0),
//                    Region.Top => new Coordinate((int)(x / 2), 0),
//                    Region.TopRight => new Coordinate(x, 0),

//                    Region.Left => new Coordinate(0, (int)(y / 2)),
//                    Region.Middle => new Coordinate((int)(x / 2), (int)(y / 2)),
//                    Region.Right => new Coordinate(x, (int)(y / 2)),

//                    Region.BottomLeft => new Coordinate(0, y),
//                    Region.Bottom => new Coordinate((int)(x / 2), y),
//                    Region.BottomRight => new Coordinate(x, y),
//                    _ => throw new NotImplementedException(),
//                };
//            }

//            public static IEnumerable<(Rect, FrameworkElement)> SelectInnerElementRects(Region region, ICollection<FrameworkElement> elements, Rect rect, bool useDesiredSize)
//            {
//                var max = Math.Max(rect.Width, rect.Height);
//                var isWiderThanTall = max == rect.Width;
//                var min = isWiderThanTall ? rect.Height : rect.Width;

//                var division = elements.Count > 1 ? (isWiderThanTall ?
//                    Math.Max(0, max - elements.Last().DesiredSize.Width) :
//                    Math.Max(0, max - elements.Last().DesiredSize.Height)) /
//                    (elements.Count - 1) : max;

//                if (useDesiredSize == false)
//                {
//                    if (elements.Sum(a => a.DesiredSize.Width) < rect.Width)
//                        division = max / elements.Count;

//                    if (elements.Sum(a => a.DesiredSize.Height) < rect.Height)
//                        division = max / elements.Count;
//                }
//                else
//                {
//                    var maxWidth = elements.Max(a => a.DesiredSize.Width);
//                    var maxHeight = elements.Max(a => a.DesiredSize.Height);
//                    division = isWiderThanTall ?
//                        maxWidth > 0 ? maxWidth : elements.Max(a => a.ActualHeight) :
//                        maxHeight > 0 ? maxHeight : elements.Max(a => a.ActualWidth);
//                }

//                var point = rect.Location;
//                var size = isWiderThanTall ? new System.Windows.Size(division, min) : new System.Windows.Size(min, division);
//                foreach (var elem in elements)
//                {
//                    if (elem.DesiredSize.Width == 0 || elem.DesiredSize.Height == 0 || useDesiredSize == false)
//                    {
//                        yield return (new Rect(point, size), elem);
//                        point = isWiderThanTall ? new Point(point.X + division, point.Y) : new Point(point.X, point.Y + division);
//                    }
//                    else
//                    {
//                        Rect childRect = new Rect(point, new System.Windows.Size(
//                            Math.Max(elem.DesiredSize.Width, isWiderThanTall ? division : rect.Width),
//                            Math.Max(elem.DesiredSize.Height, isWiderThanTall ? rect.Height : division)));

//                        GetChildRect(region, elem, rect, ref point, ref childRect, isWiderThanTall);

//                        point = new Point(point.X + (isWiderThanTall ? Math.Min(childRect.Width, division) : 0), point.Y + (isWiderThanTall ? 0 : Math.Min(childRect.Height, division)));

//                        yield return (childRect, elem);
//                    }
//                }

//                static void GetChildRect(Region region, UIElement elem, Rect rect, ref Point lastPoint, ref Rect childRect, bool isWiderThanTall)
//                {
//                    if (elem.DesiredSize.Width > rect.Size.Width)
//                    {
//                        lastPoint = new Point(rect.X + rect.Size.Width / 2 - elem.DesiredSize.Width / 2, lastPoint.Y);
//                        childRect = new Rect(lastPoint, childRect.Size);
//                    }
//                    if (elem.DesiredSize.Height > rect.Size.Height)
//                    {
//                        lastPoint = new Point(lastPoint.X, rect.Y + rect.Size.Height / 2 - elem.DesiredSize.Height / 2);
//                        childRect = new Rect(lastPoint, childRect.Size);
//                    }
//                    if ((lastPoint.X + elem.DesiredSize.Width) > (rect.X + rect.Size.Width))
//                    {
//                        lastPoint = region switch
//                        {
//                            Region.Right => new Point(rect.X - childRect.Width, isWiderThanTall ? rect.Y : lastPoint.Y),
//                            Region.BottomRight => new Point(rect.Width + rect.X - childRect.Width, isWiderThanTall ? rect.Y : lastPoint.Y),
//                            Region.TopRight => new Point(rect.Width + rect.X - childRect.Width, isWiderThanTall ? rect.Y : lastPoint.Y),
//                            _ => new Point(rect.X, isWiderThanTall ? rect.Y : lastPoint.Y),
//                        };
//                        childRect = new Rect(lastPoint, childRect.Size);
//                    }
//                    if ((lastPoint.Y + elem.DesiredSize.Height) > (rect.Y + rect.Size.Height))
//                    {
//                        lastPoint = region switch
//                        {
//                            Region.BottomRight => new Point(isWiderThanTall ? lastPoint.X : rect.X, rect.Height + rect.Y - childRect.Height),
//                            Region.BottomLeft => new Point(isWiderThanTall ? lastPoint.X : rect.X, rect.Height + rect.Y - childRect.Height),
//                            Region.Bottom => new Point(isWiderThanTall ? lastPoint.X : rect.X, rect.Height + rect.Y - childRect.Height),
//                            _ => new Point(isWiderThanTall ? lastPoint.X : rect.X, rect.Y),
//                        };
//                        childRect = new Rect(lastPoint, childRect.Size);
//                    }
//                }
//            }

//            public static Rect ConvertToRect(Region region, List<Coordinate> innerCoordsSet, System.Windows.Size finalSize, int x, int y, double widthRatio = 1, double heightRatio = 1)
//            {
//                var ((startX, diffX), (startY, diffY)) =

//                    region switch
//                    {
//                        Region.Middle => (Middle_X(), Middle_Y()),
//                        Region.Top => (Middle_X(), Edge_Y()),
//                        Region.Bottom => (Middle_X(), Edge_Y()),
//                        Region.Left => (Edge_X(), Middle_Y()),
//                        Region.Right => (Edge_X(), Middle_Y()),
//                        _ => (Edge_X(), Edge_Y()),
//                    };

//                (double, double) Middle_X()
//                {
//                    return x > 2 ? MiddleX(innerCoordsSet, finalSize, x, widthRatio) : EdgeX(innerCoordsSet, finalSize, x, widthRatio);
//                }
//                (double, double) Middle_Y()
//                {
//                    return x > 2 ? MiddleY(innerCoordsSet, finalSize, y, heightRatio) : EdgeY(innerCoordsSet, finalSize, y, heightRatio);
//                }
//                (double, double) Edge_X()
//                {
//                    return EdgeX(innerCoordsSet, finalSize, x, widthRatio);
//                }
//                (double, double) Edge_Y()
//                {
//                    return EdgeY(innerCoordsSet, finalSize, y, heightRatio);
//                }

//                return (new Rect(new Point(startX, startY), new System.Windows.Size(diffX, diffY)));

//                static (double start, double end) EdgeY(List<Coordinate> innerCoordsSet, System.Windows.Size finalSize, int y, double heightRatio = 1)
//                {
//                    var startY = (innerCoordsSet.Min(a => a.Y) * finalSize.Height / y);
//                    var diffY = UnderFactor(heightRatio, y) * (((innerCoordsSet.Max(a => a.Y) + 1) * finalSize.Height / y) - startY);
//                    var es = (finalSize.Height - diffY) / startY;
//                    var of = OverFactor(heightRatio, y);
//                    startY *= OverFactor(heightRatio, y);

//                    return (startY, diffY);
//                }

//                static (double start, double end) MiddleY(List<Coordinate> innerCoordsSet, System.Windows.Size finalSize, int y, double heightRatio = 1)
//                {
//                    var startY = (innerCoordsSet.Min(a => a.Y) * finalSize.Height / y);
//                    startY *= UnderFactor(heightRatio, y);
//                    var diffY = OverFactor(heightRatio, y) * (((innerCoordsSet.Max(a => a.Y) + 1) * finalSize.Height / y)) - startY;

//                    return (startY, diffY);
//                }

//                static (double start, double end) EdgeX(List<Coordinate> innerCoordsSet, System.Windows.Size finalSize, int x, double widthRatio = 1)
//                {
//                    var startX = (innerCoordsSet.Min(a => a.X) * finalSize.Width / x);
//                    var diffX = UnderFactor(widthRatio, x) * (((innerCoordsSet.Max(a => a.X) + 1) * finalSize.Width / x) - startX);
//                    startX *= OverFactor(widthRatio, x);

//                    return (startX, diffX);
//                }

//                static (double start, double end) MiddleX(List<Coordinate> innerCoordsSet, System.Windows.Size finalSize, int x, double widthRatio = 1)
//                {
//                    var startX = (innerCoordsSet.Min(a => a.X) * finalSize.Width / x);
//                    startX *= UnderFactor(widthRatio, x);
//                    var diffX = OverFactor(widthRatio, x) * (((innerCoordsSet.Max(a => a.X) + 1) * finalSize.Width / x)) - startX;
//                    return (startX, diffX);
//                }

//                static double UnderFactor(double ratio, int size) => 1d / ratio;

//                static double OverFactor(double ratio, int size)
//                {
//                    return size switch
//                    {
//                        3 => 1 + (1 - 1 / ratio) / 2,
//                        2 => 1 + (1 - 1 / ratio),
//                        1 => 1,
//                        _ => throw new NotImplementedException(),
//                    };
//                }
//            }

//            struct MinMax<T>
//            {
//                public MinMax(T min, T max)
//                {
//                    Min = min;
//                    Max = max;
//                }

//                public T Min { get; }

//                public T Max { get; }
//            }
//        }

//        struct Coordinate : IComparable<Coordinate>
//        {
//            public Coordinate(int X, int Y)
//            {
//                this.X = X;
//                this.Y = Y;

//            }

//            public int X { get; }
//            public int Y { get; }

//            public override string ToString()
//            {
//                return "X:" + X + " Y: " + Y;
//            }

//            public static System.Drawing.Size Size(List<Coordinate> coordinates)
//            {
//                var minX = coordinates.Min(a => a.X);
//                var minY = coordinates.Min(a => a.Y);
//                var maxX = coordinates.Max(a => a.X);
//                var maxY = coordinates.Max(a => a.Y);

//                return new System.Drawing.Size(maxX - minX, maxY - minY);
//            }

//            public static double Size(System.Drawing.Size size)
//            {
//                return Math.Pow(size.Width, 2) + Math.Pow(size.Height, 2);
//            }

//            public int CompareTo([AllowNull] Coordinate other)
//            {
//                return this.X.CompareTo(other.X) + this.Y.CompareTo(other.Y);
//            }

//            public bool IsDiagonal(Coordinate coordinate)
//            {
//                return Math.Abs(coordinate.X - this.X) == 1 && Math.Abs(coordinate.Y - this.Y) == 1;
//            }
//            public bool IsSide(Coordinate coordinate)
//            {
//                return (Math.Abs(coordinate.X - this.X) == 0 && Math.Abs(coordinate.Y - this.Y) == 1
//                    || Math.Abs(coordinate.X - this.X) == 1 && Math.Abs(coordinate.Y - this.Y) == 0);
//            }

//            public bool IsTouching(Coordinate coordinate)
//            {
//                return Math.Abs(coordinate.Y - this.Y) <= 1 && Math.Abs(coordinate.X - this.X) <= 1;
//            }
//        }

//        class ElementsBag
//        {
//            int totalCount = 0;
//            int leftCount = 0, centerCount = 0, rightCount = 0,
//             topCount = 0, middleCount = 0, bottomCount = 0,
//             center2Count = 0, middle2Count = 0;

//            int bottomleftWidthCount = 0, bottomrightWidthCount = 0, topleftWidthCount = 0, toprightWidthCount = 0;

//            int bottomleftHeightCount = 0, bottomrightHeightCount = 0,
//            topleftHeightCount = 0, toprightHeightCount = 0;

//            public readonly List<FrameworkElement>
//          bottomleft = new List<FrameworkElement>(),
//          bottomright = new List<FrameworkElement>(),
//          topright = new List<FrameworkElement>(),
//          topleft = new List<FrameworkElement>(),
//          left = new List<FrameworkElement>(),
//          right = new List<FrameworkElement>(),
//          middle = new List<FrameworkElement>(),
//          top = new List<FrameworkElement>(),
//          bottom = new List<FrameworkElement>();

//            public (int x, int y) GetSize()
//            {
//                return totalCount switch
//                {
//                    1 => (1, 1),
//                    2 => (1, 2),
//                    _ => (Math.Min(leftCount + bottomleftWidthCount + bottomrightWidthCount, 1) +
//                    Math.Min(centerCount + center2Count, 1) +
//                    Math.Min(rightCount + bottomrightWidthCount + toprightWidthCount, 1),
//                    Math.Min(bottomCount + bottomleftHeightCount + bottomrightHeightCount, 1) +
//                    Math.Min(topCount + toprightHeightCount + topleftHeightCount, 1) +
//                    Math.Min(middleCount + middle2Count, 1))
//                };
//            }

//            public void CountPositions(IEnumerable<FrameworkElement> elements)
//            {
//                foreach (FrameworkElement child in elements)
//                {
//                    totalCount++;
//                    var region = RegionPanel.GetRegion(child);
//                    //var heightSizing = EdgeLegacyPanel.GetHeightSizing(child);
//                    //var widthSizing = EdgeLegacyPanel.GetWidthSizing(child);

//                    int widthCount = 1;
//                    int heightCount = 1;

//                    switch (region)
//                    {
//                        case Region.TopLeft:
//                            topleft.Add(child);
//                            topleftWidthCount += widthCount;
//                            topleftHeightCount += heightCount;
//                            break;
//                        case Region.BottomLeft:
//                            bottomleft.Add(child);
//                            bottomleftWidthCount += widthCount;
//                            bottomleftHeightCount += heightCount;
//                            break;
//                        case Region.BottomRight:
//                            bottomright.Add(child);
//                            bottomrightWidthCount += widthCount;
//                            bottomrightHeightCount += heightCount;
//                            break;
//                        case Region.TopRight:
//                            topright.Add(child);
//                            toprightWidthCount += widthCount;
//                            toprightHeightCount += heightCount;
//                            break;

//                        case Region.Left:
//                            left.Add(child);
//                            leftCount += widthCount;
//                            middle2Count = 1;
//                            break;

//                        case Region.Right:
//                            right.Add(child);
//                            rightCount += widthCount;
//                            middle2Count = 1;
//                            break;

//                        case Region.Top:
//                            top.Add(child);
//                            center2Count = 1;
//                            topCount += heightCount;
//                            break;

//                        case Region.Bottom:
//                            bottom.Add(child);
//                            center2Count = 1;
//                            bottomCount += heightCount;
//                            break;

//                        case Region.Middle:
//                            middle.Add(child);
//                            centerCount += widthCount;
//                            middleCount += heightCount;
//                            break;

//                    };
//                }
//            }

//            public IList<FrameworkElement> SelectElements(Region region)
//            {
//                return SelectElements(region, this);

//                static IList<FrameworkElement> SelectElements(Region region, ElementsBag bag)
//                {
//                    return region switch
//                    {
//                        Region.Right => bag.right,
//                        Region.Left => bag.left,
//                        Region.Bottom => bag.bottom,
//                        Region.Top => bag.top,
//                        Region.TopRight => bag.topright,
//                        Region.BottomLeft => bag.bottomleft,
//                        Region.BottomRight => bag.bottomright,
//                        Region.TopLeft => bag.topleft,
//                        Region.Middle => bag.middle,
//                        _ => throw new NotImplementedException("SelectElements"),
//                    };
//                }
//            }
//        }
//        //public enum Region
//        //{
//        //    TopLeft,
//        //    Top,
//        //    TopRight,
//        //    Right,
//        //    BottomRight,
//        //    Bottom,
//        //    BottomLeft,
//        //    Left,
//        //    Middle
//        //}
//    }

//}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Utility.WPF.Panels;

namespace Utility.WPF.Demo.Panels
{
    public enum CircleRegion
    {
        TopLeft,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Left,
        Middle
    }

    public enum Sizing
    {
        Auto,
        FromParent
    }

    public class EdgeLegacyPanel : Panel
    {
        #region DependencyProperties

        public double HeightRatio
        {
            get { return (double)GetValue(HeightRatioProperty); }
            set { SetValue(HeightRatioProperty, value); }
        }

        public static readonly DependencyProperty HeightRatioProperty = DependencyProperty.Register("HeightRatio", typeof(double),
            typeof(EdgeLegacyPanel), new FrameworkPropertyMetadata(0.5d, OnCorner2Changed));

        public double WidthRatio
        {
            get { return (double)GetValue(WidthRatioProperty); }
            set { SetValue(WidthRatioProperty, value); }
        }

        public static readonly DependencyProperty WidthRatioProperty =
            DependencyProperty.Register("WidthRatio", typeof(double), typeof(EdgeLegacyPanel),
                new FrameworkPropertyMetadata(0.5d, OnCorner2Changed));

        private static void OnCorner2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EdgeLegacyPanel uie)
            {
                uie.InvalidateMeasure();
            }
        }

        public static readonly DependencyProperty CircleRegionProperty = DependencyProperty.RegisterAttached("CircleRegion", typeof(CircleRegion), typeof(EdgeLegacyPanel),
                new PropertyMetadata(CircleRegion.Top, new PropertyChangedCallback(OnCornerChanged)), new ValidateValueCallback(IsValidRegion));

        private static bool IsValidRegion(object value)
        {
            return true;
        }

        /// <summary>
        /// Reads the attached property Dock from the given element.
        /// </summary>
        /// <param name="element">UIElement from which to read the attached property.</param>
        /// <returns>The property's value.</returns>
        /// <seealso cref="CircleRegionProperty" />
        [AttachedPropertyBrowsableForChildren()]
        public static CircleRegion GetCircleRegion(UIElement element)
        {
            return element != null ? (CircleRegion)element.GetValue(CircleRegionProperty) : throw new ArgumentNullException("element");
        }

        /// <summary>
        /// Writes the attached property Dock to the given element.
        /// </summary>
        /// <param name="element">UIElement to which to write the attached property.</param>
        /// <param name="dock">The property value to set</param>
        /// <seealso cref="CircleRegionProperty" />
        public static void SetCircleRegion(UIElement element, CircleRegion dock)
        {
            if (element == null) { throw new ArgumentNullException("element"); }

            element.SetValue(CircleRegionProperty, dock);
        }

        private static void OnCornerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement uie && VisualTreeHelper.GetParent(uie) is EdgeLegacyPanel p)
            {
                p.InvalidateMeasure();
            }
        }

        public static readonly DependencyProperty HeightSizingProperty = DependencyProperty.RegisterAttached("HeightSizing", typeof(Sizing), typeof(EdgeLegacyPanel),
                new FrameworkPropertyMetadata(Sizing.Auto, new PropertyChangedCallback(OnHeightSizingChanged)), new ValidateValueCallback(IsValid2Corner));

        private static bool IsValid2Corner(object value)
        {
            return true;
        }

        /// <summary>
        /// Reads the attached property Dock from the given element.
        /// </summary>
        /// <param name="element">UIElement from which to read the attached property.</param>
        /// <returns>The property's value.</returns>
        /// <seealso cref="SizingProperty" />
        [AttachedPropertyBrowsableForChildren()]
        public static Sizing GetHeightSizing(UIElement element)
        {
            return element != null ? (Sizing)element.GetValue(HeightSizingProperty) : throw new ArgumentNullException("element");
        }

        /// <summary>
        /// Writes the attached property Dock to the given element.
        /// </summary>
        /// <param name="element">UIElement to which to write the attached property.</param>
        /// <param name="dock">The property value to set</param>
        /// <seealso cref="SizingProperty" />
        public static void SetHeightSizing(UIElement element, Sizing dock)
        {
            if (element == null) { throw new ArgumentNullException("element"); }

            element.SetValue(HeightSizingProperty, dock);
        }

        private static void OnHeightSizingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement uie && VisualTreeHelper.GetParent(uie) is EdgeLegacyPanel p)
            {
                p.InvalidateMeasure();
            }
        }

        public static readonly DependencyProperty WidthSizingProperty = DependencyProperty.RegisterAttached("WidthSizing", typeof(Sizing), typeof(EdgeLegacyPanel),
                new FrameworkPropertyMetadata(Sizing.Auto, new PropertyChangedCallback(OnWidthSizingChanged)), new ValidateValueCallback(IsValid3Corner));

        private static bool IsValid3Corner(object value)
        {
            return true;
        }

        /// <summary>
        /// Reads the attached property Dock from the given element.
        /// </summary>
        /// <param name="element">UIElement from which to read the attached property.</param>
        /// <returns>The property's value.</returns>
        /// <seealso cref="SizingProperty" />
        [AttachedPropertyBrowsableForChildren()]
        public static Sizing GetWidthSizing(UIElement element)
        {
            return element != null ? (Sizing)element.GetValue(WidthSizingProperty) : throw new ArgumentNullException("element");
        }

        /// <summary>
        /// Writes the attached property Dock to the given element.
        /// </summary>
        /// <param name="element">UIElement to which to write the attached property.</param>
        /// <param name="dock">The property value to set</param>
        /// <seealso cref="SizingProperty" />
        public static void SetWidthSizing(UIElement element, Sizing dock)
        {
            if (element == null) { throw new ArgumentNullException("element"); }

            element.SetValue(WidthSizingProperty, dock);
        }

        private static void OnWidthSizingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement uie && VisualTreeHelper.GetParent(uie) is EdgeLegacyPanel p)
            {
                p.InvalidateMeasure();
            }
        }

        #endregion DependencyProperties

        //readonly Dictionary<CircleRegion, Sizer> sizes = new Dictionary<CircleRegion, Sizer>();
        //readonly Dictionary<CircleRegion, List<(Point, Vector, UIElement)>> list = new Dictionary<CircleRegion, List<(Point, Vector, UIElement)>>();

        private double leftRequiredWidth = 0, centerRequiredWidth = 0, rightRequiredWidth = 0;
        private double topRequiredHeight = 0, middleRequiredHeight = 0, bottomRequiredHeight = 0;

        private double leftRegionWidth = 0, centerRegionWidth = 0, rightRegionWidth = 0;
        private double topRegionHeight = 0, middleRegionHeight = 0, bottomRegionHeight = 0;

        private double actualLeftWidth;
        private double actualCenterWidth;
        private double actualRightWidth;
        private double actualBottomHeight;
        private double actualMiddleHeight;
        private double actualTopHeight;

        private int actualRightOffset;
        private int actualLeftOffset;
        private int actualBottomOffset;
        private int actualMiddleOffset;
        private int actualTopOffset;

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Children.Count > 0)
            {
                var aa = EdgeLegacyPanelHelper.MeasureTwo(Children.Cast<UIElement>(), availableSize);

                actualLeftWidth = aa.actualLeftWidth;
                actualCenterWidth = aa.actualCenterWidth;
                actualRightWidth = aa.actualRightWidth;
                actualBottomHeight = aa.actualBottomHeight;
                actualMiddleHeight = aa.actualMiddleHeight;
                actualTopHeight = aa.actualTopHeight;

                actualRightOffset = aa.actualRightOffset;
                actualBottomOffset = aa.actualBottomOffset;
                actualMiddleOffset = aa.actualMiddleOffset;
                actualTopOffset = aa.actualTopOffset;
                actualLeftOffset = aa.actualLeftOffset;
            }

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            TranslateTransform trans = null;
            double curX = 0, curY = 0, curLineHeight = 0;

            if (Children.Count > 0)
            {
                // Center & radius of panel
                Point center = new Point(finalSize.Width / 2, finalSize.Height / 2);

                Dictionary<CircleRegion, ISizer> sizes = new Dictionary<CircleRegion, ISizer>();
                List<(Point, Vector, UIElement)> list = new List<(Point, Vector, UIElement)>();

                foreach (UIElement child in Children)
                {
                    trans = child.RenderTransform as TranslateTransform;
                    if (trans == null)
                    {
                        child.RenderTransformOrigin = new Point(0, 0);
                        trans = new TranslateTransform();
                        child.RenderTransform = trans;
                    }
                    var region = GetCircleRegion(child);
                    var heightSizing = GetHeightSizing(child);
                    var widthSizing = GetWidthSizing(child);
                    var regionSize = GetRegionSize(region, heightSizing, widthSizing);
                    var individualSize = GetIndividualSize(region, heightSizing, widthSizing);
                    var size = GetSize(individualSize, child.DesiredSize, heightSizing, widthSizing);
                    size = new Vector(child.DesiredSize.Width == 0 || widthSizing == Sizing.FromParent ? size.X : child.DesiredSize.Width,
                        child.DesiredSize.Height == 0 || heightSizing == Sizing.FromParent ? size.Y : child.DesiredSize.Height);

                    var distance = GetDistance(region, new Vector(finalSize.Width - size.X, finalSize.Height - size.Y));

                    var sizer = sizes[region] = sizes.GetValueOrDefault(region, SizerFactory.Create(regionSize, region, GetOffset(region)));
                    var sizeAdjustment = new Vector(0, 0);
                    var last = sizer.Append(size);

                    var childPosition = GetChildPosition(center, sizeAdjustment, distance);

                    var combinedPoint = new Point(childPosition.X + last.X, childPosition.Y + last.Y);
                    list.Add((last, sizeAdjustment, child));

                    var translationPoint = child.TranslatePoint(new Point(0, 0), this);

                    AnimationHelper.Animate(trans, translationPoint, combinedPoint);
                    child.Arrange(new Rect(new Point(translationPoint.X, translationPoint.Y), size));
                }

                var actualfinalSize = sizes.Aggregate(new Size(), (a, b) =>
                 Add(b.Value.GetTotalSize(), a));

                return finalSize;
            }

            return finalSize;
        }

        public void AddLine()
        {
            var myLine = new Line();
            myLine.Stroke = Brushes.LightSteelBlue;
            myLine.X1 = 1;
            myLine.X2 = 50;
            myLine.Y1 = 1;
            myLine.Y2 = 50;
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.StrokeThickness = 2;
            this.Children.Add(myLine);
            /// myLine.Arrange(new Rect(new Point(1, 1), new Size(49, 49)));
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
        }

        private static Size Add(Size sizeA, Size sizeB)
        {
            return new Size(sizeA.Width + sizeB.Width, sizeA.Height + sizeB.Height);
        }

        private static Point GetChildPosition(Point center, Vector sizeAdjustment, Point radius)
        {
            Point childPosition = new Point(
                radius.X + sizeAdjustment.X + center.X,
                radius.Y + sizeAdjustment.Y + center.Y);
            return childPosition;
        }

        private static Vector GetSizeAdjustment(CircleRegion region, Size size)
        {
            var width = -size.Width / 2;
            var height = -size.Height / 2;
            return new Vector(width, height);
        }

        public Vector GetIndividualSize(CircleRegion region, Sizing sizingHeight, Sizing sizingWidth)
        {
            //var h = sizingHeight == Sizing.FromParent;
            //var w = sizingWidth == Sizing.FromParent;

            return region switch
            {
                CircleRegion.TopLeft => new Vector(leftRequiredWidth, topRequiredHeight),
                CircleRegion.TopRight => new Vector(rightRequiredWidth, topRequiredHeight),
                CircleRegion.BottomRight => new Vector(rightRequiredWidth, bottomRequiredHeight),
                CircleRegion.BottomLeft => new Vector(leftRequiredWidth, bottomRequiredHeight),

                CircleRegion.Top => new Vector(centerRequiredWidth, topRequiredHeight),
                CircleRegion.Bottom => new Vector(centerRequiredWidth, bottomRequiredHeight),

                CircleRegion.Right => new Vector(rightRequiredWidth, middleRequiredHeight),
                CircleRegion.Left => new Vector(leftRequiredWidth, middleRequiredHeight),

                CircleRegion.Middle => new Vector(centerRequiredWidth, middleRequiredHeight),
                _ => throw new NotImplementedException(),
            };
        }

        public Vector GetRegionSize(CircleRegion region, Sizing sizingHeight, Sizing sizingWidth)
        {
            //var h = sizingHeight == Sizing.FromParent;
            //var w = sizingWidth == Sizing.FromParent;

            return region switch
            {
                CircleRegion.TopLeft => new Vector(leftRegionWidth + actualLeftWidth, topRegionHeight + actualTopHeight),
                CircleRegion.TopRight => new Vector(rightRegionWidth + actualRightWidth, topRegionHeight + actualTopHeight),
                CircleRegion.BottomRight => new Vector(rightRegionWidth + actualRightWidth, bottomRegionHeight + actualBottomHeight),
                CircleRegion.BottomLeft => new Vector(leftRegionWidth + actualLeftWidth, bottomRegionHeight + actualBottomHeight),

                CircleRegion.Top => new Vector(centerRegionWidth + actualCenterWidth, topRegionHeight + actualTopHeight),
                CircleRegion.Bottom => new Vector(centerRegionWidth + actualCenterWidth, bottomRegionHeight + actualBottomHeight),

                CircleRegion.Right => new Vector(rightRegionWidth + actualRightWidth, middleRegionHeight + actualMiddleHeight),
                CircleRegion.Left => new Vector(leftRegionWidth + actualLeftWidth, middleRegionHeight + actualMiddleHeight),

                CircleRegion.Middle => new Vector(centerRegionWidth + actualCenterWidth, middleRegionHeight + actualMiddleHeight),
                _ => throw new NotImplementedException(),
            };
        }

        //public Vector GetRegionSize(CircleRegion region, Sizing sizingHeight, Sizing sizingWidth)
        //{
        //    var h = sizingHeight == Sizing.FromParent;
        //    var w = sizingWidth == Sizing.FromParent;

        //    return region switch
        //    {
        //        CircleRegion.TopLeft => new Vector(w ? leftRequiredWidth : actualLeftWidth, h ? topRequiredHeight : actualTopHeight),
        //        CircleRegion.TopRight => new Vector(w ? rightRequiredWidth : actualRightWidth, h ? topRequiredHeight : actualTopHeight),
        //        CircleRegion.BottomRight => new Vector(w ? rightRequiredWidth : actualRightWidth, h ? bottomRequiredHeight : actualBottomHeight),
        //        CircleRegion.BottomLeft => new Vector(w ? leftRequiredWidth : actualLeftWidth, h ? bottomRequiredHeight : actualBottomHeight),

        //        CircleRegion.Top => new Vector(w ? centerRequiredWidth : actualCenterWidth, h ? topRequiredHeight : actualTopHeight),
        //        CircleRegion.Bottom => new Vector(w ? centerRequiredWidth : actualCenterWidth, h ? bottomRequiredHeight : actualBottomHeight),

        //        CircleRegion.Right => new Vector(w ? rightRequiredWidth : actualRightWidth, h ? middleRequiredHeight : actualMiddleHeight),
        //        CircleRegion.Left => new Vector(w ? leftRequiredWidth : actualLeftWidth, h ? middleRequiredHeight : actualMiddleHeight),

        //        CircleRegion.Middle => new Vector(w ? centerRequiredWidth : actualCenterWidth, h ? middleRequiredHeight : actualMiddleHeight),
        //        _ => throw new NotImplementedException(),
        //    };

        //}

        private Vector GetSize(Vector adjustedSize, Size actualSize, Sizing sizingHeight, Sizing sizingWidth)
        {
            return new Vector(sizingWidth == Sizing.Auto ? actualSize.Width : adjustedSize.X,
                sizingHeight == Sizing.Auto ? actualSize.Height : adjustedSize.Y);
        }

        private Point GetDistance(CircleRegion CircleRegion, Vector size)
        {
            return CircleRegion switch
            {
                CircleRegion.TopLeft => new Point(-size.X / 2, -size.Y / 2),
                CircleRegion.TopRight => new Point(size.X / 2, -size.Y / 2),
                CircleRegion.BottomRight => new Point(size.X / 2, size.Y / 2),
                CircleRegion.BottomLeft => new Point(-size.X / 2, size.Y / 2),

                CircleRegion.Top => new Point(0, -size.Y / 2),
                CircleRegion.Bottom => new Point(0, size.Y / 2),

                CircleRegion.Right => new Point(size.X / 2, 0),
                CircleRegion.Left => new Point(-size.X / 2, 0),

                CircleRegion.Middle => new Point(0, 0),
                _ => throw new NotImplementedException(),
            };
        }

        public int GetOffset(CircleRegion CircleRegion)
        {
            return CircleRegion switch
            {
                CircleRegion.TopLeft => 0,
                CircleRegion.TopRight => 0,
                CircleRegion.BottomRight => 0,
                CircleRegion.BottomLeft => 0,

                CircleRegion.Top => actualTopOffset,
                CircleRegion.Bottom => actualBottomOffset,

                CircleRegion.Right => actualRightOffset,
                CircleRegion.Left => actualLeftOffset,
                CircleRegion.Middle => 0,
                _ => throw new NotImplementedException(),
            };
        }

        private double GetHypotonuse((double, double) value)
        {
            return Math.Sqrt(Math.Pow(value.Item1, 2) + (Math.Pow(value.Item2, 2)));
        }

        private double GetAngle(CircleRegion CircleRegion)
        {
            return CircleRegion switch
            {
                CircleRegion.TopLeft => 315,
                CircleRegion.Top => 0,
                CircleRegion.TopRight => 45,
                CircleRegion.Right => 90,
                CircleRegion.BottomRight => 135,
                CircleRegion.Bottom => 180,
                CircleRegion.BottomLeft => 225,
                CircleRegion.Left => 270,
                CircleRegion.Middle => 0,
                _ => throw new NotImplementedException(),
            };
        }

        //public Vector GetOffSetAdjustment(CircleRegion region, Vector size, int index)
        //{
        //    return region switch
        //    {
        //        //CircleRegion.TopLeft => new Vector(w ? leftRequiredWidth : actualLeftWidth, h ? topRequiredHeight : actualTopHeight),
        //        //CircleRegion.TopRight => new Vector(w ? rightRequiredWidth : actualRightWidth, h ? topRequiredHeight : actualTopHeight),
        //        //CircleRegion.BottomRight => new Vector(w ? rightRequiredWidth : actualRightWidth, h ? bottomRequiredHeight : actualBottomHeight),
        //        //CircleRegion.BottomLeft => new Vector(w ? leftRequiredWidth : actualLeftWidth, h ? bottomRequiredHeight : actualBottomHeight),

        //        //CircleRegion.Top => new Vector(w ? centerRequiredWidth : actualCenterWidth, h ? topRequiredHeight : actualTopHeight),
        //        CircleRegion.Top => new Vector(-size.X * index / actualCenterOffset, 0),

        //        //CircleRegion.Right => new Vector(w ? rightRequiredWidth : actualRightWidth, h ? middleRequiredHeight : actualMiddleHeight),
        //        //CircleRegion.Left => new Vector(w ? leftRequiredWidth : actualLeftWidth, h ? middleRequiredHeight : actualMiddleHeight),

        //        //CircleRegion.Middle => new Vector(w ? centerRequiredWidth : actualCenterWidth, h ? middleRequiredHeight : actualMiddleHeight),

        //        _ => new Vector(0, 0),
        //    };
        //}
    }

    public abstract class AxisSizer : ISizer
    {
        protected readonly Vector sizeAllowed;
        protected readonly int offsetCount;
        protected readonly int direction;
        protected List<List<Vector>> vectors = new List<List<Vector>> { new List<Vector>() };

        public AxisSizer(Vector sizeAllowed, int offsetCount, int direction = 1)
        {
            this.sizeAllowed = sizeAllowed;
            this.offsetCount = offsetCount;
            this.direction = direction;
        }

        public abstract Point Append(Vector size);

        public Point Append(Size size)
        {
            return this.Append(new Vector(size.Width, size.Height));
        }

        public Size GetTotalSize()
        {
            return new Size(Math.Max(sizeAllowed.X, 0), Math.Max(sizeAllowed.Y, 0));
        }

        //public static Sizer Create(CircleRegion circleRegion = CircleRegion.TopLeft)
        //{
        //    switch (circleRegion)
        //    {
        //        case CircleRegion.Bottom:
        //            return new Sizer(new YOrganiser<Vector>(), 0, -1);
        //        case CircleRegion.Top:
        //            return new Sizer(new YOrganiser<Vector>(), 0, 1);
        //        case CircleRegion.Right:
        //            return new Sizer(new XOrganiser<Vector>(), -1, 0);
        //        case CircleRegion.Left:
        //            return new Sizer(new XOrganiser<Vector>(), 1, 0);

        //        default:
        //            throw new ArgumentOutOfRangeException();

        //    }
        //}

        public static int GetCount(int number)
        {
            return (int)Math.Sqrt(number - 1) + 1;
        }
    }

    public class YAxisSizer : AxisSizer
    {
        public YAxisSizer(Vector sizeAllowed, int offsetCount, int direction = 1) : base(sizeAllowed, offsetCount, direction)
        {
        }

        public override Point Append(Vector size)
        {
            double width = 0d, height = 0d;

            var lastColumn = vectors.Last();
            if (lastColumn.Sum(a => a.Y) + size.Y > sizeAllowed.Y)
                vectors.Add(lastColumn = new List<Vector> { size });
            else
                lastColumn.Add(size);
            if (vectors.Count > 1)
                width = vectors.Take(vectors.Count - 1).Sum(a => a.DefaultIfEmpty(new Vector()).Max(c => c.X));

            if (lastColumn.Count > 1)
                height = lastColumn.Take(lastColumn.Count - 1).Sum(c => c.Y);

            var offSet = size.X * (-(offsetCount / 2d) + (vectors.Count - 1));
            return new Point(offSet, direction * height - size.Y / 2d);
        }
    }

    public class XAxisSizer : AxisSizer
    {
        public XAxisSizer(Vector sizeAllowed, int offsetCount, int direction = 1) : base(sizeAllowed, offsetCount, direction)
        {
        }

        public override Point Append(Vector size)
        {
            double width = 0d, height = 0d;

            var lastRow = vectors.Last();
            if (lastRow.Sum(a => a.X) + size.X > sizeAllowed.X)
                vectors.Add(lastRow = new List<Vector> { size });
            else
                lastRow.Add(size);
            if (vectors.Count > 1)
                height = vectors.Take(vectors.Count - 1).Sum(a => a.DefaultIfEmpty(new Vector()).Max(c => c.Y));

            if (lastRow.Count > 1)
                width = lastRow.Take(lastRow.Count - 1).Sum(c => c.X);

            var offSet = size.Y * (-(offsetCount / 2d) + (vectors.Count - 1));
            return new Point(direction * width - size.X / 2d, offSet);
        }
    }

    public class SizerFactory
    {
        public static ISizer Create(Vector size = default, CircleRegion circleRegion = CircleRegion.TopLeft, int offSetCount = 1)
        {
            switch (circleRegion)
            {
                case CircleRegion.Bottom:
                    return new YAxisSizer(size, offSetCount, -1);

                case CircleRegion.Top:
                    return new YAxisSizer(size, offSetCount);

                case CircleRegion.Right:
                    return new XAxisSizer(size, offSetCount, -1);

                case CircleRegion.Left:
                    return new XAxisSizer(size, offSetCount);

                case CircleRegion.TopRight:
                    return new SizerUtility(new XYOrganiser<Vector>(), -1, 1);

                case CircleRegion.BottomRight:
                    return new SizerUtility(new XYOrganiser<Vector>(), -1, -1);

                case CircleRegion.BottomLeft:
                    return new SizerUtility(new XYOrganiser<Vector>(), 1, -1);

                case CircleRegion.TopLeft:
                    return new SizerUtility(new XYOrganiser<Vector>(), 1, 1);

                case CircleRegion.Middle:
                    return new SizerUtility(new XYOrganiser<Vector>(), 0, 0);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public interface ISizer
    {
        Point Append(Size size);

        Point Append(Vector size);

        Size GetTotalSize();
    }

    public class SizerUtility : ISizer
    {
        private readonly IOrganiser<Vector> organiser;
        private readonly int xFactor;
        private readonly int yFactor;

        public SizerUtility(IOrganiser<Vector> organiser, int xFactor, int yFactor)
        {
            this.organiser = organiser;
            this.xFactor = xFactor;
            this.yFactor = yFactor;
        }

        public Point Append(Vector size)
        {
            var (x, y) = organiser.Add(size);
            var xValue = x.GroupBy(a => a.Item1).Select(a => a.Max(c => c.Item2.X * xFactor)).Sum();
            var yValue = y.GroupBy(a => a.Item1).Select(a => a.Max(c => c.Item2.Y * yFactor)).Sum();
            //return new Point(xValue , yValue );
            return new Point(xValue, yValue);
        }

        public Point Append(Size size)
        {
            return this.Append(new Vector(size.Width, size.Height));
        }

        public Size GetTotalSize()
        {
            var xValue = organiser.Values.GroupBy(a => a.Key.Item1).Select(a => a.Max(c => c.Value.X)).Sum();
            var yValue = organiser.Values.GroupBy(a => a.Key.Item2).Select(a => a.Max(c => c.Value.Y)).Sum();
            return new Size(xValue, yValue);
        }

        public static int GetCount(int number)
        {
            return (int)Math.Sqrt(number - 1) + 1;
        }
    }

    public class XYOrganiser<T> : IOrganiser<T> where T : struct
    {
        public Dictionary<(int?, int?), T> Values { get; } = new Dictionary<(int?, int?), T>();

        private int j = 0;
        private int x = 0, y = 0;

        public ((int?, T)[], (int?, T)[]) Add(T value)
        {
            var xValue = Values.Where(a => a.Key.Item1 < x).Select(a => (a.Key.Item1, a.Value)).ToArray();
            var yValue = Values.Where(a => a.Key.Item2 < y).Select(a => (a.Key.Item2, a.Value)).ToArray();

            Values.Add((x, y), value);

            if (y == 0)
                y = x = ++j;
            else if (x > 0 && y == j)
                x--;
            else if (y > 0 || x == 0)
            {
                y--;
                x = j;
            }

            return (xValue, yValue);
        }
    }

    //public class XOrganiser<T> : IOrganiser<T> where T : struct
    //{
    //    public Dictionary<(int?, int?), T> Values { get; } = new Dictionary<(int?, int?), T>();

    //    int x = 0, y = 0;
    //    private readonly int count;

    //    public XOrganiser(int count)
    //    {
    //        this.count = count;
    //    }

    //    public ((int?, T)[], (int?, T)[]) Add(T value)
    //    {
    //        var xValue = Values.Where(a => a.Key.Item1 < x).Select(a => (a.Key.Item1, a.Value)).ToArray();
    //        var yValue = Values.Where(a => a.Key.Item2 < y).Select(a => (a.Key.Item2, a.Value)).ToArray();

    //        Values.Add((x, y), value);

    //        if (++x >= count)
    //        {
    //            y++;
    //            x = 0;
    //        }
    //        return (xValue, yValue);
    //    }
    //}

    //public class YOrganiser<T> : IOrganiser<T> where T : struct
    //{
    //    public Dictionary<(int?, int?), T> Values { get; } = new Dictionary<(int?, int?), T>();

    //    int j = 0;
    //    int x = 0, y = 0;

    //    public ((int?, T)[], (int?, T)[]) Add(T value)
    //    {
    //        var xValue = Values.Where(a => a.Key.Item1 < x).Select(a => (a.Key.Item1, a.Value)).ToArray();
    //        var yValue = Values.Where(a => a.Key.Item2 < y).Select(a => (a.Key.Item2, a.Value)).ToArray();

    //        Values.Add((x, y), value);

    //        y = ++j;

    //        return (xValue, yValue);
    //    }
    //}

    public interface IOrganiser<T> where T : struct
    {
        Dictionary<(int?, int?), T> Values { get; }

        ((int?, T)[], (int?, T)[]) Add(T value);
    }
}

//switch (region)
//{
//    case CircleRegion.BottomLeft:
//        {
//            child.Arrange(new Rect(new Point(combinedSize.X, combinedSize.Y), size));
//            break;
//        }
//    case CircleRegion.BottomRight:
//        {
//            child.Arrange(new Rect(new Point(combinedSize.X, last.Y), size));
//            break;
//        }
//    case CircleRegion.TopLeft:
//        {
//            child.Arrange(new Rect(new Point(last.X, last.Y), size));
//            break;
//        }
//    case CircleRegion.TopRight:
//        {
//            child.Arrange(new Rect(new Point(last.X, last.Y), size));
//            break;
//        }
//    case CircleRegion.Right:
//        {
//            child.Arrange(new Rect(new Point(-last.X, 0), size));
//            break;
//        }
//    case CircleRegion.Left:
//        {
//            child.Arrange(new Rect(new Point(last.X, 0), size));
//            break;
//        }
//    case CircleRegion.Top:
//        {
//            child.Arrange(new Rect(new Point(last.X, last.Y), size));
//            break;
//        }
//    case CircleRegion.Bottom:
//        {
//            child.Arrange(new Rect(new Point(childPosition.X, childPosition.Y), child.DesiredSize));
//            break;
//        }
//    case CircleRegion.Middle:
//        {
//            child.Arrange(new Rect(new Point(childPosition.X, childPosition.Y), child.DesiredSize));
//            break;
//        }

//}
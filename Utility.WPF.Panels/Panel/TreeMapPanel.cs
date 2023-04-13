//using System;
//using System.Windows;
//using System.Windows.Controls;
//using System.Diagnostics;
//using System.Windows.Media;

//using System.Collections.Generic;

//using System.Linq;
//using System.Diagnostics.Contracts;
//using System.Threading;
//using PixelLab.Common;

////using PixelLab.Common;
//#if CONTRACTS_FULL
//using System.Diagnostics.Contracts;
//#else
////using PixelLab.Contracts;
//#endif

//namespace PixelLab.Wpf
//{
//    public class TreeMapPanel : AnimatingPanel
//    {
//        public static readonly DependencyProperty AreaProperty =
//            DependencyProperty.RegisterAttached(
//                "Area",
//                typeof(double),
//                typeof(TreeMapPanel),
//                new FrameworkPropertyMetadata(
//                    1.0,
//                    FrameworkPropertyMetadataOptions.AffectsParentArrange));

//        public static double GetArea(DependencyObject element)
//        {
//            //Contract.Requires<ArgumentException>(element != null);

//            return (double)element.GetValue(AreaProperty);
//        }

//        public static void SetArea(DependencyObject element, double value)
//        {
//            //Contract.Requires<ArgumentException>(element != null);

//            element.SetValue(AreaProperty, value);
//        }

//        protected override Size ArrangeOverride(Size finalSize)
//        {
//            if (finalSize.Width < c_tolerance || finalSize.Height < c_tolerance)
//            {
//                return finalSize;
//            }

//            UIElementCollection children = InternalChildren;
//            ComputeWeightMap(children);

//            Rect strip = new Rect(finalSize);
//            double remainingWeight = m_totalWeight;

//            int arranged = 0;
//            while (arranged < children.Count)
//            {
//                double bestStripWeight = 0;
//                double bestRatio = double.PositiveInfinity;

//                int i;

//                if (finalSize.Width < c_tolerance || finalSize.Height < c_tolerance)
//                {
//                    return finalSize;
//                }

//                if (strip.Width > strip.Height)
//                {
//                    double bestWidth = strip.Width;

//                    // Arrange Vertically
//                    for (i = arranged; i < children.Count; i++)
//                    {
//                        double stripWeight = bestStripWeight + GetWeight(i);
//                        double ratio = double.PositiveInfinity;
//                        double width = strip.Width * stripWeight / remainingWeight;

//                        for (int j = arranged; j <= i; j++)
//                        {
//                            double height = strip.Height * GetWeight(j) / stripWeight;
//                            ratio = Math.Min(ratio, height > width ? height / width : width / height);

//                            if (ratio > bestRatio)
//                            {
//                                goto ArrangeVertical;
//                            }
//                        }
//                        bestRatio = ratio;
//                        bestWidth = width;
//                        bestStripWeight = stripWeight;
//                    }

//                ArrangeVertical:
//                    double y = strip.Y;
//                    for (; arranged < i; arranged++)
//                    {
//                        UIElement child = GetChild(children, arranged);

//                        double height = strip.Height * GetWeight(arranged) / bestStripWeight;
//                        ArrangeChild(child, new Rect(strip.X, y, bestWidth, height));
//                        y += height;
//                    }

//                    strip.X = strip.X + bestWidth;
//                    strip.Width = Math.Max(0.0, strip.Width - bestWidth);
//                }
//                else
//                {
//                    double bestHeight = strip.Height;

//                    // Arrange Horizontally
//                    for (i = arranged; i < children.Count; i++)
//                    {
//                        double stripWeight = bestStripWeight + GetWeight(i);
//                        double ratio = double.PositiveInfinity;
//                        double height = strip.Height * stripWeight / remainingWeight;

//                        for (int j = arranged; j <= i; j++)
//                        {
//                            double width = strip.Width * GetWeight(j) / stripWeight;
//                            ratio = Math.Min(ratio, height > width ? height / width : width / height);

//                            if (ratio > bestRatio)
//                            {
//                                goto ArrangeHorizontal;
//                            }
//                        }
//                        bestRatio = ratio;
//                        bestHeight = height;
//                        bestStripWeight = stripWeight;
//                    }

//                ArrangeHorizontal:
//                    double x = strip.X;
//                    for (; arranged < i; arranged++)
//                    {
//                        UIElement child = GetChild(children, arranged);

//                        double width = strip.Width * GetWeight(arranged) / bestStripWeight;
//                        ArrangeChild(child, new Rect(x, strip.Y, width, bestHeight));
//                        x += width;
//                    }

//                    strip.Y = strip.Y + bestHeight;
//                    strip.Height = Math.Max(0.0, strip.Height - bestHeight);
//                }
//                remainingWeight -= bestStripWeight;
//            }

//            return finalSize;
//        }

//        private UIElement GetChild(UIElementCollection children, int index)
//        {
//            return children[m_weightMap[index]];
//        }

//        private double GetWeight(int index)
//        {
//            return m_weights[m_weightMap[index]];
//        }

//        private void ComputeWeightMap(UIElementCollection children)
//        {
//            m_totalWeight = 0;

//            if (m_weightMap == null || m_weightMap.Length != InternalChildren.Count)
//            {
//                m_weightMap = new int[InternalChildren.Count];
//            }

//            if (m_weights == null || m_weights.Length != InternalChildren.Count)
//            {
//                m_weights = new double[InternalChildren.Count];
//            }

//            for (int i = 0; i < m_weightMap.Length; i++)
//            {
//                m_weightMap[i] = i;
//                m_weights[i] = GetArea(children[i]);
//                m_totalWeight += m_weights[i];
//            }

//            Array.Sort<int>(m_weightMap, compareWeights);
//        }

//        private int compareWeights(int index1, int index2)
//        {
//            return m_weights[index2].CompareTo(m_weights[index1]);
//        }

//        private double m_totalWeight;
//        private int[] m_weightMap;
//        private double[] m_weights;

//        private const double c_tolerance = 1e-2;
//    }
//}

//namespace PixelLab.Wpf
//{
//    public abstract class AnimatingPanel : Panel
//    {
//        protected AnimatingPanel()
//        {
//            m_listener.Rendering += compositionTarget_Rendering;
//            m_listener.WireParentLoadedUnloaded(this);
//        }

//        #region DPs

//        public double Dampening
//        {
//            get { return (double)GetValue(DampeningProperty); }
//            set { SetValue(DampeningProperty, value); }
//        }

//        public static readonly DependencyProperty DampeningProperty =
//            CreateDoubleDP("Dampening", 0.2, FrameworkPropertyMetadataOptions.None, 0, 1, false);

//        public double Attraction
//        {
//            get { return (double)GetValue(AttractionProperty); }
//            set { SetValue(AttractionProperty, value); }
//        }

//        public static readonly DependencyProperty AttractionProperty =
//            CreateDoubleDP("Attraction", 2, FrameworkPropertyMetadataOptions.None, 0, double.PositiveInfinity, false);

//        public double Variation
//        {
//            get { return (double)GetValue(VariationProperty); }
//            set { SetValue(VariationProperty, value); }
//        }

//        public static readonly DependencyProperty VariationProperty =
//            CreateDoubleDP("Variation", 1, FrameworkPropertyMetadataOptions.None, 0, true, 1, true, false);

//        #endregion

//        protected virtual Point ProcessNewChild(UIElement child, Rect providedBounds)
//        {
//            return providedBounds.Location;
//        }

//        protected void ArrangeChild(UIElement child, Rect bounds)
//        {
//            m_listener.StartListening();

//            AnimatingPanelItemData data = (AnimatingPanelItemData)child.GetValue(DataProperty);
//            if (data == null)
//            {
//                data = new AnimatingPanelItemData();
//                child.SetValue(DataProperty, data);
//                Debug.Assert(child.RenderTransform == Transform.Identity);
//                child.RenderTransform = data.Transform;

//                data.Current = ProcessNewChild(child, bounds);
//            }
//            Debug.Assert(child.RenderTransform == data.Transform);

//            //set the location attached DP
//            data.Target = bounds.Location;

//            child.Arrange(bounds);
//        }

//        private void compositionTarget_Rendering(object sender, EventArgs e)
//        {
//            double dampening = this.Dampening;
//            double attractionFactor = this.Attraction * .01;
//            double variation = this.Variation;

//            bool shouldChange = false;
//            for (int i = 0; i < Children.Count; i++)
//            {
//                shouldChange = updateChildData(
//                    (AnimatingPanelItemData)Children[i].GetValue(DataProperty),
//                    dampening,
//                    attractionFactor,
//                    variation) || shouldChange;
//            }

//            if (!shouldChange)
//            {
//                m_listener.StopListening();
//            }
//        }

//        private static bool updateChildData(AnimatingPanelItemData data, double dampening, double attractionFactor, double variation)
//        {
//            if (data == null)
//            {
//                return false;
//            }
//            else
//            {
//                Debug.Assert(dampening > 0 && dampening < 1);
//                Debug.Assert(attractionFactor > 0 && !double.IsInfinity(attractionFactor));

//                attractionFactor *= 1 + (variation * data.RandomSeed - .5);

//                Point newLocation;
//                Vector newVelocity;

//                bool anythingChanged =
//                    GeoHelper.Animate(data.Current, data.LocationVelocity, data.Target,
//                        attractionFactor, dampening, c_terminalVelocity, c_diff, c_diff,
//                        out newLocation, out newVelocity);

//                data.Current = newLocation;
//                data.LocationVelocity = newVelocity;

//                var transformVector = data.Current - data.Target;
//                data.Transform.SetToVector(transformVector);

//                return anythingChanged;
//            }
//        }

//        private readonly CompositionTargetRenderingListener m_listener = new CompositionTargetRenderingListener();

//        protected static DependencyProperty CreateDoubleDP(
//          string name,
//          double defaultValue,
//          FrameworkPropertyMetadataOptions metadataOptions,
//          double minValue,
//          double maxValue,
//          bool attached)
//        {
//            return CreateDoubleDP(name, defaultValue, metadataOptions, minValue, false, maxValue, false, attached);
//        }

//        protected static DependencyProperty CreateDoubleDP(
//            string name,
//            double defaultValue,
//            FrameworkPropertyMetadataOptions metadataOptions,
//            double minValue,
//            bool includeMin,
//            double maxValue,
//            bool includeMax,
//            bool attached)
//        {
//            Contract.Requires(!double.IsNaN(minValue));
//            Contract.Requires(!double.IsNaN(maxValue));
//            Contract.Requires(maxValue >= minValue);

//            ValidateValueCallback validateValueCallback = delegate (object objValue)
//            {
//                double value = (double)objValue;

//                if (includeMin)
//                {
//                    if (value < minValue)
//                    {
//                        return false;
//                    }
//                }
//                else
//                {
//                    if (value <= minValue)
//                    {
//                        return false;
//                    }
//                }
//                if (includeMax)
//                {
//                    if (value > maxValue)
//                    {
//                        return false;
//                    }
//                }
//                else
//                {
//                    if (value >= maxValue)
//                    {
//                        return false;
//                    }
//                }

//                return true;
//            };

//            if (attached)
//            {
//                return DependencyProperty.RegisterAttached(
//                    name,
//                    typeof(double),
//                    typeof(AnimatingTilePanel),
//                    new FrameworkPropertyMetadata(defaultValue, metadataOptions), validateValueCallback);
//            }
//            else
//            {
//                return DependencyProperty.Register(
//                    name,
//                    typeof(double),
//                    typeof(AnimatingTilePanel),
//                    new FrameworkPropertyMetadata(defaultValue, metadataOptions), validateValueCallback);
//            }
//        }

//        private static readonly DependencyProperty DataProperty =
//            DependencyProperty.RegisterAttached("Data", typeof(AnimatingPanelItemData), typeof(AnimatingTilePanel));

//        private const double c_diff = 0.1;
//        private const double c_terminalVelocity = 10000;

//        private class AnimatingPanelItemData
//        {
//            public Point Target;
//            public Point Current;
//            public Vector LocationVelocity;
//            public readonly double RandomSeed = Util.Rnd.NextDouble();
//            public readonly TranslateTransform Transform = new TranslateTransform();
//        }
//    }

//    public static class GeoHelper
//    {
//        [Pure]
//        public static bool IsValid(this double value)
//        {
//            return !double.IsInfinity(value) && !double.IsNaN(value);
//        }

//        [Pure]
//        public static bool IsValid(this Point value)
//        {
//            return value.X.IsValid() && value.Y.IsValid();
//        }

//        [Pure]
//        public static bool IsValid(this Size value)
//        {
//            return value.Width.IsValid() && value.Height.IsValid();
//        }

//        [Pure]
//        public static bool IsValid(this Vector value)
//        {
//            return value.X.IsValid() && value.Y.IsValid();
//        }

//        /// <summary>
//        ///     Returns the scale factor by which an object of size <paramref name="source"/>
//        ///     should be scaled to fit within an object of size <param name="target"/>.
//        /// </summary>
//        /// <param name="target">The target size.</param>
//        /// <param name="size2">The source size.</param>
//        public static double ScaleToFit(Size target, Size source)
//        {
//            Contract.Requires(target.IsValid());
//            Contract.Requires(source.IsValid());
//            Contract.Requires(target.Width > 0);
//            Contract.Requires(source.Width > 0);

//            double targetHWR = target.Height / target.Width;
//            double sourceHWR = source.Height / source.Width;

//            if (targetHWR > sourceHWR)
//            {
//                return target.Width / source.Width;
//            }
//            else
//            {
//                return target.Height / source.Height;
//            }
//        }

//        public static bool Animate(
//            double currentValue, double currentVelocity, double targetValue,
//            double attractionFator, double dampening,
//            double terminalVelocity, double minValueDelta, double minVelocityDelta,
//            out double newValue, out double newVelocity)
//        {
//            Debug.Assert(currentValue.IsValid());
//            Debug.Assert(currentVelocity.IsValid());
//            Debug.Assert(targetValue.IsValid());

//            Debug.Assert(dampening.IsValid());
//            Debug.Assert(dampening > 0 && dampening < 1);

//            Debug.Assert(attractionFator.IsValid());
//            Debug.Assert(attractionFator > 0);

//            Debug.Assert(terminalVelocity > 0);

//            Debug.Assert(minValueDelta > 0);
//            Debug.Assert(minVelocityDelta > 0);

//            double diff = targetValue - currentValue;

//            if (diff.Abs() > minValueDelta || currentVelocity.Abs() > minVelocityDelta)
//            {
//                newVelocity = currentVelocity * (1 - dampening);
//                newVelocity += diff * attractionFator;
//                if (currentVelocity.Abs() > terminalVelocity)
//                {
//                    newVelocity *= terminalVelocity / currentVelocity.Abs();
//                }

//                newValue = currentValue + newVelocity;

//                return true;
//            }
//            else
//            {
//                newValue = targetValue;
//                newVelocity = 0;
//                return false;
//            }
//        }

//        public static bool Animate(
//        Point currentValue, Vector currentVelocity, Point targetValue,
//        double attractionFator, double dampening,
//        double terminalVelocity, double minValueDelta, double minVelocityDelta,
//        out Point newValue, out Vector newVelocity)
//        {
//            Debug.Assert(currentValue.IsValid());
//            Debug.Assert(currentVelocity.IsValid());
//            Debug.Assert(targetValue.IsValid());

//            Debug.Assert(dampening.IsValid());
//            Debug.Assert(dampening > 0 && dampening < 1);

//            Debug.Assert(attractionFator.IsValid());
//            Debug.Assert(attractionFator > 0);

//            Debug.Assert(terminalVelocity > 0);

//            Debug.Assert(minValueDelta > 0);
//            Debug.Assert(minVelocityDelta > 0);

//            Vector diff = targetValue.Subtract(currentValue);

//            if (diff.Length > minValueDelta || currentVelocity.Length > minVelocityDelta)
//            {
//                newVelocity = currentVelocity * (1 - dampening);
//                newVelocity += diff * attractionFator;
//                if (currentVelocity.Length > terminalVelocity)
//                {
//                    newVelocity *= terminalVelocity / currentVelocity.Length;
//                }

//                newValue = currentValue + newVelocity;

//                return true;
//            }
//            else
//            {
//                newValue = targetValue;
//                newVelocity = new Vector();
//                return false;
//            }
//        }

//        public static Vector Subtract(this Point point, Point other)
//        {
//            Contract.Requires(point.IsValid());
//            Contract.Requires(other.IsValid());
//            Contract.Ensures(Contract.Result<Vector>().IsValid());
//            return new Vector(point.X - other.X, point.Y - other.Y);
//        }

//        public static Vector Subtract(this Size size, Size other)
//        {
//            Contract.Requires(size.IsValid());
//            Contract.Requires(other.IsValid());
//            Contract.Ensures(Contract.Result<Vector>().IsValid());
//            return new Vector(size.Width - other.Width, size.Height - other.Height);
//        }

//        public static double Abs(this double value)
//        {
//            return Math.Abs(value);
//        }

//        public static Point GetCenter(this Rect value)
//        {
//            Contract.Requires(!value.IsEmpty);
//            Contract.Ensures(Contract.Result<Point>().IsValid());
//            return new Point(value.X + value.Width / 2, value.Y + value.Height / 2);
//        }

//        public static Rect Expand(this Rect target, double amount)
//        {
//            Contract.Requires(amount >= 0);
//            Contract.Requires(!target.IsEmpty);
//            Contract.Ensures(!Contract.Result<Rect>().IsEmpty);
//            var value = new Rect(target.X - amount, target.Y - amount, target.Width + 2 * amount, target.Height + 2 * amount);
//            Contract.Assume(!value.IsEmpty);
//            return value;
//        }

//        public static Point TopLeft(this Rect rect)
//        {
//            Contract.Requires(!rect.IsEmpty);
//            Contract.Ensures(Contract.Result<Point>().IsValid());
//            return new Point(rect.Left, rect.Top);
//        }

//        public static Point BottomRight(this Rect rect)
//        {
//            Contract.Requires(!rect.IsEmpty);
//            Contract.Ensures(Contract.Result<Point>().IsValid());
//            return new Point(rect.Right, rect.Bottom);
//        }

//        public static Point BottomLeft(this Rect rect)
//        {
//            Contract.Requires(!rect.IsEmpty);
//            Contract.Ensures(Contract.Result<Point>().IsValid());
//            return new Point(rect.Left, rect.Bottom);
//        }

//        public static Point TopRight(this Rect rect)
//        {
//            Contract.Requires(!rect.IsEmpty);
//            Contract.Ensures(Contract.Result<Point>().IsValid());
//            return new Point(rect.Right, rect.Top);
//        }

//        public static Size Size(this Rect rect)
//        {
//            Contract.Requires(!rect.IsEmpty);
//            return new Size(rect.Width, rect.Height);
//        }

//        public static Point ToPoint(this Vector vector)
//        {
//            return (Point)vector;
//        }

//        public static Vector CenterVector(this Size size)
//        {
//            return ((Vector)size) * .5;
//        }

//        public static double AngleRad(Point point1, Point point2, Point point3)
//        {
//            Debug.Assert(point1.IsValid());
//            Debug.Assert(point2.IsValid());
//            Debug.Assert(point3.IsValid());

//            double rad = AngleRad(point2.Subtract(point1), point2.Subtract(point3));

//            double rad2 = AngleRad(point2.Subtract(point1), (point2.Subtract(point3)).RightAngle());

//            if (rad2 < (Math.PI / 2))
//            {
//                return rad;
//            }
//            else
//            {
//                return (Math.PI * 2) - rad;
//            }
//        }

//        public static Vector RightAngle(this Vector vector)
//        {
//            return new Vector(-vector.Y, vector.X);
//        }

//        public static double Dot(Vector v1, Vector v2)
//        {
//            Debug.Assert(v1.IsValid());
//            Debug.Assert(v2.IsValid());

//            return v1.X * v2.X + v1.Y * v2.Y;
//        }

//        public static double AngleRad(Vector v1, Vector v2)
//        {
//            Debug.Assert(v1.IsValid());
//            Debug.Assert(v2.IsValid());

//            double dot = Dot(v1, v2);
//            double dotNormalize = dot / (v1.Length * v2.Length);
//            double acos = Math.Acos(dotNormalize);

//            return acos;
//        }

//        public static Vector GetVectorFromAngle(double angleRadians, double length)
//        {
//            Contract.Requires(angleRadians.IsValid());
//            Contract.Requires(length.IsValid());

//            double x = Math.Cos(angleRadians) * length;
//            double y = -Math.Sin(angleRadians) * length;

//            return new Vector(x, y);
//        }

//        public static readonly Size SizeInfinite = new Size(double.PositiveInfinity, double.PositiveInfinity);
//    }
//}

//namespace PixelLab.Contracts
//{
//    public static class Contract
//    {
//        [DebuggerStepThrough]
//        public static void Requires(bool truth, string message = null)
//        {
//            Util.ThrowUnless(truth, message);
//        }

//        [DebuggerStepThrough]
//        public static void Requires<TException>(bool truth, string message) where TException : Exception
//        {
//            Util.ThrowUnless<TException>(truth, message);
//        }

//        [DebuggerStepThrough]
//        public static void Requires<TException>(bool truth) where TException : Exception, new()
//        {
//            Util.ThrowUnless<TException>(truth);
//        }

//        public static void Assume(bool truth, string message = null)
//        {
//            Debug.Assert(truth, message);
//        }

//        [Conditional("NEVER")]
//        public static void Ensures(bool truth) { }

//        [Conditional("NEVER")]
//        public static void Invariant(bool truth, string message = null)
//        {
//            throw new NotSupportedException();
//        }

//        public static T Result<T>()
//        {
//            throw new NotSupportedException();
//        }

//        public static bool ForAll<T>(IEnumerable<T> source, Func<T, bool> predicate)
//        {
//            return source.All(predicate);
//        }
//    }

//    public class PureAttribute : Attribute { }

//    public class ContractClassAttribute : Attribute
//    {
//        public ContractClassAttribute(Type contractType) { }
//    }

//    public class ContractClassForAttribute : Attribute
//    {
//        public ContractClassForAttribute(Type contractForType) { }
//    }

//    public class ContractInvariantMethodAttribute : Attribute { }
//}

//using System;
//using System.Linq;
//using System.Threading;
//using System.Diagnostics;
//using System.Collections.Generic;
//using System.Reflection;
//#if CONTRACTS_FULL
//using System.Diagnostics.Contracts;
//#else
//using PixelLab.Contracts;
//#endif

//namespace PixelLab.Common
//{
//    /// <summary>
//    ///     Contains general helper methods.
//    /// </summary>
//    public static class Util
//    {
//        /// <summary>
//        /// Returns an hash aggregation of an array of elements.
//        /// </summary>
//        /// <param name="items">An array of elements from which to create a hash.</param>
//        public static int GetHashCode(params object[] items)
//        {
//            items = items ?? new object[0];

//            return items
//                .Select(item => (item == null) ? 0 : item.GetHashCode())
//                .Aggregate(0, (current, next) =>
//                {
//                    unchecked
//                    {
//                        return (current * 397) ^ next;
//                    }
//                });
//        }

//        /// <summary>
//        ///     Wraps <see cref="Interlocked.CompareExchange{T}(ref T,T,T)"/>
//        ///     for atomically setting null fields.
//        /// </summary>
//        /// <typeparam name="T">The type of the field to set.</typeparam>
//        /// <param name="location">
//        ///     The field that, if null, will be set to <paramref name="value"/>.
//        /// </param>
//        /// <param name="value">
//        ///     If <paramref name="location"/> is null, the object to set it to.
//        /// </param>
//        /// <returns>true if <paramref name="location"/> was null and has now been set; otherwise, false.</returns>
//        [Obsolete("The name of this method is pretty wrong. Use InterlockedSetNullField instead.")]
//        public static bool InterlockedSetIfNotNull<T>(ref T location, T value) where T : class
//        {
//            return InterlockedSetNullField<T>(ref location, value);
//        }

//        public static T GetEnumValue<T>(string enumName, bool ignoreCase = false)
//        {
//            Contract.Requires(!enumName.IsNullOrWhiteSpace());
//            Util.ThrowUnless(typeof(T).IsEnum);
//            return (T)Enum.Parse(typeof(T), enumName, ignoreCase);
//        }

//        /// <remarks>This will blow up wonderfully at runtime if T is not an enum type.</remarks>
//        public static Dictionary<T, string> EnumToDictionary<T>()
//        {
//            return GetEnumValues<T>().ToDictionary(v => v, v => Enum.GetName(typeof(T), v));
//        }

//        public static IEnumerable<TEnum> GetEnumValues<TEnum>()
//        {
//            var type = typeof(TEnum);
//            Util.ThrowUnless(type.IsEnum, "The provided type must be an enum");

//#if SILVERLIGHT
//            return GetEnumFields(type).Select(fi => fi.GetRawConstantValue()).Cast<TEnum>();
//#else
//            return Enum.GetValues(type).Cast<TEnum>();
//#endif
//        }

//        /// <remarks>If a field doesn't have the defined attribute, null is provided. If a field has an attribute more than once, it causes an exception.</remarks>
//        public static IDictionary<TEnum, TAttribute> GetEnumValueAttributes<TEnum, TAttribute>() where TAttribute : Attribute
//        {
//            var type = typeof(TEnum);
//            Util.ThrowUnless(type.IsEnum, "The provided type must be an enum");
//            return GetEnumFields(type).ToDictionary(f => (TEnum)f.GetRawConstantValue(), f => f.GetCustomAttributes<TAttribute>(false).FirstOrDefault());
//        }

//        /// <summary>
//        ///     Wraps <see cref="Interlocked.CompareExchange{T}(ref T,T,T)"/>
//        ///     for atomically setting null fields.
//        /// </summary>
//        /// <typeparam name="T">The type of the field to set.</typeparam>
//        /// <param name="location">
//        ///     The field that, if null, will be set to <paramref name="value"/>.
//        /// </param>
//        /// <param name="value">
//        ///     If <paramref name="location"/> is null, the object to set it to.
//        /// </param>
//        /// <returns>true if <paramref name="location"/> was null and has now been set; otherwise, false.</returns>
//        public static bool InterlockedSetNullField<T>(ref T location, T value) where T : class
//        {
//            Contract.Requires(value != null);

//            // Strictly speaking, this null check is not nessesary, but
//            // while CompareExchange is fast, it's still much slower than a
//            // null check.
//            if (location == null)
//            {
//                // This is a paranoid method. In a multi-threaded environment, it's possible
//                // for two threads to get through the null check before a value is set.
//                // This makes sure than one and only one value is set to field.
//                // This is super important if the field is used in locking, for instance.

//                var valueWhenSet = Interlocked.CompareExchange<T>(ref location, value, null);
//                return (valueWhenSet == null);
//            }
//            else
//            {
//                return false;
//            }
//        }

//        /// Returns true if the provided <see cref="Exception"/> is considered 'critical'
//        /// </summary>
//        /// <param name="exception">The <see cref="Exception"/> to evaluate for critical-ness.</param>
//        /// <returns>true if the Exception is conisdered critical; otherwise, false.</returns>
//        /// <remarks>
//        /// These exceptions are consider critical:
//        /// <list type="bullets">
//        ///     <item><see cref="OutOfMemoryException"/></item>
//        ///     <item><see cref="StackOverflowException"/></item>
//        ///     <item><see cref="ThreadAbortException"/></item>
//        ///     <item><see cref="SEHException"/></item>
//        /// </list>
//        /// </remarks>
//        public static bool IsCriticalException(this Exception exception)
//        {
//            // Copied with respect from WPF WindowsBase->MS.Internal.CriticalExceptions.IsCriticalException
//            // NullReferencException, SecurityException --> not going to consider these critical
//            while (exception != null)
//            {
//                if (exception is OutOfMemoryException ||
//                        exception is StackOverflowException ||
//                        exception is ThreadAbortException
//#if !WP7
// || exception is System.Runtime.InteropServices.SEHException
//#endif
//)
//                {
//                    return true;
//                }
//                exception = exception.InnerException;
//            }
//            return false;
//        } //*** static IsCriticalException

//        public static Random Rnd
//        {
//            get
//            {
//                Contract.Ensures(Contract.Result<Random>() != null);
//                var r = (Random)s_random.Target;
//                if (r == null)
//                {
//                    s_random.Target = r = new Random();
//                }
//                return r;
//            }
//        }

//        [DebuggerStepThrough]
//        public static void ThrowUnless(bool truth, string message = null)
//        {
//            ThrowUnless<Exception>(truth, message);
//        }

//        [DebuggerStepThrough]
//        public static void ThrowUnless<TException>(bool truth, string message) where TException : Exception
//        {
//            if (!truth)
//            {
//                throw InstanceFactory.CreateInstance<TException>(message);
//            }
//        }

//        [DebuggerStepThrough]
//        public static void ThrowUnless<TException>(bool truth) where TException : Exception, new()
//        {
//            if (!truth)
//            {
//                throw new TException();
//            }
//        }

//        private static IEnumerable<FieldInfo> GetEnumFields(Type enumType)
//        {
//            Util.ThrowUnless(enumType.IsEnum, "The provided type must be an enum");
//            return enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
//        }

//        private static readonly WeakReference s_random = new WeakReference(null);
//    } //*** public class Util
//} //*** namespace PixelLab.Common
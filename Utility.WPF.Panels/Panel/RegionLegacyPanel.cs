namespace Utility.WPF.Demo.Panels
{
    //using System;
    //using System.Collections.Generic;
    //using System.Text;

    //namespace Utility.WPF.DemoApp.Controls
    //{
    //    class CustomDockPanel
    //    {
    //    }
    //}

    // Licensed to the .NET Foundation under one or more agreements.
    // The .NET Foundation licenses this file to you under the MIT license.
    // See the LICENSE file in the project root for more information.

    //
    // Description: Contains the DockPanel class.
    //              Spec at DockPanel.xml
    //

    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// DockPanel is used to size and position children inward from the edges of available space.
    ///
    /// A <see cref="Region" /> enum (see <see cref="SetDock" /> and <see cref="GetDock" />)
    /// determines on which size a child is placed.  Children are stacked in order from these edges until
    /// there is no more space; this happens when previous children have consumed all available space, or a child
    /// with Dock set to Fill is encountered.
    /// </summary>
    public class RegionLegacyPanel : Panel
    {
        // Using a DependencyProperty as the backing store for IsContentStretched.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty IsContentStretchedProperty =
        //    DependencyProperty.Register("IsContentStretched", typeof(bool), typeof(RegionLegacyPanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));

        public static readonly DependencyProperty IsFilledFromTopProperty =
            DependencyProperty.Register("IsFilledFromTop", typeof(bool), typeof(RegionLegacyPanel), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsArrange));

        public static readonly DependencyProperty IsLastChildFilledProperty =
            DependencyProperty.Register("IsLastChildFilled", typeof(bool), typeof(RegionLegacyPanel), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// DependencyProperty for Dock property.
        /// </summary>
        /// <seealso cref="GetDock" />
        /// <seealso cref="SetDock" />
        //  [CommonDependencyProperty]
        public static readonly DependencyProperty RegionProperty =
            DependencyProperty.RegisterAttached("Region", typeof(Region), typeof(RegionLegacyPanel), new FrameworkPropertyMetadata(Region.TopLeft, new PropertyChangedCallback(OnRegionChanged)), new ValidateValueCallback(IsValidRegion));

        public static readonly DependencyProperty StartRegionProperty = DependencyProperty.Register("StartRegion", typeof(Region?), typeof(RegionLegacyPanel),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnRegionChanged)), new ValidateValueCallback(IsValidRegion));

        private static readonly Lazy<Array> regions = new Lazy<Array>(() => Enum.GetValues(typeof(Region)));

        public RegionLegacyPanel() : base()
        {
        }

        /// <summary>
        /// true places the last child in the empty space from top to bottom
        /// false places the last child in the empty space from left to right
        /// </summary>
        public bool IsFilledFromTop
        {
            get { return (bool)GetValue(IsFilledFromTopProperty); }
            set { SetValue(IsFilledFromTopProperty, value); }
        }

        /// <summary>
        /// This property controls whether the last child in the DockPanel should be stretched to fill any
        /// remaining available space.
        /// </summary>
        public bool IsLastChildFilled
        {
            get { return (bool)GetValue(IsLastChildFilledProperty); }
            set { SetValue(IsLastChildFilledProperty, value); }
        }

        ///// <summary>
        ///// true the space for items based on the available space
        ///// false the indvidual items dimensions determine their space
        ///// </summary>
        //public bool IsContentStretched
        //{
        //    get { return (bool)GetValue(IsContentStretchedProperty); }
        //    set { SetValue(IsContentStretchedProperty, value); }
        //}

        public Region? StartRegion
        {
            get { return (Region?)GetValue(StartRegionProperty); }
            set { SetValue(StartRegionProperty, value); }
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
            //it may be anyting, like FlowDocument... bug 1237275
            if (d is UIElement uie && VisualTreeHelper.GetParent(uie) is RegionLegacyPanel p)
            {
                p.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Updates DesiredSize of the DockPanel.  Called by parent UIElement.  This is the first pass of layout.
        /// </summary>
        /// <remarks>
        /// Children are measured based on their sizing properties and <see cref="Region" />.
        /// Each child is allowed to consume all of the space on the side on which it is docked; Left/Right docked
        /// children are granted all vertical space for their entire width, and Top/Bottom docked children are
        /// granted all horizontal space for their entire height.
        /// </remarks>
        /// <param name="constraint">Constraint size is an "upper limit" that the return value should not exceed.</param>
        /// <returns>The Panel's desired size.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            // UIElementCollection children = InternalChildren;

            //double parentWidth = 0;   // Our current required width due to children thus far.
            //double parentHeight = 0;   // Our current required height due to children thus far.
            //double accumulatedTopWidth = 0;   // Total width consumed by children.
            //double accumulatedBottomWidth = 0;   // Total width consumed by children.
            //double accumulatedLeftHeight = 0;   // Total height consumed by children.
            //double accumulatedRightHeight = 0;   // Total height consumed by children.

            //for (int i = 0, count = children.Count; i < count; ++i)
            //{
            //    UIElement child = children[i];
            //    Size childConstraint;             // Contains the suggested input constraint for this child.
            //    Size childDesiredSize;            // Contains the return size from child measure.

            //    if (child == null) { continue; }

            //    // Now, we adjust:
            //    // 1. Size consumed by children (accumulatedSize).  This will be used when computing subsequent
            //    //    children to determine how much space is remaining for them.
            //    // 2. Parent size implied by this child (parentSize) when added to the current children (accumulatedSize).
            //    //    This is different from the size above in one respect: A Dock.Left child implies a height, but does
            //    //    not actually consume any height for subsequent children.
            //    // If we accumulate size in a given dimension, the next child (or the end conditions after the child loop)
            //    // will deal with computing our minimum size (parentSize) due to that accumulation.
            //    // Therefore, we only need to compute our minimum size (parentSize) in dimensions that this child does
            //    //   not accumulate: Width for Top/Bottom, Height for Left/Right.
            //    switch (RegionLegacyPanel.GetRegion(child))
            //    {
            //        case Region.TopLeft:
            //            // Child constraint is the remaining size; this is total size minus size consumed by previous children.
            //            childConstraint = new Size(Math.Max(0.0, constraint.Width - accumulatedTopWidth),
            //                                       Math.Max(0.0, constraint.Height - accumulatedLeftHeight));
            //            parentHeight = Math.Max(parentHeight, accumulatedLeftHeight + childDesiredSize.Height);
            //            // Measure child.
            //            child.Measure(childConstraint);
            //            childDesiredSize = child.DesiredSize;
            //            accumulatedTopWidth += childDesiredSize.Width;

            //            break;
            //        case Region.TopRight:
            //            // Child constraint is the remaining size; this is total size minus size consumed by previous children.
            //            childConstraint = new Size(Math.Max(0.0, constraint.Width - accumulatedTopWidth),
            //                                       Math.Max(0.0, constraint.Height - accumulatedRightHeight));
            //            parentWidth = Math.Max(parentWidth, accumulatedTopWidth + childDesiredSize.Width);
            //            // Measure child.
            //            child.Measure(childConstraint);
            //            childDesiredSize = child.DesiredSize;
            //            accumulatedRightHeight += childDesiredSize.Height;
            //            break;
            //        case Region.BottomLeft:
            //            // Child constraint is the remaining size; this is total size minus size consumed by previous children.
            //            childConstraint = new Size(Math.Max(0.0, constraint.Width - accumulatedBottomWidth),
            //                                       Math.Max(0.0, constraint.Height - accumulatedLeftHeight));
            //            parentHeight = Math.Max(parentHeight, accumulatedLeftHeight + childDesiredSize.Height);
            //            // Measure child.
            //            child.Measure(childConstraint);
            //            childDesiredSize = child.DesiredSize;
            //            accumulatedLeftHeight += childDesiredSize.Width;
            //            break;
            //        case Region.BottomRight:
            //            // Child constraint is the remaining size; this is total size minus size consumed by previous children.
            //            childConstraint = new Size(Math.Max(0.0, constraint.Width - accumulatedBottomWidth),
            //                                       Math.Max(0.0, constraint.Height - accumulatedRightHeight));
            //            parentWidth = Math.Max(parentWidth, accumulatedBottomWidth + childDesiredSize.Width);
            //            // Measure child.
            //            child.Measure(childConstraint);
            //            childDesiredSize = child.DesiredSize;
            //            accumulatedBottomWidth += childDesiredSize.Height;
            //            break;
            //    }

            //}

            //// Make sure the final accumulated size is reflected in parentSize.
            //parentWidth = Math.Max(parentWidth, Math.Max(accumulatedBottomWidth, accumulatedTopWidth));
            //parentHeight = Math.Max(parentHeight, Math.Max( accumulatedLeftHeight, accumulatedRightHeight));

            //return (new Size(parentWidth, parentHeight));

            foreach (UIElement elem in InternalChildren)
                elem.Measure(constraint);

            //if (double.IsInfinity(constraint.Width) || double.IsInfinity(constraint.Height))
            //    return new Size(200, 200);
            return constraint;
        }

        /// <summary>
        /// RegionLegacyPanel computes a position and final size for each of its children based upon their
        /// <see cref="Region" /> enum and sizing properties.
        /// </summary>
        /// <param name="arrangeSize">Size that DockPanel will assume to position children.</param>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            UIElementCollection children = InternalChildren;
            int totalChildrenCount = children.Count;
            int nonFillChildrenCount = totalChildrenCount - (IsLastChildFilled ? 1 : 0);
            var Regions = regions.Value.GetEnumerator();

            double accumulatedTopLeft = 0;
            double accumulatedTopRight = 0;
            double accumulatedBottomLeft = 0;
            double accumulatedBottomRight = 0;

            double maxLeft = 0;
            double maxRight = 0;
            double maxBottom = 0;
            double maxTop = 0;

            for (int i = 0; i < totalChildrenCount; ++i)
            {
                UIElement child = children[i];
                if (child == null) { continue; }

                Size childDesiredSize = child.DesiredSize;
                Rect rcChild = default;
                //new Rect(
                //    accumulatedLeft,
                //    accumulatedTop,
                //    Math.Max(0.0, arrangeSize.Width - (accumulatedLeft + accumulatedRight)),
                //    Math.Max(0.0, arrangeSize.Height - (accumulatedTop + accumulatedBottom)));

                if (i == nonFillChildrenCount)
                {
                    rcChild = IsFilledFromTop ? new Rect(
                        Math.Max(maxLeft, accumulatedTopLeft),
                        0,
                        Math.Max(0.0, arrangeSize.Width - Math.Max(maxLeft, accumulatedTopLeft) - Math.Max(maxRight, accumulatedBottomRight)),
                        arrangeSize.Height) :
                        new Rect(
                        0,
                        Math.Max(maxTop, accumulatedTopRight),
                        arrangeSize.Width,
                        Math.Max(0.0, arrangeSize.Height - Math.Max(maxTop, accumulatedTopRight) - Math.Max(maxBottom, accumulatedBottomLeft)));
                }
                else
                    switch (GetRegion_(child))
                    {
                        case Region.TopLeft:
                            rcChild = new Rect(accumulatedTopLeft, 0, childDesiredSize.Width, childDesiredSize.Height);
                            accumulatedTopLeft += childDesiredSize.Width;
                            maxLeft = Math.Max(childDesiredSize.Width, maxLeft);
                            maxTop = Math.Max(childDesiredSize.Height, maxTop);
                            break;

                        case Region.TopRight:
                            rcChild = new Rect(Math.Max(0.0, arrangeSize.Width - childDesiredSize.Width), accumulatedTopRight, childDesiredSize.Width, childDesiredSize.Height);
                            accumulatedTopRight += childDesiredSize.Height;
                            maxRight = Math.Max(childDesiredSize.Width, maxRight);
                            maxTop = Math.Max(childDesiredSize.Height, maxTop);
                            break;

                        case Region.BottomLeft:
                            rcChild = new Rect(0, Math.Max(0.0, arrangeSize.Height - accumulatedBottomLeft - childDesiredSize.Height), childDesiredSize.Width, childDesiredSize.Height);
                            accumulatedBottomLeft += childDesiredSize.Height;
                            maxLeft = Math.Max(childDesiredSize.Width, maxLeft);
                            maxBottom = Math.Max(childDesiredSize.Height, maxBottom);
                            break;

                        case Region.BottomRight:
                            rcChild = new Rect(Math.Max(0.0, arrangeSize.Width - accumulatedBottomRight - childDesiredSize.Width),
                            Math.Max(0.0, arrangeSize.Height - childDesiredSize.Height), childDesiredSize.Width, childDesiredSize.Height);
                            accumulatedBottomRight += childDesiredSize.Width;
                            maxRight = Math.Max(childDesiredSize.Width, maxRight);
                            maxBottom = Math.Max(childDesiredSize.Height, maxBottom);
                            break;
                    }

                child.Arrange(rcChild);
            }

            return arrangeSize;

            Region GetRegion_(UIElement element)
            {
                if (StartRegion.HasValue)
                {
                    while (!Regions.MoveNext())
                        Regions = regions.Value.GetEnumerator();
                    return (Region)Regions.Current;
                }
                else
                {
                    return GetRegion(element);
                }
            }
        }

        internal static bool IsValidRegion(object o)
        {
            return o == null ||
             o is Region dock && (dock == Region.TopLeft
                    || dock == Region.TopRight
                    || dock == Region.BottomRight
                    || dock == Region.BottomLeft);
        }
    }
}
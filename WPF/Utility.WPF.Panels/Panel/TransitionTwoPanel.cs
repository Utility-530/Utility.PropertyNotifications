//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Utility.WPF.Demo.Controls.Controls
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

namespace Utility.WPF.Demo.Panels
{
    /// <summary>
    /// DockPanel is used to size and position children inward from the edges of available space.
    ///
    /// A <see cref="Corner" /> enum (see <see cref="SetDock" /> and <see cref="GetDock" />)
    /// determines on which size a child is placed.  Children are stacked in order from these edges until
    /// there is no more space; this happens when previous children have consumed all available space, or a child
    /// with Dock set to Fill is encountered.
    /// </summary>
    public class TransitionTwoPanel : Panel
    {
        /// <summary>
        /// DependencyProperty for Dock property.
        /// </summary>
        /// <seealso cref="GetDock" />
        /// <seealso cref="SetDock" />
        //  [CommonDependencyProperty]
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.RegisterAttached("Value", typeof(object), typeof(TransitionTwoPanel),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnValueChanged)), new ValidateValueCallback(IsValidCorner));

        public static readonly DependencyProperty CurrentValueProperty = DependencyProperty.Register("CurrentValue", typeof(object), typeof(TransitionTwoPanel),
         new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange));

        public object CurrentValue
        {
            get { return (object)GetValue(CurrentValueProperty); }
            set { SetValue(CurrentValueProperty, value); }
        }

        public TransitionTwoPanel() : base()
        {
        }

        /// <summary>
        /// Reads the attached property Dock from the given element.
        /// </summary>
        /// <param name="element">UIElement from which to read the attached property.</param>
        /// <returns>The property's value.</returns>
        /// <seealso cref="CornerProperty" />
        [AttachedPropertyBrowsableForChildren()]
        public static object GetValue(UIElement element)
        {
            return element != null ? element.GetValue(ValueProperty) : throw new ArgumentNullException("element");
        }

        /// <summary>
        /// Writes the attached property Dock to the given element.
        /// </summary>
        /// <param name="element">UIElement to which to write the attached property.</param>
        /// <param name="dock">The property value to set</param>
        /// <seealso cref="CornerProperty" />
        public static void SetValue(UIElement element, object dock)
        {
            if (element == null) { throw new ArgumentNullException("element"); }

            element.SetValue(ValueProperty, dock);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //it may be anyting, like FlowDocument... bug 1237275
            if (d is UIElement uie && VisualTreeHelper.GetParent(uie) is TransitionTwoPanel p)
            {
                p.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Updates DesiredSize of the DockPanel.  Called by parent UIElement.  This is the first pass of layout.
        /// </summary>
        /// <remarks>
        /// Children are measured based on their sizing properties and <see cref="Corner" />.
        /// Each child is allowed to consume all of the space on the side on which it is docked; Left/Right docked
        /// children are granted all vertical space for their entire width, and Top/Bottom docked children are
        /// granted all horizontal space for their entire height.
        /// </remarks>
        /// <param name="constraint">Constraint size is an "upper limit" that the return value should not exceed.</param>
        /// <returns>The Panel's desired size.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size childSize = new Size(availableSize.Width, availableSize.Height / InternalChildren.Count);

            foreach (UIElement elem in InternalChildren)
                elem.Measure(childSize);

            return childSize;
        }

        /// <summary>
        /// TransitionPanel computes a position and final size for each of its children based upon their
        /// <see cref="Corner" /> enum and sizing properties.
        /// </summary>
        /// <param name="arrangeSize">Size that DockPanel will assume to position children.</param>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            UIElementCollection children = InternalChildren;

            if (CurrentValue == null)
            {
                double childHeight = arrangeSize.Height / InternalChildren.Count;
                Size childSize = new Size(arrangeSize.Width, childHeight);

                double top = 0.0;
                for (int i = 0; i < children.Count; i++)
                {
                    Rect r = new Rect(new Point(0.0, top), childSize);
                    InternalChildren[i].Arrange(r);
                    top += childHeight;
                }

                return arrangeSize;
            }

            Rect rcChild;
            for (int i = 0; i < children.Count; ++i)
            {
                UIElement child = children[i];
                if (child == null)
                    continue;

                if (GetValue(child)?.Equals(CurrentValue) ?? false)
                {
                    rcChild = new Rect(0, 0, arrangeSize.Width, arrangeSize.Height);
                }

                child.Arrange(rcChild);
            }

            if (rcChild == default)
            {
                double childHeight = arrangeSize.Height / InternalChildren.Count;
                Size childSize = new Size(arrangeSize.Width, childHeight);

                double top = 0.0;
                for (int i = 0; i < children.Count; i++)
                {
                    Rect r = new Rect(new Point(0.0, top), childSize);
                    InternalChildren[i].Arrange(r);
                    top += childHeight;
                }

                return arrangeSize;
            }

            return arrangeSize;
        }

        internal static bool IsValidCorner(object o)
        {
            return true;
            //return o is Corner dock && (dock == Corner.TopLeft
            //        || dock == Corner.TopRight
            //        || dock == Corner.BottomRight
            //        || dock == Corner.BottomLeft);
        }
    }
}
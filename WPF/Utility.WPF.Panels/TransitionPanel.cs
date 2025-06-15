using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Utility.Helpers.Reflection;

namespace Utility.WPF.Panels
{
    public class TransitionPanel : Panel
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.RegisterAttached("Value", typeof(object), typeof(TransitionPanel),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnValueChanged)));

        public static readonly DependencyProperty CurrentValueProperty = DependencyProperty.Register("CurrentValue", typeof(object), typeof(TransitionPanel),
         new FrameworkPropertyMetadata(OnValueChanged, FrameworkPropertyMetadataOptions.AffectsArrange));

        public static readonly DependencyProperty PropertyNameProperty =
    DependencyProperty.Register("PropertyName", typeof(string), typeof(TransitionPanel), new PropertyMetadata(OnValueChanged));

        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }

        public object CurrentValue
        {
            get { return GetValue(CurrentValueProperty); }
            set { SetValue(CurrentValueProperty, value); }
        }

        [AttachedPropertyBrowsableForChildren()]
        public static object GetValue(UIElement element)
        {
            return element != null ? element.GetValue(ValueProperty) : throw new ArgumentNullException("element");
        }

        public static void SetValue(UIElement element, object dock)
        {
            if (element == null) { throw new ArgumentNullException("element"); }

            element.SetValue(ValueProperty, dock);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //it may be anyting, like FlowDocument... bug 1237275
            if (d is UIElement uie && VisualTreeHelper.GetParent(uie) is TransitionPanel p)
            {
                p.InvalidateMeasure();
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (double.IsFinite(availableSize.Width) && double.IsFinite(availableSize.Height))
            {
                Size childSize = new Size(availableSize.Width, availableSize.Height / InternalChildren.Count);

                foreach (UIElement elem in InternalChildren)
                    elem.Measure(childSize);

                return childSize;
            }
            return new Size(this.MinWidth + 10, this.MinHeight + 10);
        }

        /// <summary>
        /// TransitionPanel computes a position and final size for each of its children based upon their
        /// <see cref="Region" /> enum and sizing properties.
        /// </summary>
        /// <param name="arrangeSize">Size that DockPanel will assume to position children.</param>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            UIElementCollection children = InternalChildren;

            if (CurrentValue == null)
            {
                return NewMethod(arrangeSize, children);
            }

            List<Action> actions = new();

            Rect equlsRect;
            for (int i = 0; i < children.Count; ++i)
            {
                UIElement child = children[i];
                if (child == null)
                    continue;

                object value = GetValue(child);

                if (value is bool __b)
                {
                    value = __b;
                }
                else if (bool.TryParse(value?.ToString(), out bool b))
                {
                    value = b;
                }
                else if (value is int __int)
                {
                    value = __int;
                }
                else if (int.TryParse(value?.ToString(), out int _int))
                {
                    value = _int;
                }
                if (PropertyName != null)
                {
                    value = child.GetPropertyRefValue<object>(PropertyName);
                }
                // Uses the index of element if GetValue is null and Current is integer
                // or use the Property of the child whose name matches PropertyName
                if (value?.Equals(CurrentValue) == true)
                {
                    equlsRect = new Rect(0, -arrangeSize.Height, arrangeSize.Width, arrangeSize.Height);
                    child.Arrange(equlsRect);
                    AnimationHelper.AnimatePosition(child, 0, arrangeSize.Height, new Duration(TimeSpan.FromSeconds(1)));

                    actions.Add(async () =>
                    {
                        await Task.Delay(500);
                        AnimationHelper.AnimateOpacityFromZero(child);
                    });
                }
                else
                {
                    actions.Add(async () =>
                    {
                        equlsRect = new Rect(0, 0, arrangeSize.Width, arrangeSize.Height);
                        child.Arrange(equlsRect);
                        AnimationHelper.AnimatePosition(child, 0, arrangeSize.Height, new Duration(TimeSpan.FromSeconds(1)));
                        AnimationHelper.AnimateOpacityToZero(child);
                        await Task.Delay(1000);
                        var rect = new Rect(0, 0, 0, 0);
                        child.Arrange(rect);
                    });
                }
            }

            if (equlsRect == default)
            {
                return NewMethod(arrangeSize, children);
            }
            else
            {
                foreach (var action in actions)
                {
                    action();
                }
            }

            return arrangeSize;
        }

        private Size NewMethod(Size arrangeSize, UIElementCollection children)
        {
            double childHeight = arrangeSize.Height / InternalChildren.Count;
            Size childSize = new Size(arrangeSize.Width, childHeight);

            double top = 0.0;
            for (int i = 0; i < children.Count; i++)
            {
                Rect r = new(new Point(0.0, 0), childSize);
                InternalChildren[i].Arrange(r);
                AnimationHelper.AnimatePosition(InternalChildren[i], 0, top, new Duration(TimeSpan.FromSeconds(1)));
                if (InternalChildren[i].Opacity == 0)
                    AnimationHelper.AnimateOpacityFromZero(InternalChildren[i]);
                top += childHeight;
            }

            return arrangeSize;
        }
    }
}
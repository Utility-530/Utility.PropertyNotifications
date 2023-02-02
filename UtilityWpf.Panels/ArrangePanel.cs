using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using UtilityWpf.Panels.Infrastructure;

/// <summary>
/// <a href="https://www.codeproject.com/tips/721158/wpf-animated-arrange-panel"></a>
/// </summary>
namespace UtilityWpf.Panels
{
    public class ArrangePanel : Panel
    {
        private UIElement? _draggingObject;
        private Vector _delta;
        private Point _startPosition;
        private ILayoutStrategy _strategy = new VerticalTableLayoutStrategy();

        public static readonly DependencyProperty OrderProperty;
        public static readonly DependencyProperty PositionProperty;
        public static readonly DependencyProperty DesiredPositionProperty;
        public static readonly DependencyProperty OrientationProperty;
        private Size[] _measures;

        static ArrangePanel()
        {
            PositionProperty = DependencyProperty.RegisterAttached(
                "Position",
                typeof(Rect),
                typeof(ArrangePanel),
                new FrameworkPropertyMetadata(
                    new Rect(double.NaN, double.NaN, double.NaN, double.NaN),
                    FrameworkPropertyMetadataOptions.AffectsParentArrange));

            DesiredPositionProperty = DependencyProperty.RegisterAttached(
                "DesiredPosition",
                typeof(Rect),
                typeof(ArrangePanel),
                new FrameworkPropertyMetadata(
                    new Rect(double.NaN, double.NaN, double.NaN, double.NaN),
                    OnDesiredPositionChanged));

            OrderProperty = DependencyProperty.RegisterAttached(
                "Order",
                typeof(int),
                typeof(ArrangePanel),
                new FrameworkPropertyMetadata(
                    -1,
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure));

            OrientationProperty = WrapPanel.OrientationProperty.AddOwner(
                typeof(ArrangePanel),
                new FrameworkPropertyMetadata(
                    Orientation.Vertical,
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure, OnOrientationChanged));
        }

        #region dependency properties
        public static int GetOrder(DependencyObject obj)
        {
            return (int)obj.GetValue(OrderProperty);
        }

        public static void SetOrder(DependencyObject obj, int value)
        {
            obj.SetValue(OrderProperty, value);
        }

        public static Rect GetPosition(DependencyObject obj)
        {
            return (Rect)obj.GetValue(PositionProperty);
        }

        public static void SetPosition(DependencyObject obj, Rect value)
        {
            obj.SetValue(PositionProperty, value);
        }

        public static Rect GetDesiredPosition(DependencyObject obj)
        {
            return (Rect)obj.GetValue(DesiredPositionProperty);
        }

        public static void SetDesiredPosition(DependencyObject obj, Rect value)
        {
            obj.SetValue(DesiredPositionProperty, value);
        }

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }
        #endregion

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            StartReordering(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            StopReordering();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_draggingObject != null)
            {
                if (e.LeftButton == MouseButtonState.Released)
                    StopReordering();
                else
                    DoReordering(e);
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            StopReordering();
            base.OnMouseLeave(e);
        }

        private void StartReordering(MouseEventArgs e)
        {
            _startPosition = e.GetPosition(this);
            if (GetMyChildOfUiElement((UIElement)e.OriginalSource) is not UIElement _draggingObject)
                return;
            _draggingObject.SetValue(ZIndexProperty, 100);
            var position = GetPosition(_draggingObject);
            _delta = position.TopLeft - _startPosition;
            _draggingObject.BeginAnimation(PositionProperty, null);
            SetPosition(_draggingObject, position);
        }

        private void DoReordering(MouseEventArgs e)
        {
            e.Handled = true;
            Point mousePosition = e.GetPosition(this);
            var index = _strategy.GetIndex(mousePosition);
            SetOrder(_draggingObject, index);
            var topLeft = mousePosition + _delta;
            var newPosition = new Rect(topLeft, GetPosition(_draggingObject).Size);
            SetPosition(_draggingObject, newPosition);
        }

        private void StopReordering()
        {
            if (_draggingObject == null) return;

            _draggingObject.ClearValue(ZIndexProperty);
            InvalidateMeasure();
            AnimateToPosition(_draggingObject, GetDesiredPosition(_draggingObject));
            _draggingObject = null;
        }

        private UIElement? GetMyChildOfUiElement(UIElement e)
        {
            var obj = e;
            UIElement? parent = VisualTreeHelper.GetParent(obj) as UIElement;
            while (parent != null && parent != this)
            {
                obj = parent;
                parent = VisualTreeHelper.GetParent(obj) as UIElement;
            }
            return obj;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Orientation == Orientation.Vertical)
                _strategy = new VerticalTableLayoutStrategy();
            else
                _strategy = new HorizontalTableLayoutStrategy();

            InitializeEmptyOrder();
            if (_draggingObject != null)
            {
                ReorderOthers();
            }

            var measures = MeasureChildren();

            _strategy.Calculate(availableSize, measures);

            var index = -1;
            foreach (var child in Children.OfType<UIElement>().OrderBy(GetOrder))
            {
                index++;
                if (child == _draggingObject) continue;
                SetDesiredPosition(child, _strategy.GetPosition(index));
            }

            return _strategy.ResultSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var child in Children.OfType<UIElement>().OrderBy(GetOrder))
            {
                var position = GetPosition(child);
                if (double.IsNaN(position.Left))
                    position = GetDesiredPosition(child);
                child.Arrange(position);
            }
            return _strategy.ResultSize;
        }


        private Size[] MeasureChildren()
        {
            if (_measures == null || Children.Count != _measures.Length)
            {
                _measures = new Size[Children.Count];

                var infiniteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);

                foreach (UIElement child in Children)
                {
                    child.Measure(infiniteSize);
                }


                var i = 0;
                foreach (var measure in Children.OfType<UIElement>().OrderBy(GetOrder).Select(ch => ch.DesiredSize))
                {
                    _measures[i] = measure;
                    i++;
                }
            }
            return _measures;
        }

        private void ReorderOthers()
        {
            var s = GetOrder(_draggingObject);
            var i = 0;
            foreach (var child in Children.OfType<UIElement>().OrderBy(GetOrder))
            {
                if (i == s) i++;
                if (child == _draggingObject) continue;
                var current = GetOrder(child);
                if (i != current)
                {
                    SetOrder(child, i);
                }
                i++;
            }
        }

        private void InitializeEmptyOrder()
        {
            if (Children.OfType<UIElement>().Any() == false)
                return;
            var next = Children.OfType<UIElement>().Max(ch => GetOrder(ch)) + 1;
            foreach (var child in Children.OfType<UIElement>().Where(child => GetOrder(child) == -1))
            {
                SetOrder(child, next);
                next++;
            }
        }



        private static void OnDesiredPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var desiredPosition = (Rect)e.NewValue;
            AnimateToPosition(d, desiredPosition);
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ArrangePanel panel && e.NewValue is Orientation orientation)
            {
                switch (orientation)
                {
                    case Orientation.Vertical:
                        panel._strategy = new VerticalTableLayoutStrategy();
                        break;
                    case Orientation.Horizontal:
                        panel._strategy = new HorizontalTableLayoutStrategy();
                        break;
                }
                panel.InvalidateArrange();
            }
        }

        private static void AnimateToPosition(DependencyObject d, Rect desiredPosition)
        {
            var position = GetPosition(d);
            if (double.IsNaN(position.Y) || double.IsInfinity(position.Y))
            {
                SetPosition(d, desiredPosition);
                return;
            }

            var distance = Math.Max(
                (desiredPosition.TopLeft - position.TopLeft).Length,
                (desiredPosition.BottomRight - position.BottomRight).Length);

            var animationTime = TimeSpan.FromMilliseconds(distance * 2);
            var animation = new RectAnimation(position, desiredPosition, new Duration(animationTime));
            animation.DecelerationRatio = 1;
            ((UIElement)d).BeginAnimation(PositionProperty, animation);
        }
    }
}
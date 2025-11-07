using System.Globalization;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Utility.WPF.Helpers;

namespace System.Windows.Controls
{
    /// <summary>
    ///   Like the <see cref="GridSplitter"/> but can be used as element of itemscontrol which uses a grid as its itemspanel
    ///   instead of <see cref="Grid"/>.
    /// </summary>
    public class CustomGridSplitter : Thumb
    {
        private static readonly FrameworkElement targetNullObject = new FrameworkElement();
        private bool isHorizontal;
        private bool isBottomOrRight;
        private FrameworkElement target = targetNullObject;
        private FrameworkElement source;
        private double? initialLength, initialSourceLength;
        private double availableSpace;

        static CustomGridSplitter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomGridSplitter), new FrameworkPropertyMetadata(typeof(CustomGridSplitter)));
        }

        /// <summary> </summary>
        public CustomGridSplitter()
        {
            Loaded += OnLoaded;
            MouseDoubleClick += OnMouseDoubleClick;
            DragStarted += OnDragStarted;
            DragDelta += OnDragDelta;
        }

        private Grid Panel => Parent as Grid ?? this.FindParent<Grid>();

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!(Panel is Grid))
            {
                throw new InvalidOperationException($"{nameof(CustomGridSplitter)} must be directly in a Grid.");
            }

            if (GetTargetOrDefault() == default)
                throw new InvalidOperationException($"{nameof(CustomGridSplitter)} must be directly after a FrameworkElement");
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (initialLength != null)
                SetTargetLength(initialLength.Value);
        }

        private void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            isHorizontal = GetIsHorizontal(this);
            isBottomOrRight = GetIsBottomOrRight();
            target = GetTargetOrDefault() ?? targetNullObject;
            source = GetSourceOrDefault() ?? targetNullObject;
            initialLength ??= GetTargetLength();
            initialSourceLength ??= GetSourceLength();
            availableSpace = GetAvailableSpace();
        }

        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var change = isHorizontal ? e.VerticalChange : e.HorizontalChange;
            change = -change; // *(Math.Sign(e.HorizontalChange));

            var targetLength = GetSourceLength();
            var newTargetLength = targetLength + change;
            newTargetLength = Clamp(newTargetLength, 0, availableSpace);
            newTargetLength = Math.Round(newTargetLength);

            //SetTargetLength(GetTargetLength()- newTargetLength+ targetLength);
            SetSourceLength(newTargetLength);
        }

        private FrameworkElement? GetTargetOrDefault()
        {
            var children = Panel.Children.OfType<object>();
            var splitterIndex = Panel.Children.IndexOf(this);
            return children.ElementAtOrDefault(splitterIndex - 1) as FrameworkElement;
        }

        private FrameworkElement? GetSourceOrDefault()
        {
            var children = Panel.Children.OfType<object>();
            var splitterIndex = Panel.Children.IndexOf(this);
            return children.ElementAtOrDefault(splitterIndex + 1) as FrameworkElement;
        }

        private bool GetIsBottomOrRight()
        {
            return false;
            //var position = Grid.GetDock(this);
            //return position == Dock.Bottom || position == Dock.Right;
        }

        private double GetAvailableSpace()
        {
            var lastChild =
                //Panel.LastChildFill ?
                Panel.Children.OfType<object>().Last() as FrameworkElement;
            //null;

            var fixedChildren =
                from child in Panel.Children.OfType<FrameworkElement>()
                where GetIsHorizontal(child) == isHorizontal
                where child != target
                where child != lastChild
                select child;

            var panelLength = GetLength(Panel);
            var unavailableSpace = fixedChildren.Sum(c => GetLength(c));
            return panelLength - unavailableSpace;
        }

        private void SetTargetLength(double length)
        {
            if (isHorizontal) target.Height = length;
            else target.Width = length;
        }

        private void SetSourceLength(double length)
        {
            if (isHorizontal) source.Height = length;
            else source.Width = length;
        }

        private double GetTargetLength() => GetLength(target);

        private double GetSourceLength() => GetLength(source);

        private static bool GetIsHorizontal(FrameworkElement element)
        {
            return false;
            //var position = Grid.GetDock(element);
            //return GetIsHorizontal(position);
        }

        private static bool GetIsHorizontal(Dock position)
            => position == Dock.Top || position == Dock.Bottom;

        private static double Clamp(double value, double min, double max)
            => value < min ? min :
               value > max ? max :
               value;

        private double GetLength(FrameworkElement element)
            => isHorizontal ?
               element.ActualHeight :
               element.ActualWidth;

        internal class CursorConverter : IValueConverter
        {
            public static CursorConverter Instance { get; } = new CursorConverter();

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var position = (Dock)value;
                var isHorizontal = GetIsHorizontal(position);
                return isHorizontal ? Cursors.SizeNS : Cursors.SizeWE;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
                => throw new NotImplementedException();
        }
    }
}
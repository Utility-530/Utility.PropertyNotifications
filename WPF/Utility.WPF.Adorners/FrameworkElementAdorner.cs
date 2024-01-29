using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Utility.WPF.Adorners.Infrastructure;

// The code is based on:
// http://www.codeproject.com/KB/WPF/WPFJoshSmith.aspx
/// taken from
/// https://github.com/dady8889/AdornerBehavior
namespace Utility.WPF.Adorners
{
    /// <summary>
    /// Specifies the placement of the adorner in relation to the adorned control.
    /// </summary>
    public enum AdornerPlacement
    {
        Across,
        Inside,
        Outside
    }


    public abstract class FrameworkElementAdorner<T> : FrameworkElementAdorner where T : Control
    {
        private IDisposable? disposable;

        public FrameworkElementAdorner(FrameworkElement adornedElement) : base(adornedElement)
        {
        }

        public override void SetAdornedElement(DependencyObject adorner, FrameworkElement? adornedElement)
        {
            if (adornedElement == null)
            {
                disposable?.Dispose();
                return;
            }
            disposable = SetAdornedElement(adorner as T ?? throw new Exception("gd6fnb54 fdgd,,f"), adornedElement);
        }

        protected abstract IDisposable SetAdornedElement(T control, FrameworkElement? adornedElement);
    }

    /// <summary>
    /// This class is an adorner that allows a FrameworkElement derived class to adorn multiple FrameworkElements.
    /// </summary>
    public class FrameworkElementAdorner : Adorner
    {
        /// <summary>
        /// Collection of adorners.
        /// </summary>
        public static readonly DependencyProperty AdornersProperty =
            DependencyProperty.Register("Adorners", typeof(AdornerCollection), typeof(FrameworkElementAdorner));

        /// <summary>
        /// Position X of adorners.
        /// </summary>
        public static readonly DependencyProperty PositionXProperty =
            DependencyProperty.RegisterAttached("PositionX", typeof(double), typeof(FrameworkElementAdorner), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Position Y of adorners.
        /// </summary>
        public static readonly DependencyProperty PositionYProperty =
            DependencyProperty.RegisterAttached("PositionY", typeof(double), typeof(FrameworkElementAdorner), new PropertyMetadata(double.NaN));


        public FrameworkElementAdorner(FrameworkElement adornedElement) : base(adornedElement)
        {
            // AdornerBehavior is responsible for the connection and disconnection of adorners.
            // this.ConnectChildren(adornedElement);
        }

        #region Dependency Properties

        public AdornerCollection? Adorners
        {
            get => (AdornerCollection?)GetValue(AdornersProperty);
            set => SetValue(AdornersProperty, value);
        }

        public virtual void SetAdornedElement(DependencyObject adorner, FrameworkElement? adornedElement)
        {

        }

        public static double GetPositionX(DependencyObject d)
        {
            return (double)d.GetValue(PositionXProperty);
        }

        public static void SetPositionX(DependencyObject d, double value)
        {
            d.SetValue(PositionXProperty, value);
        }

        public static double GetPositionY(DependencyObject d)
        {
            return (double)d.GetValue(PositionYProperty);
        }

        public static void SetPositionY(DependencyObject d, double value)
        {
            d.SetValue(PositionYProperty, value);
        }

        #endregion Dependency Properties

        #region EventHandlers

        /// <summary>
        /// Event raised when the adorned control's size has changed.
        /// </summary>
        //private void AdornedElement_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    InvalidateMeasure();
        //}

        #endregion EventHandlers

        #region Methods




        /// <summary>
        /// Determine the X coordinate of the adorner.
        /// </summary>
        private double DetermineX(FrameworkElement adorner)
        {
            var horizontalPlacement = AdornerEx.GetHorizontalPlacement(adorner);
            var offsetX = AdornerEx.GetOffsetX(adorner);
            switch (adorner.HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    {
                        if (horizontalPlacement == AdornerPlacement.Across)
                            return -adorner.DesiredSize.Width / 2 + offsetX;
                        if (horizontalPlacement == AdornerPlacement.Outside)
                            return -adorner.DesiredSize.Width + offsetX;

                        return offsetX;
                    }
                case HorizontalAlignment.Right:
                    {
                        if (horizontalPlacement == AdornerPlacement.Across)
                        {
                            var adornerWidth = adorner.DesiredSize.Width;
                            var adornedWidth = AdornedElement.ActualWidth;
                            var x = adornedWidth - adornerWidth / 2;
                            return x + offsetX;
                        }

                        if (horizontalPlacement == AdornerPlacement.Outside)
                        {
                            var adornedWidth = AdornedElement.ActualWidth;
                            return adornedWidth + offsetX;
                        }

                        {
                            var adornerWidth = adorner.DesiredSize.Width;
                            var adornedWidth = AdornedElement.ActualWidth;
                            var x = adornedWidth - adornerWidth;
                            return x + offsetX;
                        }
                    }
                case HorizontalAlignment.Center:
                    {
                        var adornerWidth = adorner.DesiredSize.Width;
                        var adornedWidth = AdornedElement.ActualWidth;
                        var x = adornedWidth / 2 - adornerWidth / 2;
                        return x + offsetX;
                    }
                case HorizontalAlignment.Stretch:
                    {
                        return 0.0;
                    }
            }

            return 0.0;
        }

        /// <summary>
        /// Determine the Y coordinate of the adorner.
        /// </summary>
        private double DetermineY(FrameworkElement adorner)
        {
            var verticalPlacement = AdornerEx.GetVerticalPlacement(adorner);
            var offsetY = AdornerEx.GetOffsetY(adorner);

            switch (adorner.VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    {
                        if (verticalPlacement == AdornerPlacement.Across)
                            return -adorner.DesiredSize.Height / 2 + offsetY;

                        if (verticalPlacement == AdornerPlacement.Outside)
                            return -adorner.DesiredSize.Height + offsetY;

                        return offsetY;
                    }
                case VerticalAlignment.Bottom:
                    {
                        if (verticalPlacement == AdornerPlacement.Across)
                        {
                            var adornerHeight = adorner.DesiredSize.Height;
                            var adornedHeight = AdornedElement.ActualHeight;
                            var x = adornedHeight - adornerHeight / 2;
                            return x + offsetY;
                        }

                        if (verticalPlacement == AdornerPlacement.Outside)
                        {
                            var adornedHeight = AdornedElement.ActualHeight;
                            return adornedHeight + offsetY;
                        }

                        {
                            var adornerHeight = adorner.DesiredSize.Height;
                            var adornedHeight = AdornedElement.ActualHeight;
                            var x = adornedHeight - adornerHeight;
                            return x + offsetY;
                        }
                    }
                case VerticalAlignment.Center:
                    {
                        var adornerHeight = adorner.DesiredSize.Height;
                        var adornedHeight = AdornedElement.ActualHeight;
                        var x = adornedHeight / 2 - adornerHeight / 2;
                        return x + offsetY;
                    }
                case VerticalAlignment.Stretch:
                    {
                        return 0.0;
                    }
            }

            return 0.0;
        }

        /// <summary>
        /// Determine the width of the adorner.
        /// </summary>
        private double DetermineWidth(FrameworkElement frameworkElement)
        {
            var positionX = GetPositionX(frameworkElement);
            if (!double.IsNaN(positionX))
                return frameworkElement.DesiredSize.Width;

            switch (frameworkElement.HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    {
                        return frameworkElement.DesiredSize.Width;
                    }
                case HorizontalAlignment.Right:
                    {
                        return frameworkElement.DesiredSize.Width;
                    }
                case HorizontalAlignment.Center:
                    {
                        return frameworkElement.DesiredSize.Width;
                    }
                case HorizontalAlignment.Stretch:
                    {
                        return AdornedElement.ActualWidth;
                    }
            }

            return 0.0;
        }

        /// <summary>
        /// Determine the height of the adorner.
        /// </summary>
        private double DetermineHeight(FrameworkElement frameworkElement)
        {
            var positionY = GetPositionY(frameworkElement);
            if (!double.IsNaN(positionY))
                return frameworkElement.DesiredSize.Height;

            switch (frameworkElement.VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    {
                        return frameworkElement.DesiredSize.Height;
                    }
                case VerticalAlignment.Bottom:
                    {
                        return frameworkElement.DesiredSize.Height;
                    }
                case VerticalAlignment.Center:
                    {
                        return frameworkElement.DesiredSize.Height;
                    }
                case VerticalAlignment.Stretch:
                    {
                        return AdornedElement.ActualHeight;
                    }
            }

            return 0.0;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var adorner in Adorners)
            {
                var x = GetPositionX(adorner);

                if (double.IsNaN(x))
                    x = DetermineX(adorner);

                var y = GetPositionY(adorner);
                if (double.IsNaN(y))
                    y = DetermineY(adorner);

                var adornerWidth = DetermineWidth(adorner);
                var adornerHeight = DetermineHeight(adorner);
                adorner.Arrange(new Rect(x, y, adornerWidth, adornerHeight));
            }
            return finalSize;
        }

        protected override Visual? GetVisualChild(int index)
        {
            if (Adorners == null)
                return null;

            return new List<FrameworkElement>(Adorners)[index];
        }

        /// <summary>
        /// Remove all adorners from the visual tree.
        /// </summary>
        public void DisconnectChildren()
        {
            if (Adorners == null)
                return;
            foreach (var adorner in Adorners)
            {
                DisconnectAdorner(adorner);
            }
        }

        /// <summary>
        /// Add all adorners to the visual tree.
        /// </summary>
        public void ConnectChildren(FrameworkElement adornedElement)
        {
            foreach (var adorner in Adorners)
            {
                ConnectAdorner(adornedElement, adorner);
            }
        }

        /// <summary>
        /// Add a single adorner to the visual tree.
        /// </summary>
        public void ConnectAdorner(FrameworkElement adornedElement, FrameworkElement adorner)
        {
            AddLogicalChild(adorner);
            AddVisualChild(adorner);
            SetAdornedElement(adorner, adornedElement);
        }

        /// <summary>
        /// Remove a single adorner from the visual tree.
        /// </summary>
        public void DisconnectAdorner(FrameworkElement adorner)
        {
            RemoveLogicalChild(adorner);
            RemoveVisualChild(adorner);
            SetAdornedElement(adorner, null);
        }

        #endregion Methods

        #region Properties

        protected override IEnumerator LogicalChildren
        {
            get
            {
                // if Adorners is null then maybe wait until adorned element is loaded before applying the adorner
                return Adorners?.GetEnumerator()?? throw new Exception("dfg bfdgfd ..v,v");
            }
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return Adorners.Count;
            }
        }

        /// <summary>
        /// Override AdornedElement from base class for less type-checking.
        /// </summary>
        public new FrameworkElement AdornedElement
        {
            get
            {
                return (FrameworkElement)base.AdornedElement;
            }
        }

        #endregion Properties
    }

    /// <summary>
    /// List of adorners. Implements INotifyCollectionChanged.
    /// </summary>
    public sealed class AdornerCollection : IList<FrameworkElement>, IEnumerable<FrameworkElement>, IList, IEnumerable, INotifyCollectionChanged
    {
        private readonly List<FrameworkElement> _internalCollection;
        private readonly FrameworkElement _adornedElement;

        public AdornerCollection(FrameworkElement adornedElement)
        {
            _internalCollection = new List<FrameworkElement>();
            _adornedElement = adornedElement;
        }

        #region INotifyCollectionChanged

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(_adornedElement, args);
        }

        #endregion

        #region Properties

        public int Count
        {
            get
            {
                return _internalCollection.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return ((IList)_internalCollection).IsFixedSize;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return ((ICollection)_internalCollection).IsSynchronized;
            }
        }

        public object SyncRoot
        {
            get
            {
                return ((ICollection)_internalCollection).SyncRoot;
            }
        }

        #endregion

        #region IList<FrameworkElement>

        public FrameworkElement this[int index]
        {
            get
            {
                return _internalCollection[index];
            }

            set
            {
                var previous = _internalCollection[index];
                _internalCollection[index] = value;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, previous, index));
            }
        }

        public int IndexOf(FrameworkElement item)
        {
            return IndexOf((object)item);
        }

        public void Insert(int index, FrameworkElement item)
        {
            Insert(index, (object)item);
        }

        public void Add(FrameworkElement item)
        {
            Add((object)item);
        }

        public bool Contains(FrameworkElement item)
        {
            return Contains((object)item);
        }

        public void CopyTo(FrameworkElement[] array, int arrayIndex)
        {
            _internalCollection.CopyTo(array, arrayIndex);
        }

        public bool Remove(FrameworkElement item)
        {
            if (!Contains(item))
                return false;

            Remove(item);
            return true;
        }

        public IEnumerator<FrameworkElement> GetEnumerator()
        {
            foreach (var fe in _internalCollection)
                yield return fe;
        }

        #endregion

        #region IList

        object IList.this[int index]
        {
            get
            {
                return _internalCollection[index];
            }
            set
            {
                if (value is FrameworkElement frameworkElement)
                {
                    var previous = _internalCollection[index];
                    _internalCollection[index] = frameworkElement;
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, previous, index));
                }
                else
                {
                    throw new Exception("DFG32 dfbdfg bdf43");
                }
            }
        }

        public int Add(object? value)
        {
            var newIndex = ((IList)_internalCollection).Add(value);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
            return newIndex;
        }

        public bool Contains(object? value)
        {
            return ((IList)_internalCollection).Contains(value);
        }

        public int IndexOf(object? value)
        {
            return ((IList)_internalCollection).IndexOf(value);
        }

        public void Insert(int index, object? value)
        {
            ((IList)_internalCollection).Insert(index, value);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
        }

        public void Remove(object? value)
        {
            ((IList)_internalCollection).Remove(value);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value));
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)_internalCollection).CopyTo(array, index);
        }

        public void Clear()
        {
            var removed = _internalCollection.ToArray();
            _internalCollection.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed));
        }

        public void RemoveAt(int index)
        {
            var removed = _internalCollection[index];
            _internalCollection.RemoveAt(index);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed, index));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _internalCollection.GetEnumerator();
        }

        #endregion
    }
}
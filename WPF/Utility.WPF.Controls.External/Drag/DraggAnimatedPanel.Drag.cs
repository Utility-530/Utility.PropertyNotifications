/*Developed by (doiTTeam)=>doiTTeam.mail = devdoiTTeam@gmail.com*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DraggAnimatedPanel
{
    /// <summary>
    /// Description of SafariPanel_Drag.
    /// </summary>
    public partial class DraggAnimatedPanel
    {
        #region const drag

        private const double mouseDif = 2d;
        private const int mouseTimeDif = 25;

        #endregion const drag

        #region private

        private UIElement __draggedElement;

        public UIElement _draggedElement
        {
            get { return __draggedElement; }
            set
            {
                __draggedElement = value;
            }
        }

        private int _draggedIndex;

        private bool _firstScrollRequest = true;
        private ScrollViewer _scrollContainer;

        private ScrollViewer scrollViewer
        {
            get
            {
                if (_firstScrollRequest && _scrollContainer == null)
                {
                    _firstScrollRequest = false;
                    _scrollContainer = (ScrollViewer)GetParent(this as DependencyObject, (ve) => ve is ScrollViewer);
                }
                return _scrollContainer;
            }
        }

        #endregion private

        #region private drag

        private double _lastMousePosX;
        private double _lastMousePosY;
        private int _lastMouseMoveTime;
        private double _x;
        private double _y;
        private Rect _rectOnDrag;

        #endregion private drag

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && _draggedElement == null && !this.IsMouseCaptured)
                StartDrag(e);
            else if (_draggedElement != null)
                OnDragOver(e);
        }

        private void OnDragOver(MouseEventArgs e)
        {
            Point mousePos = Mouse.GetPosition(this);
            double difX = mousePos.X - _lastMousePosX;
            double difY = mousePos.Y - _lastMousePosY;

            int timeDif = e.Timestamp - _lastMouseMoveTime;
            if ((Math.Abs(difX) > mouseDif || Math.Abs(difY) > mouseDif) && timeDif > mouseTimeDif)
            {
                //this lines is for keepn draged item inside control bounds
                DoScroll();

                if (_x + difX < _rectOnDrag.Location.X)
                    _x = 0;
                else if (ItemsWidth + _x + difX > _rectOnDrag.Location.X + _rectOnDrag.Width)
                    _x = _rectOnDrag.Location.X + _rectOnDrag.Width - ItemsWidth;
                else if (mousePos.X > _rectOnDrag.Location.X && mousePos.X < _rectOnDrag.Location.X + _rectOnDrag.Width)
                    _x += difX;
                if (_y + difY < _rectOnDrag.Location.Y)
                    _y = 0;
                else if (ItemsHeight + _y + difY > _rectOnDrag.Location.Y + _rectOnDrag.Height)
                    _y = _rectOnDrag.Location.Y + _rectOnDrag.Height - ItemsHeight;
                else if (mousePos.Y > _rectOnDrag.Location.Y && mousePos.Y < _rectOnDrag.Location.Y + _rectOnDrag.Height)
                    _y += difY;
                //lines ends

                AnimateTo(_draggedElement, _x, _y, 0);
                _lastMousePosX = mousePos.X;
                _lastMousePosY = mousePos.Y;
                _lastMouseMoveTime = e.Timestamp;
                SwapElement(_x + ItemsWidth / 2, _y + ItemsHeight / 2);
            }
        }

        private void StartDrag(MouseEventArgs e)
        {
            Point mousePos = Mouse.GetPosition(this);
            _draggedElement = GetChildThatHasMouseOver();
            if (_draggedElement == null)
                return;
            _draggedIndex = Children.IndexOf(_draggedElement);
            _rectOnDrag = VisualTreeHelper.GetDescendantBounds(this);
            Point p = GetItemVisualPoint(_draggedElement);
            _x = p.X;
            _y = p.Y;
            SetZIndex(_draggedElement, 1000);
            _lastMousePosX = mousePos.X;
            _lastMousePosY = mousePos.Y;
            _lastMouseMoveTime = e.Timestamp;
            this.InvalidateArrange();
            e.Handled = true;
            this.CaptureMouse();
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (this.IsMouseCaptured)
                ReleaseMouseCapture();
        }

        private void SwapElement(double x, double y)
        {
            int index = GetIndexFromPoint(x, y);
            if (index == _draggedIndex || index < 0)
                return;
            if (index >= Children.Count)
                index = Children.Count - 1;

            int[] parameter = new int[] { _draggedIndex, index };
            if (SwapCommand != null && SwapCommand.CanExecute(parameter))
            {
                SwapCommand.Execute(parameter);
                _draggedElement = Children[index];              //this is bcause after changing the collection the element is other
                FillNewDraggedChild(_draggedElement);
                _draggedIndex = index;
            }

            this.InvalidateArrange();
        }

        private void FillNewDraggedChild(UIElement child)
        {
            if (child.RenderTransform as TransformGroup == null)
            {
                child.RenderTransformOrigin = new Point(0.5, 0.5);
                TransformGroup group = new TransformGroup();
                child.RenderTransform = group;
                group.Children.Add(new TranslateTransform());
            }
            SetZIndex(child, 1000);
            AnimateTo(child, _x, _y, 0);            //need relocate the element
        }

        private void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            FinishDrag();
        }

        private void FinishDrag()
        {
            if (_draggedElement != null)
            {
                SetZIndex(_draggedElement, 0);
                _draggedElement = null;
                this.InvalidateArrange();
            }
        }

        private void DoScroll()
        {
            if (scrollViewer != null)
            {
                Point position = Mouse.GetPosition(scrollViewer);
                double scrollMargin = Math.Min(scrollViewer.FontSize * 2, scrollViewer.ActualHeight / 2);

                if (position.X >= scrollViewer.ActualWidth - scrollMargin &&
                    scrollViewer.HorizontalOffset < scrollViewer.ExtentWidth - scrollViewer.ViewportWidth)
                {
                    scrollViewer.LineRight();
                }
                else if (position.X < scrollMargin && scrollViewer.HorizontalOffset > 0)
                {
                    scrollViewer.LineLeft();
                }
                else if (position.Y >= scrollViewer.ActualHeight - scrollMargin &&
                    scrollViewer.VerticalOffset < scrollViewer.ExtentHeight - scrollViewer.ViewportHeight)
                {
                    scrollViewer.LineDown();
                }
                else if (position.Y < scrollMargin && scrollViewer.VerticalOffset > 0)
                {
                    scrollViewer.LineUp();
                }
            }
        }
    }
}
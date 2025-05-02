using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Leepfrog.WpfFramework
{
    /// <summary>
    /// Interaction logic for DragBox.xaml
    /// </summary>
    public partial class DragBox : UserControl
    {
        public DragBox()
        {
            InitializeComponent();
        }

        // main ItemsControl / ListBox etc will have MoveItemCommand defined
        // item template should include a dragbox if item can be dragged

        // attached property to define drag scope
        public static readonly DependencyProperty MoveItemCommandProperty = DependencyProperty.RegisterAttached(
          "MoveItemCommand",
          typeof(ICommand),
          typeof(DragBox),
          new FrameworkPropertyMetadata(null)
        );
        
        public static void SetMoveItemCommand(UIElement element, ICommand value)
        {
            element.SetValue(MoveItemCommandProperty, value);
        }
        public static ICommand GetMoveItemCommand(UIElement element)
        {
            return (ICommand)element.GetValue(MoveItemCommandProperty);
        }

        public class MoveItemCommandParams
        {
            public MoveItemCommandParams(int oldIndex, int newIndex)
            {
                OldIndex = oldIndex;
                NewIndex = newIndex;
            }
            public int OldIndex { get; protected set; }
            public int NewIndex { get; protected set; }
        }

        private bool _isButtonDown = false;
        private bool _isDragging = false;
        private AdornerLayer _adornerLayer = null;
        private DragAdorner _dragAdorner = null;
        private Point _dragStartPos;
        private FrameworkElement _containerBeingDragged = null;
        private object _itemBeingDragged = null;
        private object _itemBeingDroppedOn = null;
        private ItemsControl _dragScope = null;
        private int _originalIndex = 0;
        private int _dropIndex = 0;

        private Dictionary<FrameworkElement, Thickness> _originalMargins;
        private Dictionary<FrameworkElement, Storyboard> _containerStoryboards;

        private void UserControl_MouseMove_1(object sender, MouseEventArgs e)
        {
            //-------------------------------------------------------------
            if (!_isButtonDown)
            {
                return;
            }
            //-------------------------------------------------------------
            if (!_isDragging)
            {
                //-------------------------------------------------------------
                var dragDistance = _dragStartPos - e.GetPosition(null);
                //-------------------------------------------------------------
                if (
                    (Math.Abs(dragDistance.X) >= SystemParameters.MinimumHorizontalDragDistance)
                    || (Math.Abs(dragDistance.Y) >= SystemParameters.MinimumVerticalDragDistance)
                    )
                {
                    //-------------------------------------------------------------
                    // Get the dragged Item
                    DragBox dragbox = sender as DragBox;
                    _dragScope = FindAncestor<ItemsControl>(dragbox, element => GetMoveItemCommand(element as UIElement) != null);
                    //-------------------------------------------------------------
                    if (_dragScope == null)
                    {
                        return;
                    }
                    //-------------------------------------------------------------
                    _itemBeingDragged = dragbox.DataContext;
                    _containerBeingDragged = _dragScope.ItemContainerGenerator.ContainerFromItem(_itemBeingDragged) as FrameworkElement;
                    _dragAdorner = new DragAdorner(_dragScope, _containerBeingDragged, e.GetPosition(_containerBeingDragged), 0.5);
                    _adornerLayer = AdornerLayer.GetAdornerLayer(_dragScope);
                    _adornerLayer.Add(_dragAdorner);
                    _isDragging = true;
                    _originalMargins = new Dictionary<FrameworkElement, Thickness>();
                    _originalMargins.Add(_containerBeingDragged, _containerBeingDragged.Margin);
                    _containerStoryboards = new Dictionary<FrameworkElement, Storyboard>();
                    _originalIndex = _dragScope.ItemContainerGenerator.IndexFromContainer(_containerBeingDragged);
                    _dropIndex = _originalIndex;
                    _containerBeingDragged.Opacity = 0.5;
                    _dragScope.MouseMove += _dragScope_MouseMove;
                    _dragScope.MouseLeftButtonUp += _dragScope_MouseLeftButtonUp;
                    _dragScope.CaptureMouse();
                    ReleaseMouseCapture();
                    //-------------------------------------------------------------
                }
                //-------------------------------------------------------------
            }
            //-------------------------------------------------------------
        }

        void _dragScope_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var command = GetMoveItemCommand(_dragScope);
            _dragScope.ReleaseMouseCapture();
            _dragScope.MouseLeftButtonUp -= _dragScope_MouseLeftButtonUp;
            _dragScope.MouseMove -= _dragScope_MouseMove;
            _adornerLayer.Remove(_dragAdorner);
            _dragAdorner = null;
            _adornerLayer = null;
            _isDragging = false;
            _containerBeingDragged.Opacity = 1;
            _containerBeingDragged = null;
            _isButtonDown = false;
            foreach (var sb in _containerStoryboards)
            {
                sb.Value.Stop(_dragScope);
            }
            _containerStoryboards = null;
            _dragScope = null;
            foreach (var set in _originalMargins)
            {
                set.Key.Margin = set.Value;
            }
            _originalMargins = null;
            //RAISE COMMAND!
            //command.Execute(new MoveItemCommandParams (_originalIndex, _dropIndex));
            if (_itemBeingDroppedOn != null)
            {
                (_itemBeingDragged as ISortableListItem).Sequence = (_itemBeingDroppedOn as ISortableListItem).Sequence;
            }
            _itemBeingDragged = null;
            _itemBeingDroppedOn = null;
            _originalIndex = 0;
            _dropIndex = 0;
        }

        void _dragScope_MouseMove(object sender, MouseEventArgs e)
        {
            // TODO: WE CAN GET NULL REFERENCE ERRORS HERE
            // FOR NOW, WE'LL JUST TRY CATCH
            // BUT WE SHOULD FIX IT SOMETIME
            try
            {
                //-------------------------------------------------------------
                var pos = e.GetPosition(null);
                var delta = pos - _dragStartPos;
                //-------------------------------------------------------------
                _dragAdorner.Position = e.GetPosition(_dragScope);
                _dragStartPos = pos;
                //-------------------------------------------------------------
                HitTestResult hitTestResult = null;
                VisualTreeHelper.HitTest
                    (
                        _dragScope,
                        (target) =>
                            {
                                var element = target as FrameworkElement;
                                if ((element == _containerBeingDragged) || (_containerStoryboards.ContainsKey(element)))
                                {
                                    return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
                                }
                                else
                                {
                                    return HitTestFilterBehavior.Continue;
                                }
                            },
                        (target) =>
                            {
                                hitTestResult = target;
                                return HitTestResultBehavior.Stop;
                            },
                        new PointHitTestParameters
                            (
                                new Point(5, e.GetPosition(_dragScope).Y - _dragAdorner.Offset.Y + _containerBeingDragged.ActualHeight / 2)
                            )
                    );
                //-------------------------------------------------------------
                if ((hitTestResult != null) && (hitTestResult.VisualHit != null))
                {
                    //-------------------------------------------------------------
                    // FIND THE ANCESTOR OF THE HIT TEST RESULT WHOSE DATA CONTEXT IS PART OF THE LIST
                    var item = FindAncestor<FrameworkElement>(
                        hitTestResult.VisualHit,
                        (element) =>
                            (
                                (element is FrameworkElement)
                             && (_dragScope.Items.Contains((element as FrameworkElement).DataContext))
                            )
                        );
                    //-------------------------------------------------------------
                    if (item != null)
                    {
                        //-------------------------------------------------------------
                        // GET THE CONTAINER FOR THIS ITEM
                        _itemBeingDroppedOn = item.DataContext;
                        item = _dragScope.ItemContainerGenerator.ContainerFromItem(item.DataContext) as FrameworkElement;
                        //-------------------------------------------------------------
                        // GET THE INDEX OF THAT CONTAINER
                        var newDropIndex = _dragScope.ItemContainerGenerator.IndexFromContainer(item);
                        //-------------------------------------------------------------
                        // IF WE'RE ABOVE THE ORIGINAL ITEM...
                        if (newDropIndex < _originalIndex)
                        {
                            //-------------------------------------------------------------
                            if (newDropIndex == _dropIndex)
                            {
                                newDropIndex++;
                            }
                            //-------------------------------------------------------------
                        }
                        //-------------------------------------------------------------
                        // IF WE'RE BELOW THE ORIGINAL ITEM...
                        else
                        {
                            //-------------------------------------------------------------
                            if (newDropIndex == _dropIndex)
                            {
                                newDropIndex--;
                            }
                            //-------------------------------------------------------------
                        }
                        //-------------------------------------------------------------
                        if (newDropIndex != _dropIndex)
                        {
                            //-------------------------------------------------------------
                            int thisIndex = Math.Min(_dropIndex, newDropIndex);
                            var direction = Math.Sign(_dropIndex - newDropIndex);
                            if (thisIndex > _originalIndex)
                            {
                                thisIndex++;
                            }
                            //-------------------------------------------------------------
                            // MOVE ALL ITEMS BETWEEN OLD DROP INDEX AND NEW DROP INDEX
                            for (var i = Math.Abs(newDropIndex - _dropIndex); i > 0; i--)
                            {
                                //-------------------------------------------------------------
                                // IF THIS IS THE ORIGINAL INDEX, MOVE FORWARD
                                if (thisIndex == _originalIndex)
                                {
                                    thisIndex++;
                                }
                                //-------------------------------------------------------------
                                // GET CONTAINER AT THAT INDEX
                                FrameworkElement thisContainer = _dragScope.ItemContainerGenerator.ContainerFromIndex(thisIndex) as FrameworkElement;
                                //-------------------------------------------------------------
                                // IF ORIGINAL MARGIN HASN'T ALREADY BEEN STORED...
                                if (!_originalMargins.ContainsKey(thisContainer))
                                {
                                    //-------------------------------------------------------------
                                    // STORE ORIGINAL MARGIN!
                                    _originalMargins.Add(thisContainer, thisContainer.Margin);
                                    //-------------------------------------------------------------
                                }
                                //-------------------------------------------------------------
                                var originalMargin = _originalMargins[thisContainer];
                                Thickness newMargin;
                                //-------------------------------------------------------------
                                // IF WE NEED TO ANIMATE ITEMS UP...
                                if (direction == -1)
                                {
                                    //-------------------------------------------------------------
                                    // AND WE'RE ABOVE THE START INDEX...
                                    if (thisIndex < _originalIndex)
                                    {
                                        //-------------------------------------------------------------
                                        // RESET TO NORMAL
                                        newMargin = originalMargin;
                                        //-------------------------------------------------------------
                                    }
                                    else
                                    {
                                        //-------------------------------------------------------------
                                        // ANIMATE UP
                                        newMargin = new Thickness(
                                            originalMargin.Left,
                                            originalMargin.Top - _containerBeingDragged.ActualHeight,
                                            originalMargin.Right,
                                            originalMargin.Bottom + _containerBeingDragged.ActualHeight
                                            );
                                        //-------------------------------------------------------------
                                    }
                                    //-------------------------------------------------------------
                                }
                                //-------------------------------------------------------------
                                // IF WE NEED TO ANIMATE ITEMS DOWN...
                                else
                                {
                                    //-------------------------------------------------------------
                                    // AND WE'RE BELOW THE START INDEX...
                                    if (thisIndex > _originalIndex)
                                    {
                                        //-------------------------------------------------------------
                                        // RESET TO NORMAL
                                        newMargin = originalMargin;
                                        //-------------------------------------------------------------
                                    }
                                    else
                                    {
                                        //-------------------------------------------------------------
                                        // ANIMATE DOWN
                                        newMargin = new Thickness(
                                            originalMargin.Left,
                                            originalMargin.Top + _containerBeingDragged.ActualHeight,
                                            originalMargin.Right,
                                            originalMargin.Bottom - _containerBeingDragged.ActualHeight
                                            );
                                        //-------------------------------------------------------------
                                    }
                                    //-------------------------------------------------------------
                                }
                                //-------------------------------------------------------------
                                var ani = new ThicknessAnimation
                                    (
                                        newMargin,
                                        new Duration(TimeSpan.FromSeconds(0.3))
                                    );
                                ani.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
                                Storyboard.SetTarget(ani, thisContainer);
                                Storyboard.SetTargetProperty(ani, new PropertyPath(FrameworkElement.MarginProperty));
                                //-------------------------------------------------------------
                                if (_containerStoryboards.ContainsKey(thisContainer))
                                {
                                    var sbToRemove = _containerStoryboards[thisContainer];
                                    _containerStoryboards.Remove(thisContainer);
                                    sbToRemove.Stop();
                                }
                                //-------------------------------------------------------------
                                var sb = new Storyboard();
                                sb.FillBehavior = FillBehavior.Stop;
                                sb.Children.Add(ani);
                                //-------------------------------------------------------------
                                sb.Completed += (s2, e2) =>
                                {
                                    var sbCompleted = (s2 as ClockGroup).Timeline as Storyboard;
                                    var containerCompleted = Storyboard.GetTarget(sbCompleted.Children[0]) as FrameworkElement;
                                    sbCompleted.Remove();
                                    if (_containerStoryboards == null)
                                    {
                                        return;
                                    }
                                    containerCompleted.Margin = containerCompleted.Margin;
                                    if (_containerStoryboards.ContainsKey(containerCompleted))
                                    {
                                        _containerStoryboards.Remove(containerCompleted);
                                    }
                                };
                                //-------------------------------------------------------------
                                _containerStoryboards.Add(thisContainer, sb);
                                sb.Begin(_dragScope, true);
                                //-------------------------------------------------------------
                                thisIndex++;
                                //-------------------------------------------------------------
                            }
                            //-------------------------------------------------------------
                        }
                        //-------------------------------------------------------------
                        _dropIndex = newDropIndex;
                        //-------------------------------------------------------------
                        // ANIMATE DRAGGED CONTAINER TO NEW POSITION
                        // (basically, just after previous item!)
                        {
                            var originalMargin = _containerBeingDragged.Margin;
                            double dest = 0;
                            if (_dropIndex != 0)
                            {
                                var containerBefore = _dragScope.ItemContainerGenerator.ContainerFromIndex(_dropIndex - 1) as FrameworkElement;
                                dest = containerBefore.TranslatePoint(new Point(0, containerBefore.ActualHeight), _dragScope).Y;
                                if (_originalMargins.ContainsKey(containerBefore))
                                {
                                    dest -= (containerBefore.Margin.Top - _originalMargins[containerBefore].Top);
                                }
                            };
                            var offset = _dragScope.TranslatePoint(new Point(0, dest), _containerBeingDragged).Y;
                            var newMargin = new Thickness
                                (
                                 originalMargin.Left,
                                 originalMargin.Top + offset,
                                 originalMargin.Right,
                                 originalMargin.Bottom - offset
                                );
                            var ani = new ThicknessAnimation
                                (
                                newMargin,
                                new Duration(TimeSpan.FromSeconds(0.3))
                                );
                            ani.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
                            Storyboard.SetTarget(ani, _containerBeingDragged);
                            Storyboard.SetTargetProperty(ani, new PropertyPath(FrameworkElement.MarginProperty));
                            if (_containerStoryboards.ContainsKey(_containerBeingDragged))
                            {
                                var sbToRemove = _containerStoryboards[_containerBeingDragged];
                                _containerStoryboards.Remove(_containerBeingDragged);
                                sbToRemove.Stop();
                            }
                            //-------------------------------------------------------------
                            var sb = new Storyboard();
                            sb.FillBehavior = FillBehavior.Stop;
                            sb.Children.Add(ani);
                            sb.Completed += (s2, e2) =>
                            {
                                var sbCompleted = (s2 as ClockGroup).Timeline as Storyboard;
                                sbCompleted.Remove();
                                if (_containerStoryboards == null)
                                {
                                    return;
                                }
                                _containerBeingDragged.Margin = _containerBeingDragged.Margin;
                                if (_containerStoryboards.ContainsKey(_containerBeingDragged))
                                {
                                    _containerStoryboards.Remove(_containerBeingDragged);
                                }
                            };
                            //-------------------------------------------------------------
                            _containerStoryboards.Add(_containerBeingDragged, sb);
                            sb.Begin(_dragScope, true);
                        }
                        //-------------------------------------------------------------
                    }
                    //-------------------------------------------------------------
                }
            }
            catch (NullReferenceException)
            {

            }
            catch (ArgumentNullException)
            {

            }
        }

        private void UserControl_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            _dragStartPos = e.GetPosition(null);
            Mouse.Capture(this);
            _isButtonDown = true;
        }

        private void UserControl_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
            _isButtonDown = false;
        }

        // Helper to search up the VisualTree
        private static T FindAncestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            return FindAncestor<T>(current, null);
        }
        private static T FindAncestor<T>(DependencyObject current, Func<DependencyObject, bool> filter)
            where T : DependencyObject
        {
            do
            {
                if (
                    (current is T) && 
                    (
                     (filter==null) 
                  || (filter(current))
                    )
                   )
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.Windows.Threading;


namespace Leepfrog.WpfFramework.Controls
{

    public class Picker : ListBox
    {
        static Picker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Picker), new FrameworkPropertyMetadata(typeof(Picker)));

        }

        public Picker()
        {
            ((INotifyCollectionChanged)Items).CollectionChanged += itemsChanged;
            _updateAfterScrollTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            _updateAfterScrollTimer.Tick += updateAfterScrollTimer_Tick;
        }

        private void itemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _hasBeenTouched = false;
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var lostItem = e.OldItems.OfType<object>().FirstOrDefault();
                if (lostItem != null && Object.Equals(_previousSelection, lostItem))
                {
                    _lastRemoved = _previousSelection;
                    _previousSelection = null;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var newItem = e.NewItems.OfType<object>().FirstOrDefault();
                if (newItem != null && Object.Equals(_lastRemoved, newItem))
                {
                    SelectedItem = newItem;
                    _lastRemoved = null;
                }
            }
        }

        public bool IsRepeating
        {
            get { return (bool)GetValue(IsRepeatingProperty); }
            set { SetValue(IsRepeatingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsRepeating.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsRepeatingProperty =
            DependencyProperty.Register("IsRepeating", typeof(bool), typeof(Picker), new PropertyMetadata(false));



        public bool IsRoundedLeft
        {
            get { return (bool)GetValue(IsRoundedLeftProperty); }
            set { SetValue(IsRoundedLeftProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsRoundedLeft.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsRoundedLeftProperty =
            DependencyProperty.Register("IsRoundedLeft", typeof(bool), typeof(Picker), new PropertyMetadata(true));



        public bool IsRoundedRight
        {
            get { return (bool)GetValue(IsRoundedRightProperty); }
            set { SetValue(IsRoundedRightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsRoundedRight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsRoundedRightProperty =
            DependencyProperty.Register("IsRoundedRight", typeof(bool), typeof(Picker), new PropertyMetadata(true));



        private AnimatableScrollViewer _scrollViewer;
        private Grid _spaceTop;
        private Grid _spaceBottom;
        private Grid _highlight;
        private FrameworkElement _presenter;
        private TranslateTransform _translate;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            // stop whole window moving when we reach top or bottom!
            _scrollViewer = (AnimatableScrollViewer)GetTemplateChild("PART_ScrollViewer");
            _spaceTop = (Grid)GetTemplateChild("PART_SpaceTop");
            _spaceBottom = (Grid)GetTemplateChild("PART_SpaceBottom");
            _highlight = (Grid)GetTemplateChild("PART_Highlight");
            _presenter = (FrameworkElement)GetTemplateChild("PART_Items");
            _translate = (TranslateTransform)GetTemplateChild("PART_Translate");
            if ( _scrollViewer != null )
            {
                _scrollViewer.ManipulationBoundaryFeedback += ScrollViewer_ManipulationBoundaryFeedback;
                _scrollViewer.ScrollChanged += _scrollViewer_ScrollChanged;
                _scrollViewer.GotTouchCapture += _scrollViewer_GotTouchCapture;
                _scrollViewer.LostTouchCapture += _scrollViewer_LostTouchCapture;
            }
        }

        private void _scrollViewer_GotTouchCapture(object sender, TouchEventArgs e)
        {
            _hasBeenTouched = true;
        }

        private void _scrollViewer_LostTouchCapture(object sender, TouchEventArgs e)
        {
            scrollToItem(SelectedItem);
        }

        private bool _hasBeenTouched = false;

        private void _scrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!_hasBeenTouched)
            {
                _updateAfterScrollTimer.Stop();
                _updateAfterScrollTimer.Start();
                return;
            }
            if ( _ani != null )
            {
                return;
            }
            // get the item in the middle of the list
            object item = null;
            var repeat = false;
            do
            {
                var result = VisualTreeHelper.HitTest(_scrollViewer, new Point(0, _scrollViewer.ActualHeight / 2));
                if (
                    (result.VisualHit == _spaceTop)
                 && (Items.Count > 0)
                   )
                {
                    if (IsRepeating)
                    {
                        _translate.Y -= _presenter.ActualHeight;
                        _updateAfterScrollTimer.Stop();
                        _updateAfterScrollTimer.Start();
                        return;
                    }
                    else
                    {
                        item = Items[0];
                    }
                }
                else if (
                         (result.VisualHit == _spaceBottom)
                      && (Items.Count > 0)
                        )
                {
                    if (IsRepeating)
                    {
                        _translate.Y += _presenter.ActualHeight;
                        _updateAfterScrollTimer.Stop();
                        _updateAfterScrollTimer.Start();
                        return;
                    }
                    else
                    {
                        item = Items[Items.Count - 1];
                    }
                }
                else
                {
                    item = (result.VisualHit as FrameworkElement).DataContext;
                }
            } while (repeat);

            // select it!
            if ( SelectedItem != item )
            {
                SelectedItem = item;
            }
            _updateAfterScrollTimer.Stop();
            _updateAfterScrollTimer.Start();
        }

        DispatcherTimer _updateAfterScrollTimer = new DispatcherTimer(DispatcherPriority.Normal, Application.Current.Dispatcher);

        private void updateAfterScrollTimer_Tick(object sender, EventArgs e)
        {
            _updateAfterScrollTimer.Stop();
            scrollToItem(SelectedItem);
        }

        private void ScrollViewer_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            // DON'T MOVE THE WHOLE WINDOW!
            e.Handled = true;
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (e.RemovedItems != null && (e.AddedItems == null || e.AddedItems.Count == 0))
            {
                _previousSelection = e.RemovedItems.OfType<object>().FirstOrDefault();
            }
            // IF ONLY A SINGLE ITEM ADDED...
            if ( e.AddedItems.Count == 1 )
            {
                // SCROLL TO PUT THIS ITEM IN THE CENTRE
                var item = e.AddedItems[0];
                scrollToItem(item);
            }
            base.OnSelectionChanged(e);
        }


        protected DoubleAnimation _ani;
        private object _lastRemoved;
        private object _previousSelection;

        protected void scrollToItem(object item)
        {
            if ( AreAnyTouchesCapturedWithin )
            {
                return;
            }
            if ( _updateAfterScrollTimer.IsEnabled )
            {
                return;
            }
            var container = (FrameworkElement)ItemContainerGenerator.ContainerFromItem(item);
            if ( container == null )
            {
                // TODO: do this somewhere else instead!
                return;
            }

            _ani = new DoubleAnimation();
            _ani.From = _scrollViewer.VerticalOffset - _translate.Y;
            _ani.To = _scrollViewer.VerticalOffset - _translate.Y + container.TranslatePoint(new Point(0, container.ActualHeight / 2), _scrollViewer).Y - _scrollViewer.ActualHeight / 2;
            _ani.Duration = TimeSpan.FromSeconds(0.3);
            _ani.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut, Power = 4 };
            _ani.Completed += (s2, e2) => { _ani = null; };
            _scrollViewer.BeginAnimation(AnimatableScrollViewer.CurrentVerticalOffsetProperty, _ani);

            _translate.Y = 0;
            var ani = new DoubleAnimation();
            ani.To = container.ActualHeight;
            ani.Duration = TimeSpan.FromSeconds(0.3);
            ani.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut, Power = 4 };
            _highlight.BeginAnimation(Grid.HeightProperty, ani);
        }
    }
}

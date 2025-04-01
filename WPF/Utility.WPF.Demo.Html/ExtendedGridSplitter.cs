using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;


namespace miRobotEditor.Controls
{
    [Localizable(false), TemplatePart(Name = "HorizontalGridSplitterHandle", Type = typeof(ToggleButton)),
     TemplatePart(Name = "LabelHandle", Type = typeof(ToggleButton)),
     TemplatePart(Name = "SwitchArrows", Type = typeof(ToggleButton)),
     TemplatePart(Name = "VerticalGridSplitterHandle", Type = typeof(ToggleButton)),
     TemplatePart(Name = "HorizontalTemplate", Type = typeof(FrameworkElement)),
     TemplatePart(Name = "VerticalTemplate", Type = typeof(FrameworkElement))]
    public class ExtendedGridSplitter : GridSplitter
    {
        private const string ElementLabelName = "LabelHandle";
        private const string ElementSwitcharrowHandleName = "SwitchArrows";
        private const string ElementHorizontalHandleName = "HorizontalGridSplitterHandle";
        private const string ElementVerticalHandleName = "VerticalGridSplitterHandle";
        private const string ElementHorizontalTemplateName = "HorizontalTemplate";
        private const string ElementVerticalTemplateName = "VerticalTemplate";
        private const string ElementGridsplitterBackground = "GridSplitterBackground";
        private const double AnimationTimeMillis = 200.0;
        public static bool Animating = false;
        private static ToggleButton _elementHorizontalGridSplitterButton;
        private static ToggleButton _elementVerticalGridSplitterButton;

        public static readonly DependencyProperty CollapseModeProperty = DependencyProperty.Register("CollapseMode",
            typeof(GridSplitterCollapseMode), typeof(ExtendedGridSplitter),
            new PropertyMetadata(GridSplitterCollapseMode.None, OnCollapseModePropertyChanged));

        public static readonly DependencyProperty HorizontalHandleStyleProperty =
            DependencyProperty.Register("HorizontalHandleStyle", typeof(Style), typeof(ExtendedGridSplitter), null);

        public static readonly DependencyProperty FocusColorProperty = DependencyProperty.Register("FocusColor",
            typeof(Color), typeof(ExtendedGridSplitter), null);

        public static readonly DependencyProperty UnfocusColorProperty = DependencyProperty.Register("UnfocusColor",
            typeof(Color), typeof(ExtendedGridSplitter), null);

        public static readonly DependencyProperty LabelStyleProperty = DependencyProperty.Register("LabelStyle",
            typeof(Style), typeof(ExtendedGridSplitter), null);

        public static readonly DependencyProperty SwitchArrowStyleProperty =
            DependencyProperty.Register("SwitchArrowStyle", typeof(Style), typeof(ExtendedGridSplitter), null);

        public static readonly DependencyProperty VerticalHandleStyleProperty =
            DependencyProperty.Register("VerticalHandleStyle", typeof(Style), typeof(ExtendedGridSplitter), null);

        public static readonly DependencyProperty IsAnimatedProperty = DependencyProperty.Register("IsAnimated",
            typeof(bool), typeof(ExtendedGridSplitter), null);

        public static readonly DependencyProperty IsCollapsedProperty = DependencyProperty.Register("IsCollapsed",
            typeof(bool), typeof(ExtendedGridSplitter), new PropertyMetadata(OnIsCollapsedPropertyChanged));

        private static Grid _parentGrid;

        private static readonly DependencyProperty RowHeightAnimationProperty =
            DependencyProperty.Register("RowHeightAnimation", typeof(double), typeof(ExtendedGridSplitter),
                new PropertyMetadata(RowHeightAnimationChanged));

        private RowDefinition _animatingRow;
        private Rectangle _elementGridSplitterBackground;
        private GridCollapseDirection _gridCollapseDirection = GridCollapseDirection.Rows;
        private double _savedActualValue;
        private GridLength _savedGridLength;

        public ExtendedGridSplitter()
        {
            DefaultStyleKey = typeof(ExtendedGridSplitter);
            HorizontalAlignment = HorizontalAlignment.Stretch;
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Center;
            VerticalContentAlignment = VerticalAlignment.Stretch;
            Height = 20.0;
            CollapseMode = GridSplitterCollapseMode.Previous;
            Loaded += delegate
            {
                Collapse();
                IsAnimated = true;
            };
        }

        public ToggleButton GridSplitterButton
        {
            get { return _elementVerticalGridSplitterButton; }
            set { _elementVerticalGridSplitterButton = value; }
        }

        public GridSplitterCollapseMode CollapseMode
        {
            get { return (GridSplitterCollapseMode)GetValue(CollapseModeProperty); }
            set { SetValue(CollapseModeProperty, value); }
        }

        public Style SwitchArrowStyle
        {
            get { return (Style)GetValue(SwitchArrowStyleProperty); }
            set { SetValue(SwitchArrowStyleProperty, value); }
        }

        public Style LabelStyle
        {
            get { return (Style)GetValue(LabelStyleProperty); }
            set { SetValue(LabelStyleProperty, value); }
        }

        public Color FocusColor
        {
            get { return (Color)GetValue(FocusColorProperty); }
            set { SetValue(FocusColorProperty, value); }
        }

        public Color UnfocusColor
        {
            get { return (Color)GetValue(UnfocusColorProperty); }
            set { SetValue(UnfocusColorProperty, value); }
        }

        public Style HorizontalHandleStyle
        {
            get { return (Style)GetValue(HorizontalHandleStyleProperty); }
            set { SetValue(HorizontalHandleStyleProperty, value); }
        }

        public Style VerticalHandleStyle
        {
            get { return (Style)GetValue(VerticalHandleStyleProperty); }
            set { SetValue(VerticalHandleStyleProperty, value); }
        }

        public bool IsAnimated
        {
            get { return (bool)GetValue(IsAnimatedProperty); }
            set { SetValue(IsAnimatedProperty, value); }
        }

        public bool IsCollapsed
        {
            get { return (bool)GetValue(IsCollapsedProperty); }
            set { SetValue(IsCollapsedProperty, value); }
        }

        public event EventHandler<EventArgs> Collapsed;
        public event EventHandler<EventArgs> Expanded;

        private static void OnIsCollapsedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var extendedGridSplitter = d as ExtendedGridSplitter;
            var isCollapsed = (bool)e.NewValue;
            if (extendedGridSplitter != null)
            {
                extendedGridSplitter.OnIsCollapsedChanged(isCollapsed);
            }
        }

        private static void OnCollapseModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var extendedGridSplitter = d as ExtendedGridSplitter;
            var collapseMode = (GridSplitterCollapseMode)e.NewValue;
            if (extendedGridSplitter != null)
            {
                extendedGridSplitter.OnCollapseModeChanged(collapseMode);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _elementHorizontalGridSplitterButton =
                (GetTemplateChild("HorizontalGridSplitterHandle") as ToggleButton);
            _elementVerticalGridSplitterButton = (GetTemplateChild("VerticalGridSplitterHandle") as ToggleButton);
            _elementGridSplitterBackground = (GetTemplateChild("GridSplitterBackground") as Rectangle);
            if (_elementHorizontalGridSplitterButton != null)
            {
                _elementHorizontalGridSplitterButton.Checked += GridSplitterButtonChecked;
                _elementHorizontalGridSplitterButton.Unchecked += GridSplitterButtonUnchecked;
            }
            if (_elementVerticalGridSplitterButton != null)
            {
                _elementVerticalGridSplitterButton.Checked += GridSplitterButtonChecked;
                _elementVerticalGridSplitterButton.Unchecked += GridSplitterButtonUnchecked;
            }
            _gridCollapseDirection = GridCollapseDirection.Rows;
            OnCollapseModeChanged(CollapseMode);
            OnIsCollapsedChanged(IsCollapsed);
        }

        protected virtual void OnIsCollapsedChanged(bool isCollapsed)
        {
            if (_elementVerticalGridSplitterButton != null)
            {
                _elementVerticalGridSplitterButton.IsChecked = isCollapsed;
            }
        }

        protected virtual void OnCollapseModeChanged(GridSplitterCollapseMode collapseMode)
        {
            if (_elementVerticalGridSplitterButton != null)
            {
                _elementVerticalGridSplitterButton.Visibility = Visibility.Visible;
            }
            switch (collapseMode)
            {
                case GridSplitterCollapseMode.Next:
                    if (_elementVerticalGridSplitterButton != null)
                    {
                    }
                    break;
                case GridSplitterCollapseMode.Previous:
                    if (_elementVerticalGridSplitterButton != null)
                    {
                    }
                    break;
            }
        }

        public void Collapse()
        {
            var grid = Parent as Grid;
            if (grid != null)
            {
                _parentGrid = grid;
            }
            else
            {
                grid = _parentGrid;
            }
            if (_gridCollapseDirection == GridCollapseDirection.Rows)
            {
                var num = (int)GetValue(Grid.RowProperty);
                if (CollapseMode == GridSplitterCollapseMode.Previous)
                {
                    if (grid != null)
                    {
                        _savedGridLength = grid.RowDefinitions[num + 1].Height;
                        _savedActualValue = grid.RowDefinitions[num + 1].ActualHeight;
                        if (IsAnimated)
                        {
                            AnimateCollapse(grid.RowDefinitions[num + 1]);
                        }
                        else
                        {
                            grid.RowDefinitions[num + 1].SetValue(RowDefinition.HeightProperty, new GridLength(0.0));
                        }
                    }
                }
                else
                {
                    if (grid != null)
                    {
                        _savedGridLength = grid.RowDefinitions[num + 1].Height;
                        _savedActualValue = grid.RowDefinitions[num + 1].ActualHeight;
                        if (IsAnimated)
                        {
                            AnimateCollapse(grid.RowDefinitions[num - 1]);
                        }
                        else
                        {
                            grid.RowDefinitions[num + 1].SetValue(RowDefinition.HeightProperty, new GridLength(0.0));
                        }
                    }
                }
            }
            IsCollapsed = true;
        }

        public void Expand()
        {
            var grid = Parent as Grid;
            if (grid != null)
            {
                _parentGrid = grid;
            }
            if (CollapseMode == GridSplitterCollapseMode.Previous)
            {
                if (IsAnimated)
                {
                    if (grid != null)
                    {
                        AnimateExpand(grid.RowDefinitions[2]);
                    }
                }
                else
                {
                    if (grid != null)
                    {
                        grid.RowDefinitions[2].SetValue(RowDefinition.HeightProperty, _savedGridLength);
                    }
                }
            }
            else
            {
                if (IsAnimated)
                {
                    if (grid != null)
                    {
                        AnimateExpand(grid.RowDefinitions[0]);
                    }
                }
                else
                {
                    if (grid != null)
                    {
                        grid.RowDefinitions[0].SetValue(RowDefinition.HeightProperty, _savedGridLength);
                    }
                }
            }
            IsCollapsed = false;
        }

        protected virtual void OnCollapsed(EventArgs e)
        {
            var collapsed = Collapsed;
            if (collapsed != null)
            {
                collapsed(this, e);
            }
        }

        protected virtual void OnExpanded(EventArgs e)
        {
            var expanded = Expanded;
            if (expanded != null)
            {
                expanded(this, e);
            }
        }

        private void GridSplitterButtonChecked(object sender, RoutedEventArgs e)
        {
            if (!IsCollapsed)
            {
                Collapse();
                IsCollapsed = true;
                _elementGridSplitterBackground.IsHitTestVisible = false;
                _elementGridSplitterBackground.Opacity = 0.5;
                OnCollapsed(EventArgs.Empty);
            }
        }

        private void GridSplitterButtonUnchecked(object sender, RoutedEventArgs e)
        {
            if (IsCollapsed)
            {
                Expand();
                IsCollapsed = false;
                _elementGridSplitterBackground.IsHitTestVisible = true;
                OnExpanded(EventArgs.Empty);
            }
        }

        private static void RowHeightAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var extendedGridSplitter = d as ExtendedGridSplitter;
            if (extendedGridSplitter != null)
            {
                extendedGridSplitter._animatingRow.Height = new GridLength((double)e.NewValue);
            }
        }

        private void AnimateCollapse(object definition)
        {
            var doubleAnimation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(200.0))
            };
            var storyboard = new Storyboard();
            storyboard.Children.Add(doubleAnimation);
            _animatingRow = (RowDefinition)definition;
            Storyboard.SetTarget(doubleAnimation, this);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("RowHeightAnimation", new object[0]));
            doubleAnimation.From = _animatingRow.ActualHeight;
            doubleAnimation.To = 0.0;
            storyboard.Begin();
        }

        private void AnimateExpand(object definition)
        {
            var doubleAnimation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(200.0))
            };
            var storyboard = new Storyboard();
            storyboard.Children.Add(doubleAnimation);
            _animatingRow = (RowDefinition)definition;
            Storyboard.SetTarget(doubleAnimation, this);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("RowHeightAnimation", new object[0]));
            doubleAnimation.From = _animatingRow.ActualHeight;
            doubleAnimation.To = _savedActualValue;
            storyboard.Begin();
        }

        private enum GridCollapseDirection
        {
            Rows
        }
    }

    public enum GridSplitterCollapseMode
    {
        None,
        Previous,
        Next,
   
    }
}
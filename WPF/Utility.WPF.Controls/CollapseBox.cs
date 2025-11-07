using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ReactiveUI;

namespace Utility.WPF.Controls
{
    [TemplatePart(Name = "PART_PathTransform", Type = typeof(RotateTransform))]
    public class CollapseBox : System.Windows.Controls.Primitives.ToggleButton
    {
        private RotateTransform _pathTransform;
        private ContentControl _collapsedContent;
        private ContentControl _expandedContent;

        public static readonly DependencyProperty CollapsedContentProperty =
            DependencyProperty.Register(nameof(CollapsedContent), typeof(object), typeof(CollapseBox), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty ExpandedContentProperty =
            DependencyProperty.Register(nameof(ExpandedContent), typeof(object), typeof(CollapseBox), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty ExpandTimeProperty =
            DependencyProperty.Register(nameof(ExpandTime), typeof(TimeSpan), typeof(CollapseBox), new PropertyMetadata(TimeSpan.FromMilliseconds(200)));

        public static readonly DependencyProperty TickThicknessProperty =
            DependencyProperty.Register(nameof(TickThickness), typeof(double), typeof(CollapseBox), new PropertyMetadata(1d));

        public static readonly DependencyProperty CollapsedHeightProperty =
            DependencyProperty.Register(nameof(CollapsedHeight), typeof(double), typeof(CollapseBox), new PropertyMetadata(25d, PropertyChanged));

        public static readonly DependencyProperty TickSizeProperty =
            DependencyProperty.Register(nameof(TickSize), typeof(double), typeof(CollapseBox), new PropertyMetadata(12d, PropertyChanged));

        public static readonly DependencyProperty RevertTickProperty =
            DependencyProperty.Register(nameof(RevertTick), typeof(bool), typeof(CollapseBox), new PropertyMetadata(false, PropertyChanged));

        public static readonly DependencyProperty TickVerticalAlignmentProperty =
            DependencyProperty.Register(nameof(TickVerticalAlignment), typeof(VerticalAlignment), typeof(CollapseBox), new PropertyMetadata(VerticalAlignment.Bottom));

        public static readonly DependencyProperty ExpandedHeightProperty =
            DependencyProperty.Register(nameof(ExpandedHeight), typeof(double), typeof(CollapseBox), new PropertyMetadata(default(double), PropertyChanged));

        public static readonly DependencyProperty ExpandOverContentProperty =
            DependencyProperty.Register(nameof(ExpandOverContent), typeof(bool), typeof(CollapseBox), new PropertyMetadata(false));

        static CollapseBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CollapseBox), new FrameworkPropertyMetadata(typeof(CollapseBox)));
        }

        public CollapseBox() : base()
        {
            this.WhenAnyValue(a => a.Content)
                .WithLatestFrom(this.WhenAnyValue(a => a.ExpandedContent))
                .Subscribe(a =>
            {
                if (a.Second == null && a.First != null)
                {
                    ExpandedContent = a.First;
                    Content = DependencyProperty.UnsetValue;
                }
            });
            ShowContents();
        }

        public object CollapsedContent
        {
            get => GetValue(CollapsedContentProperty);
            set => SetValue(CollapsedContentProperty, value);
        }

        public object ExpandedContent
        {
            get => GetValue(ExpandedContentProperty);
            set => SetValue(ExpandedContentProperty, value);
        }

        public TimeSpan ExpandTime
        {
            get => (TimeSpan)GetValue(ExpandTimeProperty);
            set => SetValue(ExpandTimeProperty, value);
        }

        public double TickThickness
        {
            get => (double)GetValue(TickThicknessProperty);
            set => SetValue(TickThicknessProperty, value);
        }

        public double TickSize
        {
            get => (double)GetValue(TickSizeProperty);
            set => SetValue(TickSizeProperty, value);
        }

        public bool RevertTick
        {
            get => (bool)GetValue(RevertTickProperty);
            set => SetValue(RevertTickProperty, value);
        }

        public VerticalAlignment TickVerticalAlignment
        {
            get => (VerticalAlignment)GetValue(TickVerticalAlignmentProperty);
            set => SetValue(TickVerticalAlignmentProperty, value);
        }

        public double CollapsedHeight
        {
            get => (double)GetValue(CollapsedHeightProperty);
            set
            {
                SetValue(CollapsedHeightProperty, value);
                if (IsChecked.Value == false)
                    Height = CollapsedHeight;
            }
        }

        public double ExpandedHeight
        {
            get => (double)GetValue(ExpandedHeightProperty);
            set
            {
                SetValue(ExpandedHeightProperty, value);
                if (IsChecked.Value)
                    Height = ExpandedHeight;
            }
        }

        /// <summary>
        /// Expands the ExpandedContent such that it occupies all of the space of CollapsedContent
        /// i.e only the ExpandedContent is visible when in the IsExpanded state.
        /// </summary>
        public bool ExpandOverContent
        {
            get => (bool)GetValue(ExpandOverContentProperty);
            set => SetValue(ExpandOverContentProperty, value);
        }

        private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            switch (d)
            {
                case CollapseBox collapseBox when e.Property == ExpandedHeightProperty:
                    collapseBox.ExpandedHeightChanged(e);
                    break;

                case CollapseBox collapseBox when e.Property == CollapsedHeightProperty:
                    collapseBox.CollapsedHeightChanged(e);
                    break;
            }
        }

        private void ExpandedHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            AnimateChecked(IsChecked ?? false, true);
        }

        private void CollapsedHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            AnimateChecked(IsChecked ?? false, true);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _pathTransform = GetTemplateChild("PART_PathTransform") as RotateTransform;
            _collapsedContent = GetTemplateChild("PART_CollapsedContent") as ContentControl;
            _expandedContent = GetTemplateChild("PART_ExpandedContent") as ContentControl;
            ShowContents();
        }

        protected override void OnChecked(RoutedEventArgs e)
        {
            base.OnChecked(e);

            AnimateChecked(true);
        }

        protected override void OnUnchecked(RoutedEventArgs e)
        {
            base.OnUnchecked(e);

            AnimateChecked(false);
        }

        private void AnimateChecked(bool expand, bool instant = false)
        {
            var pathAnim = new DoubleAnimation((expand ? -90 : 90) * (RevertTick ? -1 : 1), instant ? new Duration(TimeSpan.Zero) : new Duration(ExpandTime));
            var heightAnim = new DoubleAnimation(expand ? GetActualExpandedHeight() : CollapsedHeight, instant ? new Duration(TimeSpan.Zero) : new Duration(ExpandTime));
            heightAnim.Completed += (sender, ea) =>
            {
                ShowContents();
            };
            if (_collapsedContent != null)
                _collapsedContent.Visibility = Visibility.Collapsed;
            if (_expandedContent != null)
                _expandedContent.Visibility = Visibility.Collapsed;

            if (_pathTransform != null)
                _pathTransform.BeginAnimation(RotateTransform.AngleProperty, pathAnim);
            if (Template != null)
                BeginAnimation(HeightProperty, heightAnim);
            if (instant)
                ShowContents();
        }

        private void ShowContents()
        {
            bool expand = IsChecked ?? false;
            if (_pathTransform != null)
                _pathTransform.Angle = (expand ? -90 : 90) * (RevertTick ? -1 : 1);
            if (_collapsedContent != null)
                _collapsedContent.Visibility = expand && ExpandOverContent == true ? Visibility.Collapsed : Visibility.Visible;
            if (_expandedContent != null)
                _expandedContent.Visibility = expand ? Visibility.Visible : Visibility.Collapsed;
            Height = expand ? GetActualExpandedHeight() : CollapsedHeight;
        }

        private double GetActualExpandedHeight()
        {
            return PreliminaryExpandedHeight() + (ExpandOverContent ? 0 : CollapsedHeight);

            double PreliminaryExpandedHeight()
            {
                if (ExpandedHeight == default)
                {
                    var element = this.ExpandedContent switch
                    {
                        FrameworkElement elem => elem,
                        _ => new ContentControl { Content = ExpandedContent }
                    };
                    element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    return element.DesiredSize.Height;
                }
                else
                    return ExpandedHeight;
            }
        }
    }
}
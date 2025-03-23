using Jellyfish.Annotations;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using Utility.Changes;
using Utility.Reactives;
using Utility.WPF.Helpers;
using Utility.WPF.Reactives;

namespace Utility.WPF.Demo.Adorners
{
    /// <summary>
    /// Interaction logic for TreeViewHover.xaml
    /// </summary>
    public partial class TreeViewHoverUserControl : UserControl
    {

        public TreeViewHoverUserControl()
        {
            InitializeComponent();
        }
    }



    public class MouseMoveItems : Behavior<TreeView>
    {

        public ObservableCollection<object> Items { get; } = new();

        protected override void OnAttached()
        {
            base.OnAttached();


            AssociatedObject
                .MouseMoves()
                .Where(a => a.item != null)
                .Subscribe(treeViewItem =>
                {
                    if (Items.Count > 5)
                    {
                        Items.RemoveAt(0);
                    }
                    if (Items.Contains(treeViewItem.item) == false)
                        Items.Add(treeViewItem.item);
                });
        }
    }

    public class HighlightItems : Behavior<TreeView>
    {
        public static readonly DependencyProperty ItemsProperty =
        DependencyProperty.Register("Items", typeof(IEnumerable), typeof(HighlightItems), new PropertyMetadata(changed));
        private AdornerLayer layer;
        Dictionary<TreeViewItem, DropTargetHintAdorner> dictionary = new();

        private static void changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HighlightItems hitems && e.NewValue is IEnumerable enumerable)
            {
                hitems.update(enumerable);
            }
        }

        private void update(IEnumerable enumerable)
        {


            enumerable.AndChanges<object>().Subscribe(a =>
            {
                foreach (var item in a)
                {
                    TreeViewItem tvi = null;
                    layer ??= AdornerLayer.GetAdornerLayer(AssociatedObject);

                    if (item is Change { Type: { } type, Value: var _value } _tiv)
                    {
                        if (_value is TreeViewItem value)

                            tvi = value;

                        else
                            tvi = (TreeViewItem)(AssociatedObject.ItemContainerGenerator.ContainerFromItem(item));
                        if (type == Changes.Type.Add)
                        {
                            dictionary[tvi] = new DropTargetHintAdorner(tvi, new Ellipse { Fill = Brushes.Red, Height = 20, Width = 20 });
                            layer.Add(dictionary[tvi]);
                        }
                        else if (type == Changes.Type.Remove)
                        {
                            layer.Remove(dictionary[tvi]);
                            dictionary.Remove(tvi);
                        }         
                        else if (type == Changes.Type.Reset)
                        {
                            layer.Clear();
                            dictionary.Clear();
                        }
                    }

                }
            });
        }

        public IEnumerable Items
        {
            get { return (IEnumerable)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }


        protected override void OnAttached()
        {
            base.OnAttached();
        }
    }


    /// <summary>
    /// This adorner is used to display hints for where items can be dropped.
    /// </summary>
    public class DropTargetHintAdorner : Adorner
    {
        private readonly ContentPresenter presenter;
        [CanBeNull]
        private readonly AdornerLayer adornerLayer;

        //public static readonly DependencyProperty objectProperty
        //    = DependencyProperty.Register(nameof(Object),
        //                                  typeof(object),
        //                                  typeof(DropTargetHintAdorner),
        //                                  new PropertyMetadata(default(object)));

        //public object Object
        //{
        //    get => (object)this.GetValue(objectProperty);
        //    set => this.SetValue(objectProperty, value);
        //}

        public DropTargetHintAdorner(UIElement adornedElement, UIElement content)
            : base(adornedElement)
        {
            //this.SetCurrentValue(objectProperty, _object);
            this.IsHitTestVisible = false;
            this.AllowDrop = false;
            this.SnapsToDevicePixels = true;
            //this.adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            //this.adornerLayer?.Add(this);

            this.presenter = new ContentPresenter()
            {
                IsHitTestVisible = false,

            };
            //var binding = new Binding(nameof(this.Object))
            //{
            //    Source = this,
            //    Mode = BindingMode.OneWay
            //};
            this.presenter.Content = content;
            //this.presenter.SetBinding(ContentPresenter.ContentProperty, binding);
        }

        /// <summary>
        /// Detach the adorner from its adorner layer.
        /// </summary>
        public void Detach()
        {
            if (this.adornerLayer is null)
            {
                return;
            }

            if (!this.adornerLayer.Dispatcher.CheckAccess())
            {
                this.adornerLayer.Dispatcher.Invoke(this.Detach);
                return;
            }

            this.adornerLayer.Remove(this);
        }

        private static Rect GetBounds(FrameworkElement element, UIElement visual)
        {
            return new Rect(
                element.TranslatePoint(new Point(0, 0), visual),
                element.TranslatePoint(new Point(element.ActualWidth, element.ActualHeight), visual));
        }

        protected override Size MeasureOverride(Size constraint)
        {
            this.presenter.Measure(constraint);
            return this.presenter.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var bounds = GetBounds(this.AdornedElement as FrameworkElement, this.AdornedElement);
            this.presenter.Arrange(bounds);
            return bounds.Size;
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.presenter;
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Update hint text and state for the adorner.
        /// </summary>
        /// <param name="hintData"></param>
        public void Update(object hintData)
        {
        }

    }
}

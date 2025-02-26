using Jellyfish.Annotations;
using Microsoft.Xaml.Behaviors;
using System.Collections;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using Utility.Changes;
using Utility.Reactives;
using Utility.WPF.Helpers;

namespace Utility.Nodes.Demo.Queries
{
    /// <summary>
    /// Interaction logic for TreeViewHover.xaml
    /// </summary>


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
                            tvi = (TreeViewItem)AssociatedObject.ItemContainerGenerator.ContainerFromItem(_value);
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
                            layer.RemoveAdorners();
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
            IsHitTestVisible = false;
            AllowDrop = false;
            SnapsToDevicePixels = true;
            //this.adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            //this.adornerLayer?.Add(this);

            presenter = new ContentPresenter()
            {
                IsHitTestVisible = false,

            };
            //var binding = new Binding(nameof(this.Object))
            //{
            //    Source = this,
            //    Mode = BindingMode.OneWay
            //};
            presenter.Content = content;
            //this.presenter.SetBinding(ContentPresenter.ContentProperty, binding);
        }

        /// <summary>
        /// Detach the adorner from its adorner layer.
        /// </summary>
        public void Detach()
        {
            if (adornerLayer is null)
            {
                return;
            }

            if (!adornerLayer.Dispatcher.CheckAccess())
            {
                adornerLayer.Dispatcher.Invoke(Detach);
                return;
            }

            adornerLayer.Remove(this);
        }

        private static Rect GetBounds(FrameworkElement element, UIElement visual)
        {
            return new Rect(
                element.TranslatePoint(new Point(0, 0), visual),
                element.TranslatePoint(new Point(element.ActualWidth, element.ActualHeight), visual));
        }

        protected override Size MeasureOverride(Size constraint)
        {
            presenter.Measure(constraint);
            return presenter.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var bounds = GetBounds(AdornedElement as FrameworkElement, AdornedElement);
            presenter.Arrange(bounds);
            return bounds.Size;
        }

        protected override Visual GetVisualChild(int index)
        {
            return presenter;
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

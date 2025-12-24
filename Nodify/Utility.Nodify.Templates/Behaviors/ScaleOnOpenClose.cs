using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using Microsoft.Xaml.Behaviors;
using Nodify;
using Utility.WPF.Controls.ComboBoxes;
using Utility.WPF.Helpers;

namespace Utility.Nodify.Views.Infrastructure
{
    public class ScaleOnOpenClose : Behavior<ComboBoxTreeView>
    {
        private NodifyEditor? editor;

        double minRequiredSpace = 400;
        double originalTransformY = 0;
        double originalScale = 0;
        double scale = 0;
        private Canvas? canvas;

        protected override void OnAttached()
        {
            //AssociatedObject.FindChild<Popup>().Opened += AssociatedObject_DropDownOpened;
            AssociatedObject.Loaded += AssociatedObject_Loaded;
            AssociatedObject.DropDownOpening += AssociatedObject_DropDownOpening;
            AssociatedObject.DropDownClosed += AssociatedObject_DropDownClosed;
            editor = AssociatedObject.FindParent<NodifyEditor>();

            //AssociatedObject.Opened += AssociatedObject_Opened;
            base.OnAttached();
        }

        private void AssociatedObject_Opened(object sender, RoutedEventArgs e)
        {
        }

        void _scaleTransform(out ScaleTransform scaleTransform, out TranslateTransform translateTransform)
        {
            var transformGroup = editor.ViewportTransform as TransformGroup;
            if (transformGroup == null) throw new Exception("VFDGDS");

            scaleTransform = transformGroup.Children.OfType<ScaleTransform>().FirstOrDefault();
            translateTransform = transformGroup.Children.OfType<TranslateTransform>().FirstOrDefault();

            if (scaleTransform == null || translateTransform == null) throw new Exception("VF£cc ed");
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            canvas = editor.ChildOfType<Canvas>();
            canvas.SizeChanged += Canvas_SizeChanged;
            var popup = AssociatedObject.FindChild<Popup>();
            popup.Opened += Popup_Opened;
            popup.Closed += Popup_Closed;
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var popup = AssociatedObject.FindChild<Popup>();
            popup.IsOpen = false;
            editor.IsEnabled = editor.IsHitTestVisible = true;
            _scaleTransform(out var scaleTransform, out var translateTransform);
            translateTransform.Y = originalTransformY;
            //translateTransform.Y = originalTransformY;
            //AnimateScale(scaleTransform, originalScale, TimeSpan.FromSeconds(0.5));
            scaleTransform.ScaleX = originalScale;
            scaleTransform.ScaleY = originalScale;
        }

        private void Popup_Closed(object? sender, EventArgs e)
        {
            _scaleTransform(out var scaleTransform, out var translateTransform);
            editor.IsEnabled = editor.IsHitTestVisible = true;
            translateTransform.Y = originalTransformY;
            //translateTransform.Y = originalTransformY;
            //AnimateScale(scaleTransform, originalScale, TimeSpan.FromSeconds(0.5));
            scaleTransform.ScaleX = originalScale;
            scaleTransform.ScaleY = originalScale;
     
        }

        private void Popup_Opened(object? sender, EventArgs e)
        {
   
            //minRequiredSpace = (sender as FrameworkElement).ActualHeight;
        }


        private void AssociatedObject_DropDownClosed(object? sender, EventArgs e)
        {
            editor.IsEnabled = editor.IsHitTestVisible = true;
            _scaleTransform(out var scaleTransform, out var translateTransform);
            //translateTransform.Y = originalTransformY;
            AnimateScale(scaleTransform, originalScale, TimeSpan.FromSeconds(0.5));
        }

        double heightFromBottomOfContainer(ComboBoxTreeView node, Canvas canvas, NodifyEditor Editor)
        {

            _scaleTransform(out var scaleTransform, out var translateTransform);

            // Get ComboBox position
            var nodePosition = node.TransformToAncestor(canvas).Transform(new Point(0, 0));

            // Calculate node's bottom edge in canvas coordinates
            var nodeBottom = nodePosition.Y + node.ActualHeight;

            // Apply transformations to get viewport coordinates
            var transformedNodeBottom = (nodeBottom * scale) + translateTransform.Y;

            // Space from node bottom to canvas bottom in viewport coordinates
            var spaceFromBottom = (canvas.ActualHeight - transformedNodeBottom) / scale;

            return spaceFromBottom;

        }
        double heightFromTopOfContainer(ComboBoxTreeView node, Canvas canvas, NodifyEditor Editor)
        {
            _scaleTransform(out var scaleTransform, out var translateTransform);
            // Get node position in canvas coordinates
            var nodePosition = node.TransformToAncestor(canvas).Transform(new Point(0, 0));

            // Apply transformations to get viewport coordinates
            var transformedNodeTop = (nodePosition.Y * scale) + translateTransform.Y;

            // Space from container top (0) to node top
            var spaceFromTop = transformedNodeTop / scale;

            return spaceFromTop;

        }


        private void AssociatedObject_DropDownOpening(object? sender, EventArgs e)
        {
            var comboBox = AssociatedObject as ComboBoxTreeView;
            if (comboBox == null) return;

            var renderTransform = editor.ViewportTransform;

            _scaleTransform(out var scaleTransform, out var translateTransform);


            var canvas = editor.ChildOfType<Canvas>();
            if (canvas == null) return;


            originalTransformY = translateTransform.Y;
            scale = originalScale = scaleTransform.ScaleY;

            while ((heightFromBottomOfContainer(this.AssociatedObject, canvas, editor) < minRequiredSpace ||
                heightFromTopOfContainer(this.AssociatedObject, canvas, editor) < 10))
            {
                if (canvas.ActualHeight/ scaleTransform.ScaleY > minRequiredSpace)
                    translateTransform.Y -= 5;     
                else
                    scale *= 0.95;
            }

            //AnimateScale(scaleTransform, scale, TimeSpan.FromSeconds(0.3));
            //scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
            //scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, null);

            scaleTransform.ScaleX = scale;
            scaleTransform.ScaleY = scale;
            editor.IsEnabled = editor.IsHitTestVisible = false;
        }

        public void AnimateScale(ScaleTransform scaleTransform, double targetScale, TimeSpan duration)
        {
            var animation = new DoubleAnimation
            {
                To = targetScale,
                Duration = duration,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut },
                //FillBehavior = FillBehavior.Stop,
            };

            // Animate both X and Y
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animation);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms.Design.Behavior;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using LanguageExt;
using Microsoft.Xaml.Behaviors;
using Nodify;
using Utility.Nodes;
using Utility.WPF.Controls.ComboBoxes;
using Utility.WPF.Helpers;

namespace Utility.Nodify.Views.Infrastructure
{

    public class CloseWhenScrollingExceedControlsBounds : Behavior<ComboBoxTreeView>
    {
        private NodifyEditor? editor;

        double minRequiredSpace = 400;
        double originalTransformY = 0;
        double originalScale = 0;
        double scale = 0;
        private Canvas? canvas;

        protected override void OnAttached()
        {

            editor = AssociatedObject.FindParent<NodifyEditor>();
            if (AssociatedObject.IsLoaded)
                AssociatedObject_Loaded(default, default);
            else
                AssociatedObject.Loaded += AssociatedObject_Loaded;
            base.OnAttached();
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            canvas = editor.ChildOfType<Canvas>();
            var popup = AssociatedObject.FindChild<Popup>();
            popup.Opened += Popup_Opened;
            popup.Closed += Popup_Closed;
        }

        private void Popup_Closed(object? sender, EventArgs e)
        {

            canvas.PreviewMouseMove -= Canvas_PreviewMouseMove;
        }

        private void Popup_Opened(object? sender, EventArgs e)
        {
            canvas.PreviewMouseMove += Canvas_PreviewMouseMove;
            //minRequiredSpace = (sender as FrameworkElement).ActualHeight;
        }

        private void Canvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            // Check if mouse is coming from the popup area
            var canvas = sender as Canvas;
            var popup = AssociatedObject.FindChild<Popup>();

            if (popup != null && popup.IsOpen && popup.Child is FrameworkElement element)
            {
                // Get mouse position relative to popup content
                Point mousePos = e.GetPosition(element);
                Debug.WriteLine(mousePos.Y);
                // Check if mouse is outside popup bounds
                if (mousePos.X < 0 || mousePos.Y < 0 ||
                    mousePos.X > (20 + element.ActualWidth) ||
                    mousePos.Y > (20 + element.ActualHeight))
                {
                    e.Handled = true;
                    popup.IsOpen = false;
                }
            }
        }

    }
}

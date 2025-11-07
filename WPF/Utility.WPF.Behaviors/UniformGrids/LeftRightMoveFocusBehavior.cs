using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using Utility.WPF.Helpers;

namespace Utility.WPF.Behaviors.UniformGrids
{
    public class LeftRightMoveFocusBehavior : Behavior<ListBox>
    {
        private UniformGrid uniformGrid;

        protected override void OnAttached()
        {
            if (AssociatedObject.IsLoaded)
            {
                OnLoaded();
            }
            else
            {
                AssociatedObject.Loaded += (a, e) => OnLoaded();
            }

            base.OnAttached();
        }

        private void OnLoaded()
        {
            if (VisualTreeExHelper.FindItemsPanel(AssociatedObject) is not UniformGrid uniformGrid)
            {
                throw new Exception($"ItemsPanel must be of type {nameof(UniformGrid)}");
            }
            this.uniformGrid = uniformGrid;
            AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewKeyDown -= AssociatedObject_PreviewKeyDown;

            base.OnDetaching();
        }

        private void AssociatedObject_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            OnPreviewKeyDown(e);
        }

        protected void OnPreviewKeyDown(KeyEventArgs e)
        {
            int rows = uniformGrid.Rows;
            int columns = uniformGrid.Columns;

            int index = AssociatedObject.SelectedIndex;

            int row = index / columns;  // divide
            int column = index % columns;  // modulus

            if (e.Key == Key.Left && column == 0)
            {
                ChangeIndex(index - 1);
            }

            if (e.Key == Key.Right && column == columns - 1)
            {
                ChangeIndex(index + 1);
            }

            void ChangeIndex(int newIndex)
            {
                if (newIndex < 0)
                    return;
                var container = AssociatedObject.ItemContainerGenerator.ContainerFromIndex(newIndex);
                if (container is FrameworkElement frameworkElement)
                    frameworkElement.Focus();
                e.Handled = true;
            }
        }
    }
}
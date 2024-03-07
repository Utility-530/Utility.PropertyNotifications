using Utility.Nodify.Core;
using Utility.Nodify.Demo;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Nodify;

namespace Utility.Nodify.Demo
{
    public partial class DiagramView : ResourceDictionary
    {
        public DiagramView()
        {
            EventManager.RegisterClassHandler(typeof(NodifyEditor), UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(CloseOperationsMenu));
            EventManager.RegisterClassHandler(typeof(ItemContainer), ItemContainer.DragStartedEvent, new RoutedEventHandler(CloseOperationsMenu));
            EventManager.RegisterClassHandler(typeof(NodifyEditor), UserControl.MouseRightButtonUpEvent, new MouseButtonEventHandler(OpenOperationsMenu));
        }

        private void OpenOperationsMenu(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled && e.OriginalSource is NodifyEditor editor && !editor.IsPanning && editor.DataContext is Engine.ViewModels.DiagramViewModel diagram)
            {
                e.Handled = true;
                diagram.OpenAt(editor.MouseLocation);
            }
        }

        private void CloseOperationsMenu(object sender, RoutedEventArgs e)
        {
            ItemContainer? itemContainer = sender as ItemContainer;
            NodifyEditor? editor = sender as NodifyEditor ?? itemContainer?.Editor;

            if (!e.Handled && editor?.DataContext is Engine.ViewModels.DiagramViewModel calculator)
            {
                calculator.Menu.Close();
            }
        }
    }
}

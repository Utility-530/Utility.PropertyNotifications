using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Nodify;
using System.Drawing;
using Utility.Nodes;
using Microsoft.CodeAnalysis.CSharp;
using Splat;
using Utility.ServiceLocation;

namespace Utility.Nodify.Demo
{
    public partial class DiagramView : ResourceDictionary
    {
        public DiagramView()
        {
            EventManager.RegisterClassHandler(typeof(NodifyEditor), UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(CloseOperationsMenu));
            EventManager.RegisterClassHandler(typeof(ItemContainer), ItemContainer.DragLeaveEvent, new RoutedEventHandler(CloseOperationsMenu));
            EventManager.RegisterClassHandler(typeof(NodifyEditor), UserControl.MouseRightButtonUpEvent, new MouseButtonEventHandler(OpenOperationsMenu));
        }

        private void OpenOperationsMenu(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled && sender is NodifyEditor editor && !editor.IsPanning && editor.DataContext is DiagramViewModel diagram)
            {
                e.Handled = true;
                diagram.OpenAt(new PointF((float)editor.MouseLocation.X, (float)editor.MouseLocation.Y));
            }
        }

        private void CloseOperationsMenu(object sender, RoutedEventArgs e)
        {
            ItemContainer? itemContainer = sender as ItemContainer;
            NodifyEditor? editor = sender as NodifyEditor ?? itemContainer?.Editor;

            if (!e.Handled && editor?.DataContext is DiagramViewModel calculator)
            {
                calculator.Menu.Close();
            }
        }

        private void PART_Connector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public static CSharpCompilation Compilation => Globals.Resolver.Resolve<CSharpCompilation>() ?? throw new System.InvalidOperationException("CSharpCompilation service not registered.");
    }
}

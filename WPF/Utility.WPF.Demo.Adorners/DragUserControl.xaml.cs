using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Utility.Helpers;
using Utility.WPF.Adorners;
using Utility.WPF.Adorners.Infrastructure;
using Utility.WPF.Helpers;

namespace Utility.WPF.Demo.Adorners
{
    /// <summary>
    /// Interaction logic for DragUserControl.xaml
    /// </summary>
    public partial class DragUserControl : UserControl
    {
        private Dictionary<UIElement, ResizeAdorner> resizeAdorners = new();

        public DragUserControl()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(canvas);
            foreach (UIElement ui in canvas.Children)
            {
                layer.AddIfMissingAdorner(resizeAdorners.Get(ui, () => new ResizeAdorner(ui)));
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            foreach (UIElement ui in canvas.Children)
            {
                ui.RemoveAdorners();
            }
        }
    }
}
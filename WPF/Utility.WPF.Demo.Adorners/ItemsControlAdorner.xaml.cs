using System.Windows;
using System.Windows.Controls;
using Utility.Commands;

namespace Utility.WPF.Demo.Adorners
{
    /// <summary>
    /// Interaction logic for ItemsControlAdorner.xaml
    /// </summary>
    public partial class ItemsControlAdorner : UserControl
    {
        public ItemsControlAdorner()
        {
            InitializeComponent();
            ShowAdornerExample();
        }

        private void ShowAdornerExample()
        {
            // Assuming you have an ItemsControl named "MyItemsControl" in XAML
            // and you want to show a "+" button where the next item would be

            // Add the adorner (you would typically do this after the ItemsControl is loaded)
            //this.Loaded += (s, e) =>
            //{
            //    LastItemPositionAdornerHelper.AddLastItemAdorner(MyItemsControl, () =>
            //    {
            //        MessageBox.Show("Add new item clicked!");
            //    });
            //};
        }
    }

    public class MessageBoxCommand : Command
    {
        public MessageBoxCommand() : base(() => MessageBox.Show("Add new item clicked!"), () => true)
        {
        }

        public static MessageBoxCommand Instance { get; } = new MessageBoxCommand();
    }
}

///// <summary>
///// Helper class to easily add and remove the adorner
///// </summary>
//public static class LastItemPositionAdornerHelper
//{
//    public static void AddLastItemAdorner(ItemsControl itemsControl, Action content)
//    {
//        if (itemsControl == null || content == null) return;

//        var adornerLayer = AdornerLayer.GetAdornerLayer(itemsControl);
//        if (adornerLayer != null)
//        {
//            var adorner = new LastItemAdorner(itemsControl, content);
//            adornerLayer.Add(adorner);
//        }
//    }

//    public static void RemoveLastItemAdorner(ItemsControl itemsControl)
//    {
//        if (itemsControl == null) return;

//        var adornerLayer = AdornerLayer.GetAdornerLayer(itemsControl);
//        if (adornerLayer != null)
//        {
//            var adorners = adornerLayer.GetAdorners(itemsControl);
//            if (adorners != null)
//            {
//                foreach (var adorner in adorners)
//                {
//                    if (adorner is LastItemAdorner)
//                    {
//                        adornerLayer.Remove(adorner);
//                    }
//                }
//            }
//        }
//    }
//}
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace Utility.WPF.Demo.Panels
{
    /// <summary>
    /// Interaction logic for DemoCirclePanel.xaml
    /// </summary>
    public partial class RegionPanel3View : UserControl
    {
        private readonly Array array;

        public RegionPanel3View()
        {
            InitializeComponent();
            array = this.Resources["Array"] as Array;

            if (false)
                for (int i = 0; i < 3; i++)
                {
                    var itemsControl = new StackPanel();
                    foreach (var item2 in array)
                    {
                        var clone = DeepClone2(item2);
                        itemsControl.Children.Add(clone as UIElement);
                    }
                    CirclePanel1.Children.Add(itemsControl);
                    i++;
                }
            else
            {
                foreach (var item2 in array)
                    CirclePanel1.Children.Add(item2 as UIElement);
            }
        }

        private void Bottom_Click(object sender, RoutedEventArgs e)
        {
            SetRegion(Region.Bottom);
        }

        private void Top_Click(object sender, RoutedEventArgs e)
        {
            SetRegion(Region.Top);
        }

        private void Right_Click(object sender, RoutedEventArgs e)
        {
            SetRegion(Region.Right);
        }

        private void Left_Click(object sender, RoutedEventArgs e)
        {
            SetRegion(Region.Left);
        }

        private void Middle_Click(object sender, RoutedEventArgs e)
        {
            SetRegion(Region.Middle);
        }

        private void SetRegion(Region region)
        {
            int i = 0;
            foreach (var item in array)
            {
                if (i++ > 0)
                    RegionPanel.SetRegion(item as UIElement, region);
            }
        }
        public static T DeepClone2<T>(T from)
        {
            string gridXaml = XamlWriter.Save(from);

            //Load it into a new object:
            StringReader stringReader = new StringReader(gridXaml);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            T newGrid = (T)XamlReader.Load(xmlReader);
            return newGrid;
        }
    }
}
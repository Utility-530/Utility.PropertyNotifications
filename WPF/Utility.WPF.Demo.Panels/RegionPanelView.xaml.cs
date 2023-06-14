using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Utility.WPF.Demo.Panels
{
    /// <summary>
    /// Interaction logic for EdgePanelDemo.xaml
    /// </summary>
    public partial class RegionPanelView : UserControl
    {
        private readonly Random random = new Random();
        private readonly CircleRegion[] cr = Enum.GetValues(typeof(CircleRegion)).Cast<CircleRegion>().ToArray();

        private readonly Brush[] brushes = new[] { Brushes.Teal, Brushes.Red, Brushes.RoyalBlue, Brushes.GreenYellow, Brushes.Gray, Brushes.BurlyWood,
            Brushes.Tomato , Brushes.Coral};

        private readonly ObservableCollection<CircleRegion> collection = new ObservableCollection<CircleRegion>();

        public RegionPanelView()
        {
            InitializeComponent();
            RegionsDataGrid.ItemsSource = collection;

            //foreach (var rec in EdgePanelControl.Children.OfType<UIElement>())
            //{
            //    var region = EdgeLegacyPanel.GetCircleRegion(rec);
            //    if (collection.Contains(region) == false)
            //        collection.Add(region);
            //}
        }

        private void AddToCollection(object sender, RoutedEventArgs e)
        {
            //    var next = random.Next(0, cr.Length - 1);
            //    var crs = cr[next];
            //    //var crs = CircleRegion.Right;
            //    var rec = new Rectangle { Fill = brushes[next], Opacity = 0.5 };

            //    EdgeLegacyPanel.SetCircleRegion(rec, crs);

            //    this.EdgePanelControl.Children.Add(rec);
            //    if (collection.Contains(crs) == false)
            //        collection.Add(crs);
        }

        private void RemoveFromCollection(object sender, RoutedEventArgs e)
        {
            //    while (EdgePanelControl.Children.Count > 0)
            //    {
            //        this.EdgePanelControl.Children.RemoveAt(this.EdgePanelControl.Children.Count - 1);
            //    }
        }

        private void EdgePanelControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //    HeightBox.Text = e.NewSize.Height.ToString("F");
            //    WidthBox.Text = e.NewSize.Width.ToString("F");
        }
    }
}
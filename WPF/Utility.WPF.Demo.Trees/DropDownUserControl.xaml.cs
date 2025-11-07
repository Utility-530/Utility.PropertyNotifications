using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Utility.WPF.Demo.Trees
{
    /// <summary>
    /// Interaction logic for DropDownUserControl.xaml
    /// </summary>
    public partial class DropDownUserControl : UserControl
    {
        public DropDownUserControl()
        {
            InitializeComponent();
            var beijing = new AdministrationViewModel() { Name = "北京市", Id = Guid.NewGuid().ToString() };
            beijing.GetType().GetProperties(System.Reflection.BindingFlags.Public);
        }

        private void singleTree_Initialized(object sender, EventArgs e)
        {
            var trv = sender as TreeView;
        }

        private void singleTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var trv = sender as TreeView;
            var trvItem = trv.SelectedItem as AdministrationViewModel;
            singleHeader.Text = trvItem.Name.ToString();
            singlePopup.IsOpen = false;
        }

        private void singleHeader_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            singlePopup.Placement = System.Windows.Controls.Primitives.PlacementMode.RelativePoint;
            singlePopup.VerticalOffset = singleHeader.Height;
            singlePopup.StaysOpen = true;
            singlePopup.Height = singleTree.Height;
            singlePopup.Width = singleHeader.Width;
            singlePopup.IsOpen = true;
        }
    }

    public class AdministrationViewModel
    {
        public AdministrationViewModel()
        {
        }

        private string id;

        public string Id
        {
            get => id;
            set => id = value;
        }

        private string name;

        public string Name
        {
            get => name;
            set => name = value;
        }
    }
}
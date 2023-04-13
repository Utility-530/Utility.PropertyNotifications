using System.Collections;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace Utility.WPF.Demo.Extrinsic
{
    /// <summary>
    /// Interaction logic for FilterUserControl.xaml
    /// </summary>
    public partial class FilterUserControl : UserControl
    {
        private readonly ICollectionView view;

        public FilterUserControl()
        {
            InitializeComponent();
            var characters = this.TryFindResource("Characters") as IEnumerable;
            view = CollectionViewSource.GetDefaultView(characters);
            listBox.ItemsSource = view;
        }
    }
}
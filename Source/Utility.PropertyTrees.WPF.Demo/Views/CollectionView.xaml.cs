using Fasterflect;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Utility.PropertyTrees.WPF.Demo.Views
{
    /// <summary>
    /// Interaction logic for CollectionView.xaml
    /// </summary>
    public partial class CollectionView : UserControl
    {
        public CollectionView()
        {
            InitializeComponent();

            this.AddButton.Click += AddButton_Click;
            this.DeleteButton.Click += DeleteButton_Click;
            this.RefreshButton.Click += RefreshButton_Click;
            this.WhenAnyValue(a => a.Collection)
                .WhereNotNull()
                .Subscribe(a => ListBox.ItemsSource = a);
        }


        public IEnumerable Collection
        {
            get { return (IEnumerable)GetValue(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }

        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register("Collection", typeof(IEnumerable), typeof(CollectionView), new PropertyMetadata(null));


        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ListBox.ItemsSource = null;
            ListBox.ItemsSource = Collection;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (Collection is IList collection)
            {
                if (ListBox.SelectedItems.Count > 0)
                {
                    foreach (var item in ListBox.SelectedItems)
                    {
                        collection.Remove(item);
                    }
                    RefreshButton_Click(default, default);
                }
            }
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var type = Collection.GetType().GetGenericArguments().Single();
            var instance = Activator.CreateInstance(type);
            if (Collection is IList collection)
            {
                collection.Add(instance);
                RefreshButton_Click(default, default);
            }
        }
    }
}

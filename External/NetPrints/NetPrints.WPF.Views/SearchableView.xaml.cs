using NetPrints.WPF.Controls;
using System.Windows;
using System.Windows.Controls;

namespace NetPrints.WPF.Views
{
    /// <summary>
    /// Interaction logic for SearchableView.xaml
    /// </summary>
    public partial class SearchableView : UserControl
    {
        public SearchableView()
        {
            InitializeComponent();

            SearchableComboBox.SelectionChanged += SearchableComboBox_SelectionChanged;
        }

        private void SearchableComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(SelectionChangedEvent, e));
        }

        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(
        "SelectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchableView));

        // Provide CLR accessors for the event
        public event RoutedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }



    }
}

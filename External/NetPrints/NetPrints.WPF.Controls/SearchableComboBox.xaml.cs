using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace NetPrints.WPF.Controls
{
    /// <summary>
    /// Interaction logic for SearchableComboBox.xaml
    /// </summary>
    public partial class SearchableComboBox : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(SearchableComboBox), new PropertyMetadata());

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(SearchableComboBox), new PropertyMetadata());


        public static readonly DependencyProperty PredicateProperty =
            DependencyProperty.Register("Predicate", typeof(Predicate<object>), typeof(SearchableComboBox), new PropertyMetadata());



        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(
        "SelectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchableComboBox));

        // Provide CLR accessors for the event
        public event RoutedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }


        private ListCollectionView ListView
        {
            get => searchList.ItemsSource as ListCollectionView;
            set => searchList.ItemsSource = value;
        }

        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }


        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }


        public Predicate<object> Predicate
        {
            get { return (Predicate<object>)GetValue(PredicateProperty); }
            set { SetValue(PredicateProperty, value); }
        }

        public SearchableComboBox()
        {
            InitializeComponent();
        }


        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ItemsSourceProperty)
            {
                if (ItemsSource != null)
                {
                    ListView = new ListCollectionView(ItemsSource.Cast<object>().ToList());
                    ListView.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
                    if (ListView.CanFilter)
                    {
                        ListView.Filter = Predicate;
                    }

                    searchList.ItemsSource = ListView;
                }
            }  
            if (e.Property == PredicateProperty)
            {
                if (Predicate != null)
                {                  
                    if (ListView.CanFilter)
                    {
                        ListView.Filter = Predicate;
                    }
                    ListView?.Refresh();
                }
            }
            base.OnPropertyChanged(e);
        }

        private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5));
            SearchText = searchText.Text;
        }

        private void OnListItemSelected(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is { } data)
            {

                RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, Array.Empty<object>(), new List<object> { data }));
                //ViewModel?.OnItemSelected(data.Value);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Unselect
            searchList.SelectedItem = null;
        }
    }


}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Utility.WPF.Helpers;

namespace Utility.WPF.Attached
{
    /// <summary>
    /// https://stackoverflow.com/questions/34427456/retrieve-list-of-visible-items-in-datagrid
    /// </summary>
    public class DataGridExtensions
    {
        public static readonly DependencyProperty ObserveVisiblePersonsProperty = DependencyProperty.RegisterAttached(
            "ObserveVisiblePersons", typeof(bool), typeof(DataGrid),
            new PropertyMetadata(false, OnObserveVisiblePersonsChanged));

        public static readonly DependencyProperty VisibleItemsProperty = DependencyProperty.RegisterAttached(
            "VisibleItems", typeof(IEnumerable), typeof(DataGrid),
            new PropertyMetadata(null));

        private static readonly DependencyProperty SenderDataGridProperty = DependencyProperty.RegisterAttached(
            "SenderDataGrid", typeof(DataGrid), typeof(DataGrid), new PropertyMetadata(default(DataGrid)));

        public static void SetObserveVisiblePersons(DependencyObject element, bool value) => element.SetValue(ObserveVisiblePersonsProperty, value);

        public static bool GetObserveVisiblePersons(DependencyObject element) => (bool)element.GetValue(ObserveVisiblePersonsProperty);

        private static void SetSenderDataGrid(DependencyObject element, DataGrid value) => element.SetValue(SenderDataGridProperty, value);

        private static DataGrid GetSenderDataGrid(DependencyObject element) => (DataGrid)element.GetValue(SenderDataGridProperty);

        public static void SetVisibleItems(DependencyObject element, object[] value) => element.SetValue(VisibleItemsProperty, value);

        public static IEnumerable GetVisibleItems(DependencyObject element) => (IEnumerable)element.GetValue(VisibleItemsProperty);

        private static void OnObserveVisiblePersonsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid dataGrid)
            {
                dataGrid.Loaded += (_, _) => DataGridLoaded(dataGrid);
                dataGrid.Unloaded -= (_, _) => DataGridLoaded(dataGrid);
            }
        }

        private static void DataGridLoaded(DataGrid dataGrid)
        {
            if (dataGrid.FindChildren<ScrollViewer>().FirstOrDefault() is ScrollViewer scrollViewer)
            {
                SetSenderDataGrid(scrollViewer, dataGrid);
                scrollViewer.ScrollChanged += ScrollViewerOnScrollChanged;
            }
        }

        private static void ScrollViewerOnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
                if (scrollViewer.FindChildren<ScrollBar>().FirstOrDefault(s => s.Orientation == Orientation.Vertical) is ScrollBar verticalScrollBar)
                {
                    DataGrid dataGrid = GetSenderDataGrid(scrollViewer);

                    int totalCount = dataGrid.Items.Count;
                    int firstVisible = (int)verticalScrollBar.Value;
                    int lastVisible = (int)(firstVisible + totalCount - verticalScrollBar.Maximum);

                    SetVisibleItems(dataGrid, GetVisibibleItems(firstVisible, lastVisible, dataGrid).ToArray());
                }

            static IEnumerable<object> GetVisibibleItems(int firstVisible, int lastVisible, DataGrid dataGrid)
            {
                for (int i = firstVisible; i <= lastVisible; i++)
                {
                    yield return dataGrid.Items[i];
                }
            }
        }
    }
}
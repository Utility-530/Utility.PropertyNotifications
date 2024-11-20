using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;

namespace Utility.Trees.Demo.Connections
{
    public class SortableListView : ListView
    {
        public static readonly DependencyProperty HeaderTemplateArrowUpProperty =
    DependencyProperty.Register("HeaderTemplateArrowUp", typeof(DataTemplate), typeof(SortableListView), new PropertyMetadata());

        public static readonly DependencyProperty HeaderTemplateArrowDownProperty =
    DependencyProperty.Register("HeaderTemplateArrowDown", typeof(DataTemplate), typeof(SortableListView), new PropertyMetadata());

        public static readonly DependencyProperty HeaderTemplateDefaultProperty =
    DependencyProperty.Register("HeaderTemplateDefault", typeof(DataTemplate), typeof(SortableListView), new PropertyMetadata());


        GridViewColumnHeader _lastHeaderClicked = null;

        public SortableListView()
        {
            EventManager.RegisterClassHandler(typeof(GridViewColumnHeader), GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ListViewColumnHeaderClick));
        }


        private void ListViewColumnHeaderClick(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;

            if (headerClicked == null)
                return;

            if (headerClicked.Role == GridViewColumnHeaderRole.Padding)
                return;

            var sortingColumn = (headerClicked.Column.DisplayMemberBinding as Binding)?.Path?.Path;
            if (sortingColumn == null)
                return;

            var direction = ApplySort(Items, sortingColumn);

            if (direction == ListSortDirection.Ascending)
            {
                headerClicked.Column.HeaderTemplate = HeaderTemplateArrowUp;
            }
            else
            {
                headerClicked.Column.HeaderTemplate = HeaderTemplateArrowDown;
            }

            // Remove arrow from previously sorted header
            if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
            {
                _lastHeaderClicked.Column.HeaderTemplate = HeaderTemplateDefault;
            }

            _lastHeaderClicked = headerClicked;
        }



        public DataTemplate HeaderTemplateArrowUp
        {
            get { return (DataTemplate)GetValue(HeaderTemplateArrowUpProperty); }
            set { SetValue(HeaderTemplateArrowUpProperty, value); }
        }




        public DataTemplate HeaderTemplateArrowDown
        {
            get { return (DataTemplate)GetValue(HeaderTemplateArrowDownProperty); }
            set { SetValue(HeaderTemplateArrowDownProperty, value); }
        }

 
        public DataTemplate HeaderTemplateDefault
        {
            get { return (DataTemplate)GetValue(HeaderTemplateDefaultProperty); }
            set { SetValue(HeaderTemplateDefaultProperty, value); }
        }


        public static ListSortDirection ApplySort(ICollectionView view, string propertyName)
        {
            ListSortDirection direction = ListSortDirection.Ascending;
            if (view.SortDescriptions.Count > 0)
            {
                SortDescription currentSort = view.SortDescriptions[0];
                if (currentSort.PropertyName == propertyName)
                {
                    if (currentSort.Direction == ListSortDirection.Ascending)
                        direction = ListSortDirection.Descending;
                    else
                        direction = ListSortDirection.Ascending;
                }
                view.SortDescriptions.Clear();
            }
            if (!string.IsNullOrEmpty(propertyName))
            {
                view.SortDescriptions.Add(new SortDescription(propertyName, direction));
            }
            return direction;
        }
    }
}

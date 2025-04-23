using System.Windows.Controls;
using System.Windows;

namespace Utility.WPF.Controls.Objects
{
    public class TreeListViewItem : TreeViewItem
    {
        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(GridViewColumnCollection), typeof(TreeListViewItem));
        public static readonly DependencyProperty AllowsColumnReorderProperty = DependencyProperty.Register("AllowsColumnReorder", typeof(bool), typeof(TreeListViewItem));

        static TreeListViewItem()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListViewItem), new FrameworkPropertyMetadata(typeof(TreeListViewItem)));
        }

        public TreeListViewItem()
        {
            Columns = new GridViewColumnCollection();

        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property.Name == "ItemsSource"
                && e.OldValue != e.NewValue
                && e.NewValue != null)
            {
                GridView gridView = new GridView()
                {
                    AllowsColumnReorder = true
                };

                var columns = AutoListViewColumnBehavior.CreateColumns(this);
                foreach (var col in columns)
                    Columns.Add(col);
            }
        }


        #region Properties

        /// <summary>
        /// Gets or sets the collection of System.Windows.Controls.GridViewColumn
        /// objects that is defined for this TreeListView.
        /// </summary>
        public GridViewColumnCollection Columns
        {
            get { return (GridViewColumnCollection)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether columns in a TreeListView can be
        /// reordered by a drag-and-drop operation. This is a dependency property.
        /// </summary>
        public bool AllowsColumnReorder
        {
            get { return (bool)GetValue(AllowsColumnReorderProperty); }
            set { SetValue(AllowsColumnReorderProperty, value); }
        }

        #endregion Properties


        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListViewItem();
        }
    }
}



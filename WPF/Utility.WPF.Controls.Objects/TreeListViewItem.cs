using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utility.Models;
using Utility.Structs;

namespace Utility.WPF.Controls.Objects
{
    public class TreeListViewItem : TreeViewItem
    {
        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(GridViewColumnCollection), typeof(TreeListViewItem));
        public static readonly DependencyProperty AllowsColumnReorderProperty = DependencyProperty.Register("AllowsColumnReorder", typeof(bool), typeof(TreeListViewItem));
        public static readonly DependencyProperty SchemaProperty = DependencyProperty.Register("Schema", typeof(Schema), typeof(TreeListViewItem), new PropertyMetadata(changed));

        private static void changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Dictionary<GridViewColumn, Dimension> widths = new();
            if (d is TreeListViewItem x && e.NewValue is Schema schema)
            {
                foreach (var column in x.Columns.ToArray())
                {
                    if (x.template(schema, column) is DataTemplate template)
                        column.CellTemplate = template;

                    if (x.exclude(schema, column))
                    {
                        x.Columns.Remove(column);
                        continue;
                    }
                    if (x.width(schema, column) is Dimension dim)
                        widths.Add(column, dim);
                }

                if (widths.Any())
                {
                    var total = widths.Sum(a => a.Value.Value);
                    foreach (var c in widths)
                    {
                        c.Key.Width = c.Value.Value;
                    }
                }
            }
        }

        static TreeListViewItem()
        {
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
                //GridView gridView = new GridView()
                //{
                //    AllowsColumnReorder = true
                //};

                var columns = AutoListViewColumnHelpers.CreateColumns(this);
                foreach (var col in columns)
                {
                    if (template(Schema, col) is { } t)
                        col.CellTemplate = t;
                    if (exclude(Schema, col))
                    {
                    }
                    else
                        Columns.Add(col);
                }
            }
        }

        private bool exclude(Schema schema, GridViewColumn column)
        {
            return schema?.Properties.SingleOrDefault(a => a.Name.Equals((column.Header as ContentControl)?.Content)) is SchemaProperty { IsVisible: false };
        }

        private DataTemplate? template(Schema schema, GridViewColumn column)
        {
            var str = (column.Header as ContentControl)?.Content;
            if (schema?.Properties.SingleOrDefault(a => a.Name.Equals(str)) is SchemaProperty { Template: { } template })
                return Application.Current.Resources[template] as DataTemplate;
            return null;
        }

        private Dimension? width(Schema schema, GridViewColumn column)
        {
            var str = (column.Header as ContentControl)?.Content;
            if (schema?.Properties.SingleOrDefault(a => a.Name.Equals(str)) is SchemaProperty { ColumnWidth: { } width })
                return width;
            return null;
        }

        #region Properties

        public Schema Schema
        {
            get { return (Schema)GetValue(SchemaProperty); }
            set { SetValue(SchemaProperty, value); }
        }

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
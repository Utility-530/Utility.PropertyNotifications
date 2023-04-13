using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace Utility.WPF.Helpers
{
    public static class DataGridHelpers
    {
        public static System.Drawing.Point? GetTableIndices(System.Windows.DependencyObject dep)
        {
            while (dep != null && !(dep is DataGridCell) && !(dep is DataGridColumnHeader))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
            {
                return null;
            }
            else if (dep is DataGridColumnHeader header)
            {
                DataGridColumnHeader columnHeader = header;
                //// find the property that this cell's column is bound to
                //string boundPropertyName = DataGridHelpers.FindBoundProperty(columnHeader.Column);

                var columnIndex = columnHeader.Column.DisplayIndex;

                //ClickedItemDisplay.Text = $"Header clicked [{  columnIndex}] = { boundPropertyName}";
                return new System.Drawing.Point(0, columnIndex);
            }
            else if (dep is DataGridCell cell)
            {
                // navigate further up the tree
                while (dep != null && dep is not DataGridRow)
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }
                if (dep is DataGridRow row)
                {
                    //object value = GetValue(row, cell.Column);
                    var columnIndex = cell.Column.DisplayIndex;
                    if (row.FindRowIndex() is int rIndex)
                    {
                        return new System.Drawing.Point(rIndex, columnIndex);
                    }
                }
                //ClickedItemDisplay.Text = string.Format("Cell clicked [{0}, {1}] = {2}", rowIndex, columnIndex, value.ToString());
            }

            return null;
        }

        /// <summary>
        /// Determine the index of a DataGridRow
        /// </summary>
        /// <param name="row"></param>
        /// <returns>
        public static int? FindRowIndex(this DataGridRow row)
        {
            if (ItemsControl.ItemsControlFromItemContainer(row) is DataGrid dataGrid)
            {
                int index = dataGrid.ItemContainerGenerator.IndexFromContainer(row);

                return index;
            }
            return null;
        }

        /// <summary>
        /// Find the value that is bound to a DataGridCell
        /// </summary>
        /// <param name="row"></param>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static object? GetValue(DataGridRow row, DataGridColumn column)
        {
            // find the property that this cell's column is bound to
            if (FindBoundProperty(column) is not { } boundPropertyName)
                return null;

            // find the object that is realted to this row
            object data = row.Item;

            // extract the property value
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(data);
            PropertyDescriptor property = properties[boundPropertyName];
            object value = property.GetValue(data);

            return value;
        }

        /// <summary>
        /// Find the name of the property which is bound to the given column
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        public static string? FindBoundProperty(DataGridColumn col)
        {
            if (col is not DataGridBoundColumn { Binding: Binding { Path: { Path: { } boundPropertyName } } })
                return null;
            return boundPropertyName;
        }

        public static T? GetVisualChild<T>(Visual parent) where T : Visual
        {
            T? child = default;
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        public static DataGridRow GetSelectedRow(this DataGrid grid)
        {
            return (DataGridRow)grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem);
        }

        public static DataGridRow GetRow(this DataGrid grid, int index)
        {
            DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // May be virtualized, bring into view and try again.
                grid.UpdateLayout();
                grid.ScrollIntoView(grid.Items[index]);
                row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        public static DataGridCell? GetCell(this DataGrid grid, DataGridRow row, int column)
        {
            if (row != null)
            {
                DataGridCellsPresenter? presenter = GetVisualChild<DataGridCellsPresenter>(row);

                if (presenter == null)
                {
                    grid.ScrollIntoView(row, grid.Columns[column]);
                    presenter = GetVisualChild<DataGridCellsPresenter>(row);
                }

                DataGridCell? cell = (DataGridCell?)presenter?.ItemContainerGenerator.ContainerFromIndex(column);
                return cell;
            }
            return null;
        }

        public static DataGridCell? GetCell(this DataGrid grid, int row, int column)
        {
            DataGridRow rowContainer = grid.GetRow(row);
            return grid.GetCell(rowContainer, column);
        }
    }
}
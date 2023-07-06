/*
 * Author:  Leif Goodwin
 * Date:    2017-01-17
  <a href="https://www.codeproject.com/Articles/1166016/WPF-DataGrid-with-SharedSizeGroup-Columns-Property"/>
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Media;
using System.Linq;
using Utility.WPF.Helpers;
using UnitsNet;

namespace Utility.WPF.Controls.DataGrids
{
    public static class DataGridExtensionMethods
    {
        //private static childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        //{
        //    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        //    {
        //        DependencyObject child = VisualTreeHelper.GetChild(obj, i);
        //        if (child != null && child is childItem)
        //        {
        //            return (childItem)child;
        //        }
        //        else
        //        {
        //            childItem childOfChild = FindVisualChild<childItem>(child);
        //            if (childOfChild != null)
        //            {
        //                return childOfChild;
        //            }
        //        }
        //    }
        //    return null;
        //}

        static Dictionary<DataGrid, double>? difference = new();

        /*
         * Ensure that columns are the right width and no wider
         */
        public static void ShrinkColumnsToFit(this DataGrid dataGrid)
        {
            for (int column = 0; column < dataGrid.Columns.Count; ++column)
            {
                dataGrid.Columns[column].Width = new DataGridLength(1.0);
            }
            dataGrid.UpdateLayout();

            for (int column = 0; column < dataGrid.Columns.Count; ++column)
            {
                dataGrid.Columns[column].Width = new DataGridLength(1.0, DataGridLengthUnitType.Auto);
            }
            dataGrid.UpdateLayout();
        }



        public static void AdjustColumnWidths(this DataGrid dataGrid, FrameworkElement parent, double margin)
        {
            List<double> columnWidths = new List<double>();

            /*
             * Key:   The SharedSizeGroup value
             * Value: The indices of columns with the key value
             */
            Dictionary<string, List<int>> sharedKeyMap = new Dictionary<string, List<int>>();

            for (int column = 0; column < dataGrid.Columns.Count; ++column)
            {
                string key = DataGridLayoutBehavior.GetSharedSizeGroup(dataGrid.Columns[column]);
                if (!string.IsNullOrEmpty(key))
                {
                    if (!sharedKeyMap.ContainsKey(key))
                    {
                        sharedKeyMap[key] = new List<int>();
                    }
                    sharedKeyMap[key].Add(column);
                }

                columnWidths.Add(dataGrid.Columns[column].ActualWidth);
            }

            if (sharedKeyMap.Count > 0)
            {
                /*
                 * Set the width of a given column to the width of the widest cell
                 */
                foreach (var item in dataGrid.Items)
                {
                    DataGridRow dataGridRow = dataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;

                    if (dataGridRow != null)
                    {
                        var presenter = dataGridRow.FindChild<DataGridCellsPresenter>();
                        if (presenter != null)
                        {
                            for (int column = 0; column < dataGrid.Columns.Count; ++column)
                            {
                                DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                                if (cell != null)
                                {
                                    if (columnWidths[column] < cell.Column.ActualWidth)
                                    {
                                        columnWidths[column] = cell.Column.ActualWidth;
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (var mapItem in sharedKeyMap)
                {
                    double sharedWidth = 0;
                    if (mapItem.Value.Count > 1)
                    {
                        /*
                         * All columns in the shared group have the width of the original widest column
                         */
                        foreach (var column in mapItem.Value)
                        {
                            sharedWidth = Math.Max(sharedWidth, columnWidths[column]);
                        }

                        foreach (var column in mapItem.Value)
                        {
                            dataGrid.Columns[column].Width = sharedWidth;
                        }
                    }
                }
            }
            else
            {

                var windowActualWidth = parent.FindParent<Window>().ActualWidth;

                var x = parent.DesiredSize.Width;
                if (parent.ActualWidth == 0)
                    return;


                if (difference.TryGetValue(dataGrid, out var diff) == false && windowActualWidth > parent.ActualWidth )
                {
                    difference[dataGrid] = diff = windowActualWidth - parent.ActualWidth;
                }

     
                    var totalWidth =  Math.Min(windowActualWidth - diff, parent.ActualWidth) - (dataGrid.Margin.Left + dataGrid.Margin.Right) - margin;

                    var sharedWidth = (totalWidth / dataGrid.Columns.Count);

                    for (int column = 0; column < dataGrid.Columns.Count; ++column)
                    {
                        dataGrid.Columns[column].Width = sharedWidth;
                    }
                
            }
        }
    }
}

/*
 * Author:  Leif Goodwin
 * Date:    2017-01-17
 * Purpose: Defines an attached behaviour which adds additional formatting 
 *          options to the DataGrid control. 
 * 
 *              1) Configure shared size group columns (as per the WPF Grid 
 *                 sharedsizegroup property).
 *              2) Shrink the columns to fit their content.
 *              3) Enable/disable the formatting. 
 *              <a href="https://www.codeproject.com/Articles/1166016/WPF-DataGrid-with-SharedSizeGroup-Columns-Property"/>             
 */

using System.Windows;


namespace Utility.WPF.Controls.DataGrids
{
    public static class DataGridLayoutBehavior
    {


        /*
         * Attached property: SharedSizeGroup
         */

        public static readonly DependencyProperty SharedSizeGroupProperty = DependencyProperty.RegisterAttached
        (
            "SharedSizeGroup",
            typeof(string),
            typeof(DataGridLayoutBehavior),
            new UIPropertyMetadata("", null)
        );

        public static string GetSharedSizeGroup(DependencyObject obj)
        {
            return (string)obj.GetValue(SharedSizeGroupProperty);
        }

        public static void SetSharedSizeGroup(DependencyObject obj, string value)
        {
            obj.SetValue(SharedSizeGroupProperty, value);
        }
    }
}

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

using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace Utility.WPF.Controls.DataGrids
{
    public class ShrinkColumnsToFitBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            DataGridExtensionMethods.ShrinkColumnsToFit(AssociatedObject);
            base.OnAttached();
        }
    }
}
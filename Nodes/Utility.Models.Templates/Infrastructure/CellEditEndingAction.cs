using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Bogus;
using DryIoc;
using Microsoft.Xaml.Behaviors;
using Utility.PropertyNotifications;

namespace Utility.Models.Templates
{
    public class CellEditEndingAction : TriggerAction<DataGrid>
    {
        protected override void Invoke(object parameter)
        {
            var e = parameter as DataGridCellEditEndingEventArgs;

            //var dataGrid = AssociatedObject as DataGrid;

            //if (args != null && args.EditAction == DataGridEditAction.Commit)
            //{
            //    var propertyName = (args.Column as DataGridTextColumn)?.Binding.GetPath();

            //    var expr = (args.Column as DataGridTextColumn).GetBindingExpression(binding.Path);
            //    (args.Column as DataGridTextColumn)?.Binding?.UpdateSource();

            //    // Get the object backing the row
            //    if (args.Row.DataContext is INotifyPropertyChanged changed)
            //        changed.RaisePropertyChanged(propertyName);
            //}

            if (e.Column is DataGridBoundColumn col)
            {
                var binding = col.Binding as Binding;
                var element = e.EditingElement as FrameworkElement;

                if (binding != null && element != null)
                {
                    DependencyProperty dp = null;

                    // Detect editor type
                    if (element is TextBox)
                        dp = TextBox.TextProperty;
                    else if (element is CheckBox)
                        dp = CheckBox.IsCheckedProperty;
                    var expr = element.GetBindingExpression(dp);
                    expr?.UpdateSource();
                }
            }

            // Now your VM value is updated
            var row = e.Row.Item;
            var propertyName = (e.Column as DataGridBoundColumn)?.Binding.GetPath();
            if (e.Row.DataContext is INotifyPropertyChanged changed)
                changed.RaisePropertyChanged(propertyName);
        }
    }

    public static class BindingExtensions
    {
        public static string GetPath(this BindingBase binding)
        {
            if (binding is Binding b)
            {
                if (!string.IsNullOrEmpty(b.Path.Path))
                {
                    return b.Path.Path;
                }
            }
            return null;
        }
    }
}

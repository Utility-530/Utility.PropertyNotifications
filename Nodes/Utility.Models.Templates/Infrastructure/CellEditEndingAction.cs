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
using Splat;
using Utility.Entities;
using Utility.Interfaces.Generic;
using Utility.Interfaces.Generic.Data;
using Utility.PropertyNotifications;

namespace Utility.Models.Templates
{
    public class CellEditEndingAction : TriggerAction<DataGrid>
    {
        protected override void Invoke(object parameter)
        {
            var e = parameter as DataGridCellEditEndingEventArgs;

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
    public class AddNewItemAction : TriggerAction<DataGrid>
    {

        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register(nameof(Type), typeof(Type), typeof(AddNewItemAction), new PropertyMetadata());

        protected override void Invoke(object parameter)
        {
            if(parameter is AddingNewItemEventArgs e)
            {
                e.NewItem = Locator.Current.GetService<IFactory<IId<Guid>>>().Create(Type);
            }
        }

        public Type Type
        {
            get { return (Type)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
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

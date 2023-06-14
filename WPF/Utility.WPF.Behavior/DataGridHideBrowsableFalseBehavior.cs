using Microsoft.Xaml.Behaviors;
using System.ComponentModel;
using System.Windows.Controls;

namespace Utility.WPF.Behavior
{
    public class DataGridHideBrowsableFalseBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            AssociatedObject.AutoGeneratingColumn += AssociatedObject_AutoGeneratingColumn;
            base.OnAttached();
        }

        private void AssociatedObject_AutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (((PropertyDescriptor)e.PropertyDescriptor).IsBrowsable == false)
                e.Cancel = true;
        }
    }
}
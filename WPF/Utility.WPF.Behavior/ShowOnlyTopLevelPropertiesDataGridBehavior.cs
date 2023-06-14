using Microsoft.Xaml.Behaviors;
using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace Utility.WPF.Behavior
{
    public class ShowOnlyTopLevelPropertiesDataGridBehavior : Behavior<DataGrid>
    {
        private Type? componentType;

        protected override void OnAttached()
        {
            AssociatedObject.AutoGeneratingColumn += AssociatedObject_AutoGeneratingColumn;
            base.OnAttached();
        }

        private void AssociatedObject_AutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            // Assumes the first ComponentType is the highest derived type;
            // then ignores all properties from all types below this type's level.
            if (e.PropertyDescriptor is PropertyDescriptor propertyDescriptor)
            {
                if (componentType == null)
                    componentType = propertyDescriptor.ComponentType;
                e.Cancel = propertyDescriptor.ComponentType != componentType;
            }
        }
    }
}
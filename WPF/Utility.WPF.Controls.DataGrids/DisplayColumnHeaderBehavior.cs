using Microsoft.Xaml.Behaviors;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls;

namespace Utility.WPF.Controls.DataGrids
{
    /// <summary>
    /// <a href="https://www.codeproject.com/Articles/389764/A-Smart-Behavior-for-DataGrid-AutoGenerateColumn"></a>
    /// </summary>
    public class DisplayColumnHeaderBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            AssociatedObject.AutoGeneratingColumn +=
                new EventHandler<DataGridAutoGeneratingColumnEventArgs>(OnAutoGeneratingColumn);
        }

        protected override void OnDetaching()
        {
            AssociatedObject.AutoGeneratingColumn -=
                new EventHandler<DataGridAutoGeneratingColumnEventArgs>(OnAutoGeneratingColumn);
        }

        protected void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string? displayName = GetPropertyDisplayName(e.PropertyDescriptor);
            if (!string.IsNullOrEmpty(displayName))
            {
                e.Column.Header = displayName;
            }
            else
            {
                e.Cancel = true;
            }
        }

        protected static string? GetPropertyDisplayName(object descriptor)
        {
            if (descriptor is PropertyDescriptor pd)
            {
                if (pd.Attributes[typeof(DisplayNameAttribute)] is DisplayNameAttribute attr && attr != DisplayNameAttribute.Default)
                {
                    return attr.DisplayName;
                }
            }
            else if (descriptor is PropertyInfo pi)
            {
                foreach (var att in pi.GetCustomAttributes(typeof(DisplayNameAttribute), true))
                {
                    if (att is DisplayNameAttribute attribute && attribute != DisplayNameAttribute.Default)
                    {
                        return attribute.DisplayName;
                    }
                }

            }
            return null;
        }
    }
}
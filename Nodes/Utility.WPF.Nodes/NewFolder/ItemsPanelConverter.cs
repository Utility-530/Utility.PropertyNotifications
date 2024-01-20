using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using Utility.Enums;
using Utility.WPF.Factorys;
using Utility.WPF.Nodes.NewFolder;
using Utility.Trees.Abstractions;
using Utility.Nodes;
using Utility.PropertyDescriptors;
using Utility.Objects;

namespace VisualJsonEditor.Test.Infrastructure
{
    public class ItemsPanel
    {
        public Arrangement Type { get; set; }

        public System.Windows.Controls.Orientation? Orientation { get; set; }

        public int? Rows { get; set; }

        public int? Columns { get; set; }

        public string? TemplateKey { get; set; }
    }


    public class ItemsPanelConverter : IValueConverter
    {
        //SQLIteRepos

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value is IReadOnlyTree{ Data: ViewModel { } viewmodel })
            //{
            //    var itemsPanel = viewmodel.ToItemsPanel();
            //    return convert(itemsPanel);
            //}



            return DependencyProperty.UnsetValue;


        }

        protected static ItemsPanelTemplate convert(ItemsPanel itemsPanel)
        {
            if (itemsPanel == null)
                return null;
            else if (itemsPanel.TemplateKey is { } t)
            {
                return Application.Current.TryFindResource(t) as ItemsPanelTemplate;
            }
            return ItemsPanelFactory.Template(itemsPanel.Rows, itemsPanel.Columns, itemsPanel.Orientation, itemsPanel.Type);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static ItemsPanelConverter Instance { get; } = new();
    }

}

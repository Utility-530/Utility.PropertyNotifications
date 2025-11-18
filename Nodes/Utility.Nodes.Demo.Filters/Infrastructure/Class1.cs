using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using Newtonsoft.Json;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Nodes.Demo.Filters.Infrastructure;
using Utility.Trees;
using Utility.WPF.Behaviors;
using Utility.WPF.Helpers;

namespace Utility.Nodes.Demo.Filters
{

    public class Activator
    {
        public static Model<string> Instance=> new Model<string> { Name = "first" };

    }
    public class CustomStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is IRoot data)
            {
                return RootStyle;
            }
            return DefaultStyle;
        }

        public Style DefaultStyle { get; set; }
        public Style RootStyle { get; set; }
    }


    public class TreeConverter : IValueConverter
    {
        string _value = null;
        NodeViewModel node = null;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                if (s != _value)
                {
                    _value = s;
                    node = JsonConvert.DeserializeObject<NodeViewModel>(s);
                    TempNodeEngine.Instance.Add(node);
                }
                return new[] { node };
            }
            else if (value is null)
            {
                //var coll = new ObservableCollection<INode>();
                var model = new AndOrModel() { Name = "and_or" };
                var andOr = new NodeViewModel(model);

                TempNodeEngine.Instance.Add(andOr);
                //coll.Add(andOr as INode);
                return new[] { andOr };
            }
            throw new Exception("44656767 44");
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<INodeViewModel> nodes)
            {
                return JsonConvert.SerializeObject(nodes.First());
            }

            else
                throw new Exception("sadS 434");
        }
    }

    public class AddObjectAction : TriggerAction<FrameworkElement>
    {

        protected override void Invoke(object parameter)
        {
            if (parameter is Utility.WPF.EditRoutedEventArgs { IsAccepted: true, Edit: { } instance } value)
            {
                value.Handled = true;
                var x = AssociatedObject;
                ;
                if (x.DataContext is Tree tree)
                {
                    tree.Add(instance);
                }
            }
        }
       
    }


    public class CancelSelectionItemChange : TriggerAction<FrameworkElement>
    {

        protected override void Invoke(object parameter)
        {
            if (parameter is RoutedPropertyChangedEventArgs<object> eventArgs)
            {
                eventArgs.Handled = true;
            }
        }
    }

    public class CustomUpdateSourceBindingAction : UpdateSourceBindingAction
    {

        protected override void Invoke(object parameter)
        {

            if (parameter is MouseEventArgs { OriginalSource: UIElement originalSource, Source: TreeView source } mouseEventArgs)
            {

                // establish that the mouse click has taken place in the top level treeview i.e source 
                var x = HitTestHelper.GetSelectedItem<TreeViewItem>(originalSource, source);
                //if (x is TreeViewItem { Header: INode { Data: AndOrModel } })
                //    return;
                if (x is TreeViewItem { Header: IRoot })
                    return;
                // and not in the bottom-level treeview
                var y = HitTestHelper.GetSelectedItem<TreeViewItem>(originalSource, AssociatedObject);
                // because the former indicates a selecteditemchanged event is about to occur 
                if (x != null && y == null)
                    base.Invoke(parameter);
            }
        }

    }


    public class FooterDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return Template;
        }

        public DataTemplate Template { get; set; }
    }

}

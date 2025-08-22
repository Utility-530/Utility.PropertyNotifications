using System.Windows;
using System.Windows.Controls;
using Utility.Interfaces.Generic;
using Utility.PropertyDescriptors;
using Utility.Trees.Abstractions;
using Utility.Trees.Decisions;

namespace Utility.Nodes.WPF
{
    public class StyleSelector : System.Windows.Controls.StyleSelector
    {
        private bool isInitialised = false;
        private ResourceDictionary? res;

        public override Style SelectStyle(object item, DependencyObject container)
        {
            //return base.SelectStyle(item, container);//
            if (container.GetType() == typeof(TreeViewItem))
                return base.SelectStyle(item, container);

            object input = null;
            if (item is TreeViewItem _item)
            {
                input = _item.Header;
            }
            else
            {
                input = item;
            }
            if (isInitialised == false)
            {
                Uri ??= new Uri(@$"{typeof(StyleSelector).Assembly.GetName().Name};component/Themes/Styles.xaml", UriKind.RelativeOrAbsolute);
                res = Application.LoadComponent(Uri) as ResourceDictionary;
                Application.Current.Resources.Add(Uri, res);
                isInitialised = true;
            }
            if (SelectKey(item) is string s)
            {
                var style = Application.Current.Resources[Predicate.Backput] as Style;
                if (style == null)
                    style = res[Predicate.Backput] as Style;


                return style;
            }

            return base.SelectStyle(item, container);
        }

        public string SelectKey(object input)
        {
            Predicate.Reset();
            Predicate.Input = input;
            Predicate.Evaluate();
            return Predicate.Backput?.ToString();
        }

        public Uri Uri { get; set; }

        public string DefaultStyle { get; set; } = "ExpandedTreeViewItem";

        public static StyleSelector Instance { get; } = new();

        public DecisionTree Predicate { get; set; }

        public StyleSelector()
        {
            Predicate = new StringDecisionTree(new Decision(item => (item as IReadOnlyTree) != null) { })
                {
                    new StringDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as IHeaderDescriptor != null), md =>"HeaderItem"),
                    new StringDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as IMethodDescriptor != null){  }, md=>"ExpandedTreeViewItem"),
                    ///new StringDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as ICollectionDescriptor != null && (item.Data as ICollectionDescriptor).ElementType == typeof(Table)){  }, md=>"ComboStyle"),

                    new StringDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as ICollectionDescriptor != null){  }, md=>"LineStyle"),
                    new StringDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as ICollectionHeadersDescriptor != null){  }, md=>"ItemsStyle"),
                    new StringDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as IPropertiesDescriptor != null){  }, md=>"ItemsStyle"),
                    new StringDecisionTree(new Decision<IGetParent<IReadOnlyTree>>(item => item.Parent !=null && item.Parent.Data is ICollectionDescriptor){  }, md =>"ThinLineStyle"),

                    //new StringDecisionTree(new Decision<IReadOnlyTree>(item => item.Parent!=null){  })
                    //{
                    //    new StringDecisionTree(new Decision<IReadOnlyTree>(item => (item.Parent .Data as ICollectionItemDescriptor != null)){  })
                    //    {
                    //        new StringDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as IReferenceDescriptor != null){  }, md =>"CollapsedTableItem"),
                    //        new StringDecisionTree(new Decision<IReadOnlyTree>(item =>  item.Data as IReferenceDescriptor == null){  }, md =>"ExpandedTableItem")
                    //    },
                    //    //new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => ((item.Parent ).Parent!=null)){  })
                    //    //{
                    //    //    new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => (((item.Parent ).Parent).Data as ICollectionItemDescriptor!=null)){  }, md=> MakeTemplate(md)),
                    //    //},
                    //},
                    //new StringDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as ICollectionItemDescriptor != null){  }, md =>"ButtonsAddRemoveStyle"),
                    ////new StringDecisionTree<IReadOnlyTree>(new Decision<IReadOnlyTree>(item => (item.Data as PropertyDescriptor) != null), md => "ExpandedTreeViewItem"),
                    //new StringDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as IValueDescriptor != null){  }, md =>"ExpandedTreeViewItem"),
                    //new StringDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as IPropertyDescriptor != null){  }, md =>"ItemsStyle"),
                    //new StringDecisionTree(new Decision<IReadOnlyTree>(item => true), md => DefaultStyle),
                };
        }
    }
}

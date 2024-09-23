using System.Windows;
using System.Windows.Controls;
using Utility.Descriptors;
using Utility.Interfaces;
using Utility.Trees.Abstractions;
using Utility.Trees.Demo.MVVM.Infrastructure;

namespace Utility.Trees.Demo.MVVM
{

    public class StyleSelector : System.Windows.Controls.StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is TreeViewItem _item)
            {
                Predicate.Reset();
                Predicate.Input = _item.Header;
                Predicate.Evaluate();
                if (Predicate.Backput is string s)
                {
                    var style = App.Current.Resources[Predicate.Backput] as Style;
                    if (s is "CollapsedTreeViewItem")
                    {

                    }
                    return style;
                }
            }
            return null;
        }
        public static StyleSelector Instance { get; } = new();

        public DecisionTree Predicate { get; set; }

        private StyleSelector()
        {
            Predicate = new StringDecisionTree(new Decision(item => (item as IReadOnlyTree) != null) { })
                {
                    new StringDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as IHeaderDescriptor != null), md =>"HeaderItem"),
                    new StringDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as IMethodDescriptor != null){  }, md=>"ExpandedTreeViewItem"),
                    new StringDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as ICollectionDescriptor != null && (item.Data as ICollectionDescriptor).ElementType == typeof(Table)){  }, md=>"ComboStyle"),

                    new StringDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as ICollectionDescriptor != null){  }, md=>"LineStyle"),
                    new StringDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as ICollectionHeadersDescriptor != null){  }, md=>"ItemsStyle"),
                    new StringDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as IPropertiesDescriptor != null){  }, md=>"ItemsStyle"),
                    new StringDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as ICollectionItemReferenceDescriptor != null){  }, md =>"ThinLineStyle"),

                    new StringDecisionTree(new Decision<IReadOnlyTree>(item => item.Parent!=null){  })
                    {
                        new StringDecisionTree(new Decision<IReadOnlyTree>(item => (item.Parent .Data as ICollectionItemDescriptor != null)){  })
                        {
                             new StringDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as IReferenceDescriptor != null){  }, md =>"CollapsedTableItem"),
                             new StringDecisionTree(new Decision<IReadOnlyTree>(item =>  item.Data as IReferenceDescriptor == null){  }, md =>"ExpandedTableItem")
                        },
                        //new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => ((item.Parent ).Parent!=null)){  })
                        //{
                        //    new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => (((item.Parent ).Parent).Data as ICollectionItemDescriptor!=null)){  }, md=> MakeTemplate(md)),
                        //},
                    },
                    new StringDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as ICollectionItemDescriptor != null){  }, md =>"ButtonsAddRemoveStyle"),
                    //new StringDecisionTree<IReadOnlyTree>(new Decision<IReadOnlyTree>(item => (item.Data as PropertyDescriptor) != null), md => "ExpandedTreeViewItem"),
                    new StringDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as IValueDescriptor != null){  }, md =>"ExpandedTreeViewItem"),
                     new StringDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as IPropertyDescriptor != null){  }, md =>"ItemsStyle"),
                    new StringDecisionTree(new Decision<IReadOnlyTree>(item => true), md => "ExpandedTreeViewItem"),
                };
        }


    }



}

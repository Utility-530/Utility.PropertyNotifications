using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Utility.Trees.Demo
{


    class StyleSelectorConstants
    {
        public const string FileName = @"C:\Users\rytal\source\repos\UtilityStandard\Utility.Trees\Utility.Trees.Resources\Styles.xaml";
    }

    public class MyStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {

            try
            {
                var resourceDictionary = new ResourceDictionary
                {
                    Source = new Uri(StyleSelectorConstants.FileName, UriKind.Absolute)
                };

                //var key = "TreeItemTemplate";
                var key = "TreeViewItemStyle";
                //var key = new DataTemplateKey(typeof(Tree));
                var style = resourceDictionary[key] as Style;

                return style;
            }
            catch(Exception ex)
            {
                return base.SelectStyle(item, container);
            }
   
        }
        public static MyStyleSelector Instance { get; } = new MyStyleSelector();
    }
}

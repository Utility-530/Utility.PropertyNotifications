using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Utility.WPF.Controls.Base;

namespace Utility.WPF.Controls
{
    public class BreadCrumbs : CustomItemsControl
    {

        static BreadCrumbs()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadCrumbs), new FrameworkPropertyMetadata(typeof(BreadCrumbs)));
        }



    }
}

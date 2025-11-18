using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Utility.Nodes;
using Utility.WPF.Controls.Trees.Infrastructure;
using Utility.WPF.Helpers;
using static Utility.WPF.Controls.Trees.Infrastructure.TreeTabHelper;

namespace Utility.WPF.Demo.Trees
{
    /// <summary>
    /// Interaction logic for TabTreeUserControl.xaml
    /// </summary>
    public partial class TabTreeUserControl : UserControl
    {
        public TabTreeUserControl()
        {
            InitializeComponent();
        }
    }
    public class TabsViewModel
    {
        public IEnumerable Collection => new NodeViewModel[]{new ()
        {
           new NodeViewModel(){ Name = "A" }, new NodeViewModel(){ Name = "A" }, new NodeViewModel(){ Name = "A" }, new NodeViewModel(){ Name = "A" }
        } };

        public static AddItemActionCallback AddItemCallback { get; } = (a) =>
        {
            //a.NewItem = "new";
        };
    }
}





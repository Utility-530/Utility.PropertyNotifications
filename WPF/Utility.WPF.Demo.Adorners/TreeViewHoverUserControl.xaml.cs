using Microsoft.Xaml.Behaviors;
using NetFabric.Hyperlinq;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.WPF.Adorners.Behaviors;
using Utility.WPF.Behaviors;
using Utility.WPF.Controls;
using Utility.WPF.Reactives;

namespace Utility.WPF.Demo.Adorners
{
    /// <summary>
    /// Interaction logic for TreeViewHover.xaml
    /// </summary>
    public partial class TreeViewHoverUserControl : UserControl
    {

        public TreeViewHoverUserControl()
        {
            InitializeComponent();

            treeView
                .MouseMoves()
                .Where(a => a.item != null)
                .Subscribe(treeViewItem =>
                {
                    var behaviors = Interaction.GetBehaviors(treeViewItem.item);
                    if (behaviors.OfType<DropDownBehavior>().Any() == false)
                    {
                        var dropDown = new DropDownBehavior()
                        {
                            IsShown = true,
                            ItemsSource = new[] { "A", "B", "C" }
                        };
                        behaviors.Add(dropDown);
  
                    }
                    else
                    {
                        var single = behaviors.OfType<DropDownBehavior>().Single();
                        single.IsShown = true;       
                    }
                });
        }

       
    }
}
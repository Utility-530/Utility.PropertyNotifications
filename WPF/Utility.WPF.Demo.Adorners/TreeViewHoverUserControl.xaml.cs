using Microsoft.Xaml.Behaviors;
using NetFabric.Hyperlinq;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using Utility.WPF.Adorners.Behaviors;
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
                    var x = Interaction.GetBehaviors(treeViewItem.item);
                    if (x.OfType<DropDownBehavior>().Any() == false)
                    {
                        x.Add(new DropDownBehavior()
                        {
                            IsShown = true,
                            ItemsSource = new[] { "A", "B", "C" }
                        });
                    }
                    else
                    {
                        var single = x.OfType<DropDownBehavior>().Single();
                        single.IsShown = true;
                    }
                });
        }

       
    }
}
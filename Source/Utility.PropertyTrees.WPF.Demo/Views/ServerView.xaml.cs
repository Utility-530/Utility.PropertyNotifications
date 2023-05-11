using DryIoc;
using System;
using System.Windows.Controls;
using Utility.Observables;
using static Utility.PropertyTrees.WPF.Demo.LightBootStrapper;

namespace Utility.PropertyTrees.WPF.Demo.Views
{
    /// <summary>
    /// Interaction logic for ServerView.xaml
    /// </summary>
    public partial class ServerView : UserControl
    {
        private MainViewModel viewModel => container.Resolve<MainViewModel>();

        public ServerView()
        {
            InitializeComponent();


            viewModel.Subscribe(a =>
            {
                if (a is ViewModelEvent { Name: var name, TreeView: var treeView } clientResponseEvent)
                {
                    var group = new Expander { Header = name, FontSize = 12, Content = treeView, IsExpanded = false };
                    ResponsePanel.Children.Add(group);
                }
            });
        }
    }
}

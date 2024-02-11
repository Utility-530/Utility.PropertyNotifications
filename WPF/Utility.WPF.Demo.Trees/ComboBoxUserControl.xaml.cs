using ReactiveUI;
using System;
using System.Reflection;
using System.Windows.Controls;
using Utility.Trees;

namespace Utility.WPF.Demo.Trees
{
    /// <summary>
    /// Interaction logic for ComboBoxUserControl.xaml
    /// </summary>
    public partial class ComboBoxUserControl : UserControl
    {
        public ComboBoxUserControl()
        {
            InitializeComponent();
            TypeSelector.Assemblies = new Assembly[] { Assembly.GetEntryAssembly(), typeof(Tree).Assembly };
            TypeSelector.WhenAnyValue(a => a.SelectedItems)
                .Subscribe(a =>
                {

                });

            TypeSelector.Type = typeof(Tree);
        }
    }
}

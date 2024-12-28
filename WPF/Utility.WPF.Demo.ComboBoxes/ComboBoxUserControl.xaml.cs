using ReactiveUI;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Utility.Meta;
using Utility.Trees;
using Utility.WPF.Controls.ComboBoxes;

namespace Utility.WPF.Demo.ComboBoxes
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
            Type2Selector.Assemblies = new Assembly[] { typeof(Tree).Assembly, new SystemAssembly() };
            TypeSelector.Type = typeof(Tree);
            Type2Selector.Type = typeof(string);
            DataTemplate2Selector.FullKey = "{\"Assembly\":\"Utility.WPF.Demo.Data\",\"ResourceDictionary\":\"datatemplate/character.baml\",\"Element\":\"CharacterBaseClass\"}";
            TypeSelector.WhenAnyValue(a => a.SelectedItems)
                .Subscribe(a =>
                {

                });

            ItemsPanelTemplate2Selector.Key = "WrapPanel";
        }

        public static Assembly[] Assemblies => new[] { typeof(Utility.WPF.Demo.Data.Model.Character).Assembly };
    }





}

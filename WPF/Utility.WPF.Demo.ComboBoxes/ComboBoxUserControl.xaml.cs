using System.IO;
using System.Reflection;
using System.Windows.Controls;
using Splat;
using Utility.Interfaces.Exs;
using Utility.Meta;
using Utility.Trees;

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
            TypeSelector.Type = typeof(Tree);

            Type2Selector.Assemblies = new Assembly[] { typeof(Tree).Assembly, new SystemAssembly() };
            Type2Selector.Type = typeof(string);
            FileSelector.FileSystemInfo = new DirectoryInfo("O:\\Users\\ry33tal\\");
            DataTemplate2Selector.FullKey = "{\"Assembly\":\"Utility.WPF.Demo.Data\",\"ResourceDictionary\":\"datatemplate/character.baml\",\"Element\":\"CharacterBaseClass\"}";

            ItemsPanelTemplate2Selector.Key = "WrapPanel";
            PropertySelector.Type = typeof(Tree);
            ParameterSelector.NodeRoot = Locator.Current.GetService<INodeRoot>();
            ParameterSelector.Assemblies = new Assembly[] { new SystemAssembly() };
            AssemblySelector.NodeRoot = Locator.Current.GetService<INodeRoot>();

        }

        public static Assembly[] Assemblies => new[] { typeof(Utility.WPF.Demo.Data.Model.Character).Assembly };
    }
}
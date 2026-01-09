using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DryIoc;
using Splat;
using Utility.Interfaces.Exs;

namespace Utility.WPF.Demo.ComboBoxes
{
    /// <summary>
    /// Interaction logic for ReflectionUserControl.xaml
    /// </summary>
    public partial class ReflectionUserControl : UserControl
    {
        public ReflectionUserControl()
        {
            InitializeComponent();

            AssemblySelector.NodeRoot = Locator.Current.GetService<INodeRoot>();
        }
    }
}

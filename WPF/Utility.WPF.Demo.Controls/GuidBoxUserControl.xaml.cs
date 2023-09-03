using System;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.WPF.Controls;

namespace Utility.WPF.Demo.Controls
{
    /// <summary>
    /// Interaction logic for GuidBoxUserControl.xaml
    /// </summary>
    public partial class GuidBoxUserControl : UserControl
    {
        public GuidBoxUserControl()
        {
            InitializeComponent();
            One.Guid = Guid.NewGuid();
            var binding = new Binding { Source = Guid.NewGuid() };
            Two.SetBinding(GuidBox.GuidProperty, binding);
        }
    }
}

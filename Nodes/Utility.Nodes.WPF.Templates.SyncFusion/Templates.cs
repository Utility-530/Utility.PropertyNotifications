using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Utility.Nodes.WPF.Templates.SyncFusion
{
    public partial class Templates : ResourceDictionary
    {
        public Templates()
        {
            InitializeComponent();
        }

        public static Templates Instance { get; } = new();    
    }
}

using System.Windows;

namespace Utility.WPF.Templates
{
    public partial class ValueTemplates : ResourceDictionary
    {
        public ValueTemplates()
        {
            //InitializeComponent();
        }

        public static ValueTemplates Instance { get; } = new();
    }
}
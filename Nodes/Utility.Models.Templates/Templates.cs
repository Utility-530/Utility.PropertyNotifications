namespace Utility.Models.Templates
{
    using System.Windows;

    public partial class Templates : ResourceDictionary
    {
        public Templates()
        {
            InitializeComponent();
        }

        public static Templates Instance { get; } = new();
    }
}

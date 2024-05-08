using System.Windows;

namespace Utility.WPF.Templates
{
    public partial class Templates : ResourceDictionary
    {
        private Templates()
        {
        }

        public static Templates Instance { get; } = new(); 
    }
}
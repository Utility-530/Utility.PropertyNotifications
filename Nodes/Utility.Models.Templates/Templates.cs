namespace Utility.Models.Templates
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Utility.ServiceLocation;

    public partial class Templates : ResourceDictionary
    {
        public Templates()
        {
            this["DataTemplateSelector"] ??= Globals.Resolver.TryResolve<DataTemplateSelector>();
            this["StyleSelector"] ??= Globals.Resolver.TryResolve<StyleSelector>();     
            InitializeComponent();   
        }

        public static Templates Instance { get; } = new();
    }
}

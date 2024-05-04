using Utility.ViewModels;

namespace Utility.WPF.Nodes.NewFolder
{
    public class DataPresentation
    {
        public DataPresentationType Type { get; internal set; }
        public string? TemplateKey { get; internal set; }
        public int Style { get; internal set; }
    }
}
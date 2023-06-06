using Utility.Enums;

namespace Utility.Models
{
    public interface  IViewModel
    {
        string Arrangement { get; set; }

        Position2D Dock { get; set; }

        int GridRow { get; set; }

        int GridColumn { get; set; }

        bool IsExpanded { get; set; }

        string DataTemplateKey { get; set; }
    }
}

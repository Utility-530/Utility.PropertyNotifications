using Utility.Enums;

namespace Utility.Models
{
    public interface IViewModel
    {
        string Arrangement { get; set; }

        string Type { get; set; }
        Position2D Dock { get; set; }

        int GridRow { get; set; }

        int GridColumn { get; set; }

        int GridRowSpan { get; set; }

        int GridColumnSpan { get; set; }

        bool IsExpanded { get; set; }

        string DataTemplateKey { get; set; }

        // margin
        double Left { get; set; }
        double Right { get; set; }
        double Top { get; set; }

        double Bottom { get; set; }

        // margin
        string Tooltip { get; set; }
    }
}

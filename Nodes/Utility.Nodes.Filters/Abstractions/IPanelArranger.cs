using Utility.Enums;

namespace Utility.Nodes.Filters
{
    public interface IPanelArranger
    {
        Arrangement Arrangement { get; }
        int Rows { get; }
        int Columns { get; }
        Orientation Orientation { get; }
    }
}

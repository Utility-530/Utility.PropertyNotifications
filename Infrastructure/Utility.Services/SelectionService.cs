using Utility.Models;
using Utility.Services.Meta;

namespace Utility.Services
{
    public record SelectionParam() : Param<SelectionService>(nameof(SelectionService.Select), "selection");
    public record SelectionReturnParam() : Param<SelectionService>(nameof(SelectionService.Select));

    public class SelectionService
    {
        public static object Select(object selection) => selection;
    }
}

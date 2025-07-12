using Utility.Models;

namespace Utility.Services
{
    public record SelectionParam() : MethodParameter<SelectionService>(nameof(SelectionService.Select), "selection");
    public record SelectionReturnParam() : MethodParameter<SelectionService>(nameof(SelectionService.Select));

    public class SelectionService
    {
        public static object Select(object selection) => selection;
    }
}

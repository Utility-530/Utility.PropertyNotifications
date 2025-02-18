using MaterialDesignThemes.Wpf;

namespace Utility.Nodes.Demo.Infrastructure
{

    public abstract class ViewModel
    {
        public abstract PackIconKind Icon { get; }
        public abstract string Name { get; }
    }

    public class TopViewModel : ViewModel
    {
        public override string Name => "Top";

        public override PackIconKind Icon => PackIconKind.AlignVerticalTop;
    }

    public class BreadcrumbsViewModel : ViewModel
    {
        public override string Name => "Breadcrumbs";

        public override PackIconKind Icon => PackIconKind.Bread;
    }
    public class RootViewModel : ViewModel
    {
        public override string Name => "Root";

        public override PackIconKind Icon => PackIconKind.SquareRoot;
    }
    public class DescendantsViewModel : ViewModel
    {
        public override string Name => "Descendants";

        public override PackIconKind Icon => PackIconKind.OrderBoolDescending;
    }
}

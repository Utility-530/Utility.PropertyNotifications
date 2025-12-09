using Utility.Interfaces.Exs.Diagrams;
using Utility.Services.Meta;
using System.Reactive.Linq;
using Utility.Changes;
using Utility.Interfaces.Exs;

namespace Utility.Nodes.Demo.Filters.Services
{
    public record ComboServiceInputParam() : Param<ComboService>(nameof(ComboService.Generate), "nodeRoot");
    public record ComboServiceOutputParam() : Param<ComboService>(nameof(ComboService.Generate));

    public class ComboService
    {
        public static IObservable<Change<INodeViewModel>> Generate(INodeRoot nodeRoot)
        {
            return Observable.Create<Change<INodeViewModel>>(observer =>
            {
                observer.OnNext(Change.Reset<INodeViewModel>());
                return nodeRoot.Subscribe(root =>
                {
                    observer.OnNext(Change.Add<INodeViewModel>(root));

                }, () => observer.OnCompleted());
            });
        }
    }
}
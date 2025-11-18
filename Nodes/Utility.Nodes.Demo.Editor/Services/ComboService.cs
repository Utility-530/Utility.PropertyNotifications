using Utility.Interfaces.Exs.Diagrams;
using Utility.Services.Meta;
using System.Reactive.Linq;
using Utility.Changes;
using Utility.Interfaces.Exs;

namespace Utility.Nodes.Demo.Filters.Services
{
    public record ComboServiceInputParam() : Param<ComboService>(nameof(ComboService.Generate), "nodeSource");
    public record ComboServiceOutputParam() : Param<ComboService>(nameof(ComboService.Generate));

    public class ComboService
    {
        public static IObservable<Change<INodeViewModel>> Generate(INodeSource nodeSource)
        {
            return Observable.Create<Change<INodeViewModel>>(observer =>
            {
                observer.OnNext(Change<INodeViewModel>.Reset());
                return nodeSource.Roots().Subscribe(root =>
                {
                    observer.OnNext(Change<INodeViewModel>.Add(root));

                }, () => observer.OnCompleted());
            });
        }
    }
}
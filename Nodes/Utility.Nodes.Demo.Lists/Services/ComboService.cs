using Splat;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Utility.Models.Trees;
using Utility.Nodes.Filters;
using Utility.Nodes.WPF;
using Utility.PropertyNotifications;

namespace Utility.Nodes.Demo.Lists.Services
{
    public class ComboService : IObservable<ViewModel>
    {
        ReplaySubject<ViewModel> viewModels = new(1);
        public ComboService()
        {

            Locator.Current.GetService<MethodCache>()
                .Get(nameof(Utility.Nodes.Filters.NodeMethodFactory.BuildListRoot))
                .Subscribe(node =>
                {
                    node
                    .WithChangesTo(a => a.Current)
                    .Subscribe(a =>
                    {
                        if (a.Data is ModelTypeModel { Value.Type: { } type } data)
                        {
                            var x = new ChildViewModel(data.Value.Alias) { Data = data };
                            viewModels.OnNext(x);
                        }
                    });
                });
        }

        public IDisposable Subscribe(IObserver<ViewModel> observer)
        {
            return viewModels.Subscribe(observer);
        }
    }
}

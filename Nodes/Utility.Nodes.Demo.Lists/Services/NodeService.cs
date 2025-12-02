using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Splat;
using Utility.Interfaces;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Keys;
using Utility.PropertyDescriptors;
using Utility.Repos;
using Utility.ServiceLocation;
using Utility.Services.Meta;
using Utility.Trees.Extensions.Async;

namespace Utility.Nodes.Demo.Lists.Services
{

    internal record InValueParam() : Param<NodeService>(nameof(NodeService.Convert), "value");
    internal record OutValueParam() : Param<NodeService>(nameof(NodeService.Convert));

    internal class NodeService
    {
        public static IObservable<INodeViewModel> Convert(object value)
        {
            return Observable.Create<INodeViewModel>(observer =>
            {
                try
                {
                    var tree = Infrastructure.TreeRepository.Instance.CreateRoot(value) as INodeViewModel;
                    Locator.Current.GetService<INodeRoot>()
                    .Create(tree)
                    .Subscribe(a =>
                    {
                        observer.OnNext(a);
                        observer.OnCompleted();
                        //a.Descendants().Subscribe(d =>
                        //{
                        //    (d.NewItem as INodeViewModel).IsExpanded = true;
                        //});
                    });
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                }
                return () => { };
            });
        }
    }
}

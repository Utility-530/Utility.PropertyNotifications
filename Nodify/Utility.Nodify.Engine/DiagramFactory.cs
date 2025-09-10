using DryIoc;
using System;
using System.Threading.Tasks;
using Utility.Models;
using Utility.Nodify.Base.Abstractions;
using Utility.Nodify.Core;

namespace Utility.Nodify.Engine
{
    public class DiagramFactory : IDiagramFactory
    {
        Guid guid = new("004cf888-a762-4149-a3b9-7a0911cdf1a9");
        private IContainer container;

        public DiagramFactory(IContainer container)
        {
            this.container = container;            
        }

        public Task Build(IDiagramViewModel diagram)
        {         
            var treeResolver = new TreeResolver();

            var tree = new NamedTree() { Key = guid.ToString() };
            //_factories["004cf888-a762-4149-a3b9-7a0911cdf1a9"] = new(() => tree);

            var tcs = new TaskCompletionSource();
            treeResolver.Children(tree)
                .Subscribe(item =>
                {
                    var node = container.Resolve<IViewModelFactory>().CreateNode(item);
                    //{
                       
                    //    Data = item,
                    //    Guid = Guid.Parse((item as IGetKey).Key),
                    //    Key = (item as IReadOnlyTree).Index.ToString()
                    //    //Location = settings.NodeLocationGenerator(settings, ++i),
                    //};
                    diagram.Nodes.Add(node);
                }, () =>
                {
                    tcs.SetResult();
                });

            return tcs.Task;
            //ConnectNodesTree2(diagram);    
        }
    }
}


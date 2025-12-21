using DryIoc;
using System.Collections.ObjectModel;
using Utility.Changes;
using Utility.Interfaces.Exs;
using Utility.Models.Diagrams;
using Utility.Nodes;
using Utility.ServiceLocation;

namespace Nodify.Playground
{
    public class NodesGeneratorMaster
    {
        public ObservableCollection<ConnectionViewModel> GenerateConnections()
        {
            ObservableCollection<ConnectionViewModel> connections = [];

            (Utility.Globals.Resolver.Resolve<IServiceResolver>() as IObservable<Set<IResolvableConnection>>)
                .Subscribe(set =>
                {
                    foreach (var item in set)
                    {
                        if (item.Value is MethodConnection { In: { } @in, Out: { } @out } mConn && item.Type == Utility.Changes.Type.Add)
                        {
                            if (@out is MethodConnector m && @in is { } mIn)
                            {
                                continue;
                            }
                            var input = Shared.outputConnectors.SingleOrDefault(a => a.Key == @in).Value;
                            var output = Shared.outputConnectors.SingleOrDefault(a => a.Key == @out).Value;
                            var connection = new ConnectionViewModel
                            {
                                Input = input,
                                Output = output,
                                Data = mConn,
                                Key = Guid.NewGuid().ToString()
                            };

                            connections.Add(connection);
                        }
                        else
                            throw new Exception("E$$3");
                    }
                });

            return connections;
        }

    }
}
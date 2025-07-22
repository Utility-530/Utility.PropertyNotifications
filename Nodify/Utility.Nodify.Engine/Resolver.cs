using Utility.Nodify.Core;
using System;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DryIoc;
using Key = Utility.Nodify.Core.Key;
using System.Collections.ObjectModel;
using Utility.PropertyNotifications;
using Chronic;
using Utility.Nodify.Base;
using Keys = Utility.Nodify.Operations.Keys;

namespace Utility.Nodify.Operations
{


    public class Resolver : NotifyPropertyClass
    {
        SynchronizationContext context = SynchronizationContext.Current ?? throw new Exception("dfs 3!!!");

        private readonly IContainer container;
        private IObservable<object> next => container.Resolve<IObservable<object>>(Keys.Next);
        private IObservable<object> previous => container.Resolve<IObservable<object>>(Keys.Previous);
        private ICollection<Exception> exceptions => container.Resolve<ICollection<Exception>>(Keys.Exceptions);
        private IList<Message> past => container.Resolve<RangeObservableCollection<Message>>(Keys.Past);
        private IList<Message> current => container.Resolve<RangeObservableCollection<Message>>(Keys.Current);
        private IList<Message> future => container.Resolve<RangeObservableCollection<Message>>(Keys.Future);
        private IDiagramViewModel diagram => container.Resolve<IDiagramViewModel>();
        //private IEnumerable<IConnectionViewModel> connections => container.Resolve<IEnumerable<IConnectionViewModel>>();

        public Resolver(IContainer container)
        {
            this.container = container;
            next
                .Subscribe(async a =>
                {
                    if (future.Any() == false)
                        Cycle();
                    if (future.Any() == false)
                        return;

                    var next = future[0];

                    current.Add(next);
                    if (current.Last() is NodeMessage { Node: { Core: { } core } node } nodeMessage)
                    {
                        //if (node.Output.Any())
                        //{
                        Exception? ex = null;
                        IOValue? output = null;

                        try
                        {
                            if (core is IOperation operation)
                            {
                                output = operation.Execute(nodeMessage.Inputs);
                                node.State = NodeState.OutputValueChanged;
                                future.RemoveAt(0);
                            }
                            else
                                throw new Exception("dfsd");
                            //output = operations[next.Key.Name].Operation.Execute(nodeMessage.Inputs);
                        }
                        catch (Exception e)
                        {
                            ex = e;
                        }
                        context.Post((a) =>
                        {
                            past.Add(next);
                            if (current.Remove(next) == false)
                            {
                                throw new Exception("v222d sdww");
                            }
                            if (ex != null)
                            {
                                next = next with { Exception = ex };
                                //exceptions.Add(ex);
                            }
                            else if (output != null)
                            {
                                bool success = false;
                                foreach (var connector in node.Output)
                                {
                                    if (output.Title == connector.Title || output.Title == default)
                                    {
                                        connector.Value = output.Value;
                                        connector.Connections.ForEach(c => c.State = NodeState.OutputValueChanged);
                                        success = true;
                                    }
                                }
                                if (success == false)
                                    throw new Exception(" 3 34565");
                            }
                            else
                            {
                                throw new Exception("d11 fs 3??l!!!");
                            }
                        }, default);
                        //}
                        //else
                        //{
                        //    if (current.Remove(next) == false)
                        //    {
                        //        throw new Exception("vd sdww");
                        //    }
                        //}
                    }
                    else if (current.Last() is ConnectionMessage { Connection: { Input: { } input, Output: { } output } connection } connectorMessage)
                    {
                        //var connection = connections[next.Key];
                        //if (connection.Input != null)
                        //{
                        try
                        {
                            past.Add(next);
                            if (current.Remove(next) == false)
                            {
                                throw new Exception("v222d sdww");
                            }
                            //if (filters.ContainsKey(next.Key.Name))
                            //{
                            //    if (filters[next.Key.Name].Filter.Execute(connectorMessage.Output))
                            //     output.Value = input.Value;
                            //}
                            //else
                            {
                                input.Value = output.Value;
                                connection.State = NodeState.InputValueChanged;
                                future.RemoveAt(0);
                            }
                        }
                        catch
                        {

                        }
                        //}
                    }
                    else
                    {
                        throw new Exception("143 34vd sdww");
                    }
                },
                e =>
                {

                });

            //previous = new DelegateCommand(() =>
            //{
            //    var previous = Past.Last();
            //    Future.Insert(0, previous);
            //    Future.RemoveAt(0);
            //});
            this.container = container;
        }


        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void Cycle()
        {
            foreach (var _node in diagram.Nodes)
            {
                if (_node.State == NodeState.None)
                {
                    //if (_node.Input.Any())
                    //{
                    //    var values = _node.Input.Select(a => new IOValue(a.Title, a.Value)).ToArray();
                    //    future.Add(new NodeMessage(_node.Key, values, _node));
                    //}
                }
                if (_node.State == NodeState.InputValueChanged)
                {
                    var values = _node.Input.Select(a => new IOValue(a.Title, a.Value)).ToArray();
                    future.Add(new NodeMessage(_node.Key, values, _node));
                }
            }

            foreach (var connection in diagram.Connections)
            {
                if (connection.State == NodeState.OutputValueChanged)
                {
                    future.Add(new ConnectionMessage(connection.Key, connection));
                }

            }
        }
    }
}

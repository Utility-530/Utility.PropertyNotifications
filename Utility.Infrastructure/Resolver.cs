using DryIoc;
using DynamicData;
using System.Collections.ObjectModel;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.PropertyTrees.Infrastructure;
using History = Utility.PropertyTrees.Infrastructure.History;

namespace Utility.Infrastructure
{
    public class Resolver : BaseObject 
    {
        private readonly Container container;
        private readonly Outputs[] connections;
        private readonly History history;
        Guid Guid = Guid.Parse("222b7df3-9ae7-49d6-aabc-a492c6254718");
        public override Key Key => new(Guid, nameof(Resolver), typeof(Resolver));


        public Resolver(Container container)
        {
            this.container = container;
            history = container.Resolve<History>();
            connections = this.container.Resolve<Outputs[]>();
            // vertices
            //var nodes = container.Resolve<IBase>();
        }

        public void Initialise()
        {
            container.RegisterInitializer<object>((initialized, b) => { Broadcast(new InitialisedEvent(initialized)); });
            Broadcast(connections);
        }

        public override bool OnNext(object next)
        {
            if (next is not IBase @base)
            {
                throw new Exception("ds s");
            }

            if (next is History)
            {
                if (@base.Output is not ChangeSet { } changeSet)
                {
                    throw new Exception("ujuj  sdsdf");
                }

                foreach (var connection in Connections(@base.Key))
                    Broadcast(changeSet, connection);

                foreach (var item in changeSet)
                {
                    Broadcast(item);
                }
            }
            else
            {
                var connections = Connections(@base.Key);
                var _value = @base.Output;


                //foreach (var connection in connections)
                //{
                //    Broadcast(_value, connection);
                //}

                foreach (var connection in connections.Where(a => a.IsPriority))
                {
                    Broadcast(_value, connection);
                }
                foreach (var connection in connections.Where(a => a.IsPriority == false))
                {
                    var order = new Order(Guid.NewGuid(), _value, connection);
                    history.OnNext(order);
                }
            }
            return true;
        }

        protected override void Broadcast(object obj)
        {
            Output = obj;
            this.OnNext(this);
        }

        private void Broadcast(Change item)
        {
            if (item is not Change { Type: var type, Value: HistoryOrder { History: var hist, Order: var order } })
            {
                throw new Exception("22 j  sdsdf");
            }
            switch (hist, type)
            {
                case (Enums.History.Present, Models.ChangeType.Add):
                    if (order is Order { Value: var value, Connection: var connection })
                    {
                        Broadcast(value, connection);
                        break;
                    }
                    else
                        throw new Exception("s44 dv3 ");
            }
        }

        private void Broadcast(object order, IConnection connection)
        {
            //if (connection.Observers.Any() == false)
            //    throw new Exception("kl99dsf  sdffdsdff");

            try

            {
                //if (connection.SkipContext == false || SynchronizationContext.Current ==null)
                if (true)
                {
                    (Context ?? throw new Exception("missing context"))
                        .Post(a =>
                        {        
                            foreach (var observer in connection.Observers)
                            {
                                if(observer.OnNext(order))
                                {

                                }

                                foreach(var conn in Connections(this.Key))
                                {
                                    foreach (var obs in conn.Observers)
                                        obs.OnNext(new BroadcastEvent(observer));
                                }
                            }
                        }, default);
                }
                else
                {
                    foreach (var observer in connection.Observers)
                    {
                        observer.OnNext(order);
                        //Broadcast(new BroadcastEvent(observer));
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        ICollection<IConnection> Connections(IEquatable equatable)
        {
            if (equatable is not Key key)
            {
                throw new Exception("£ dfgdf");
            }

            Collection<IConnection> observers = new();

            //foreach (var outputConnection in container.ResolveMany<Outputs>())
            foreach (var outputConnection in connections)
            {
                if (outputConnection.Predicate(key))
                {
                    observers.AddRange(outputConnection.Connections);
                }
            }

            return observers;
        }

    }

    public record Order(Guid Key, object Value, IConnection Connection);

}

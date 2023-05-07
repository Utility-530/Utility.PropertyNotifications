using Utility.Infrastructure;
using Utility.Models;
using System.Collections.Generic;
using System;
using Utility.Interfaces.Generic;
using System.Linq;

namespace Utility.GraphShapes
{
    public class GraphController : BaseObject
    {
        public static Guid Guid => System.Guid.Parse("0bd8ea77-29c7-4039-aac2-94405423c398");

        public override Key Key => new(Guid, nameof(GraphController), typeof(GraphController));
        List<object> events = new();

        public override bool OnNext(object value)
        {
            if (value is Outputs[] connections)
            {
                this.Outputs = connections;
                this.OnPropertyChanged(nameof(Outputs));
                foreach (var @event in events)
                {
                    OnNext(@event);
                }
                return true;    
            }
            else if (value is InitialisedEvent initialisedEvent)
            {
                if (initialisedEvent.Source is IKey<Key> { Key: var key })
                {
                    if (this.Outputs == null)
                    {
                        events.Add(initialisedEvent);
                    }
                    else
                        foreach (var outputs in this.Outputs)
                        {
                            if (outputs.Predicate(key))
                            {
                                var source = new PocVertex(key.Name);
                                if (Graph.ContainsVertex(source) == false)
                                    Graph.AddVertex(source);

                                foreach (var connection in outputs.Connections)
                                {
                                    var target = new PocVertex(connection.ToString());

                                    var edge = new PocEdge(connection.ToString(), source, target);
                                    if (Graph.ContainsEdge(edge) == false)
                                    {
                                        Graph.AddVertex(target);
                                        Graph.AddEdge(edge);
                                        target.Count++;
                                    }
                                    else
                                    {
                                        var match = Graph.Vertices.SingleOrDefault(a => a.Equals(source));
                                        match.Count++;
                                    }
                                }
                            }
                        }
                }
                return true;
            }  
            else if (value is BroadcastEvent broadcastEvent)
            {
                if (broadcastEvent.Source is IKey<Key> { Key: var key })
                {
                    if (this.Outputs == null)
                    {
                        events.Add(broadcastEvent);
                    }
                    else
                        foreach (var outputs in this.Outputs)
                        {
                            if (outputs.Predicate(key))
                            {
                                var source = new PocVertex(key.Name);
                                if (Graph.ContainsVertex(source) == false)
                                    Graph.AddVertex(source);

                                foreach (var connection in outputs.Connections)
                                {
                                    var target = new PocVertex(connection.ToString());

                                    var edge = new PocEdge(connection.ToString(), source, target);
                                    if (Graph.ContainsEdge(edge) == false)
                                    {
                                        Graph.AddVertex(target);
                                        Graph.AddEdge(edge);
                                        target.Broadcast++;
                                    }
                                    else
                                    {
                                        var match = Graph.Vertices.SingleOrDefault(a => a.Equals(source));
                                        match.Broadcast++;
                                    }
                                }
                            }
                        }
                }
                return true;
            }
            return base.OnNext(value);
        }

        public PocGraph Graph { get; } = new PocGraph();

        public Outputs[] Outputs { get; set; }

    }
}

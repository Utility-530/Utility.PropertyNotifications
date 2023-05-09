using Utility.Infrastructure;
using Utility.Models;
using System.Collections.Generic;
using System;
using Utility.Interfaces.Generic;
using System.Linq;
using UnitsNet;

namespace Utility.Graph.Shapes
{
    public class GraphController : BaseObject
    {
        private List<object> events = new();
        private PocVertex? selectedVertex;

        public static Guid Guid => System.Guid.Parse("0bd8ea77-29c7-4039-aac2-94405423c398");

        public override Key Key => new(Guid, nameof(GraphController), typeof(GraphController));

        public PocGraph Graph { get; } = new PocGraph();

        public Outputs[]? Outputs { get; set; }

        public PocVertex SelectedVertex { get => selectedVertex; set => this.Set(ref selectedVertex, value); }

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
            if (value is SelectEvent { Vertex: var vertex } selectEvent)
            {
                if (Add(value))
                    return true;
                SelectedVertex = vertex;
            }
            else if (value is BreakEvent { Vertex: var vert } breakEvent)
            {
                if (Add(value))
                    return true;
                foreach (var outputs in this.Outputs ?? throw new Exception("vsd 333:KK"))
                {
                    var target = vert.ID.Equals(outputs.Key.Name);
                    if (target)
                        foreach (var connection in outputs.Connections)
                        {
                            connection.IsPriority = !connection.IsPriority;
                        }
                }
            }
            else if (value is InitialisedEvent @event)
            {
                if (Add(value))
                    return true;
                if (@event.Source is IKey<Key> { Key: var key })
                {
                    foreach (var outputs in this.Outputs ?? throw new Exception("vsd 333:KK"))
                    {
                        if (outputs.Match(key))
                        {
                            if (key.Name == "ViewBuilder")
                            {

                            }
                            var source = new PocVertex(key.Name);
                            source.Subscribe(this);
                            if (Graph.ContainsVertex(source) == false)
                                Graph.AddVertex(source);

                            foreach (var connection in outputs.Connections)
                            {
                                var target = new PocVertex(connection.ToString());
                                target.Subscribe(this);
                                var edge = new PocEdge(connection.ToString(), source, target);
                                if (Graph.ContainsEdge(edge) == false)
                                {
                                    Graph.AddVertex(target);
                                    Graph.AddEdge(edge);
                                    target.Events.Add(@event);
                                }
                                else
                                {
                                    var match = Graph.Vertices.SingleOrDefault(a => a.Equals(source));
                                    match.Events.Add(@event);
                                }
                            }
                        }
                    }
                }
                return true;
            }
            else if (value is BroadcastEvent broadcastEvent)
            {
                if (Add(value))
                    return true;
                if (broadcastEvent.Source is IKey<Key> { Key: var key } &&
                    broadcastEvent.Target is IKey<Key> { Key: var key2 })
                {
                    foreach (var _vertex in Graph.Vertices)
                    {
                        if (_vertex.ID.Equals(key.Name))
                            _vertex.Events.Add(broadcastEvent);
                        if (_vertex.ID.Equals(key2.Name))
                            _vertex.Events.Add(broadcastEvent);
                    }
                }
                else
                    throw new Exception("dfg 4fed sd");
                return true;
            }

            return base.OnNext(value);
        }

        bool Add(object value)
        {
            if (this.Outputs == null)
            {
                events.Add(value);
                return true;
            }
            return false;
        }
    }
}

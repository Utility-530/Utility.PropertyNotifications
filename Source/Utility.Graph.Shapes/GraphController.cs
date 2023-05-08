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
        private PocVertex? selectedVertex;

        public PocGraph Graph { get; } = new PocGraph();

        public Outputs[]? Outputs { get; set; }

        public PocVertex SelectedVertex { get => selectedVertex; set => this.Set(ref selectedVertex, value); }

        public override bool OnNext(object value)
        {
            if (value is SelectEvent { Vertex: var vertex })
            {
                SelectedVertex = vertex;
            }
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
            else if (value is Event @event)
            {
                if (@event.Source is IKey<Key> { Key: var key })
                {
                    if (this.Outputs == null)
                    {
                        events.Add(@event);
                    }
                    else
                        foreach (var outputs in this.Outputs)
                        {
                            if (outputs.Match(key))
                            {
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
            return base.OnNext(value);
        }
    }
}

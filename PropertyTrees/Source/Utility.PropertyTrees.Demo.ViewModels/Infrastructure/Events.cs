using Netly;
using Utility.Models;
using Utility.Trees.Abstractions;
using Utility.PropertyTrees;

public record StartEvent(RootProperty Property) : Event;


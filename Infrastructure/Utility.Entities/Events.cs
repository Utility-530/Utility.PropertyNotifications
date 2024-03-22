using System;
using System.ComponentModel;
using System.Reflection;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Structs;
using Utility.Trees.Abstractions;

namespace Utility.Infrastructure;

public record InitialisedEvent(object Value) : Event();
//public record ConnectionsEvent(Outputs[] Outputs, object Source) : Event();
public record BroadcastEvent(object Source, object Target) : Event();
public record BroadcastSuccessEvent(object Source, object Target) : BroadcastEvent(Source, Target);
public record BroadcastFailureEvent(object Source, object Target) : BroadcastEvent(Source, Target);
//public record FindPropertyRequest(Key Key) : Request;
public record FindPropertyResponse(IEquatable[] Keys) : Response(Keys);
public record GetPropertyResponse(object Value) : Response(Value);
public record SetPropertyResponse(object Value) : Response(Value);
public record PropertyRequest(IEquatable Key) : Request;
public record SetPropertyRequest(IEquatable Key, object Value) : PropertyRequest(Key);
public record GetPropertyRequest(IEquatable Key) : PropertyRequest(Key);
public record HistoryRequest(Guid Key, IKey Base, object Value, ICollection<IKey> Keys) : Request;
public record DirectionEvent() : Event();
public record ForwardEvent() : Event();
public record BackEvent() : Event();
public record ObjectCreationRequest(Type Type, Type[] RegistrationTypes, object[] Args) : Request;
public record ObjectCreationResponse(object Instance) : Response(Instance);
public record TypeRequest(Type Type) : Request;
public record TypeResponse(Type Type) : Response(Type);
public record MethodParametersRequest(MethodInfo MethodInfo, object Data) : Request;
public record MethodParametersResponse(object?[]? Parameters) : Response(Parameters);

public record ClickChange(object Source, IReadOnlyTree Node) : Event();
public record DoubleClickChange(object Source, IReadOnlyTree Node) : Event();
public record OnHoverChange(object Source, IReadOnlyTree Node, bool IsMouseOver, Point Point) : Event();
public record OnLoadedChange(object Source, IReadOnlyTree Node) : Event();
public record TreeViewItemInitialised(object Source, IReadOnlyTree Node) : Event();

public record RefreshRequest(DateTime DateTime) : Request;
public record SaveRequest(DateTime DateTime) : Request;

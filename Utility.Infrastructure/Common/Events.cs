using Utility.Interfaces.NonGeneric;
using Utility.Models;

namespace Utility.Infrastructure;

public record InitialisedEvent(object Value) : Event();
public record ConnectionsEvent(Outputs[] Outputs, object Source) : Event();
public record BroadcastEvent(object Source, object Target) : Event();
public record BroadcastSuccessEvent(object Source, object Target) : BroadcastEvent(Source, Target);
public record BroadcastFailureEvent(object Source, object Target) : BroadcastEvent(Source, Target);
public record FindPropertyRequest(Key Key) : Request;
public record FindPropertyResponse(IEquatable Key) : Response(Key);
public record GetPropertyResponse(object Value) : Response(Value);
public record SetPropertyResponse(object Value) : Response(Value);
public record PropertyRequest(IEquatable Key) : Request;
public record SetPropertyRequest(IEquatable Key, object Value) : PropertyRequest(Key);
public record GetPropertyRequest(IEquatable Key) : PropertyRequest(Key);
public record HistoryRequest(Guid Key, Key Base, object Value, ICollection<Key> Keys) : Models.Request;
public record DirectionEvent() : Event();
public record ForwardEvent() : Event();
public record BackEvent() : Event();


public record ObjectCreationRequest(Type Type, Type[] RegistrationTypes, object[] Args) : Request;
public record ObjectCreationResponse(object Instance) : Response(Instance);




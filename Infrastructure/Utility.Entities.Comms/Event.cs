
using System;
using Utility.Interfaces.NonGeneric;

namespace Utility.Entities.Comms;

public record Cause() : IGetGuid
{
    Guid IGetGuid.Guid => GetType().GUID;
}

public record Effect() : Cause();

public record Response(object Value) : Effect();

public record Request() : Cause();

public record Event() : Cause(), IEvent;

public record IsCompleteEvent(string Key, bool? Value) : Event();

public record CloseRequest(string Key, bool Value) : Event();

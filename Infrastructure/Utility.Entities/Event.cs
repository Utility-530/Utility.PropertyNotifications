
using Utility.Interfaces.NonGeneric;

namespace Utility.Models;

public record Cause() : IGuid
{
    Guid IGuid.Guid => this.GetType().GUID;
}

public record Effect() : Cause();

public record Response(object Value) : Effect();

public record Request() : Cause();

public record Event() : Cause(), IEvent;

public record IsCompleteEvent(string Key, bool? Value) : Event();

public record CloseRequest(string Key, bool Value) : Event();

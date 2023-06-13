
using Utility.Interfaces.NonGeneric;

namespace Utility.Models;

public record Cause() : IGuid
{
    Guid IGuid.Guid => this.GetType().GUID;
}

public record Effect() : Cause();

public record Response(object Value) : Effect();




public record Request() : Cause();

public record Event() : Cause();

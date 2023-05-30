
using Utility.Interfaces.NonGeneric;

namespace Utility.Models
{
    public record Cause : IGuid
    {
        public virtual Guid Guid => this.GetType().GUID;
    }

    public record Effect() : Cause;

    public record Response(object Value) : Effect;


    //public Response(object Value) : this(Value, 1, 1)
    //{
    //}

    //public bool IsComplete => Completed == Total;


    public record Request : Cause;

    public record Event : Cause;

    //public record IO(Guid Guid);

    public record Input(GuidBase Guid, Cause Cause);

    public record Output(GuidBase Guid, Input Input, Effect Effect) : Input(Guid, Effect);

}

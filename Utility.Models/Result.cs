
namespace Utility.Models
{
    public record Cause;

    public record Effect;

    public record Response(double Completed, double Total): Effect
    {
        public Response() : this(1, 1)
        {
        }

        public bool IsComplete => Completed == Total;
    }

    public record Request() : Cause;

    public record Event(): Cause;

}

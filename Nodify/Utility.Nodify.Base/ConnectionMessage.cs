namespace Utility.Nodify.Core
{
    public record ConnectionMessage(Key Key, IConnectionViewModel Connection) : Message(Key, default);

}

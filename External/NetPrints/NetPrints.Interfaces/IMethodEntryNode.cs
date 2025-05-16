namespace NetPrints.Graph
{
    public interface IMethodEntryNode
    {
        void AddArgument();
        void AddGenericArgument();
        void RemoveArgument();
        void RemoveGenericArgument();
    }
}
namespace Utility.PropertyTrees.Abstractions
{
    public interface IPropertyGridEngine
    {
        Task<IPropertyNode> Convert(object options);
    }
}
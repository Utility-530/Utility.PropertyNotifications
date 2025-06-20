namespace Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModelAttribute(string guid, string? transformMethod = default) : Attribute
    {
        public Guid Guid { get; } = Guid.Parse(guid);
        public string? TransformMethod { get; } = transformMethod;
    }
}

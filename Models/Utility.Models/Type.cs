namespace Utility.Models.Types
{
    public record Type
    {
        public string Name { get; init; }
        public string Namespace { get; init; }
        public string Assembly { get; init; }

        public static implicit operator System.Type(Type type) => System.Type.GetType($"{type.Namespace}.{type.Name}, {type.Assembly}");
    }
}

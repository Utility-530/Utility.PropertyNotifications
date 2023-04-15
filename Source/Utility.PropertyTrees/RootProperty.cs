namespace Utility.PropertyTrees
{
    public class RootProperty : PropertyBase
    {
        public RootProperty(Guid guid) : base(guid)
        {
        }

        public override string Name => PropertyType.Name;

        public override bool IsReadOnly => false;

        public override object Value { get; set; }
    }
}
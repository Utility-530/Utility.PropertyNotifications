
namespace Utility.Descriptors.Types
{
    public class PropertyType : CustomType
    {
        private readonly Type parentType;
        private readonly string name;

        public PropertyType(Type parentType, string name)
        {
            this.parentType = parentType;
            this.name = name;
        }

        public override Kind Kind => Kind.Property;

        public override Assembly Assembly => this.parentType.Assembly;


        public override string? Namespace => this.parentType.Namespace;



        public override string Name => name;


    }
}


namespace Utility.PropertyDescriptors.Types
{
    public class MethodType : CustomType
    {
        private readonly Type parentType;
        private readonly string name;

        public MethodType(Type parentType, string name)
        {
            this.parentType = parentType;
            this.name = name;
        }

        public override Kind Kind => Kind.Method;
        public override Assembly Assembly => this.parentType.Assembly;


        public override string? Namespace => this.parentType.Namespace;



        public override string Name => name;


    }
}

using NetPrints.Core;
using NetPrints.Interfaces;

namespace NetPrintsEditor.Dialogs
{
    public class DelegateSpecifier : ISpecifier
    {
        public ITypeSpecifier Type
        {
            get;
        }

        public ITypeSpecifier FromType
        {
            get;
        }

        public DelegateSpecifier(ITypeSpecifier type, ITypeSpecifier fromType)
        {
            Type = type;
            FromType = fromType;
        }
    }
}

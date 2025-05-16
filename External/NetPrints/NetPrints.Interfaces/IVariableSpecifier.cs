using NetPrints.Enums;

namespace NetPrints.Core
{
    public interface ISpecifier
    { }
    public interface IVariableSpecifier:ISpecifier
    {
        MemberVisibility Visibility { get; set; }
    }
}
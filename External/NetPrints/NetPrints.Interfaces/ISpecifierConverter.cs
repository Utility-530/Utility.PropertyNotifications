using NetPrints.Core;

namespace NetPrintsEditor.Converters
{
    public interface ISpecifierConverter
    {
        (string text, string iconPath) Convert(ISpecifier value);
    }
}
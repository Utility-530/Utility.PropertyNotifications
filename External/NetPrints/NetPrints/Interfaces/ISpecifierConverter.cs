using NetPrints.Core;

namespace NetPrintsEditor.Converters
{
    public interface ISpecifierConverter
    {
        string ConvertToIconPath(ISpecifier value);
        string ConvertToText(ISpecifier value);
    }
}
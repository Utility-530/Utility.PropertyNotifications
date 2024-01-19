namespace Utility.Properties
{
    [Flags]
    public enum PropertyType
    {
        Root = 1,
        Reference = 2,
        Value = 4,
        CollectionItem = 8
    }
}



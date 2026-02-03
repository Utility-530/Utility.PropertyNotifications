namespace Utility.WPF.ComboBoxes.Roslyn
{
    public static class SymbolKindWeights
    {
        public static int Get(IntelliSenseSymbolKind kind) => kind switch
        {
            IntelliSenseSymbolKind.Type => 0,
            IntelliSenseSymbolKind.Method => 10,
            IntelliSenseSymbolKind.Property => 20,
            IntelliSenseSymbolKind.Field => 30,
            IntelliSenseSymbolKind.Event => 40,
            IntelliSenseSymbolKind.Local => 50,
            IntelliSenseSymbolKind.Namespace => 60,
            IntelliSenseSymbolKind.Keyword => 70,
            _ => 100
        };
    }


}

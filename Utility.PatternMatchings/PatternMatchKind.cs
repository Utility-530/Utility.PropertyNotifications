namespace Utility.PatternMatchings
{
    public enum PatternMatchKind
    {
        //Uppercase initials only (CWL)
        Acronym,
        //Exact, case-sensitive
        Exact,
        //Case-sensitive prefix
        Prefix,
        //Case-insensitive prefix
        PrefixIgnoreCase,
        //CamelCase initials (CWL)
        CamelCase,
        //Word-boundary starts (non-camel)
        WordStart,
        //Contiguous substring
        Substring,
        //Ordered, non-contiguous
        Fuzzy
    }
}


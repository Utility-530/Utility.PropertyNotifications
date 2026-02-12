using System.Reflection;
using Microsoft.CodeAnalysis;

namespace Utility.Roslyn
{
    public static class AccessibilityExtensions
    {
        public static BindingFlags ToBindingFlags(this Accessibility accessibility)
        {
            switch (accessibility)
            {
                case Accessibility.Public:
                    return BindingFlags.Public;
                case Accessibility.Private:
                case Accessibility.Protected:
                case Accessibility.Internal:
                case Accessibility.ProtectedOrInternal:
                case Accessibility.ProtectedAndInternal:
                    return BindingFlags.NonPublic;
                case Accessibility.NotApplicable:
                default:
                    return BindingFlags.Default;
            }
        }
    }
}

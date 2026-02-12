using System;
using System.Text;

namespace Utility.PatternMatchings
{
    public static class NamespaceWeighting
    {
        public static int GetPenalty(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return 0;

            int depth = 0;
            foreach (char c in fullName)
                if (c == '.')
                    depth++;

            return depth * 5;
        }
    }


}

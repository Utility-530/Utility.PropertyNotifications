using System;
using Utility.WPF.Helpers;

namespace Utility.WPF.ResourceDictionarys
{
    public class AutoResourceDictionary : SharedResourceDictionary
    {
        private Type? type;

        public Type Type
        {
            set
            {
                if (type == value)
                    return;
                type = value;
                foreach (var (_, resourceDictionary) in type.Assembly.SelectResourceDictionaries())
                {
                    AddToMergedDictionaries(resourceDictionary);
                }
            }
        }
    }
}
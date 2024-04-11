using System;
using System.Collections;
using Utility.Keys;

namespace Utility.ProjectStructure
{
    public record DictionaryEntryKey(DictionaryEntry Entry) : StringKey(Entry.Key?.ToString() ?? Guid.NewGuid().ToString())
    {
        //private readonly Lazy<FrameworkElement> lazy = new(() => Lazy(Entry));
        //static FrameworkElement Lazy(DictionaryEntry entry)
        //{
        //    try
        //    {
        //        return Factorys.FrameworkElementConverter.GetFrameworkElement(entry.Value);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new TextBlock { Text = ex.Message };
        //    }
        //    //else
        //    //    throw new Exception("sdg33333__d");
        //}
        public string GroupKey => Entry.Value?.GetType().Name.ToString();

        //public override object Value => FrameworkElementConverter.GetFrameworkElement(Entry.Value);
    }
}


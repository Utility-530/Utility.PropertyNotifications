using System;
using System.Collections;
using Utility.WPF.Factorys;

namespace Utility.ProjectStructure
{
    public record DictionaryEntryKeyValue(DictionaryEntry Entry) : KeyValue(Entry.Key?.ToString() ?? Guid.NewGuid().ToString())
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
        public override string GroupKey => Entry.Value?.GetType().Name.ToString();

        public override object Value => FrameworkElementConverter.GetFrameworkElement(Entry.Value);
    }
}


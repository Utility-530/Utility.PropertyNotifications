using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Factorys;

namespace Utility.WPF.Meta
{
    public record DataTemplateKeyValue(DictionaryEntry Entry) : KeyValue(Entry.Key.ToString() + " " + "(" + Entry.Value?.GetType().Name.ToString() + ")")
    {
        private readonly Lazy<FrameworkElement> lazy = new(() => Lazy(Entry));
        static FrameworkElement Lazy(DictionaryEntry entry)
        {
            try
            {
                return Factorys.FrameworkElementConverter.GetFrameworkElement(entry.Value);
            }
            catch (Exception ex)
            {
                return new TextBlock { Text = ex.Message };
            }
            //else
            //    throw new Exception("sdg33333__d");
        }

        public override FrameworkElement Value => lazy.Value;
    }

}
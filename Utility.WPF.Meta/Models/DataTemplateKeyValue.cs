using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Factorys;

namespace Utility.WPF.Meta
{
    public class DataTemplateKeyValue : KeyValue
    {
        private readonly Lazy<FrameworkElement> lazy;

        public DataTemplateKeyValue(DictionaryEntry entry) : base(entry.Key.ToString() + " " + "(" + entry.Value?.GetType().Name.ToString() + ")")
        {
            Entry = entry;
            lazy = new(() => Lazy(entry));

            static FrameworkElement Lazy(DictionaryEntry entry)
            {
                try
                {
                    return Factorys.FrameworkElementFactory.GetFrameworkElement(entry.Value);
                }
                catch (Exception ex)
                {
                    return new TextBlock { Text = ex.Message };
                }
                //else
                //    throw new Exception("sdg33333__d");
            }
        }


        public override FrameworkElement Value => lazy.Value;

        public DictionaryEntry Entry { get; }
    }

}
using System;
using System.Windows;
using System.Windows.Controls;


namespace Utility.WPF.Meta
{
    public record FrameworkElementKeyValue(string Key, Type Type)  : KeyValue(Key)
    {
        private readonly Lazy<FrameworkElement?> lazy = new(() =>
        {
            try
            {
                return (FrameworkElement?)Activator.CreateInstance(Type);
            }
            catch (Exception ex)
            {
                return new TextBlock { Text = ex.Message };
            }
        });

        public Type Type { get; }

        public override FrameworkElement? Value => lazy.Value;

    }

}
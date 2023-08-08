using System;
using System.Windows;
using System.Windows.Controls;


namespace Utility.WPF.Meta
{
    public class FrameworkElementKeyValue : KeyValue
    {
        private readonly Lazy<FrameworkElement?> lazy;

        public FrameworkElementKeyValue(string key, Type type) : base(key)
        {
            Key = key;
            Type = type;
            lazy = new(() =>
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
        }

        public string Key { get; }

        public Type Type { get; }

        public override FrameworkElement? Value => lazy.Value;



    }

}
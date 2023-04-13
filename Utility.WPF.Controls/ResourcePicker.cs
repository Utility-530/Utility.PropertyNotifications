using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Utility.WPF.Controls
{
    public class ResourcePicker : ComboBox
    {
        private string[]? keys;

        static ResourcePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ResourcePicker), new FrameworkPropertyMetadata(typeof(ResourcePicker)));
        }

        public ResourcePicker()
        {
            this.SelectionChanged += ResourcePicker_SelectionChanged;
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
        }

        private void ResourcePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResourceDictionary? dic = null;
            var uri = e.AddedItems.OfType<Uri>().FirstOrDefault();
            if (uri?.Equals(default) == false)
            {
                using Stream fs = Application.GetResourceStream(uri).Stream;
                dic = (ResourceDictionary)XamlReader.Load(fs);
            }
            dic ??= e.AddedItems.OfType<ResourceDictionary>().FirstOrDefault();

            if (keys != null)
            {
                var remove = Application.Current.Resources.MergedDictionaries
                    .Select((a, i) => (a, i))
                    .SingleOrDefault(c => c.a.Keys.OfType<string>().SequenceEqual(keys)).i;
                Application.Current.Resources.MergedDictionaries.RemoveAt(remove);
            }

            Application.Current.Resources.MergedDictionaries.Add(dic);
            keys = dic?.Keys.OfType<string>().ToArray();
        }
    }
}
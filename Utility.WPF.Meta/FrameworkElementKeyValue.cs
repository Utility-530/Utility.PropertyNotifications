using MoreLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Tynamix.ObjectFiller;
using Utility.Common;
using Utility.Helpers;
using Utility.WPF.Helper;
using UtilityWpf;

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

        public static IEnumerable<FrameworkElementKeyValue> ViewTypes(Assembly assembly) => assembly
       .GetTypes()
       .Where(a => typeof(UserControl).IsAssignableFrom(a))
       .GroupBy(type =>
       (type.Name.Contains("UserControl") ? type.Name?.ReplaceLast("UserControl", string.Empty) :
       type.Name.Contains("View") ? type.Name?.ReplaceLast("View", string.Empty) :
       type.Name)!)
       .OrderBy(a => a.Key)
       .ToDictionaryOnIndex()
       .Select(a => new FrameworkElementKeyValue(a.Key, a.Value));

        public static IEnumerable<FrameworkElementKeyValue> Types(Assembly assembly) => assembly
       .GetTypes()
       .Where(a => typeof(FrameworkElement).IsAssignableFrom(a))
       .Where(a => typeof(Application).IsAssignableFrom(a) == false && typeof(Window).IsAssignableFrom(a) == false)
       .GroupBy(type =>
       (type.Name.Contains("UserControl") ? type.Name?.ReplaceLast("UserControl", string.Empty) :
       type.Name.Contains("View") ? type.Name?.ReplaceLast("View", string.Empty) :
       type.Name)!)
       .OrderBy(a => a.Key)
       .ToDictionaryOnIndex()
       .Select(a => new FrameworkElementKeyValue(a.Key, a.Value));
    }

    public class ResourceDictionaryKeyValue : KeyValue
    {
        private readonly Lazy<MasterDetailGrid> lazy;

        public ResourceDictionaryKeyValue(string key, ResourceDictionary resourceDictionary) : base(key)
        {
            ResourceDictionary = resourceDictionary;
            lazy = new(() => new MasterDetailGrid(resourceDictionary.Cast<DictionaryEntry>().Select(a => new DataTemplateKeyValue(a)).ToArray()));
        }


        public ResourceDictionary ResourceDictionary { get; }

        public override FrameworkElement Value => lazy.Value;

        public static IEnumerable<ResourceDictionaryKeyValue> ResourceViewTypes(Assembly assembly) =>
            assembly
            .SelectResourceDictionaries(predicate: entry => Predicate(entry.Key.ToString()), ignoreXamlReaderExceptions: true)
       //.GroupBy(type =>
       //(type.Name.Contains("UserControl") ? type.Name?.ReplaceLast("UserControl", string.Empty) :
       //type.Name.Contains("View") ? type.Name?.ReplaceLast("View", string.Empty) :
       //type.Name)!)

       .OrderBy(a => a.entry.Key)
       //.ToDictionaryOnIndex()
       .Select(a => new ResourceDictionaryKeyValue(a.entry.Key.ToString().Split("/").Last().Remove(".baml"), a.resourceDictionary));

        private static bool Predicate(string key)
        {
            var rKey = key.Remove(".baml");

            foreach (var ignore in new[] { "view", "usercontrol", "app" })
            {
                if (rKey.EndsWith(ignore))
                    return false;
            }
            return true;
        }
    }

    public class DataTemplateKeyValue : KeyValue
    {
        private readonly Lazy<FrameworkElement> lazy;

        public DataTemplateKeyValue(DictionaryEntry entry) : base(entry.Key.ToString() + " " + "(" + entry.Value?.GetType().Name.ToString() + ")")
        {
            Entry = entry;
            lazy = new(() =>
            {
                try
                {
                    return GetFrameworkElement(entry.Value);
                }
                catch (Exception ex)
                {
                    return new TextBlock { Text = ex.Message };
                }
                //else
                //    throw new Exception("sdg33333__d");
            });

            static FrameworkElement GetFrameworkElement(object value)
            {
                switch (value)
                {
                    case DataTemplate dataTemplate:
                        {
                            object? content = null;
                            if (dataTemplate.DataType is Type datatype)
                            {
                                try
                                {
                                    content = new Filler(datatype).Create();
                                }
                                catch (Exception ex)
                                {
                                    content = new AutoMoqer(datatype).Build().Service;
                                }
                            }
                            else
                            {
                                content = new object();
                            }
                            return new ContentControl
                            {
                                ContentTemplate = dataTemplate
                            };
                        }
                    case FrameworkElement frameworkElement:
                        return frameworkElement;
                    case Brush solidColorBrush:
                        {
                            Viewbox viewBox = new();
                            var rect = new Rectangle { Fill = solidColorBrush, Height = 1, Width = 1 };
                            viewBox.Child = rect;
                            return viewBox;
                        }
                    case Geometry geometry:
                        return new Path
                        {
                            Stretch = Stretch.Fill,
                            Stroke = Brushes.Black,
                            StrokeThickness = 1,
                            Data = geometry
                        };
                    case Style
                    { TargetType: var type }
style:
                        {
                            var instance = Activator.CreateInstance(type) as Control;
                            instance.Style = style;
                            return instance;
                        }
                    case IValueConverter converter:
                        {
                            var mb = converter.GetType().GetMethods().First();
                            return new TextBlock { Text = mb.AsString() };
                        }
                    default:
                        throw new Exception($"Unexpected type {value.GetType().Name} in {nameof(DataTemplateKeyValue)}");
                }
            }
        }

        public override FrameworkElement Value => lazy.Value;

        public DictionaryEntry Entry { get; }



        public static IEnumerable<ResourceDictionaryKeyValue> ResourceViewTypes(Assembly assembly) => assembly
        .SelectResourceDictionaries(predicate: entry => Predicate(entry.Key.ToString()), ignoreXamlReaderExceptions: true)
        //.GroupBy(type =>
        //(type.Name.Contains("UserControl") ? type.Name?.ReplaceLast("UserControl", string.Empty) :
        //type.Name.Contains("View") ? type.Name?.ReplaceLast("View", string.Empty) :
        //type.Name)!)

        .OrderBy(a => a.entry.Key)
        //.ToDictionaryOnIndex()
        .Select(a => new ResourceDictionaryKeyValue(a.entry.Key.ToString(), a.resourceDictionary));

        private static bool Predicate(string key)
        {
            var rKey = key.Remove(".baml");

            foreach (var ignore in new[] { "view", "usercontrol", "app" })
            {
                if (rKey.EndsWith(ignore))
                    return false;
            }
            return true;
        }
    }

    internal static class Helper
    {
        public static Dictionary<string, T> ToDictionaryOnIndex<T>(this IEnumerable<IGrouping<string, T>> groupings)
            => groupings
                .SelectMany(grp => grp.Index().ToDictionary(kvp => kvp.Key > 0 ? grp.Key + kvp.Key : grp.Key, c => c.Value))
                .ToDictionary(a => a.Key, a => a.Value);
    }
}
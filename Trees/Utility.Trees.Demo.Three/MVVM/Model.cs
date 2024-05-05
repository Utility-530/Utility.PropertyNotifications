using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using Utility.PropertyNotifications;
using System.Collections;
using Utility.Descriptors;
using Utility.Interfaces.NonGeneric;
using Utility.Trees.Abstractions;
using Views.Trees;
using System.Collections.ObjectModel;

namespace Utility.Trees.Demo.MVVM
{
    public record Model : NotifyProperty
    {
        private Type type;

        public Type Type
        {
            get
            {

                this.RaisePropertyCalled(type);
                return type;

            }
            set
            {
                if (value.Equals(type))
                    return;
                type = value;
                this.RaisePropertyReceived(value);
            }
        }

        public bool IsReadOnly { get; internal set; }

        public class StyleSelector : System.Windows.Controls.StyleSelector
        {
            public override Style SelectStyle(object item, DependencyObject container)
            {
                if (item is TreeViewItem { })
                {
                    var style = App.Current.Resources["ButtonsFlankTreeViewItemStyle"] as Style;
                    return style;
                }
                return base.SelectStyle(item, container);
            }

            //public ResourceDictionary NewTemplates => new()
            //{
            //    Source = new Uri($"/{typeof(CustomStyleSelector).Assembly.GetName().Name};component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
            //};

            public static StyleSelector Instance { get; } = new();
        }


        internal class Filter : ITreeViewFilter
        {
            public bool Convert(object item)
            {
                if (item is IReadOnlyTree { Data: IMethodDescriptor { Type: { } type } })
                {
                    if (type.IsArray)
                    {
                        return false;
                    }
                }


                if (item is IReadOnlyTree { Data: IDescriptor { ParentType: { } componentType, Name: { } displayName } propertyNode })
                {
                    if (componentType.Name == "Array")
                    {
                        if (displayName == "IsFixedSize")
                            return false;
                        if (displayName == "IsReadOnly")
                            return false;
                        if (displayName == "IsSynchronized")
                            return false;
                        if (displayName == "LongLength")
                            return false;
                        if (displayName == "Length")
                            return false;
                        if (displayName == "Rank")
                            return false;
                        if (displayName == "SyncRoot")
                            return false;
                    }
                    if (componentType.Name == "String")
                    {

                        if (displayName == "Length")
                            return false;
                    }

                    if (componentType.IsAssignableTo(typeof(IList)))
                    {
                        if (displayName == nameof(IList.Remove))
                            return false;
                        if (displayName == nameof(IList.GetEnumerator))
                            return false;
                        if (displayName == nameof(IList.CopyTo))
                            return false;
                        if (displayName == nameof(IList.IndexOf))
                            return false;
                        if (displayName == nameof(IList.Contains))
                            return false;
                        if (displayName == nameof(IList.Add))
                            return false;
                        if (displayName == nameof(IList.RemoveAt))
                            return false;
                        if (displayName == nameof(ObservableCollection<object>.Move))
                            return false;
                        if (displayName == nameof(IList.Remove))
                            return false;
                        if (displayName == nameof(IList.Insert))
                            return false;
                    }
                    return true;
                }

                return true;
            }

            public static Filter Instance { get; } = new();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Utility.WPF.Controls.Lists.Infrastructure;
using Utility.WPF.Helpers;
using Utility.WPF.Abstract;

namespace Utility.WPF.Controls.Lists.Infrastructure
{
    public static class CheckBoxesHelper
    {
        public static void Bind(FrameworkElement element, object item, object sender)
        {
            if (sender is not IIsCheckedPath checkedPath ||
                sender is not Selector selector)
            {
                throw new Exception("sdf4 fdgdgp;p;p");
            }

            BindingFactory factory = new(item);
            if (string.IsNullOrEmpty(checkedPath.IsCheckedPath) == false)
                element.SetBinding(ToggleButton.IsCheckedProperty, factory.TwoWay(checkedPath.IsCheckedPath));


            //if (string.IsNullOrEmpty(selector.SelectedValuePath) == false)
            //{
            //    element.SetBinding(FrameworkElement.TagProperty, factory.OneWay(selector.SelectedValuePath));
            //}
            if (string.IsNullOrEmpty(selector.DisplayMemberPath) == false)
            {
                element.SetBinding(FrameworkElement.TagProperty, factory.OneWay(selector.DisplayMemberPath));
            }
            else if(element is ContentControl contentControl && contentControl.ContentTemplateSelector is not null
                && string.IsNullOrEmpty(selector.SelectedValuePath) == false)
            {
                element.SetBinding(FrameworkElement.TagProperty, factory.OneWay(selector.SelectedValuePath));
            }
            else
            {
               throw new Exception($"Expected " +
                    $"{nameof(Selector.DisplayMemberPath)} " +
                    $"not to be null.");
            }
        }

        public static Dictionary<object, bool?> ToDictionary(this Selector selector)
        {
            //if (sender is not System.Windows.Controls.Primitives.Selector selector)
            //{
            //    throw new System.Exception("sdf4 fdgdgp;p;p");
            //}

            //if (string.IsNullOrEmpty(selector.SelectedValuePath) == false ||
            //    string.IsNullOrEmpty(selector.DisplayMemberPath) == false)
            //{
            var items = selector.ItemsOfType<CheckBox>().ToArray();
            var output = items.Where(a => a is { Tag: { } tag }).ToDictionary(a => a.Tag, a => a.IsChecked);
            return output;
            //}
        }
    }
}

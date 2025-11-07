using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Utility.Helpers.NonGeneric;
using Utility.WPF.Abstract;
using Utility.WPF.Helpers;

namespace Utility.WPF.Controls.Lists.Infrastructure
{
    public static class CheckBoxesHelper
    {
        public static void Bind(FrameworkElement element, object item, object sender, DependencyProperty? dependencyProperty = null)
        {
            if (sender is not IIsCheckedPath checkedPath ||
                sender is not Selector selector)
            {
                throw new Exception("sdf4 fdgdgp;p;p");
            }

            dependencyProperty ??= ListBox.DisplayMemberPathProperty;

            if (BindingOperations.GetBinding(element, ToggleButton.IsCheckedProperty) != null)
            {
                return;
            }

            BindingFactory factory = new(item);
            if (string.IsNullOrEmpty(checkedPath.IsCheckedPath) == false)
                element.SetBinding(ToggleButton.IsCheckedProperty, factory.TwoWay(checkedPath.IsCheckedPath));

            //if (string.IsNullOrEmpty(selector.SelectedValuePath) == false)
            //{
            //    element.SetBinding(FrameworkElement.TagProperty, factory.OneWay(selector.SelectedValuePath));
            //}
            if (selector.GetValue(dependencyProperty) is string str)
            {
                element.SetBinding(FrameworkElement.TagProperty, factory.OneWay(str));
            }
            else if (element is ContentControl contentControl && contentControl.ContentTemplateSelector is not null
                && string.IsNullOrEmpty(selector.SelectedValuePath) == false)
            {
                element.SetBinding(FrameworkElement.TagProperty, factory.OneWay(selector.SelectedValuePath));
            }
            else
            {
                //throw new Exception($"Expected " +
                //     $"{nameof(Selector.DisplayMemberPath)} " +
                //     $"not to be null.");
            }
        }

        public static Dictionary<object, bool?> ToDictionary(this Selector selector)
        {
            if (selector is not IIsCheckedPath checkedPath)
            {
                throw new Exception("77 sdf4 fdgdgp;p;p");
            }

            //if (sender is not System.Windows.Controls.Primitives.Selector selector)
            //{
            //    throw new System.Exception("sdf4 fdgdgp;p;p");
            //}

            //if (string.IsNullOrEmpty(selector.SelectedValuePath) == false ||
            //    string.IsNullOrEmpty(selector.DisplayMemberPath) == false)
            //{
            //var items = selector.ItemsOfType<CheckBox>().ToArray();

            Dictionary<object, bool?> output = new();
            int i = 0;
            foreach (var item in selector.Items)
            {
                if (item is CheckBox checkBox)
                {
                    var dataContext = selector.ItemsSource.ElementAt(i++);
                    //item.DataContext = dataContext;
                    //Bind(item, dataContext, selector);

                    output.Add(checkBox.Tag, checkBox.IsChecked);
                }
                else
                {
                    var type = item.GetType();
                    if (string.IsNullOrEmpty(selector.DisplayMemberPath) == false)
                    {
                        var key = type.GetProperty(selector.DisplayMemberPath).GetValue(item);
                        var value = checkedPath.IsCheckedPath != null ? (bool?)type.GetProperty(checkedPath.IsCheckedPath).GetValue(item) : false;

                        try
                        {
                            output.Add(key, value);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
            //var output = items.Where(a => a is { Tag: { } tag }).ToDictionary(a => a.Tag, a => a.IsChecked);
            return output;
            //}
        }
    }
}
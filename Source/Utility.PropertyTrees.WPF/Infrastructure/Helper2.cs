using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utility.PropertyTrees.Abstractions;

namespace Utility.PropertyTrees.WPF.Infrastructure
{
    public static class Helper2
    {
        public static void Update(RoutedEventArgs e)
        {
            //if (e.OriginalSource is ToggleButton button) {
            //   if (button.DataContext is IPropertyGridItem item && item.Property != null && item.Property.IsEnum &&
            //       item.Property.IsFlagsEnum) {
            //      if (button.IsChecked.HasValue) {
            //         ulong itemValue = Helper5.EnumToUInt64(item.Property, item.Value);
            //         ulong propertyValue = Helper5.EnumToUInt64(item.Property, item.Property.Value);
            //         ulong newValue;
            //         if (button.IsChecked.Value) {
            //            if (itemValue == 0) {
            //               newValue = 0;
            //            }
            //            else {
            //               newValue = propertyValue | itemValue;
            //            }
            //         }
            //         else {
            //            newValue = propertyValue & ~itemValue;
            //         }

            //         object propValue = Helper5.EnumToObject(item.Property, newValue);
            //         item.Property.Value = propValue;

            //         if (button.GetVisualSelfOrParent<ListBoxItem>() is ListBoxItem li) {
            //            if (ItemsControl.ItemsControlFromItemContainer(li) is ItemsControl parent) {
            //               if (button.IsChecked == true && itemValue == 0) {
            //                  foreach (IPropertyGridItem gridItem in parent.Items.OfType<IPropertyGridItem>()) {
            //                     gridItem.IsChecked =
            //                        Helper5.EnumToUInt64(item.Property, gridItem.Value) == 0;
            //                  }
            //               }
            //               else {
            //                  foreach (IPropertyGridItem gridItem in parent.Items.OfType<IPropertyGridItem>()) {
            //                     ulong gridItemValue =
            //                        Helper5.EnumToUInt64(item.Property, gridItem.Value);
            //                     if (gridItemValue == 0) {
            //                        gridItem.IsChecked = newValue == 0;
            //                        continue;
            //                     }

            //                     gridItem.IsChecked = (newValue & gridItemValue) == gridItemValue;
            //                  }
            //               }
            //            }
            //         }
            //      }
            //   }
            //}
        }

        public static void OnUIElementPreviewKeyUp(KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Space)
            {
                if (e.OriginalSource is ListBoxItem item)
                {
                    if (item.DataContext is IPropertyGridItem gridItem)
                    {
                        if (gridItem.IsChecked.HasValue)
                        {
                            gridItem.IsChecked = !gridItem.IsChecked.Value;
                        }
                    }
                }
            }
        }

        //public static void ChangeText(ExecutedRoutedEventArgs e) {
        //   if (e.OriginalSource is TextBox tb) {
        //      if (SoftFluent.Windows.PropertyTrees.NewGuidCommand.Equals(e.Command)) {
        //         tb.Text = Guid.NewGuid().ToString(Extensions.NormalizeGuidParameter(e.Parameter));
        //         return;
        //      }

        //      if (SoftFluent.Windows.PropertyTrees.EmptyGuidCommand.Equals(e.Command)) {
        //         tb.Text = Guid.Empty.ToString(Extensions.NormalizeGuidParameter(e.Parameter));
        //         return;
        //      }

        //      if (SoftFluent.Windows.PropertyTrees.IncrementGuidCommand.Equals(e.Command)) {
        //         Guid g = ConversionHelper.ChangeType(tb.Text.Trim(), Guid.Empty);
        //         byte[] bytes = g.ToByteArray();
        //         bytes[15]++;
        //         tb.Text = new Guid(bytes).ToString(Extensions.NormalizeGuidParameter(e.Parameter));
        //         return;
        //      }
        //   }
        //}
    }
}
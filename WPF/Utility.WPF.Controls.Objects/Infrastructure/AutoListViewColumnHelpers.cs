using AutoGenListView.Attributes;
using Humanizer;
using Itenso.Windows.Controls.ListViewLayout;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Utility.Helpers.Reflection;
using Utility.Structs;
using ColumnAttribute = Utility.Attributes.ColumnAttribute;


namespace Utility.WPF.Controls.Objects
{
    public static class AutoListViewColumnHelpers
    {
        public static IEnumerable<GridViewColumn> CreateColumns2(ItemsControl lv, Type type)
        {
            //Type dataType = type ?? lv.ItemsSource.GetType().GetMethod("get_Item").ReturnType;
            //// create the gridview

            //var columns = new List<GridViewColumn>();

            PropertyInfo[] properties = [.. type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)];

            //dataType.GetProperties()
            //            .Where(x => x.GetCustomAttributes(true).FirstOrDefault(y => y is ColVisibleAttribute) != null)
            //            .ToArray();
            // For each appropriately decorated property in the item "type", makea column and bind 
            // the propertty to it
            foreach (PropertyInfo info in properties)
            {
                var x = info.GetCustomAttributes();
                if (x.Any(a => a is JsonIgnoreAttribute or System.Text.Json.Serialization.JsonIgnoreAttribute))
                {
                    continue;
                }
      
                (bool success, ColumnAttribute? dna) = info.GetAttributeSafe<ColumnAttribute>();

                if (success && dna.Ignore == true)
                    continue;

                string displayName = dna?.DisplayName ?? info.Name.Humanize(LetterCasing.Title);


                double width = GetWidthFromAttribute(lv, info, displayName, dna?.Width);
                // If the property is being renamed with the DisplayName attribute, use the new name. 
                // Otherwise use the property's actual name
                //string displayName = info.TryGetAttribute<DisplayNameAttribute>(out var dna) ? dna!.DisplayName : info.Name.Humanize(LetterCasing.Title);

                DataTemplate cellTemplate = createCellTemplate(info.Name, Brushes.Orchid) ?? BuildCellTemplateFromAttribute(info);
                //    ?? Utility.WPF.Factorys.TemplateGenerator.CreateDataTemplate(() =>
                //{
                //    var x = new ContentControl() {  };
                //    x.SetBinding(ContentControl.ContentProperty, new Binding()
                //    {
                //        Path = new PropertyPath("."),
                //        Mode = BindingMode.OneWay
                //    });
                //    return x;
                //});

                //double width = GetWidthFromAttribute(lv, info, displayName);

                // if the cellTemplate is null, create a typical binding object for display 
                // member binding
                Binding binding = (cellTemplate != null) ? null : new Binding() { Path = new PropertyPath(info.Name), Mode = BindingMode.OneWay };

                // create the column, and add it to the gridview
                GridViewColumn column = new()
                {
                    Header = displayName,

                    //DisplayMemberBinding = (cellTemplate == null) ? binding : null,
                    DisplayMemberBinding = binding,
                    CellTemplate = cellTemplate,
                    Width = width,
                };

                //if (success)
                //{
                //    if (dna.MaxWidth != default || dna.MinWidth != default)
                //    {
                //        if (dna.MinWidth != default)
                //            RangeColumn.SetMinWidth(column, dna.MinWidth);
                //        if (dna.MaxWidth != default)
                //            RangeColumn.SetMaxWidth(column, dna.MaxWidth);
                //    }
                //    else
                //        switch (dna.UnitType)
                //        {
                //            case DimensionUnitType.Star:
                //                ProportionalColumn.SetWidth(column, dna.Width);
                //                break;
                //            case DimensionUnitType.Pixel:
                //                FixedColumn.SetWidth(column, dna.Width);
                //                break;
                //            default:
                //                FixedColumn.SetWidth(column, GetWidthFromAttribute(lv, info, displayName));
                //                break;


                //        }
                //}

                yield return column;
            }


        }
        /// <summary>
        /// Creates the columns for all of the properties decorated with the ColVisibleAttribute attribute.
        /// </summary>
        /// <param name="lv"></param>
        public static List<GridViewColumn> CreateColumns(ItemsControl lv)
        {
            if (lv.ItemsSource is not JArray enumerable)
                return new List<GridViewColumn>();

            if (enumerable.Parent?.Parent["$type"]?.Value<string>() is not string str)
                return new List<GridViewColumn>();

            if (Type.GetType(str) is not { } dataType)
                return new List<GridViewColumn>();

            var elementType = TypeHelper.GetElementType(dataType);


            var columns = new List<GridViewColumn>();
            PropertyInfo[] properties = elementType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy).ToArray();

            // For each appropriately decorated property in the item "type", makea column and bind 
            // the propertty to it
            foreach (PropertyInfo info in properties)
            {
                // If the property is being renamed with the DisplayName attribute, use the new name. 
                // Otherwise use the property's actual name
                (bool success, ColumnAttribute? dna) = info.GetAttributeSafe<ColumnAttribute>();

                if (success && dna.Ignore == true)
                    continue;

                string displayName = success ? dna.DisplayName : info.Name.Humanize(LetterCasing.Title);

                //DataTemplate cellTemplate = BuildCellTemplateFromAttribute(info);
                double width = GetWidthFromAttribute(lv, info, displayName, dna?.Width);

                // if the cellTemplate is null, create a typical binding object for display 
                // member binding
                Binding binding = new()
                {
                    Path = new PropertyPath($"[{info.Name}]"),
                    Mode = BindingMode.OneWay
                };

                // create the column, and add it to the gridview
                GridViewColumn column = new()
                {
                    Header = new GridViewColumnHeader { Content = displayName, Width = width },
                    //DisplayMemberBinding = binding,
                    CellTemplate = Utility.WPF.Factorys.TemplateGenerator.CreateDataTemplate(() =>
                    {
                        var x = new ContentControl() { ContentTemplateSelector = JsonObjectTypeTemplateSelector.Instance };
                        x.SetBinding(ContentControl.ContentProperty, binding);
                        return x;
                    }),
                    Width = width,
                };
                columns.Add(column);
            }
            return columns;
        }

        /// <summary>
        /// Determine the width of the column, using the largest of either the calculated width, 
        /// or the decorated width (using ColWidth attribute).
        /// </summary>
        /// <param name="property"></param>
        /// <param name="displayName"></param>
        /// <returns></returns>
        private static double GetWidthFromAttribute(Control listView, PropertyInfo property, string displayName, double? dimension = default)
        {
            // Get the decorated width (if specified)
            //ColWidthAttribute widthAttrib = (ColWidthAttribute)(property.GetCustomAttributes(true).FirstOrDefault(x => x is ColWidthAttribute));
            double width = dimension.HasValue ? dimension.Value : 0d;
            // calc the actual width, and use the larger of the decorated/calculated widths
            width = Math.Max(CalcTextWidth(listView, displayName, listView.FontFamily, listView.FontSize) + 35, width);
            return width;
        }

        /// <summary>
        /// Build the CellTemplate based on the specified ColCelltemplate attribute.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private static DataTemplate BuildCellTemplateFromAttribute(PropertyInfo property)
        {
            // the attriubte is validated when it's defined, so we don't have to worry about it 
            // by the time we get here. Or do we?
            DataTemplate? cellTemplate = null;
            ColCellTemplateAttribute cell = (ColCellTemplateAttribute)(property.GetCustomAttributes(true).FirstOrDefault(x => x is ColCellTemplateAttribute));
            if (cell != null)
            {
                //string xaml = string.Format(_CELL_TEMPLATE_, property.Name,
                //                            cell.FontName, cell.FontSize, cell.FontWeight, cell.FontStyle,
                //                            cell.Foreground, cell.Background,
                //                            cell.HorzAlign, cell.VertAlign);
                //cellTemplate = (DataTemplate)this.XamlReaderLoad(xaml);

                cellTemplate = Utility.WPF.Factorys.TemplateGenerator.CreateDataTemplate(() =>
                {
                    var grid = new Grid
                    {

                    };
                    var rect = new Rectangle() { Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(cell.Background)) };

                    var textBlock = new TextBlock()
                    {
                        FontFamily = new FontFamily(cell.FontName),
                        FontSize = double.Parse(cell.FontSize),
                        FontWeight = Enum.Parse<FontWeight>(cell.FontWeight),
                        FontStyle = Enum.Parse<FontStyle>(cell.FontStyle),
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(cell.Foreground)),
                        HorizontalAlignment = Enum.Parse<HorizontalAlignment>(cell.HorzAlign),
                        VerticalAlignment = Enum.Parse<VerticalAlignment>(cell.VertAlign)
                    };
                    textBlock.SetBinding(TextBlock.TextProperty, new Binding(property.Name) { Mode = BindingMode.OneWay });
                    grid.Children.Add(rect);
                    grid.Children.Add(textBlock);
                    return grid;

                });
            }
            return cellTemplate;
        }

        private static DataTemplate createCellTemplate(string bindingPath, Brush hoverColor)
        {
            var template = new DataTemplate();

            var borderFactory = new FrameworkElementFactory(typeof(Border));
    

            // Define the style for hover effect
            var borderStyle = new Style(typeof(Border));
            borderStyle.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.Transparent));

            var trigger = new Trigger
            {
                Property = UIElement.IsMouseOverProperty,
                Value = true
            };
            trigger.Setters.Add(new Setter(Border.BackgroundProperty, hoverColor));
            borderStyle.Triggers.Add(trigger);

            borderFactory.SetValue(Border.StyleProperty, borderStyle);

            // TextBlock inside the border
            var textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            textBlockFactory.SetBinding(TextBlock.TextProperty, new Binding(bindingPath));
            textBlockFactory.SetValue(TextBlock.FontSizeProperty, 14.0);
            //textBlockFactory.SetValue(TextBlock.PaddingProperty, new Thickness(5));
            textBlockFactory.AddHandler(TextBlock.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Cell_MouseLeftButtonDown));

            // Nest TextBlock in Border
            borderFactory.AppendChild(textBlockFactory);

            template.VisualTree = borderFactory;
            return template;
            static void Cell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                if (sender is TextBlock tb && !string.IsNullOrEmpty(tb.Text))
                {
                    Clipboard.SetText(tb.Text);
                }
            }
        }

        /// <summary>
        ///  Calculates the width of the specified text base on the framework element and the 
        ///  specified font family/size.
        /// </summary>
        /// <param name="fe"></param>
        /// <param name="text"></param>
        /// <param name="family"></param>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        private static double CalcTextWidth(FrameworkElement fe, string text, FontFamily family, double fontSize)
        {
            FormattedText formattedText = new FormattedText(text,
                                                            CultureInfo.CurrentUICulture,
                                                            FlowDirection.LeftToRight,
                                                            new Typeface(family.Source),
                                                            fontSize,
                                                            Brushes.Black,
                                                            VisualTreeHelper.GetDpi(fe).PixelsPerDip);

            return formattedText.WidthIncludingTrailingWhitespace;
        }
    }
}



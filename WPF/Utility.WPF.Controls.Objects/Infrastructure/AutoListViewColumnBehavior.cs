using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using ReactiveUI;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Interactivity;
using AutoGenListView.Attributes;
using System.Windows.Shapes;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Utility.WPF.Templates;
using conv = Utility.WPF.Converters;

namespace Utility.WPF.Controls.Objects
{





    public class AutoListViewColumnBehavior : Behavior<Control>
    {
        // Allows the list view to repopulate when the ItemsSource property value changes




        #region column rendering

        // This string represents the CellTemplate value if the ColFontAttribute is specified on a 
        // property.
        //private readonly string _CELL_TEMPLATE_ = string.Concat("<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" ",
        //                                                         "xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" ",
        //                                                         "x:Key=\"nonPropTemplate\">",
        //                                                         "<Grid>",
        //                                                         "<Rectangle Fill=\"{6}\" Opacity=\"0.6\"/>",
        //                                                         "<TextBlock ",
        //                                                         "FontFamily=\"{1}\" ",
        //                                                         "FontSize=\"{2}\" ",
        //                                                         "FontWeight=\"{3}\" ",
        //                                                         "FontStyle=\"{4}\" ",
        //                                                         "Foreground=\"{5}\" ",
        //                                                         "HorizontalAlignment=\"{7}\" ",
        //                                                         "VerticalAlignment=\"{8}\" ",
        //                                                         ">",
        //                                                         "<TextBlock.Text><Binding Path=\"{0}\" Mode=\"OneWay\" /></TextBlock.Text> ",
        //                                                         "</TextBlock>",
        //                                                         "</Grid>",
        //                                                         "</DataTemplate>");

        public static List<GridViewColumn> CreateColumns2(ItemsControl lv)
        {
            Type dataType = lv.ItemsSource.GetType().GetMethod("get_Item").ReturnType;
            // create the gridview

            var columns = new List<GridViewColumn>();
            PropertyInfo[] properties = dataType.GetProperties()
                                                .Where(x => x.GetCustomAttributes(true)
                                                .FirstOrDefault(y => y is ColVisibleAttribute) != null)
                                                .ToArray();
            // For each appropriately decorated property in the item "type", makea column and bind 
            // the propertty to it
            foreach (PropertyInfo info in properties)
            {
                // If the property is being renamed with the DisplayName attribute, use the new name. 
                // Otherwise use the property's actual name
                DisplayNameAttribute dna = (DisplayNameAttribute)(info.GetCustomAttributes(true).FirstOrDefault(x => x is DisplayNameAttribute));
                string displayName = (dna == null) ? info.Name : dna.DisplayName;

                DataTemplate cellTemplate = BuildCellTemplateFromAttribute(info);
                double width = GetWidthFromAttribute(lv, info, displayName);

                // if the cellTemplate is null, create a typical binding object for display 
                // member binding
                Binding binding = (cellTemplate != null) ? null : new Binding() { Path = new PropertyPath(info.Name), Mode = BindingMode.OneWay };

                // create the column, and add it to the gridview
                GridViewColumn column = new GridViewColumn()
                {
                    Header = displayName,
                    //DisplayMemberBinding = (cellTemplate == null) ? binding : null,
                    DisplayMemberBinding = binding,
                    CellTemplate = cellTemplate,
                    Width = width,
                };
                columns.Add(column);
            }
            return columns;


        }
        /// <summary>
        /// Creates the columns for all of the properties decorated with the ColVisibleAttribute attribute.
        /// </summary>
        /// <param name="lv"></param>
        public static List<GridViewColumn> CreateColumns(ItemsControl lv)
        {

            Type? dataType = null;
            if (lv.ItemsSource is not JArray enumerable)
                return new List<GridViewColumn>();


            dataType = Type.GetType(enumerable.Parent.Parent["$type"].Value<string>());

            if (dataType is null)
                return new List<GridViewColumn>();
            var elementType = Utility.Helpers.TypeHelper.GetElementType(dataType);


            var columns = new List<GridViewColumn>();
            PropertyInfo[] properties = elementType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy).ToArray();

            // For each appropriately decorated property in the item "type", makea column and bind 
            // the propertty to it
            foreach (PropertyInfo info in properties)
            {
                // If the property is being renamed with the DisplayName attribute, use the new name. 
                // Otherwise use the property's actual name
                DisplayNameAttribute dna = (DisplayNameAttribute)(info.GetCustomAttributes(true).FirstOrDefault(x => x is DisplayNameAttribute));
                string displayName = (dna == null) ? info.Name : dna.DisplayName;

                //DataTemplate cellTemplate = BuildCellTemplateFromAttribute(info);
                double width = GetWidthFromAttribute(lv, info, displayName);

                // if the cellTemplate is null, create a typical binding object for display 
                // member binding
                Binding binding = new Binding()
                {
                    Path = new PropertyPath($"[{info.Name}]"), 
                    Mode = BindingMode.OneWay
                };

                // create the column, and add it to the gridview
                GridViewColumn column = new GridViewColumn()
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
        private static double GetWidthFromAttribute(Control listView, PropertyInfo property, string displayName)
        {
            // Get the decorated width (if specified)
            ColWidthAttribute widthAttrib = (ColWidthAttribute)(property.GetCustomAttributes(true).FirstOrDefault(x => x is ColWidthAttribute));
            double width = (widthAttrib != null) ? widthAttrib.Width : 0d;
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


        #endregion column rendering


    }
}



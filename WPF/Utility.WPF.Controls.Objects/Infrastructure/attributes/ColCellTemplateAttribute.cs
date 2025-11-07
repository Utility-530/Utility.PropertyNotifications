using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Media;

namespace AutoGenListView.Attributes
{
    public class ColCellTemplateAttribute : Attribute
    {
        private enum FieldID
        { Name, Size, Style, Weight, Foreground, Background, HorzAlign, VertAlign }

        private enum AttribHorzAlignments
        { Center, Left, Right, Stretch }

        private enum AttribVertAlignments
        { Bottom, Center, Stretch, Top }

        private enum AttribFontWeights
        { Black, Bold, DemiBold, ExtraBlack, ExtraBold, ExtraLight, Heavy, Light, Medium, Normal, Regular, SemiBold, Thin, UltraBlack, UltraBold, UltraLight }

        private enum AttribFontStyles
        { Italic, Normal, Oblique }

        private static List<string> installedFonts = null;
        private const string _DEFAULT_FORMAT_ = "{{Binding RelativeSource={{RelativeSource FindAncestor,AncestorType=ListViewItem}}, Path={0}}}";
        public string FontName { get; set; }
        public string FontSize { get; set; }
        public string FontStyle { get; set; }
        public string FontWeight { get; set; }
        public string Foreground { get; set; }
        public string Background { get; set; }
        public string HorzAlign { get; set; }
        public string VertAlign { get; set; }

        // Passing null (or invalid) values for any of the parameters will cause the attribute to
        // use whatever the default values are.
        public ColCellTemplateAttribute(string name, string size, string style, string weight, string fore, string back, string horz, string vert)
        {
            this.FontName = (!string.IsNullOrEmpty(name)) ? this.Validate(FieldID.Name, name) : string.Format(_DEFAULT_FORMAT_, "FontFamily");
            this.FontSize = (!string.IsNullOrEmpty(size)) ? this.Validate(FieldID.Size, size) : string.Format(_DEFAULT_FORMAT_, "FontSize");
            this.FontStyle = (!string.IsNullOrEmpty(style)) ? this.Validate(FieldID.Style, style) : string.Format(_DEFAULT_FORMAT_, "FontStyle");
            this.FontWeight = (!string.IsNullOrEmpty(weight)) ? this.Validate(FieldID.Weight, weight) : string.Format(_DEFAULT_FORMAT_, "FontWeight");
            this.Foreground = (!string.IsNullOrEmpty(fore)) ? this.Validate(FieldID.Foreground, fore) : string.Format(_DEFAULT_FORMAT_, "Foreground");
            this.Background = (!string.IsNullOrEmpty(back)) ? this.Validate(FieldID.Background, back) : string.Format(_DEFAULT_FORMAT_, "Background");
            this.HorzAlign = (!string.IsNullOrEmpty(horz)) ? this.Validate(FieldID.HorzAlign, horz) : "Center";
            this.VertAlign = (!string.IsNullOrEmpty(vert)) ? this.Validate(FieldID.VertAlign, vert) : "Center";
        }

        private string Validate(FieldID id, string value)
        {
            string result = value;
            switch (id)
            {
                case FieldID.Background:
                    {
                        try
                        {
                            Color color = (Color)(ColorConverter.ConvertFromString(value));
                            result = value;
                        }
                        catch (Exception)
                        {
                            result = string.Format(_DEFAULT_FORMAT_, "Background");
                        }
                    }
                    break;

                case FieldID.Foreground:
                    {
                        try
                        {
                            Color color = (Color)(System.Windows.Media.ColorConverter.ConvertFromString(value));
                            result = value;
                        }
                        catch (Exception)
                        {
                            result = string.Format(_DEFAULT_FORMAT_, "Foreground");
                        }
                    }
                    break;

                case FieldID.HorzAlign:
                    {
                        AttribHorzAlignments align;
                        if (!Enum.TryParse(value, out align))
                        {
                            result = AttribHorzAlignments.Left.ToString();
                        }
                    }
                    break;

                case FieldID.Name:
                    {
                        if (installedFonts == null)
                        {
                            using (InstalledFontCollection fontsCollection = new InstalledFontCollection())
                            {
                                installedFonts = (from x in fontsCollection.Families select x.Name).ToList();
                            }
                        }
                        if (!installedFonts.Contains(value))
                        {
                            result = string.Format(_DEFAULT_FORMAT_, "FontFamily");
                        }
                    }
                    break;

                case FieldID.Size:
                    {
                        double dbl;
                        if (!double.TryParse(value, out dbl))
                        {
                            result = string.Format(_DEFAULT_FORMAT_, "FontSize");
                        }
                    }
                    break;

                case FieldID.Style:
                    {
                        AttribFontWeights enumVal;
                        if (!Enum.TryParse(value, out enumVal))
                        {
                            result = string.Format(_DEFAULT_FORMAT_, "FontStyle");
                        }
                    }
                    break;

                case FieldID.VertAlign:
                    {
                        AttribVertAlignments align;
                        if (!Enum.TryParse(value, out align))
                        {
                            result = AttribVertAlignments.Center.ToString();
                        }
                    }
                    break;

                case FieldID.Weight:
                    {
                        AttribFontWeights weight;
                        if (!Enum.TryParse(value, out weight))
                        {
                            result = string.Format(_DEFAULT_FORMAT_, "FontWeight");
                        }
                    }
                    break;
            }
            return result;
        }
    }
}
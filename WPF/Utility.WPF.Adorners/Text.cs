namespace Utility.WPF.Adorners;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Linq;

public enum Place
{
    Inside, Outside
}

public class Text : Adorner
{
    public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(
        "Text", typeof(string), typeof(Text), new FrameworkPropertyMetadata(string.Empty, OnChanged));

    public static readonly DependencyProperty PositionProperty = DependencyProperty.RegisterAttached(
        "Position", typeof(Dock?), typeof(Text), new FrameworkPropertyMetadata(default, OnChanged));

    public static readonly DependencyProperty PlaceProperty = DependencyProperty.RegisterAttached(
        "Place", typeof(Place?), typeof(Text), new FrameworkPropertyMetadata(default, OnChanged));

    public static readonly DependencyProperty GuidProperty =
        DependencyProperty.Register("Guid", typeof(Guid), typeof(Text));


    public string? text;
    public Dock? position;
    public Place? place;
    private readonly Brush? brush;

    /// <summary>
    ///   Used to avoid calling <see cref="M:System.Windows.UIElement.InvalidateVisual"/> unnecessarily.
    /// </summary>
    private bool isPlaceholderVisible;

    static Text()
    {
        IsHitTestVisibleProperty.OverrideMetadata(typeof(Text), new FrameworkPropertyMetadata(false));
        ClipToBoundsProperty.OverrideMetadata(typeof(Text), new FrameworkPropertyMetadata(true));
    }

    public Text(FrameworkElement adornedElement, string? text = null, Dock? position = null, Place? place = null, Brush? brush = null) : base(adornedElement)
    {
        this.text = text;
        this.position = position;
        this.place = place;
        this.brush = brush;
        //this.SetValue(TextProperty, text);
        //this.SetValue(PositionProperty, position);
        //this.SetValue(PlaceProperty, place);
    }

    public Guid Guid
    {
        get { return (Guid)GetValue(GuidProperty); }
        set { SetValue(GuidProperty, value); }
    }

    #region Event Handlers

    private static void OnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        //if (((string)e.OldValue).Equals((string)e.NewValue, StringComparison.Ordinal))
        //    return;
        if (sender is not FrameworkElement adornedElement)
            throw new Exception("sdf234 dfg f,l,lgd");
        if (adornedElement.IsLoaded)
            AddAdorner(adornedElement, e.NewValue);
        else
            adornedElement.Loaded += AdornedElement_Loaded;

        static void AddAdorner(FrameworkElement adornedElement, object newValue)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);


            bool flag = false;
            foreach (var adorner in adornerLayer.GetAdorners(adornedElement) ?? Array.Empty<Adorner>())
            {
                if (adorner is Text text)
                {
                    flag = true;
                    if (newValue is Dock { } _position)
                    {
                        text.position = _position;
                    }
                    if (newValue is string { } _text)
                    {
                        text.text = _text;
                    }
                    if (newValue is Place { } _place)
                    {
                        text.place = _place;
                    }

                    //text.text = (string?)adornedElement.GetValue(Text.TextProperty);
                    //text.position = (Dock?)adornedElement.GetValue(Text.PositionProperty);
                    //text.place = (Place?)adornedElement.GetValue(Text.PlaceProperty);


                    //adorner
                    //adornerLayer.Add(new Text(adornedElement, 
                    //    (string?)adornedElement.GetValue(Text.TextProperty),
                    //    (Dock?)adornedElement.GetValue(Text.PositionProperty), 
                    //    (Place?)adornedElement.GetValue(Text.PlaceProperty)));
                    //var text1 = adorner.GetValue(Text.TextProperty);
                    //var text2 = adornedElement.GetValue(Text.TextProperty);
                    //if (text1 == text2)
                    //    adornerLayer.Remove(adorner);
                    //if(string.IsNullOrEmpty(text1.ToString()))
                    //{
                    //    //adornerLayer.Remove(adorner);
                    //}


                }
            }
            //InvalidateTextAdorners(adornerLayer, adornedElement);

            if (flag == false)
                adornerLayer.Add(new Text(adornedElement,
                    (string?)adornedElement.GetValue(TextProperty),
                    (Dock?)adornedElement.GetValue(PositionProperty),
                    (Place?)adornedElement.GetValue(PlaceProperty)));
            else
                InvalidateTextAdorners(adornerLayer, adornedElement);

        }

        static void AdornedElement_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement adornedElement = (FrameworkElement)sender;
            //adornedElement.Loaded -= AdornedElement_Loaded;
            if (string.IsNullOrEmpty(GetText(adornedElement)) == false)
                AddAdorner(adornedElement, GetText(adornedElement));
            else
            {
            }
        }
    }

    public static void InvalidateTextAdorners(AdornerLayer adornerLayer, FrameworkElement adornedElement)
    {
        if (adornerLayer == null)
            return;
        Adorner[] adorners = adornerLayer.GetAdorners(adornedElement);
        if (adorners != null)
            foreach (Adorner adorner in adorners)
                if (adorner is Text)
                {
                    adorner.InvalidateVisual();
                    return;
                }
    }

    private void AdornedElement_ContentChanged(object sender, RoutedEventArgs e)
    {
        if (isPlaceholderVisible)
            InvalidateVisual();
    }

    #endregion Event Handlers

    #region Attached Property Getters and Setters
    public static string GetText(UIElement adornedElement)
    {
        if (adornedElement == null)
            throw new ArgumentNullException("adornedElement");
        return (string)adornedElement.GetValue(TextProperty);
    }

    public static void SetText(UIElement adornedElement, string placeholderText)
    {
        if (adornedElement == null)
            throw new ArgumentNullException("adornedElement");
        adornedElement.SetValue(TextProperty, placeholderText);
    }

    public static Dock? GetPosition(UIElement adornedElement)
    {
        if (adornedElement == null)
            throw new ArgumentNullException("adornedElement");
        return (Dock?)adornedElement.GetValue(PositionProperty);
    }

    public static void SetPosition(UIElement adornedElement, Dock? dock)
    {
        if (adornedElement == null)
            throw new ArgumentNullException("adornedElement");
        adornedElement.SetValue(PositionProperty, dock);
    }

    public static Place? GetPlace(UIElement adornedElement)
    {
        if (adornedElement == null)
            throw new ArgumentNullException("adornedElement");
        return (Place?)adornedElement.GetValue(PlaceProperty);
    }

    public static void SetPlace(UIElement adornedElement, Place? dock)
    {
        if (adornedElement == null)
            throw new ArgumentNullException("adornedElement");
        adornedElement.SetValue(PlaceProperty, dock);
    }

    #endregion Attached Property Getters and Setters

    public static void AddAdorner(DependencyObject sender, string sText, List<Guid> guids, Func<FrameworkElement, string, Text> textFactory)
    {
        if (sender is not FrameworkElement adornedElement)
            throw new Exception("s42df234 dfg f,l,lgd");
        if (adornedElement.IsLoaded)
            AddAdorner(adornedElement);
        else
            adornedElement.Loaded += (s, a) => AddAdorner((FrameworkElement)sender);

        void AddAdorner(FrameworkElement adornedElement)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);

            bool flag = false;
            foreach (var adorner in adornerLayer.GetAdorners(adornedElement) ?? Array.Empty<Adorner>())
            {
                if (adorner is Text { Guid: var guid } text)
                {
                    if (guids.Contains(guid) == false)
                        continue;
                    flag = true;
                    text.text = sText;
                }
            }

            if (flag == false)
            {
                var guid = Guid.NewGuid();
                guids.Add(guid);
                //adornerLayer.Add(new Text(adornedElement, (string)e.NewValue, Dock.Left, Place.Outside) { Guid = guid });
                var text = textFactory.Invoke(adornedElement, sText);
                text.Guid = guid;
                adornerLayer.Add(text);
            }
            else
                InvalidateTextAdorners(adornerLayer, adornedElement);
        }
    }


    /// <summary>
    ///   Draws the content of a <see cref="T:System.Windows.Media.DrawingContext"/> object during the render pass of a <see cref="T:Huan.Windows.Documents.Placeholder"/> element.
    /// </summary>
    /// <param name="drawingContext">
    ///   The <see cref="T:System.Windows.Media.DrawingContext" /> object to draw. This context is provided to the layout system.
    /// </param>
    protected override void OnRender(DrawingContext drawingContext)
    {
        /*   (adornedElement.IsFocused&& hideOnFocus) ||*/
        //!this.IsElementEmpty() ||
        switch (place ?? GetPlace(AdornedElement) ?? Place.Inside)
        {
            case Place.Inside:
                OnRenderInside(drawingContext);
                break;

            case Place.Outside:
                OnRenderOutside(drawingContext);
                break;
        }
    }

    private void OnRenderOutside(DrawingContext drawingContext)
    {
        string placeholderText = text ?? (string)AdornedElement.GetValue(TextProperty);

        //bool hideOnFocus = (bool)adornedElement.GetValue(HideOnFocusProperty);
        if (string.IsNullOrEmpty(placeholderText))
        {
            isPlaceholderVisible = false;
            return;
        }

        if (AdornedElement is Control adornedElement)
        {
            isPlaceholderVisible = true;
            var formattedText = FormattedTextForControl(adornedElement, placeholderText);
            double left, top = 0.0;
            switch (position ?? GetPosition(adornedElement) ?? Dock.Left)
            {
                case Dock.Top:
                    {
                        if (adornedElement.FlowDirection == FlowDirection.RightToLeft)
                            left = adornedElement.BorderThickness.Right + adornedElement.Padding.Right + 2.0;
                        else
                            left = adornedElement.BorderThickness.Left + adornedElement.Padding.Left + 2.0;

                        if (adornedElement.FlowDirection == FlowDirection.RightToLeft)
                        {
                            // Somehow everything got drawn reflected. Add a transform to correct.
                            drawingContext.PushTransform(new ScaleTransform(-1.0, 1.0, RenderSize.Width / 2.0, 0.0));
                            drawingContext.DrawText(formattedText, new Point(left, top + -(formattedText?.Height ?? 0) - 2));
                            drawingContext.Pop();
                        }
                        else
                            drawingContext.DrawText(formattedText, new Point(left, top + -(formattedText?.Height ?? 0) - 2));
                        break;
                    }
                case Dock.Bottom:
                    {
                        if (adornedElement.FlowDirection == FlowDirection.RightToLeft)
                            left = adornedElement.BorderThickness.Right + adornedElement.Padding.Right + 2.0;
                        else
                            left = adornedElement.BorderThickness.Left + adornedElement.Padding.Left + 2.0;

                        if (adornedElement.FlowDirection == FlowDirection.RightToLeft)
                        {
                            // Somehow everything got drawn reflected. Add a transform to correct.
                            drawingContext.PushTransform(new ScaleTransform(-1.0, 1.0, RenderSize.Width / 2.0, 0.0));
                            drawingContext.DrawText(formattedText, new Point(left, top + 2 + adornedElement.Height));
                            drawingContext.Pop();
                        }
                        else
                            drawingContext.DrawText(formattedText, new Point(left, top + 2 + adornedElement.Height));
                        break;
                    }

                case Dock.Left:
                    {
                        if (adornedElement.FlowDirection == FlowDirection.RightToLeft)
                        {
                            throw new Exception("safasd Not Supported");
                        }

                        left = -formattedText.Width - adornedElement.BorderThickness.Left - adornedElement.Padding.Left - 2.0;
                        top = HeightDifference(adornedElement, formattedText) / 2d;
                        drawingContext.DrawText(formattedText, new Point(left, top));
                        break;
                    }
                case Dock.Right:
                    {
                        if (adornedElement.FlowDirection == FlowDirection.RightToLeft)
                        {
                            throw new Exception("safasd Not Supported");
                        }

                        left = adornedElement.Width + adornedElement.BorderThickness.Left + adornedElement.Padding.Left + 2.0;
                        top = HeightDifference(adornedElement, formattedText) / 2d;
                        drawingContext.DrawText(formattedText, new Point(left, top));
                        break;
                    }
            }
        }
        else if (AdornedElement is FrameworkElement element)
        {
            isPlaceholderVisible = true;
            var formattedText = FormattedTextForFrameworkElement(placeholderText);
            switch (position ?? GetPosition(element) ?? Dock.Left)
            {
                case Dock.Top:
                    {
                        drawingContext.DrawText(formattedText, new Point(2, -(formattedText?.Height ?? 0) - 2));
                        break;
                    }
                case Dock.Bottom:
                    {
                        drawingContext.DrawText(formattedText, new Point(2, 2 + element.Height));
                        break;
                    }
                case Dock.Left:
                    {
                        drawingContext.DrawText(formattedText, new Point(-formattedText.Width - 2.0, HeightDifference(element, formattedText) / 2d));
                        break;
                    }
                case Dock.Right:
                    {
                        drawingContext.DrawText(formattedText, new Point(element.Width + 2.0, HeightDifference(element, formattedText) / 2d));
                        break;
                    }
            }
        }
    }

    public const int MinimumWidth = 4;

    [Obsolete]
    private FormattedText? FormattedTextForFrameworkElement(string placeholderText)
    {
        TextAlignment computedTextAlignment = ComputedTextAlignment();
        Brush foreground = brush ?? (Brush?)AdornedElement.GetValue(Control.ForegroundProperty) ?? SystemColors.GrayTextBrush.Clone();
        Size size = AdornedElement.RenderSize;
        double maxHeight = size.Height;
        double maxWidth = size.Width - MinimumWidth;
        if (maxHeight <= 0 || maxWidth <= 0)
            return default;

        Typeface typeface = new Typeface(new FontFamily(), FontStyles.Normal, new FontWeight(), new FontStretch());
        return new FormattedText(
           placeholderText,
           CultureInfo.CurrentCulture,
           FlowDirection.LeftToRight,
           typeface,
           12,
           foreground)
        {
            TextAlignment = computedTextAlignment,
            MaxTextHeight = maxHeight,
            MaxTextWidth = maxWidth
        };
    }

    [Obsolete]
    private FormattedText? FormattedTextForControl(Control adornedElement, string placeholderText)
    {
        TextAlignment computedTextAlignment = ComputedTextAlignment();
        Brush foreground = brush ?? (Brush?)AdornedElement.GetValue(Control.ForegroundProperty) ?? GetDefault();
        Size size = adornedElement.RenderSize;
        double maxHeight = size.Height - adornedElement.BorderThickness.Top - adornedElement.BorderThickness.Bottom - adornedElement.Padding.Top - adornedElement.Padding.Bottom;
        double maxWidth = size.Width - adornedElement.BorderThickness.Left - adornedElement.BorderThickness.Right - adornedElement.Padding.Left - adornedElement.Padding.Right - 4.0;
        if (maxHeight <= 0 || maxWidth <= 0)
            return default;

        Typeface typeface = new Typeface(new FontFamily("Consolas"), FontStyles.Normal, adornedElement.FontWeight, adornedElement.FontStretch);
        return new FormattedText(
        placeholderText,
        CultureInfo.CurrentCulture,
        adornedElement.FlowDirection,
        typeface,
        adornedElement.FontSize,
        foreground)
        {
            TextAlignment = computedTextAlignment,
            MaxTextHeight = maxHeight,
            MaxTextWidth = maxWidth
        };

        Brush GetDefault()
        {
            var brush = SystemColors.GrayTextBrush.Clone();
            // Foreground brush does not need to be dynamic. OnRender called when SystemColors changes.
            brush.Opacity = adornedElement.Foreground.Opacity;
            return brush;
        }
    }

    private void OnRenderInside(DrawingContext drawingContext)
    {
        string placeholderText;
        //bool hideOnFocus = (bool)adornedElement.GetValue(HideOnFocusProperty);
        if (string.IsNullOrEmpty(placeholderText = text ?? (string)AdornedElement.GetValue(TextProperty)))
        {
            isPlaceholderVisible = false;
            return;
        }

        if (AdornedElement is Control adornedElement)
        {
            isPlaceholderVisible = true;
            var formattedText = FormattedTextForControl(adornedElement, placeholderText);

            double left, top = 0.0;
            switch (position ?? GetPosition(adornedElement))
            {
                case Dock.Top:
                    {
                        if (adornedElement.FlowDirection == FlowDirection.RightToLeft)
                            left = adornedElement.BorderThickness.Right + adornedElement.Padding.Right + 2.0;
                        else
                            left = adornedElement.BorderThickness.Left + adornedElement.Padding.Left + 2.0;

                        if (adornedElement.FlowDirection == FlowDirection.RightToLeft)
                        {
                            // Somehow everything got drawn reflected. Add a transform to correct.
                            drawingContext.PushTransform(new ScaleTransform(-1.0, 1.0, RenderSize.Width / 2.0, 0.0));
                            drawingContext.DrawText(formattedText, new Point(left, top + 2));
                            drawingContext.Pop();
                        }
                        else
                            drawingContext.DrawText(formattedText, new Point(left, top + 2));
                        break;
                    }
                case Dock.Bottom:
                    {
                        if (adornedElement.FlowDirection == FlowDirection.RightToLeft)
                            left = adornedElement.BorderThickness.Right + adornedElement.Padding.Right + 2.0;
                        else
                            left = adornedElement.BorderThickness.Left + adornedElement.Padding.Left + 2.0;

                        if (formattedText is not null)
                            if (adornedElement.FlowDirection == FlowDirection.RightToLeft)
                            {
                                // Somehow everything got drawn reflected. Add a transform to correct.
                                drawingContext.PushTransform(new ScaleTransform(-1.0, 1.0, RenderSize.Width / 2.0, 0.0));
                                drawingContext.DrawText(formattedText, new Point(left, top + HeightDifference(adornedElement, formattedText)));
                                drawingContext.Pop();
                            }
                            else
                                drawingContext.DrawText(formattedText, new Point(left, top + HeightDifference(adornedElement, formattedText)));
                        break;
                    }

                case Dock.Left:
                    {
                        if (adornedElement.FlowDirection == FlowDirection.RightToLeft)
                        {
                            throw new Exception("safasd Not Supported");
                        }

                        left = 2;
                        top = HeightDifference(adornedElement, formattedText) / 2d;
                        drawingContext.DrawText(formattedText, new Point(left, top));
                        break;
                    }
                case Dock.Right:
                    {
                        if (adornedElement.FlowDirection == FlowDirection.RightToLeft)
                        {
                            throw new Exception("safasd Not Supported");
                        }

                        left = WidthDifference(adornedElement, formattedText) + adornedElement.BorderThickness.Left + adornedElement.Padding.Left - 2.0;
                        top = HeightDifference(adornedElement, formattedText) / 2d;
                        drawingContext.DrawText(formattedText, new Point(left, top));
                        break;
                    }
            }
        }
        else if (AdornedElement is FrameworkElement element)
        {
            double margin = 2;
            isPlaceholderVisible = true;
            if (AdornedElement.RenderSize.Height == 0 || AdornedElement.RenderSize.Width < (double)Text.MinimumWidth)
            {
                return;
            }
            var formattedText = FormattedTextForFrameworkElement(placeholderText);
            if (formattedText == null)
                throw new Exception("t34 dfbvfdghbgfd");
            switch (position ?? GetPosition(element))
            {
                case Dock.Top:
                    {
                        drawingContext.DrawText(formattedText, new Point(margin, margin));
                        break;
                    }
                case Dock.Bottom:
                    {
                        drawingContext.DrawText(formattedText, new Point(margin, HeightDifference(element, formattedText)));
                        break;
                    }
                case Dock.Left:
                    {
                        drawingContext.DrawText(formattedText, new Point(margin, HeightDifference(element, formattedText) / 2d));
                        break;
                    }
                case Dock.Right:
                    {
                        drawingContext.DrawText(formattedText, new Point(WidthDifference(element, formattedText) - margin, HeightDifference(element, formattedText) / 2d));
                        break;
                    }
            }
        }


    }

    double HeightDifference(FrameworkElement element, FormattedText formattedText)
    {
        if (double.IsNaN(element.Height) || element.Height == 0)
        {
            if (double.IsNaN(element.ActualHeight) || element.ActualHeight == 0)
                return formattedText.Height;
            else
                return element.ActualHeight - formattedText.Height;
        }
        return element.Height - formattedText.Height;
    }

    double WidthDifference(FrameworkElement element, FormattedText formattedText)
    {
        if (double.IsNaN(element.Width) || element.Width == 0)
        {
            if (double.IsNaN(element.ActualWidth) || element.ActualWidth == 0)
                return formattedText.Width;
            else
                return element.ActualWidth - formattedText.Width;
        }
        return element.Width - formattedText.Width;
    }
    /// <returns>
    ///   the computed text alignment of the adorned element.
    /// </returns>
    private TextAlignment ComputedTextAlignment()
    {
        return (AdornedElement as Control)?.HorizontalContentAlignment switch
        {
            HorizontalAlignment.Left => TextAlignment.Left,
            HorizontalAlignment.Right => TextAlignment.Right,
            HorizontalAlignment.Center => TextAlignment.Center,
            HorizontalAlignment.Stretch => TextAlignment.Justify,
            _ => TextAlignment.Left,
        };
    }
}

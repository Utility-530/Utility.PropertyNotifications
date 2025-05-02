using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Media;
using System.Globalization;

namespace Leepfrog.WpfFramework.Behaviors
{
    /// <summary>
    /// Behaviour to be applied to Label controls
    /// When label's target is bound to a required field, IsTargetRequired = true
    /// </summary>
    public static class FormattedTextBehavior
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached(
            "Text",
            typeof(string),
            typeof(FormattedTextBehavior),
            new UIPropertyMetadata(string.Empty, OnTextChanged));

        public static string GetText(DependencyObject d)
        {
            return (string)d.GetValue(TextProperty);
        }

        public static void SetText(DependencyObject d, string value)
        {
            d.SetValue(TextProperty, value);
        }
        private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var tb = sender as TextBlock;
            // IF SENDER IS NOT A TEXTBLOCK, JUST EXIT
            if (tb == null)
            {
                return;
            }

            string text = (string)(e.NewValue);

            // CLEAR THE OLD TEXT
            tb.Inlines.Clear();
            tb.Text = string.Empty;
            bool isBold = false;
            bool isUnderline = false;
            bool isItalic = false;
            Brush color = new SolidColorBrush(Colors.Black);
            bool isGrabbingToken = false;
            bool isGrabbingText = false;
            bool isQuoting = false;
            int tokenStart = -1;

            void addCurrentGroup(int i)
            {
                if (
                    (i == 0)
                 || (i == tokenStart)
                   )
                {
                    return;
                }
                tb.Inlines.Add(new Run()
                {
                    Text = text.Substring(tokenStart, i - tokenStart),
                    FontWeight = (isBold || isQuoting) ? FontWeights.Bold : FontWeights.Normal,
                    TextDecorations = isUnderline ? TextDecorations.Underline : null,
                    FontStyle = isItalic ? FontStyles.Italic : FontStyles.Normal,
                    Foreground = color
                }
                );
                isGrabbingText = false;
            }

            // SPLIT THE NEW TEXT INTO GROUPS, SPLITTING BY []
            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] == '“')
                {
                    addCurrentGroup(i);
                    isQuoting = true;
                    tokenStart = i + 1;

                }
                else if (text[i] == '”')
                {
                    addCurrentGroup(i);
                    isQuoting = false;
                    tokenStart = i + 1;
                }
                if (text[i] == '[')
                {
                    // that's the end of the previous group
                    addCurrentGroup(i);

                    // grab the token
                    isGrabbingToken = true;
                    tokenStart = i + 1;
                }
                else if (text[i] == ']')
                {
                    // that's the end of the token
                    isGrabbingToken = false;
                    var token = text.Substring(tokenStart, i - tokenStart);
                    bool val = true;
                    if (token.StartsWith("/"))
                    {
                        val = false;
                        token = token.Substring(1);
                    }
                    switch (token.Substring(0, 1).ToUpper())
                    {
                        case "#":
                            if (val)
                            {
                                if (token == "#NeutralWhite")
                                {
                                    token = "#f4ede7";
                                                            }
                                if ((token.Length == 7) || (token.Length == 9))
                                {
                                    color = new SolidColorBrush((Color)ColorConverter.ConvertFromString(token));
                                }
                            }
                            else
                            {
                                color = new SolidColorBrush(Colors.Black);
                            }

                            break;
                        case "B":
                            isBold = val;
                            break;
                        case "U":
                            isUnderline = val;
                            break;
                        case "I":
                            isItalic = val;
                            break;
                    }
                    tokenStart = i + 1;
                }
                else if (
                    (!isGrabbingToken) && (!isGrabbingText)
                    )
                {
                    isGrabbingText = true;
                    tokenStart = i;
                }
            }
            addCurrentGroup(text.Length);

        }

    }
}

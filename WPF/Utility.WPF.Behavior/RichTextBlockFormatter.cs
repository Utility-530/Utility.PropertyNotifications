// Copyright (c) Microsoft Corporation.  All rights reserved.
using ColorCode;
using ColorCode.Common;
using ColorCode.Parsing;
using ColorCode.Styling;
using SourceChord.FluentWPF;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using Utility.WPF.Behavior.Helper;

namespace Utility.WPF.Behavior
{
    /// <summary>
    /// Creates a <see cref="RichTextBlockFormatter"/>, for rendering Syntax Highlighted code to a RichTextBlock.
    /// Based on
    /// <a href="https://github.com/CommunityToolkit/ColorCode-Universal/blob/master/ColorCode.UWP/RichTextBlockFormatter.cs"></a>
    /// </summary>
    public class RichTextBlockFormatter : CodeColorizerBase
    {
        private InlineCollection inlineCollection;
        private ElementTheme? theme;

        /// <summary>
        /// Creates a <see cref="RichTextBlockFormatter"/>, for rendering Syntax Highlighted code to a RichTextBlock.
        /// </summary>
        /// <param name="Theme">The Theme to use, determines whether to use Default Light or Default Dark.</param>
        public RichTextBlockFormatter(ElementTheme? theme = default, ILanguageParser? languageParser = default) : this(theme == ElementTheme.Dark ? StyleDictionary.DefaultDark : StyleDictionary.DefaultLight, languageParser)
        {
            this.theme = theme;
        }

        /// <summary>
        /// Creates a <see cref="RichTextBlockFormatter"/>, for rendering Syntax Highlighted code to a RichTextBlock.
        /// </summary>
        /// <param name="style">The Custom styles to Apply to the formatted Code.</param>
        /// <param name="languageParser">The language parser that the <see cref="RichTextBlockFormatter"/> instance will use for its lifetime.</param>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public RichTextBlockFormatter(StyleDictionary style, ILanguageParser? languageParser = null) : base(style, languageParser)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        /// <summary>
        /// Adds Syntax Highlighted Source Code to the provided InlineCollection.
        /// </summary>
        /// <param name="sourceCode">The source code to colorize.</param>
        /// <param name="language">The language to use to colorize the source code.</param>
        /// <param name="InlineCollection">InlineCollection to add the Text to.</param>
        public void FormatInlines(string sourceCode, ILanguage Language, InlineCollection inlineCollection)
        {
            this.inlineCollection = inlineCollection;
            languageParser.Parse(sourceCode, Language, (parsedSourceCode, captures) => Write(parsedSourceCode, captures));
        }

        protected override void Write(string parsedSourceCode, IList<Scope> scopes)
        {
            inlineCollection.AddRange(SelectSpans(parsedSourceCode, scopes));
        }

        private IEnumerable<Span> SelectSpans(string parsedSourceCode, IList<Scope> scopes)
        {
            var styleInsertions = new List<TextInsertion>();

            foreach (Scope scope in scopes)
                GetStyleInsertionsForCapturedStyle(scope, styleInsertions);

            styleInsertions.SortStable((x, y) => x.Index.CompareTo(y.Index));

            int offset = 0;

            Scope? PreviousScope = null;

            foreach (var styleinsertion in styleInsertions)
            {
                var text = parsedSourceCode[offset..styleinsertion.Index];
                yield return CreateSpan(text, PreviousScope);
                if (!string.IsNullOrWhiteSpace(styleinsertion.Text))
                {
                    yield return CreateSpan(text, PreviousScope);
                }
                offset = styleinsertion.Index;

                PreviousScope = styleinsertion.Scope;
            }

            var remaining = parsedSourceCode.Substring(offset);
            // Ensures that those loose carriages don't run away!
            if (remaining != "\r")
            {
                yield return CreateSpan(remaining, null);
            }
        }

        private Span CreateSpan(string Text, Scope? scope)
        {
            var span = new Span();
            var run = new Run
            {
                Text = Text
            };

            // Styles and writes the text to the span.
            if (scope != null)
                StyleRun(run, scope.Name);
            span.Inlines.Add(run);

            return span;
        }

        private void StyleRun(Run Run, string scopeName)
        {
            string foreground;
            string background;
            bool bold;
            bool italic;
            if (Styles.Contains(scopeName))
            {
                var style = Styles[scopeName];

                foreground = style.Foreground;
                background = style.Background;
                italic = style.Italic;
                bold = style.Bold;
            }
            else
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(foreground))
                Run.Foreground = foreground.ToColor();

            if (!string.IsNullOrWhiteSpace(background))
                Run.Background = background.ToColor();

            if (italic)
                Run.FontStyle = FontStyles.Italic;

            if (bold)
                Run.FontWeight = FontWeights.Bold;
        }

        private void GetStyleInsertionsForCapturedStyle(Scope scope, ICollection<TextInsertion> styleInsertions)
        {
            styleInsertions.Add(new TextInsertion
            {
                Index = scope.Index,
                Scope = scope
            });

            foreach (Scope childScope in scope.Children)
                GetStyleInsertionsForCapturedStyle(childScope, styleInsertions);

            styleInsertions.Add(new TextInsertion
            {
                Index = scope.Index + scope.Length
            });
        }
    }

    public static class FormatterHelper
    {
        /// <summary>
        /// Adds Syntax Highlighted Source Code to the provided RichTextBlock.
        /// </summary>
        /// <param name="sourceCode">The source code to colorize.</param>
        /// <param name="language">The language to use to colorize the source code.</param>
        /// <param name="RichText">The Control to add the Text to.</param>
        public static Paragraph Format(this System.Windows.Controls.RichTextBox box, string sourceCode, ILanguage? Language = null, RichTextBlockFormatter? formatter = null)
        {
            box.Document.Blocks.Clear();
            var paragraph = new Paragraph();
            box.Document.Blocks.Add(paragraph);
            (formatter ?? new RichTextBlockFormatter()).FormatInlines(sourceCode, (Language ?? Languages.CSharp), paragraph.Inlines);
            return paragraph;
        }
    }
}
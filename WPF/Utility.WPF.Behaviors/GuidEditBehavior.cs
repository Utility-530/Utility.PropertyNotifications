using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;

namespace Utility.WPF.Behaviors
{

    public class GuidEditBehavior : Behavior<TextBox>
    {
        private Brush borderBrush;

        const int guidLength = 36;
        const int guidhexcharacters = 32;
        int caretIndex = guidLength;

        private string _guid = "";

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.TextChanged += AssociatedObject_TextChanged;
            borderBrush = AssociatedObject.BorderBrush;
            AssociatedObject.FontFamily = new System.Windows.Media.FontFamily("Consolas");
            AssociatedObject.Height = 20;
            AssociatedObject.Width = 244;
            _guid = this.AssociatedObject.Text;

            DataObject.AddPastingHandler(AssociatedObject, OnPaste);

            void OnPaste(object sender, DataObjectPastingEventArgs e)
            {
                var isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
                if (!isText) return;

                var text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.TextChanged -= AssociatedObject_TextChanged;
        }


        private void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            HashSet<int> specialOffsets = new HashSet<int> { 8, 13, 18, 23 };
            foreach (var change in e.Changes)
            {
                if (change.AddedLength > 1 && change.RemovedLength == 0)
                {
                    caretIndex = FindLastNonWhitespaceOrHyphenIndex(AssociatedObject.Text) + 1;
                    if (change.Offset == 9 || change.Offset == 14 || change.Offset == 19 || change.Offset == 20)
                    {

                        AssociatedObject.Text = AssociatedObject.Text.Remove(change.Offset - 1, 1);
                    }
                }
                if (change.RemovedLength > 1 && change.AddedLength == 0)
                {
                    if (change.Offset == 9 || change.Offset == 14 || change.Offset == 19 || change.Offset == 24)
                    {
                        caretIndex = FindLastNonWhitespaceOrHyphenIndex(AssociatedObject.Text) + 1;
                        //this.AssociatedObject.Text = format(AssociatedObject.Text);

                    }
                    else
                    {
                        caretIndex = FindLastNonWhitespaceOrHyphenIndex(AssociatedObject.Text) + 1;
                        //this.AssociatedObject.Text = format(AssociatedObject.Text);

                    }

                }
                if (change.RemovedLength == 1 && change.AddedLength == 0)
                {
                    HandleRemovedText(change);
                }
                else if (change.AddedLength > 1 && change.RemovedLength == 0)
                {
                    HandleAddedText(change);
                }
                else if (change.AddedLength == 1 && change.RemovedLength == 0)
                {
                    HandleSingleCharAddition(change);
                }
            }

            FinalizeFormatting();


            void HandleAddedText(TextChange change)
            {
                caretIndex = FindLastNonWhitespaceOrHyphenIndex(AssociatedObject.Text) + 1;
                //AssociatedObject.Text = FormatText(AssociatedObject.Text);
            }

            void HandleSingleCharAddition(TextChange change)
            {
                if (!IsHexadecimal(AssociatedObject.Text[change.Offset]))
                {
                    if (AssociatedObject.CaretIndex > FindLastNonWhitespaceOrHyphenIndex(ExtractNonHexCharactersExcludingHyphens(AssociatedObject.Text)))
                    {
                        caretIndex = FindLastNonWhitespaceOrHyphenIndex(ExtractNonHexCharactersExcludingHyphens(AssociatedObject.Text)) + 1;
                    }
                    else
                        caretIndex = AssociatedObject.CaretIndex;
                }
                else if (specialOffsets.Contains(change.Offset))
                {
                    caretIndex = change.Offset + 2;
                    AssociatedObject.Text = FormatText(AssociatedObject.Text);
                }
                else
                {
                    caretIndex = change.Offset + 1;
                    AssociatedObject.Text = FormatText(AssociatedObject.Text);
                }
            }

            void HandleRemovedText(TextChange change)
            {
                if (specialOffsets.Contains(change.Offset))
                {
                    caretIndex = change.Offset;
                    AssociatedObject.Text = AssociatedObject.Text.Remove(change.Offset - 1, 1) + " -";
                }

                if (change.Offset > 23) caretIndex = change.Offset;
                else if (change.Offset < 23 && change.Offset > 18)
                {
                    AdjustCaretAndInsertSpace(19, 18);
                }
                else if (change.Offset < 18 && change.Offset > 13)
                {
                    AdjustCaretAndInsertSpace(14, 13);
                }
                else if (change.Offset < 13 && change.Offset > 8)
                {
                    AdjustCaretAndInsertSpace(9, 8);
                }
                else if (change.Offset < 8 && change.Offset > -1)
                {
                    caretIndex = change.Offset;
                    AssociatedObject.Text = AssociatedObject.Text.Insert(change.Offset, " ");
                }

                void AdjustCaretAndInsertSpace(int specialOffset, int adjustmentOffset)
                {
                    caretIndex = change.Offset;
                    if (change.Offset == specialOffset)
                        caretIndex = adjustmentOffset;
                    AssociatedObject.Text = AssociatedObject.Text.Insert(change.Offset, " ");
                }

            }

            void FinalizeFormatting()
            {
                _guid = this.AssociatedObject.Text;
                var formattedText = FormatText(trim(ExtractNonHexCharacters(RemoveInternalWhiteSpace(_guid)), guidLength));

                if (AssociatedObject.Text != formattedText)
                    AssociatedObject.Text = formattedText;

                if (caretIndex == 36)
                {
                    caretIndex = FindLastNonWhitespaceOrHyphenIndex(AssociatedObject.Text) + 1;
                }
                AssociatedObject.CaretIndex = caretIndex;

                updateBorderBrush();

                void updateBorderBrush()
                {
                    AssociatedObject.BorderBrush =
                        ExtractNonHexCharacters(AssociatedObject.Text).Length < guidhexcharacters ?
                        Brushes.Red :
                        borderBrush;
                }
            }
        }

        private string trim(string text, int limit)
        {
            if (text.Length > limit)
                return text.Remove(limit, text.Length - limit);
            return text;
        }

        private static string FormatText(string str)
        {
            var input = str.Replace("-", "");
            if (input.Length < guidhexcharacters)
                input = input.PadRight(guidhexcharacters);

            return string.Format("{0}-{1}-{2}-{3}-{4}",
                input.Substring(0, 8),
                input.Substring(8, 4),
                input.Substring(12, 4),
                input.Substring(16, 4),
                input.Substring(20, 12));
        }

        private static int FindLastNonWhitespaceOrHyphenIndex(string input)
        {
            for (int i = input.Length - 1; i >= 0; i--)
            {
                if (input[i] != ' ' && input[i] != '-')
                    return i;
            }
            return -1;
        }

        private static string RemoveInternalWhiteSpace(string input)
        {
            var li = FindLastNonWhitespaceOrHyphenIndex(input);
            for (int i = input.Length - 1; i >= 0; i--)
            {
                if (input[i] == ' ' && i < li)
                {
                    input = input.Remove(i, 1);
                }
            }
            return input;
        }

        private static string ExtractNonHexCharacters(string input)
        {
            StringBuilder result = new StringBuilder();
            foreach (char c in input)
            {
                if (IsHexadecimal(c))
                {
                    result.Append(char.ToLower(c));
                }
            }
            return result.ToString();
        }

        private static string ExtractNonHexCharactersExcludingHyphens(string input)
        {
            StringBuilder result = new StringBuilder();
            foreach (char c in input)
            {
                if (IsHexadecimal(c) || c == '-')
                {
                    result.Append(char.ToLower(c));
                }
            }
            return result.ToString();
        }

        private static int Hyphens(int index)
        {
            int hyphenCount = 0;
            var guid = Guid.Empty.ToString();
            // Ensure index is within bounds of the string
            if (index < 0 || index >= guid.Length)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

            // Iterate through the string from the beginning to the specified index
            for (int i = 0; i <= index; i++)
            {
                if (guid[i] == '-')
                {
                    hyphenCount++;
                }
            }

            return hyphenCount;
        }

        private static bool IsHexadecimal(char c)
        {
            return (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');
        }



    }
}


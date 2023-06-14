using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Behavior
{
    using ColorCode;
    using Evan.Wpf;
    using SourceChord.FluentWPF;
    using System.Windows.Media;

    public class SyntaxHighlightBehavior : Behavior<RichTextBox>
    {
        public static readonly DependencyProperty TextProperty = DependencyHelper.Register<string>(new(PropertyChanged));
        public static readonly DependencyProperty LineHeightProperty = DependencyHelper.Register<double>(new(5d, LineHeightPropertyChanged));
        public static readonly DependencyProperty LanguageProperty = DependencyHelper.Register<ILanguage>(new(Languages.CSharp, PropertyChanged));
        public static readonly DependencyProperty ElementThemeProperty = DependencyHelper.Register<ElementTheme>(new(ElementTheme.Light, PropertyChanged));

        #region properties

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public double LineHeight
        {
            get { return (double)GetValue(LineHeightProperty); }
            set { SetValue(LineHeightProperty, value); }
        }

        public ILanguage Language
        {
            get { return (ILanguage)GetValue(LanguageProperty); }
            set { SetValue(LanguageProperty, value); }
        }

        public ElementTheme ElementTheme
        {
            get { return (ElementTheme)GetValue(ElementThemeProperty); }
            set { SetValue(ElementThemeProperty, value); }
        }

        #endregion properties

        protected override void OnAttached()
        {
            base.OnAttached();
        }

        public static void PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is SyntaxHighlightBehavior { AssociatedObject: { } box } behavior)
                behavior.Update(box);
        }

        private static void LineHeightPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is SyntaxHighlightBehavior { AssociatedObject.Document: { } document })
                document.LineHeight = (double)e.NewValue;
        }

        private void Update(RichTextBox richTextBox)
        {
            var paragraph = richTextBox.Format(Text, Language, new RichTextBlockFormatter(ElementTheme));
            paragraph.Background ??= ElementTheme switch
            {
                ElementTheme.Dark => new SolidColorBrush(Colors.LightGray),
                ElementTheme.Light => new SolidColorBrush(Colors.White),
                ElementTheme.Default => new SolidColorBrush(Colors.White),
                _ => throw new System.NotImplementedException(),
            };
        }
    }
}
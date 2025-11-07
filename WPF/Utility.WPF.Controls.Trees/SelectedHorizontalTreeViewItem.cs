using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Utility.WPF.Controls.Trees
{
    public partial class CustomTreeViewItem
    {
        public static readonly DependencyProperty FooterProperty =
    DependencyProperty.Register("Footer", typeof(object), typeof(CustomTreeViewItem), new PropertyMetadata());

        public static readonly DependencyProperty FooterTemplateProperty =
    DependencyProperty.Register("FooterTemplate", typeof(DataTemplate), typeof(CustomTreeViewItem), new PropertyMetadata());

        public static readonly DependencyProperty FooterTemplateSelectorProperty =
    DependencyProperty.Register("FooterTemplateSelector", typeof(DataTemplateSelector), typeof(CustomTreeViewItem), new PropertyMetadata());

        public object Footer
        {
            get { return (object)GetValue(FooterProperty); }
            set { SetValue(FooterProperty, value); }
        }

        public DataTemplate FooterTemplate
        {
            get { return (DataTemplate)GetValue(FooterTemplateProperty); }
            set { SetValue(FooterTemplateProperty, value); }
        }

        public DataTemplateSelector FooterTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(FooterTemplateSelectorProperty); }
            set { SetValue(FooterTemplateSelectorProperty, value); }
        }

        private void apply_template()
        {
            var acb = this.GetTemplateChild("EditPresenter") as ContentPresenter;
            if (acb == null)
                return;

            acb.AddHandler(Control.PreviewMouseUpEvent, new RoutedEventHandler((s, e) =>
            {
                //if (e.OriginalSource is Button)
                //{
                //    this.IsSelected = true;
                //}
            }));
            acb.AddHandler(TextBox.LostFocusEvent, new RoutedEventHandler((s, e) =>
            {
                if (e.OriginalSource is TextBox)
                    inputBox_LostFocus(s, e);
            }));

            acb.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler((s, e) =>
            {
                if (e.OriginalSource is TextBox)
                    inputBox_GotFocus(s, e);
            }));

            acb.AddHandler(TextBox.LoadedEvent, new RoutedEventHandler((s, e) =>
            {
                inputBox_Loaded(s, e);
            }));
            // PreviewKeyDown, because KeyDown does not bubble up for Enter
        }

        private void inputBox_Loaded(object s, RoutedEventArgs e)
        {
        }

        private void inputBox_GotFocus(object s, RoutedEventArgs e)
        {
            (e.OriginalSource as TextBox).Focusable = true;
            (e.OriginalSource as TextBox).Focus();
            Keyboard.Focus((e.OriginalSource as FrameworkElement));

            (e.OriginalSource as FrameworkElement).PreviewKeyDown += handler;
        }

        private void handler(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Tab:
                case Key.Enter: // accept tag
                                //if (!string.IsNullOrWhiteSpace(Text))
                                //{
                                //    //if (isDuplicate(parent, Text))
                                //    //    break;
                                //    //if (!string.IsNullOrEmpty(valueBeforeEditing))
                                //    //{
                                //    //    parent.ApplyTemplate(this, true);
                                //    //    if (Text != valueBeforeEditing)
                                //    //        parent.RaiseTagEdited(this);
                                //    //}
                                //    //else
                                //    //    parent.ApplyTemplate(this);
                                //    //var SelectedItem = parent.InitializeNewTag(); //creates another tag
                                //    //SelectedItem.IsSelected = true;
                                //}
                                //else

                    this.AcceptComboTreeViewItem_Click(this, default);
                    //(AddCommand as ICommand)?.Execute(null);
                    (this as TreeViewItem).Focus();
                    break;

                case Key.Escape: // reject tag
                                 // isEscapeClicked = true;
                    this.AcceptComboTreeViewItem_Click(this, default);
                    //(AddCommand as ICommand)?.Execute(null);
                    this.Focus();
                    //parent.RemoveTag(this, true); // do not raise RemoveTag event
                    break;

                case Key.Back:
                    //if (string.IsNullOrWhiteSpace(Text))
                    //{
                    //    inputBox_LostFocus(this, new RoutedEventArgs());
                    //    var previousTagIndex = ((IList)parent.ItemsSource).Count - 1;
                    //    if (previousTagIndex < 0) break;

                    //    var previousTag = ((IList)parent.ItemsSource)[previousTagIndex] as TagItem;
                    //    previousTag.Focus();
                    //    previousTag.IsEditing = true;
                    //}
                    break;
            }
        }

        private void inputBox_LostFocus(object s, RoutedEventArgs e)
        {
            this.IsEditing = false;

            (e.OriginalSource as FrameworkElement).PreviewKeyDown -= handler;
        }
    }
}
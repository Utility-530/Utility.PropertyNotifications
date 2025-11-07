using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI;

namespace Utility.WPF.Controls
{
    [TemplatePart(Name = PartListBox, Type = typeof(ListBox))]
    public class HeaderedListBox : HeaderedItemsControl
    {
        public static readonly DependencyProperty ShowCountInHeaderProperty = HeaderedItemsControlEx.ShowCountInHeaderProperty.AddOwner(typeof(HeaderedListBox), new PropertyMetadata(true));

        private const string PartListBox = "PART_ListBox";
        private ListBox ListBoxPart;

        static HeaderedListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HeaderedListBox), new FrameworkPropertyMetadata(typeof(HeaderedListBox)));
        }

        public HeaderedListBox()
        {
            HeaderedItemsControlEx.HeaderChanges(this, this.WhenAnyValue(a => a.ShowCountInHeader))
            .Subscribe(a => Header = a);
        }

        public bool ShowCountInHeader
        {
            get { return (bool)GetValue(ShowCountInHeaderProperty); }
            set { SetValue(ShowCountInHeaderProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.ListBoxPart = GetTemplateChild(PartListBox) as ListBox;

            this.ListBoxPart.SelectionChanged += ListBoxPart_SelectionChanged;
        }

        private void ListBoxPart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Header = e.AddedItems.Cast<object>().First();
        }
    }
}
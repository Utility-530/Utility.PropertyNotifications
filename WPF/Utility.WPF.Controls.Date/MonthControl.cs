using System;
using System.Collections;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Utility.WPF.Reactives;
using Utility.WPF.Helpers;
using Utility.Reactives;

namespace Utility.WPF.Controls.Date;

public class MonthControl : Control
{
    public static readonly DependencyProperty ItemsSourceProperty = ItemsControl.ItemsSourceProperty.AddOwner(typeof(MonthControl), new PropertyMetadata(Changed));
    public static readonly DependencyProperty SelectedItemProperty = ListBox.SelectedItemProperty.AddOwner(typeof(MonthControl), new PropertyMetadata(Changed));
    public static readonly DependencyProperty ValueConverterProperty = DependencyProperty.Register("ValueConverter", typeof(IValueConverter), typeof(MonthControl), new PropertyMetadata(Changed));
    public static readonly RoutedEvent SelectionChangedEvent = ListBox.SelectionChangedEvent.AddOwner(typeof(MonthControl));

    private DayListBox? listBox;

    //static MonthControl()
    //{
    //    DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthGridControl), new FrameworkPropertyMetadata(typeof(MonthGridControl)));
    //}

    #region properties

    public IValueConverter ValueConverter
    {
        get => (IValueConverter)GetValue(ValueConverterProperty);
        set => SetValue(ValueConverterProperty, value);
    }

    public object SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public DateTime? SelectedDate => (DateTime?)SelectedItem;

    public IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public event RoutedEventHandler SelectionChanged
    {
        add => AddHandler(SelectionChangedEvent, value);
        remove => RemoveHandler(SelectionChangedEvent, value);
    }

    #endregion properties

    public override void OnApplyTemplate()
    {
        listBox = this.GetTemplateChild("CalendarDaysListBox") as DayListBox;
        listBox.ValueConverter = ValueConverter;
        listBox.ItemsSource = this.ItemsSource;
        listBox.SelectedItem = this.SelectedItem;
        listBox.SelectionChanged += ListBox_SelectionChanged;
        base.OnApplyTemplate();
    }

    private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is MonthControl { listBox: { } listBox, } control)
        {
            listBox.ValueConverter = control.ValueConverter;
            listBox.ItemsSource = control.ItemsSource;
            listBox.SelectedItem = control.SelectedItem;
        }
    }

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox)
        {
            this.SelectedItem = listBox.SelectedItem;
        }
    }
}

/// <summary>
/// Override <see cref="PrepareContainerForItemOverride(DependencyObject, object)"/>
/// and sets the content of the <see cref="DayControl"/> with <see cref="ValueConverter"/>
/// </summary>
public class DayListBox : ListBox
{
    public static readonly DependencyProperty ValueConverterProperty =
        DependencyProperty.Register("ValueConverter", typeof(IValueConverter), typeof(DayListBox), new PropertyMetadata());

    protected override sealed void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
        if (element is not ListBoxItem t)
            throw new System.Exception("s fdsd  77ffs");
        t.ApplyTemplate();
        var dayControl = t.ChildOfType<DayControl>();
        _ = this.Observe(a => a.ValueConverter)
            .WhereIsNotNull()
            .Take(1)
            .Subscribe(a => dayControl.Content = a.Convert(item, null, null, null));
        base.PrepareContainerForItemOverride(element, item);
    }

    public IValueConverter ValueConverter
    {
        get => (IValueConverter)GetValue(ValueConverterProperty);
        set => SetValue(ValueConverterProperty, value);
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
    }
}
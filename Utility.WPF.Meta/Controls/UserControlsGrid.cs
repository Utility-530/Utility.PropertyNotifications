using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.WPF.Helpers;

namespace Utility.WPF.Meta;

/// <summary>
/// Master-Detail control for <see cref="UserControlsGrid"/> in <seet cref="UserControlsGrid.Assembly"></set>
/// </summary>
public class MasterDetailGrid : Grid
{
    public MasterDetailGrid(ICollection<KeyValue> enumerable)
    {
        var viewListBox = new ViewListBox() { ItemsSource = enumerable };
        ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Auto) });
        ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Star) });
        Children.Add(viewListBox);
        Children.Add(ControlFactory.CreateMainGrid(viewListBox));
    }
}

/// <summary>
/// Master-Detail control for <see cref="UserControlsGrid"/> in <seet cref="UserControlsGrid.Assembly"></set>
/// </summary>
public class UserControlsGrid : MasterDetailGrid
{

    public UserControlsGrid(Assembly? assembly = null) :
        base((assembly ?? Assembly.GetEntryAssembly()).ViewTypes().ToArray())
    {

        Resources.Add(new DataTemplateKey(
        typeof(ResourceDictionaryKeyValue)),
        TemplateGenerator.CreateDataTemplate(() =>
        {
            var textBlock = new TextBlock
            {
                MinHeight = 20,
                FontSize = 14,
                MinWidth = 100
            };
            Binding binding = new()
            {
                Path = new PropertyPath(nameof(KeyValue.Key)),
            };
            textBlock.SetBinding(TextBlock.TextProperty, binding);
            return textBlock;
        }));


    }
}

public class ResourceDictionariesGrid : MasterDetailGrid
{
    public static readonly DependencyProperty AssemblyProperty =
        DependencyProperty.Register("Assembly", typeof(Assembly), typeof(ResourceDictionariesGrid), new PropertyMetadata(Assembly.GetEntryAssembly()));


    public ResourceDictionariesGrid(Assembly? assembly = null) :
        base(ResourceDictionaryKeyValue.ResourceViewTypes(assembly ?? Assembly.GetEntryAssembly()).ToArray())
    {
    }

    public Assembly Assembly
    {
        get { return (Assembly)GetValue(AssemblyProperty); }
        set { SetValue(AssemblyProperty, value); }
    }
}

internal class ViewListBox : ListBox
{
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
        InitialiseItem(element as ListBoxItem);
        base.PrepareContainerForItemOverride(element, item);

        static void InitialiseItem(ListBoxItem? item)
        {
            item.SetBinding(ContentControl.ContentProperty, new Binding
            {
                Path = new PropertyPath(nameof(FrameworkElementKeyValue.Key)),
            });
        }
    }
}

internal static class ControlFactory
{
    public static Grid CreateMainGrid(ListBox listBox)
    {
        var grid = new Grid();
        Grid.SetColumn(grid, 1);
        int i = 0;
        foreach (var (item, gut) in CreateContent(CreateBinding(listBox)))
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.0, gut) });
            grid.Children.Add(item);
            Grid.SetRow(item, i);
            i++;
        }
        return grid;

        static Binding CreateBinding(ListBox listBox)
        {
            return new Binding
            {
                Path = new PropertyPath(nameof(ListBox.SelectedItem)),
                Source = listBox,
            };
        }

        static IEnumerable<(FrameworkElement, GridUnitType)> CreateContent(Binding selectedItemBinding)
        {
            yield return (CreateTextBlock(selectedItemBinding), GridUnitType.Auto);
            yield return (CreateContentControl(selectedItemBinding), GridUnitType.Star);

            static TextBlock CreateTextBlock(Binding selectedItemBinding)
            {
                TextBlock textBlock = new()
                {
                    Margin = new Thickness(20),
                    FontSize = 20
                };
                _ = textBlock.SetBinding(TextBlock.TextProperty, new Binding()
                {
                    Path = new PropertyPath(nameof(FrameworkElementKeyValue.Key)),
                });
                _ = textBlock.SetBinding(FrameworkElement.DataContextProperty, selectedItemBinding);
                return textBlock;
            }

            static ContentControl CreateContentControl(Binding selectedItemBinding)
            {
                ContentControl contentControl = new() { Content = "Empty" };
                _ = contentControl.SetBinding(ContentControl.ContentProperty, new Binding
                {
                    Path = new PropertyPath(nameof(KeyValue.Value)),
                }); ;
                _ = contentControl.SetBinding(FrameworkElement.DataContextProperty, selectedItemBinding);
                return contentControl;
            }
        }
    }
}

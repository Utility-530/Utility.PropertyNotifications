using SourceChord.FluentWPF;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Utility.Helpers;
using Utility.Infrastructure;
using Utility.Models;
using Utility.WPF.Adorners.Infrastructure;
using Utility.WPF.Helpers;
using static Utility.Observables.NonGeneric.ObservableExtensions;

namespace Utility.PropertyTrees.WPF
{
    [Flags]
    public enum ElementVisibility
    {
        Never = 0,
        OnInitialised = 1,
        OnHover = 2,
        OnClick = 4,
        Always = 8,
    }

    public class MethodsBuilder : BaseObject
    {
        static readonly ItemsPanelTemplate horizontalTemplate = TemplateGenerator.CreateItemsPanelTemplate<WrapPanel>(factory =>
        {
            factory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
        });

        public record AdornerParentRelationShip(UIElement Child, TreeViewItem Parent);

        private readonly Dictionary<MethodNode, AdornerParentRelationShip> dictionary = new();
        private readonly Dictionary<TreeViewItem, ItemsControl> treeDictionary = new();

        private TreeViewItem? hover;
        private TreeViewItem? click;
        private TreeViewItem? initialised;

        public Dictionary<string, ElementVisibility> visibilityDictionary = new() {
            { "Add", ElementVisibility.Always },
            { "Remove", ElementVisibility.OnHover| ElementVisibility.OnClick },
            { "MoveDown", ElementVisibility.OnHover| ElementVisibility.OnClick },
            { "MoveUp", ElementVisibility.OnHover| ElementVisibility.OnClick },
            { "Clear", ElementVisibility.Always},
            { "Send", ElementVisibility.Always },
            { "Connect", ElementVisibility.Always },
            { "Foo", ElementVisibility.OnHover| ElementVisibility.OnClick },
            { "Bar", ElementVisibility.Always },
            { "AddByType", ElementVisibility.Always },
            { "AddByName", ElementVisibility.Always },
            { "AddByKey", ElementVisibility.Always },
            { "Refresh", ElementVisibility.Always },
            { "Update", ElementVisibility.Always },

        };

        public override Key Key => new(Guids.MethodBuilder, nameof(MethodsBuilder), typeof(MethodsBuilder));

        public void OnNext(RefreshRequest request)
        {
            dictionary.Clear();
        }

        public void OnNext(OnHoverChange change)
        {

            if (change is { Source: var _hover })
            {
                if (hover != _hover)
                {
                    hover = _hover as TreeViewItem;
                    if (change is { Source: TreeViewItem treeViewItem, Node: PropertyBase property })
                    {
                        AddMethods(treeViewItem, property, ElementVisibility.OnHover);
                    }


                    Refresh();
                }
            }
        }

        public void OnNext(SelectionChange change)
        {
            if (change is { Source: TreeViewItem treeViewItem })
            {
                if (click != treeViewItem)
                {
                    click = treeViewItem;
                    if (change is { Node: PropertyBase property })
                        AddMethods(treeViewItem, property, ElementVisibility.OnClick);

                    Refresh();
                }
            }

        }

        public void OnNext(TreeViewItemInitialised change)
        {

            if (change is { Source: TreeViewItem treeViewItem })
            {
                if (initialised != treeViewItem)
                {
                    initialised = treeViewItem;
                    if (change is { Node: PropertyBase property })
                        AddMethods(treeViewItem, property, ElementVisibility.OnInitialised);
                    Refresh();
                }
            }
        }

        public void AddMethods(TreeViewItem treeViewItem, PropertyBase property, ElementVisibility visibility)
        {
            var itemsControl = treeDictionary.GetValueOrCreate(treeViewItem, () =>
            {
                var _itemsControl = new ItemsControl()
                {
                    ItemsPanel = horizontalTemplate,
                    Background = Brushes.Transparent,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(2, 2, 2, 2)
                };

          
                return _itemsControl;
            });

            Utility.WPF.Adorners.Infrastructure.AdornerHelper.AddIfMissingAdorner(treeViewItem, itemsControl);
            treeViewItem.SetValue(AdornerEx.IsEnabledProperty, true);

            property
                .Methods
                .Subscribe(item =>
                {
                    if (item is not NotifyCollectionChangedEventArgs args)
                        throw new Exception("rev re");


                    foreach (MethodNode methodNode in args.NewItems)
                    {

                        dictionary.GetValueOrCreate(methodNode, () =>
                        {
                            Border border = new() { Background = Brushes.White, BorderBrush = Brushes.Black };
                            Button button = new()
                            {
                                Content = methodNode.Content,
                                Command = methodNode.Command,
                                Background = Brushes.White,
                            };
                            border.Child = button;
                            itemsControl.Items.Add(border);

                            return new(border, treeViewItem);
                        });
                        //}
                        //if (visibilityDictionary.TryGetValue(methodNode.MethodInfo.Name, out var elementVisibility))
                        //{
                        //    bool isMatch = IsMatch(treeViewItem, elementVisibility);
                        //    dictionary[methodNode].Child.SetValue(UIElement.VisibilityProperty,
                        //        isMatch ?
                        //        Visibility.Visible :
                        //        Visibility.Collapsed);
                        //}
                        //else
                        //    throw new Exception("sdfdfs 3333");
                    }
                });
        }

        void Refresh()
        {

            foreach (var item in dictionary)
            {
                if (visibilityDictionary.TryGetValue(item.Key.MethodInfo.Name, out var elementVisibility))
                {
                    bool isMatch = IsMatch(item.Value.Parent, elementVisibility);
                    //item.Value.Parent.SetValue(AdornerEx.IsEnabledProperty, isMatch);


                    item.Value.Child.SetValue(UIElement.VisibilityProperty,
                        isMatch ?
                        Visibility.Visible :
                        Visibility.Collapsed);
                }
                else
                    throw new Exception("sdfdfs 3333");
            }
        }

        bool IsMatch(TreeViewItem treeViewItem, ElementVisibility visibility)
        {
            if (visibility.Equals(ElementVisibility.Always))
                return true;
            if (visibility.Equals(ElementVisibility.Never))
                return false;
            var flags = visibility.SeparateFlags();

            bool visible = false;
            foreach (var _visibility in new[] { ElementVisibility.OnClick, ElementVisibility.OnHover, ElementVisibility.OnInitialised })
                switch (_visibility)
                {
                    case ElementVisibility.OnClick:
                        visible |= Enumerable.Contains(flags, ElementVisibility.OnClick) && treeViewItem == click;
                        break;
                    case ElementVisibility.OnHover:
                        visible |= Enumerable.Contains(flags, ElementVisibility.OnHover) && treeViewItem == hover;
                        break;
                    case ElementVisibility.OnInitialised:
                        visible |= Enumerable.Contains(flags, ElementVisibility.OnInitialised) && treeViewItem == initialised;
                        break;
                    default:
                        throw new Exception("sdc s32111");
                }
            return visible;
        }
    }
}




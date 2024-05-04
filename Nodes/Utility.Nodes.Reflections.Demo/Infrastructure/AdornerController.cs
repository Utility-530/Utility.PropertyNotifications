using Utility.Extensions;
using Utility.WPF.Adorners.Infrastructure;
using Utility.WPF.Animations.Helpers;
using static Utility.Observables.NonGeneric.ObservableExtensions;

namespace Utility.Nodes.Reflections.Demo.Infrastructure
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

    public class AdornerController
    {
        const string Add = nameof(Add);
        const string Remove = nameof(Remove);
        const string Duplicate = nameof(Duplicate);

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

        public Dictionary<string, Condition> visibilityDictionary = new() {
            { Duplicate, new(ElementVisibility.OnHover | ElementVisibility.OnClick, (a,b)=>{

                return
                a is { Header: IReadOnlyTree { } tree } &&
                b is { Header: IReadOnlyTree { } _tree } &&
                _tree.MatchAncestor(c=> c == tree) is not null;
            })},
            { Remove,new (ElementVisibility.OnHover | ElementVisibility.OnClick, (a,b)=>{

                         return
                a is { Header: IReadOnlyTree { } tree } &&
                b is { Header: IReadOnlyTree { } _tree } &&
                _tree.MatchAncestor(c=> c== tree) is not null;

            })},
            { Add, new(ElementVisibility.OnHover | ElementVisibility.OnClick, (a,b)=> {

                        return
                a is { Header: IReadOnlyTree { } tree } &&
                b is { Header: IReadOnlyTree { } _tree } &&
                _tree.MatchAncestor(c=> c== tree) is not null;
            }) },
            //{ "MoveDown", ElementVisibility.OnHover| ElementVisibility.OnClick },
            //{ "MoveUp", ElementVisibility.OnHover| ElementVisibility.OnClick },
            //{ "Clear", ElementVisibility.Always},
            //{ "Send", ElementVisibility.Always },
            //{ "Connect", ElementVisibility.Always },
            //{ "Foo", ElementVisibility.OnHover| ElementVisibility.OnClick },
            //{ "Bar", ElementVisibility.Always },
            //{ "AddByType", ElementVisibility.Always },
            //{ "AddByName", ElementVisibility.Always },
            //{ "AddByKey", ElementVisibility.Always },
            //{ "Refresh", ElementVisibility.Always },
            //{ "Update", ElementVisibility.Always },
        };

        private static IEnumerable<MethodNode> Methods(IReadOnlyTree tree)
        {
            if (tree is { Data: ICollectionItemDescriptor _ })
            {
                yield return new MethodNode { Name = Duplicate, Content = Duplicate, Command = new Commands.Command(() => ActionController.Instance.Duplicate(tree)) };
                yield return new MethodNode { Name = Remove, Content = Remove, Command = new Commands.Command(() => ActionController.Instance.Remove(tree)) };
            }
            if (tree is { Data: ICollectionDescriptor _ })
            {
                yield return new MethodNode { Name = Add, Content = Add, Command = new Commands.Command(() => ActionController.Instance.Add(tree)) };
            }
        }

        public record Condition(ElementVisibility ElementVisibility, Func<TreeViewItem, TreeViewItem, bool> Predicate)
        {
        }

        public void OnNext(RefreshRequest request)
        {
            dictionary.Clear();
        }

        public void OnNext(OnHoverChange change)
        {

            if (change is { Source: TreeViewItem treeViewItem })
            {
                if (hover != treeViewItem)
                {
                    hover = treeViewItem;
                    if (change is { Node: IReadOnlyTree { } node })
                        AddMethods(treeViewItem, node);

                    Refresh();
                }
            }
        }

        public void OnNext(ClickChange change)
        {
            if (change is { Source: TreeViewItem treeViewItem })
            {
                if (click != treeViewItem)
                {
                    click = treeViewItem;
                    if (change is { Node: IReadOnlyTree { } node })
                        AddMethods(treeViewItem, node);

                    Refresh();
                }
            }
        }

        public void OnNext(OnLoadedChange change)
        {
            if (change is { Source: TreeViewItem treeViewItem })
            {
                if (initialised != treeViewItem)
                {
                    initialised = treeViewItem;
                    if (change is { Node: IReadOnlyTree { } node })
                        AddMethods(treeViewItem, node);
                    Refresh();
                }
            }
        }


        public void AddMethods(TreeViewItem treeViewItem, IReadOnlyTree node)
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

            if (treeViewItem.AddIfMissingAdorner(itemsControl))
            {
                //treeViewItem.Set(AdornerEx.IsEnabledProperty, true);
                Methods(node)
                .ToObservable()
                .Subscribe(methodNode =>
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
                });
            }
        }


        public class MethodNode
        {
            public string Name { get; set; }
            public object Content { get; set; }
            public ICommand Command { get; set; }

        }

        void Refresh()
        {
            foreach (var item in dictionary)
            {
                if (visibilityDictionary.TryGetValue(item.Key.Name, out var condition))
                {
                    bool isMatch = IsMatch(item.Value.Parent, condition.ElementVisibility, condition.Predicate);
                    //item.Value.Parent.Set(AdornerEx.IsEnabledProperty, isMatch);

                    if (isMatch)
                    {
                        item.Value.Child.IsHitTestVisible = true;
                        item.Value.Child.FadeIn();
                    }
                    else
                    {
                        item.Value.Child.IsHitTestVisible = false;
                        item.Value.Child.FadeOut();
                    }
                    //item.Value.Child.Set(UIElement.VisibilityProperty,
                    //    isMatch ?
                    //    Visibility.Visible :
                    //    Visibility.Collapsed);
                }
                else
                    throw new Exception("sdfdfs 3333");
            }
        }

        bool IsMatch(TreeViewItem treeViewItem, ElementVisibility visibility, Func<TreeViewItem, TreeViewItem, bool> predicate)
        {
            if (visibility.Equals(ElementVisibility.Always))
                return true;
            if (visibility.Equals(ElementVisibility.Never))
                return false;
            var flags = visibility.SeparateFlags();

            bool visible = false;
            foreach (var _visibility in new[] { ElementVisibility.OnClick, ElementVisibility.OnHover, ElementVisibility.OnInitialised })
                visible |= _visibility switch
                {
                    ElementVisibility.OnClick => flags.Contains(ElementVisibility.OnClick) && predicate(treeViewItem, click),
                    ElementVisibility.OnHover => flags.Contains(ElementVisibility.OnHover) && predicate(treeViewItem, hover),
                    ElementVisibility.OnInitialised => flags.Contains(ElementVisibility.OnInitialised) && predicate(treeViewItem, initialised),
                    _ => throw new Exception("sdc s32111"),
                };
            return visible;
        }

        public IDisposable Subscribe(IObserver<IEvent> observer)
        {
            throw new NotImplementedException();
        }

        public static AdornerController Instance { get; } = new();
    }
}




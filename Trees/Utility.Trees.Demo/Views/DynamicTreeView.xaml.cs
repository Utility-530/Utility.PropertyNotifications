using Jellyfish;
using System;
using System.Windows;
using System.Windows.Controls;
using Utility.Enums;
using Utility.Helpers;
using Utility.Trees.Abstractions;
using static Evan.Wpf.DependencyHelper;

namespace Utility.Trees.Demo
{
    /// <summary>
    /// Interaction logic for TreeView.xaml
    /// </summary>
    public partial class DynamicTreeView : UserControl
    {
        readonly static DependencyProperty
            TreeProperty = Register(new PropertyMetadata(TreeChanged)),
            DataProperty = Register(new PropertyMetadata(DataChanged));

        private static void DataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DynamicTreeView dynamicTreeView && e.NewValue is not null)
            {
                dynamicTreeView.Tree.Data = e.NewValue;
            }
        }

        private static void TreeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DynamicTreeView dynamicTreeView && e.NewValue is IDynamicTree tree)
            {
                //dynamicTreeView.ItemsControl.ItemsSource = tree.Items;
                dynamicTreeView.TreeView3.ItemsSource = tree.Items;

                dynamicTreeView.DirectionButtons.Enabled = Direction.None;
                dynamicTreeView.EditCollectionButtons.Enabled = AddRemove.None;

                tree
                    .Subscribe(a =>
                    {
                        dynamicTreeView.Change(tree.State);
                    });
            }
        }


        public DynamicTreeView()
        {
            InitializeComponent();

            DirectionButtons.Command = new RelayCommand<Direction>((a) =>
            {
                switch (a)
                {
                    case Direction.Left:
                        Tree.State = State.Back;
                        break;
                    case Direction.Right:
                        Tree.State = State.Forward;
                        break;
                    case Direction.Up:
                        Tree.State = State.Up;
                        break;
                    case Direction.Down:
                        Tree.State = State.Down;
                        break;
                }

            });

            EditCollectionButtons.Command = new RelayCommand<AddRemove>((a) =>
            {
                switch (a)
                {
                    case AddRemove.Add:
                        Tree.State = State.Add;
                        break;
                    case AddRemove.Remove:
                        Tree.State = State.Remove;
                        break;
                }
            });


            PersistenceButtons.Command = new RelayCommand<Persistence>((a) =>
            {
                switch (a)
                {
                    case Persistence.Load:
                        var load = TreePersist.Instance.Load<Persist>();
                        Tree.Tree = load;                
                        break;
                    case Persistence.Save:
                        TreePersist.Instance.Save(Tree.Tree as Tree<Persist>);
                        break;
                }
            });
        }


        private void Change(State state)
        {
            Direction direction = Direction.None;
            AddRemove addRemove = AddRemove.None;
            foreach (var flag in EnumHelper.SeparateFlag(state))
            {
                if (flag == State.Add)
                {
                    addRemove |= AddRemove.Add;
                }
                if (flag == State.Remove)
                {
                    addRemove |= AddRemove.Remove;

                }
                if (flag == State.Back)
                {
                    direction |= Direction.Left;
                }
                if (flag == State.Forward)
                {
                    direction |= Direction.Right;

                }
                if (flag == State.Up)
                {
                    direction |= Direction.Up;

                }
                if (flag == State.Down)
                {
                    direction |= Direction.Down;

                }
            }

            DirectionButtons.Enabled = direction;
            EditCollectionButtons.Enabled = addRemove;
        }

        public IDynamicTree Tree
        {
            get { return (IDynamicTree)GetValue(TreeProperty); }
            set { SetValue(TreeProperty, value); }
        }

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }

        }
    }
}

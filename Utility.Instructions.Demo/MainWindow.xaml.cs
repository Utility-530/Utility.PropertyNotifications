using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Reactive.Linq;
using System.Collections.Generic;
using Utility.Trees;
using System.Collections;
using System.Text.Json;

namespace Utility.Instructions.Demo
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz";

        public static char AlphabetCharacter(int index)
        {
            return chars[index % 26];
        }
        int count = -1;

        public string character => AlphabetCharacter(count).ToString();

        readonly Random random = new Random();
        readonly DynamicTree<object> instructions;
        readonly DynamicTree<TreeState> history;
        readonly Service service;
        //public Implementer implementor = new();
        //private readonly View View;



        //public Tree<View> ViewModelTree { get; } = new(new View { Value = "root" });
        readonly Dictionary<Guid, View> views = new();
        readonly View root;
        bool historyFlag;
        //ObservableCollection<View> currentViews = new();
        public MainWindow()
        {
            InitializeComponent();
            instructions = new("root");
            history = new(instructions.TreeState());

            service = new(instructions.Tree.Key);

            root = views[service.Root.Key] = service.Root.CloneTree() as View;
            root.IsExpanded = true;

            //this.TreeView.ItemsSource = implementor.Tree.Items;

            instructions
                .Subscribe(state =>
                {
                    int index = 0;

                    Update(state, index);
                    if (historyFlag == false)
                    {

                        history.Data = state;
                        history.Add();
                    }
                });

            history
                .Subscribe(state =>
                {
                    if (historyFlag == true)
                    {
                        instructions.OnNext(state);
                    }
                });

            TreeView2.ItemsSource = instructions.Tree.Items;
            TreeView3.ItemsSource = service.Root.Items;
            TreeView4.ItemsSource = root.Items;
            TreeView5.ItemsSource = history.Tree.Items;
        }

        private void Update(TreeState state, int index)
        {
            UpButton.IsEnabled = state.Up != null;
            DownButton.IsEnabled = state.Down != null;
            ForwardButton.IsEnabled = state.Forward != null;
            BackButton.IsEnabled = state.Back != null;
            // AddButton.IsEnabled = state.Add != null;
            RemoveButton.IsEnabled = state.Remove != null;

            foreach (var x in views.Values)
            {
                x.State = State.Default;
            }
            foreach (var item in new[] { state.Current, state.Up, state.Down, state.Forward, state.Back, state.Add })
            {
                if (item == null)
                    continue;
                if (views.ContainsKey(item.Key) == false)
                {
                    var view = new View(service, item.Data, item.Key) { Parent = views[item.Parent.Key] };
                    views.Add(item.Key, view);
                }
                var cView = views[item.Key];
                cView.State = GetState(cView, state);
                //currentViews.Add(cView);

            }

            foreach (var view in views)
            {
                service.OnNext(new Change<View, Key>(view.Value, new Key(view.Value.Parent?.Key ?? default), new Key(view.Key), index, ChangeType.Update));
            }

            State GetState(ITree tree, TreeState state)
            {
                if (tree.Equals(state.Current))
                    return State.Current;
                else if (tree.Equals(state.Forward))
                    return State.Forward;
                else if (tree.Equals(state.Back))
                    return State.Back;
                else if (tree.Equals(state.Up))
                    return State.Up;
                else if (tree.Equals(state.Down))
                    return State.Down;
                //else if (tree.Equals(state.Add))
                //    return State.Add;

                //else if (tree.Equals(state.Remove))
                //    return State.Remove;
                return State.Default;
            }
        }




        private void Add_Click(object sender, RoutedEventArgs e)
        {
            //if (instructions.CanAdd == false)

            instructions.Data = Randoms.Names.Random(random);
            instructions.Add();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            instructions.Remove();
        }



        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        //private void RemoveHistory_Click(object sender, RoutedEventArgs e)
        //{
        //    instructions.RemoveFuture();
        //}

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            historyFlag = false;

            if (instructions.CanMoveUp == false)
                throw new Exception("V3 fds");
            instructions.MoveUp();
            //foreach (var instruction in _instructions)
            //    implementor.OnPrevious(instruction);
        }

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            historyFlag = false;

            if (instructions.CanMoveForward == false)
                throw new Exception("V3 fds");
            instructions.MoveForward();
            //implementor.OnNext(instruction);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            historyFlag = false;

            if (instructions.CanMoveBack == false)
                throw new Exception("V3 fds");
            instructions.MoveBack();
            //implementor.OnPrevious(instruction);
        }

        private void Down_Click(object sender, RoutedEventArgs e)
        {
            historyFlag = false;

            if (instructions.CanMoveDown == false)
                throw new Exception("V3 fds");
            instructions.MoveDown();
            //implementor.OnPrevious(instruction);
        }


        private void Next_Click(object sender, RoutedEventArgs e)
        {
            historyFlag = false;
            if (history.CanMoveDown == false)
                throw new Exception("V3 fds");

            var current = history.Current;
            instructions.OnNext(current.Data);

        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {


        }




        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            this.TreeView.ItemTemplateSelector = null;
            this.TreeView.ItemContainerStyleSelector = null;
            this.TreeView.ItemContainerStyle = null;
            this.TreeView.ItemTemplateSelector = MyDataTemplateSelector.Instance;
            this.TreeView.ItemContainerStyleSelector = MyStyleSelector.Instance;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //var seriealisation = JsonSerializer.Serialize(implementor.Tree);
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {

        }

        #region History


        private void UpHistory_Click(object sender, RoutedEventArgs e)
        {
            historyFlag = true;
            if (history.CanMoveUp == false)
                throw new Exception("V3 fds");

            history.MoveUp();
            //var current = history.Current;

            //instructions.OnPrevious(current.Data);
        }

        private void DownHistory_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ForwardHistory_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BackHistory_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion History
    }

}

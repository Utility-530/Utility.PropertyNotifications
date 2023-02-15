using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utility.Instructions.Demo.Infrastructure;
using System.Reactive.Linq;
using System.Collections.Generic;
using Utility.Trees;
using System.Collections;

namespace Utility.Instructions.Demo
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly Random random = new Random();
        readonly DynamicTree<Instruction> instructions;
        readonly Service service;
        public Implementer implementor = new();
        private readonly View View;

        public ObservableCollection<TreeState> StateCollection { get; } = new();

        //public Tree<View> ViewModelTree { get; } = new(new View { Value = "root" });
        readonly Dictionary<Guid, View> views = new();
        readonly View root;
        ObservableCollection<View> currentViews = new();
        public MainWindow()
        {
            InitializeComponent();
            instructions = new(new Instruction() { Value = "root" });
            service = new(instructions.Tree.Key);

            root = views[service.Root.Key] = service.Root.CloneTree() as View;
            root.IsExpanded = true;
            this.TreeView.ItemsSource = implementor.Tree.Items;
            TreeView2.ItemsSource = instructions.Tree.Items;
            TreeView3.ItemsSource = service.Root.Items;
            TreeView4.ItemsSource = root.Items;
            Instructions.ItemsSource = StateCollection;
            ProceduresListBox.ItemsSource = currentViews;

            instructions.Subscribe(state =>
            {
                int index = 0;
                List<View> currentViews = new();

                Update(state, index);

                if (state.CurrentBranch.Contains(state.Current) == true)
                    ProceduresListBox.SelectedItem = state.Current;
                else
                    ProceduresListBox.SelectedItem = null;


                StateCollection.Add(state);
            });
        }

        private void Update(TreeState state, int index)
        {
            currentViews.Clear();
            foreach (var item in state.CurrentBranch.Items)
            {
                if (views.ContainsKey(item.Key) == false)
                {
                    var view = new View(service, item.Data, item.Key) { };
                    views.Add(item.Key, view);
                }
                var cView = views[item.Key];
                cView.State = GetState(item, state);
                currentViews.Add(cView);
                service.OnNext(new Change<View, Key>(cView, new Key(state.CurrentBranch.Key), new Key(item.Key), index, ChangeType.Update));
             }

            State GetState(ITree tree, TreeState state)
            {
                if (tree == state.Current)
                    return State.Current;
                else if (tree == state.Forward)
                    return State.Forward;
                else if (tree == state.Back)
                    return State.Back;
                else if (tree == state.Up)
                    return State.Up;
                return State.Default;
            }
        }




        private void ContentChange_Click(object sender, RoutedEventArgs e)
        {
            instructions.Add(new Instruction { Type = InstructionType.ChangeContent });
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            instructions.Add(new Instruction { Type = InstructionType.InsertLast, Value = Randoms.Names.Random(random) });
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (instructions.Last()?.Data is Instruction { Type: InstructionType.InsertLast, Value: var value } instruction)
                instructions.Add(new Instruction { Type = InstructionType.RemoveLast, Value = value });
        }



        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        private void RemoveHistory_Click(object sender, RoutedEventArgs e)
        {
            instructions.RemoveFuture();
        }

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            if (instructions.MoveUp() is not IEnumerable<Instruction> _instructions)
                throw new Exception("V3 fds");

            foreach (var instruction in _instructions)
                implementor.OnPrevious(instruction);
        }

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            if (instructions.MoveForward() is not Instruction instruction)
                throw new Exception("V3 fds");

            implementor.OnNext(instruction);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (instructions.MoveBack() is not Instruction instruction)
                throw new Exception("V3 fds");

            implementor.OnPrevious(instruction);
        }


        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (instructions.MoveNext() is not Instruction instruction)
                throw new Exception("V3 fds");

            implementor.OnNext(instruction);
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (instructions.MovePrevious() is not Instruction instruction)
                throw new Exception("V3 fds");

            implementor.OnPrevious(instruction);
        }




        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            TreeView.ItemTemplateSelector = null;
            TreeView.ItemContainerStyleSelector = null;
            TreeView.ItemContainerStyle = null;
            TreeView.ItemTemplateSelector = MyDataTemplateSelector.Instance;
            TreeView.ItemContainerStyleSelector = MyStyleSelector.Instance;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

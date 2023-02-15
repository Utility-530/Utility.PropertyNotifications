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
        readonly Procedures procedures = new();
        readonly Service service;
        public Implementer implementor = new();
        private readonly View View;

        public ObservableCollection<ProceduresState> StateCollection { get; } = new();

        //public Tree<View> ViewModelTree { get; } = new(new View { Value = "root" });
        readonly Dictionary<Guid, View> views = new();
        readonly View root;
        ObservableCollection<View> currentViews = new();
        public MainWindow()
        {
            InitializeComponent();
            service = new(procedures.Tree.Key);

            root = views[service.Root.Key] = service.Root.CloneTree() as View;
            root.IsExpanded = true;
            this.TreeView.ItemsSource = implementor.Tree.Items;
            TreeView2.ItemsSource = procedures.Tree.Items;
            TreeView3.ItemsSource = service.Root.Items;
            TreeView4.ItemsSource = root.Items;
            Instructions.ItemsSource = StateCollection;
            ProceduresListBox.ItemsSource = currentViews;

            procedures.Subscribe(state =>
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

        private void Update(ProceduresState state, int index)
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

            State GetState(ITree<Instruction> tree, ProceduresState state)
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
            procedures.Add(new Instruction { Type = InstructionType.ChangeContent });
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            procedures.Add(new Instruction { Type = InstructionType.InsertLast, Value = Randoms.Names.Random(random) });
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (procedures.Last() is Instruction { Type: InstructionType.InsertLast, Value: var value } instruction)
                procedures.Add(new Instruction { Type = InstructionType.RemoveLast, Value = value });
        }



        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        private void RemoveHistory_Click(object sender, RoutedEventArgs e)
        {
            procedures.RemoveFuture();
        }

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            if (procedures.MoveUp() is not IEnumerable<Instruction> instructions)
                throw new Exception("V3 fds");

            foreach (var instruction in instructions)
                implementor.OnPrevious(instruction);
        }

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            if (procedures.MoveForward() is not Instruction instruction)
                throw new Exception("V3 fds");

            implementor.OnNext(instruction);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (procedures.MoveBack() is not Instruction instruction)
                throw new Exception("V3 fds");

            implementor.OnPrevious(instruction);
        }


        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (procedures.MoveNext() is not Instruction instruction)
                throw new Exception("V3 fds");

            implementor.OnNext(instruction);
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (procedures.MovePrevious() is not Instruction instruction)
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
    }
}

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utility.Instructions.Demo.Infrastructure;
using System.Reactive.Linq;
using System.Collections.Generic;
using Utility.Trees;

namespace Utility.Instructions.Demo
{
    public enum State
    {
        Default, Current, Forward, Back, Up,
    }

    public class View
    {
        public State State { get; set; }

        public object Value { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random random = new Random();
        Procedures procedures = new();

        public Implementer implementor = new();
        public ObservableCollection<Instruction> InstructionCollection { get; } = new();
        //public ObservableCollection<Instruction> InstructionCollection { get; } = new();





        public MainWindow()
        {
            InitializeComponent();
            this.TreeView.ItemsSource = implementor.Tree.Items;
            Instructions.ItemsSource = InstructionCollection;

            TreeView2.ItemsSource = procedures.Tree.Items;
            procedures.Subscribe(state =>
            {
                var views = state.Items.Select(a=> new View { Value= a, State= GetState(a, state) }).ToArray();

                if (ProceduresListBox.ItemsSource?.OfType<object>().Contains(state.Current) == true)
                    ProceduresListBox.SelectedItem = state.Current;
                else
                    ProceduresListBox.SelectedItem = null;

                ProceduresListBox.ItemsSource = views;

                InstructionCollection.Add(state.Current?.Data as Instruction);
            });
        }

        private State GetState(ITree<Instruction> tree, ProceduresState state)
        {
            if (tree == state.Current)
                return State.Current;       
            else if (tree == state.Forward)
                return State.Forward;
            else if(tree == state.Back)
                return State.Back;
            else if(tree == state.Up)
                return State.Up;
            return State.Default;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            //if(procedures.MoveForward() == false)
            //    throw new Exception("V3 fds");

            //implementor.OnNext(procedures.Current.Data as Instruction);
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            //if (procedures.MoveBack() == false)
            //    throw new Exception("V3 fds");

            //implementor.OnPrevious(procedures.Forward.Data as Instruction);
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




        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            TreeView.ItemTemplateSelector = null;
            TreeView.ItemContainerStyleSelector = null;
            TreeView.ItemContainerStyle = null;
            TreeView.ItemTemplateSelector = MyDataTemplateSelector.Instance;
            TreeView.ItemContainerStyleSelector = MyStyleSelector.Instance;
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
    }
}

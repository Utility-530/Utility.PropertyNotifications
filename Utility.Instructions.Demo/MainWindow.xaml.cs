using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utility.Instructions.Demo.Infrastructure;

namespace Utility.Instructions.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Generator generator = new();
        public Implementer implementor = new();
        public ObservableCollection<Instruction> InstructionCollection { get; } = new();
        public ObservableCollection<Procedure> ProcedureCollection { get; } = new();


        //Procedure procedure = new () { Type = ProcedureType.InsertLast, Value = 0 };

        Procedures procedures = new();


        int count = 0;


        public MainWindow()
        {
            InitializeComponent();
            this.TreeView.ItemsSource = implementor.Tree.Items;
            Instructions.ItemsSource = InstructionCollection;
            ProceduresListBox.ItemsSource = procedures.PriorProcedures;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if(procedures.MoveNext() == false)
                throw new Exception("V3 fds");

            var procedure = procedures.Current;
            ProceduresListBox.SelectedItem = procedure;
            var instruction = generator.OnNext(procedure);
            InstructionCollection.Add(instruction);
            procedure = implementor.OnNext(instruction);
            //ProcedureCollection.Add(procedure);
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (procedures.MovePrevious() == false)
                throw new Exception("V3 fds");
            var procedure = procedures.Current;
            ProceduresListBox.SelectedItem = procedure;
            var instruction = generator.OnPrevious(procedure);
            InstructionCollection.Add(instruction);
            procedure = implementor.OnNext(instruction);
            //ProcedureCollection.Add(procedure);
        }


        public class Procedures : IEnumerator<Procedure>
        {
            public ObservableCollection<Procedure> PriorProcedures { get; } = new ObservableCollection<Procedure>
            {

            };
            int count = 0;

            Procedure current;

            public Procedure Current => current;

            object IEnumerator.Current => this.Current;


            public bool MoveNext()
            {
                if (count == PriorProcedures.Count)
                    return false;
        
                current = PriorProcedures[count];

                count++;

                return true;
            }

            public bool MovePrevious()
            {
                if (count == 0)
                    return false;

                count--;
                current = PriorProcedures[count];
                return true;
            }

            public void Reset()
            {
                count = 0;
            }
            public void Dispose()
            {
                throw new System.NotImplementedException();
            }

            public void RemoveHistory()
            {
                while (PriorProcedures.Count > count)
                    PriorProcedures.Remove(PriorProcedures[^1]);
            }
        }

        private void ContentChange_Click(object sender, RoutedEventArgs e)
        {
            procedures.RemoveHistory();
            procedures.PriorProcedures.Add(new Procedure { Type = ProcedureType.ChangeContent });
            Next_Click(default, default);

        }

        System.Random random = new System.Random();



        private void Add_Click(object sender, RoutedEventArgs e)
        {
            procedures.RemoveHistory();
            procedures.PriorProcedures.Add(new Procedure { Type = ProcedureType.InsertLast, Value = Randoms.Names.Random(random) });
            Next_Click(default, default);
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            procedures.RemoveHistory();
            var last = procedures.PriorProcedures.Last(a => a.Type == ProcedureType.InsertLast).Value;
            procedures.PriorProcedures.Add(new Procedure { Type = ProcedureType.RemoveLast, Value = last });
            Next_Click(default, default);
        }

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }
    }
}

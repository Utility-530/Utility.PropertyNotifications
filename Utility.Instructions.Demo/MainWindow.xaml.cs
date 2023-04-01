using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        DynamicTree<Persist> instructions;
        readonly DynamicTree<ITree<Persist>> history;
        readonly Service service;
        readonly Dictionary<Guid, View> views = new();

        public MainWindow()
        {
            InitializeComponent();
            instructions = new(new Tree<Persist>(new Persist() { Name = "root" }) { });
            history = new(new Tree<ITree<Persist>>(instructions.Current));
            instructions
                .Subscribe(state =>
                {
                    int index = 0;

                   // Update(state, index);
                    if (state.State != State.Default)
                    {

                        history.Data = state;
                        history.State = State.Add;
                    }
                });

            history
                .Subscribe(state =>
                {
                    if (state.State != State.Add)
                    {
                        instructions.OnNext(state.Data);
                    }
                });

            TreeView3.ItemsSource = instructions.Tree.Items; // service.Root.Items;

            TreeView5.ItemsSource = history.Tree.Items;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            instructions.Data = new Persist { Name = TextBoxContent.Text };
            instructions.State = State.Add;
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            instructions.State = State.Remove;
        }

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            instructions.State = State.Up;
        }

        private void Forward_Click(object sender, RoutedEventArgs e)
        {

            instructions.State = State.Forward;

        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            instructions.State = State.Back;

        }

        private void Down_Click(object sender, RoutedEventArgs e)
        {

            instructions.State = State.Down;

        }


        private void Next_Click(object sender, RoutedEventArgs e)
        {
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
            TreePersist.Instance.Save(instructions.Tree);
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var load = TreePersist.Instance.Load<Persist>();
            instructions = new(load);
            TreeView2.ItemsSource = instructions.Tree.Items;
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {

        }

        #region History


        private void UpHistory_Click(object sender, RoutedEventArgs e)
        {
            history.State = State.Up;
        }

        private void DownHistory_Click(object sender, RoutedEventArgs e)
        {

            history.State = State.Down;
        }

        private void ForwardHistory_Click(object sender, RoutedEventArgs e)
        {

            history.State = State.Forward;
        }

        private void BackHistory_Click(object sender, RoutedEventArgs e)
        {

            history.State = State.Back;
        }

        #endregion History
    }

}

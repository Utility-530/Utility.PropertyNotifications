using System;
using System.Windows;
using Utility.Trees.Abstractions;
using Utility.Trees.Demo.Infrastructure;
using Utility.WPF.Reactive;

namespace Utility.Trees.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
           
            //var bootStrapper = new Bootstrapper();

            //history = new();
            //tree
            //    .Subscribe(tree =>
            //    {
            //        //int index = 0;

            //        //if (history.Items.Count == 0)
            //        //{
            //        //    history.Current = new Tree(tree);
            //        //}
            //        //else if (tree != history.Current.Data)
            //        //{
            //        //    history.Data = tree as ITree<Persist>;
            //        //    history.State = State.Down;
            //        //}
            //    });

            //history
            //    .Subscribe(tree =>
            //    {
            //        if (tree.State != State.Add)
            //        {
            //            this.tree.Current = tree.Data as ITree<Persist>;
            //        }
            //    });

            //ContentControl1.Content = tree;
            //ContentControl2.Content = history;

            //DataItems.ItemsSource = bootStrapper.ResolveMany<Persist>();

            //DataItems.SelectItemChanges<Persist>()
            //    .Subscribe(a =>
            //    {
            //        tree.Current.Add( a);
            //    });

        }


    }

    public class PersistTree : Tree<Persist>
    {
        public PersistTree(Persist data, params object[] items) : base(data, items)
        {
        }

        protected override object Clone(object data)
        {
            if (data is Persist persist)
            {
                return new Persist { Guid = Guid.NewGuid() };
            }
            throw new Exception("FDSGF fddfgfd");
        }
    }

}

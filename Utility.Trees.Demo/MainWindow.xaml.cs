using System;
using System.Windows;
using Utility.Trees;

namespace Utility.Trees.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DynamicTree Trees;
        readonly DynamicTree history;

        public MainWindow()
        {
            InitializeComponent();
            Trees = new(new Tree<Persist>(new Persist() { Name = "root" }) { });
            history = new();
            Trees
                .Subscribe(tree =>
                {
                    int index = 0;

                    // Update(state, index);
                    if (history.Items.Count == 0)
                    {
                        history.Current = new Tree(tree);
                    }
                    else if(tree != history.Current.Data)
                    {
                        history.Data = tree as ITree<Persist>;
                        history.State = State.Down;
                    }
                });

            history
                .Subscribe(tree =>
                {
                    if (tree.State != State.Add)
                    {
                        Trees.Current = tree.Data as ITree<Persist>;
                    }
                });

            ContentControl1.Content = Trees;
            ContentControl2.Content = history;

        }


    }

}

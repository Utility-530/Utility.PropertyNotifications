using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Linq;
using ReactiveUI;
using System.Windows;
using System.Text;
using System;
using System.Windows.Controls.Primitives;

namespace Utility.View
{
    /// <summary>
    /// Interaction logic for CollectionView.xaml
    /// </summary>
    public partial class CollectionView
    {
        public CollectionView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                TitleTextBlock.Text = ViewModel.Name;

               
                 MainToggleButton.Events().Checked.Select(a => true)
                .Merge(MainToggleButton.Events().Unchecked.Select(a => false))
                .StartWith(default(bool))
                .Subscribe(a =>
                {
                    CollectionItemsControl.ItemsSource = a ? this.ViewModel.CollectionAll : this.ViewModel.CollectionTop;
                });
           
                var remaining = this.ViewModel
                .ObserveOnDispatcher()
                .Select(args =>
                {
                    return this.ViewModel.CollectionAll.Count - this.ViewModel.CollectionTop.Count;
                });

                _ = ViewModel
                .Where(a => a != null)
                .Scan((new StringBuilder(), 0), (stringBuilder, args) =>
                  {
                      if (stringBuilder.Item2++ > 5)
                          RemoveFirstLine(stringBuilder.Item1);

                      foreach (var ni in args)
                          stringBuilder.Item1.AppendLine(ni.ToString());

                      return (stringBuilder.Item1, stringBuilder.Item2);
                  })
                .Select(a => a.Item1.ToString())
                .BindTo(this, a => a.InformationTextBlock.Text);


                _ = remaining
                    .BindTo(this.RemainingTextBlock, a => a.Text)
                    .DisposeWith(disposable);

                remaining
                .Select(a => a > 0 ? Visibility.Visible : Visibility.Collapsed)
                .BindTo(this.RemainingGrid, a => a.Visibility)
                .DisposeWith(disposable);

                _ = this.OneWayBind(this.ViewModel, vm => vm.CollectionAll.Count, v => v.CountTextBlock.Text)
                    .DisposeWith(disposable);
            });
        }

        void RemoveFirstLine(StringBuilder lines)
        {
            var firstLine = lines.ToString().IndexOf(Environment.NewLine, StringComparison.Ordinal);

            if (firstLine >= 0)
                lines.Remove(0, firstLine + Environment.NewLine.Length);
        }
    }
}


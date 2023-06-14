using BrowseHistory;
using BrowserHistoryDemoLib.ViewModels;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using Utility.FileSystem.Transfer.Demo.ViewModel;

namespace Utility.FileSystem.Transfer.Demo
{
    /// <summary>
    /// Interaction logic for FileSelectorUserControl.xaml
    /// </summary>
    public partial class FileSelectorUserControl : UserControl
    {
        public FileSelectorUserControl()
        {
            InitializeComponent();


            (this.DataContext as FileSelectorViewModel)
                .SuggestViewModel
                .WhenAnyValue(a => a.DirectorySuggestion)
                .Subscribe(a =>
            {
                try
                {
                    if (a != null && a.Path != null)
                        Breadcrumb.AddLast(a);
                }
                catch (Exception ex)
                {

                }
            });
        }
    }
}

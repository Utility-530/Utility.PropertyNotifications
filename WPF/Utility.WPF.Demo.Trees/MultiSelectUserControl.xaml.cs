using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Utility.PropertyNotifications;

namespace Utility.WPF.Demo.Trees
{
    /// <summary>
    /// Interaction logic for MultiSelectUserControl.xaml
    /// </summary>
    public partial class MultiSelectUserControl : UserControl
    {
        public MultiSelectUserControl()
        {
            InitializeComponent();
        }
    }

    public class MainViewModel
    {
        public IList<FolderViewModel> RootFolders { get; private set; }
        public IList<FolderViewModel> SelectedFolders { get; private set; }

        public MainViewModel()
        {
            RootFolders = new ObservableCollection<FolderViewModel>();
            SelectedFolders = new ObservableCollection<FolderViewModel>();
            Fill(RootFolders, 1, 5);
        }

        private void Fill(IList<FolderViewModel> folders, int currentDepth, int count)
        {
            for (int i = 0; i < count; i++)
            {
                FolderViewModel folder = new FolderViewModel("Folder: " + currentDepth + " #" + (i + 1)) { IsExpanded = true };
                Fill(folder.Children, currentDepth + 1, count - 1);
                folders.Add(folder);
            }
        }
    }


    public class FolderViewModel : NotifyPropertyClass
    {
        public IList<FolderViewModel> Children { get; private set; }

        private readonly string _name;
        public string Name
        {
            get { return _name + " (" + GetHashCode() + ")"; }
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { this.RaisePropertyChanged(ref _isExpanded, value); }
        }

        public FolderViewModel(string name)
        {
            _name = name;
            Children = new ObservableCollection<FolderViewModel>();
        }
    }
}

using System.Windows;
using System.Windows.Input;

namespace PropertyTrees.WPF.Resources
{
    public partial class MoreDataTemplatesResourceDictionary
    {
        private void OnEditorWindowCloseCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnEditorWindowCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Window window = (Window)sender;
            window.DialogResult = false;
            window.Close();
        }
    }
}
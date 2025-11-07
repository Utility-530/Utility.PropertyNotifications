using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Controls.SplitButtons;

namespace Utility.WPF.Demo.SplitButtons
{
    public class SplitButtonsViewModel : INotifyPropertyChanged
    {
        private string _selectedAction;
        private ObservableCollection<string> _availableActions;

        public string SelectedAction
        {
            get => _selectedAction;
            set
            {
                if (_selectedAction != value)
                {
                    _selectedAction = value;
                    OnPropertyChanged(nameof(SelectedAction));
                    //LogEvent($"Data binding: SelectedAction changed to '{value}'");
                }
            }
        }

        public ObservableCollection<string> AvailableActions
        {
            get => _availableActions;
            set
            {
                if (_availableActions != value)
                {
                    _availableActions = value;
                    OnPropertyChanged(nameof(AvailableActions));
                }
            }
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged Implementation
    }

    public partial class SplitButtonsUserControl
    {
        private int _eventCounter = 0;
        private SplitButtonsViewModel splitButtonsViewModel = new SplitButtonsViewModel();

        public SplitButtonsUserControl()
        {
            InitializeComponent();

            // Initialize data binding context
            InitializeDataContext();

            // Set up the data context
            DataContext = splitButtonsViewModel;

            // Update HasContent status initially
            UpdateHasContentStatus();
        }



        #region Event Handlers

        private void SplitButton_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var splitButton = sender as SplitButton;
            if (splitButton?.SelectedItem != null)
            {
                string content = GetDisplayContent(splitButton.Content);
                string selectedItem = GetSelectedItemText(splitButton.SelectedItem);

                LogEvent($"'{content}' → Selected: {selectedItem}");

                // Handle special test cases
                if (sender == TestSplitButton)
                {
                    HandleTestButtonSelection(selectedItem);
                }

                // Update HasContent status for all buttons
                UpdateHasContentStatus();
            }
        }

        private void UpdateDataButton_Click(object sender, RoutedEventArgs e)
        {
            // Rotate through different action sets
            var random = new Random();
            var actionSets = new[]
            {
                new[] { "Create", "Read", "Update", "Delete" },
                new[] { "Start", "Stop", "Pause", "Resume" },
                new[] { "Import", "Export", "Backup", "Restore" },
                new[] { "Connect", "Disconnect", "Sync", "Refresh" }
            };

            var selectedSet = actionSets[random.Next(actionSets.Length)];
            splitButtonsViewModel.AvailableActions.Clear();

            foreach (var action in selectedSet)
            {
                splitButtonsViewModel.AvailableActions.Add(action);
            }

            splitButtonsViewModel.SelectedAction = selectedSet[0]; // Select first item
            LogEvent($"Data updated: New action set loaded ({selectedSet.Length} items)");
        }

        #endregion Event Handlers

        #region Helper Methods

        private void InitializeDataContext()
        {
            splitButtonsViewModel.AvailableActions = new ObservableCollection<string>
            {
                "Execute",
                "Process",
                "Generate",
                "Analyze"
            };

            splitButtonsViewModel.SelectedAction = splitButtonsViewModel.AvailableActions[0];
        }

        private void HandleTestButtonSelection(string selectedItem)
        {
            switch (selectedItem)
            {
                case "Clear Content":
                    TestSplitButton.Content = null;
                    LogEvent("Test: Content cleared (null)");
                    break;

                case "Set Text Content":
                    TestSplitButton.Content = "Updated Text";
                    LogEvent("Test: Content set to text");
                    break;

                case "Set Complex Content":
                    var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };
                    stackPanel.Children.Add(new TextBlock { Text = "⭐", FontSize = 14, Margin = new Thickness(0, 0, 5, 0) });
                    stackPanel.Children.Add(new TextBlock { Text = "Complex", VerticalAlignment = VerticalAlignment.Center });
                    TestSplitButton.Content = stackPanel;
                    LogEvent("Test: Content set to complex UI");
                    break;
            }
        }

        private void UpdateHasContentStatus()
        {
            if (TestSplitButton != null && HasContentStatus != null)
            {
                HasContentStatus.Text = $"HasContent: {TestSplitButton.HasContent}";
            }
        }

        private string GetDisplayContent(object content)
        {
            if (content == null) return "[No Content]";

            if (content is StackPanel panel)
            {
                // Try to extract text from StackPanel children
                foreach (var child in panel.Children)
                {
                    if (child is TextBlock textBlock && !string.IsNullOrEmpty(textBlock.Text) && !IsEmoji(textBlock.Text))
                    {
                        return textBlock.Text;
                    }
                }
                return "[Complex Content]";
            }

            return content.ToString();
        }

        private string GetSelectedItemText(object selectedItem)
        {
            if (selectedItem is ComboBoxItem item)
            {
                return item.Content?.ToString() ?? "[No Content]";
            }
            return selectedItem?.ToString() ?? "[No Item]";
        }

        private bool IsEmoji(string text)
        {
            // Simple emoji detection
            return text.Length <= 2 && text.Length > 0 && char.IsHighSurrogate(text[0]);
        }

        private void LogEvent(string message)
        {
            _eventCounter++;
            //var timestamp = DateTime.Now.ToString("HH:mm:ss");
            //var newEntry = $"[{timestamp}] {_eventCounter:D3}: {message}\n";

            //if (EventLog != null)
            //{
            //    EventLog.Text = newEntry + EventLog.Text;

            //    // Keep only last 20 entries
            //    var lines = EventLog.Text.Split('\n');
            //    if (lines.Length > 20)
            //    {
            //        EventLog.Text = string.Join("\n", lines, 0, 20);
            //    }
            //}
        }

        #endregion Helper Methods
    }
}
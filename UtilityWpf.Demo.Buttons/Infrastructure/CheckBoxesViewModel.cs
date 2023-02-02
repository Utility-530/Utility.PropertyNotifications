using System.Collections.ObjectModel;
using System.Windows.Input;
using Utility.ViewModels;

namespace UtilityWpf.Demo.Buttons
{
    public class CheckBoxesViewModel
    {
        public CheckBoxesViewModel()
        {
            Data = new ObservableCollection<CheckViewModel>
            {
                new("1", true),
                new("2", false),
                new("3", false),
            };
        }

        public ObservableCollection<CheckViewModel> Data { get; }

        public ObservableCollection<Number> Numbers { get; } = new();

        public ICommand Command { get; }
    }
}
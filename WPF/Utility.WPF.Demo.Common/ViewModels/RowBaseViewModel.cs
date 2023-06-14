using Endless;
using Microsoft.Xaml.Behaviors.Core;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Utility.Common.EventArgs;

namespace Utility.WPF.Demo.Common.ViewModels
{
    public class RowBaseViewModel
    {
        private bool isReadOnly;
        private IEnumerator<ElementViewModel>? newItem;

        public bool IsReadOnly
        {
            get => isReadOnly; set => isReadOnly = value;
        }

        public string Header { get; } = nameof(RowBaseViewModel);

        public RowBaseViewModel()
        {
            Data = new ObservableCollection<ElementViewModel>

                {
                    new(1),
                    new(2),
                    new(3)
                };

            ChangeCommand = new ActionCommand((a) =>
            {
                switch (a)
                {
                    case MovementEventArgs eventArgs:
                        foreach (var item in eventArgs.Changes)
                        {
                            Data.Move(item.OldIndex, item.Index);
                        }
                        break;

                    default:
                        break;
                }
            });
        }

        public ObservableCollection<ElementViewModel> Data { get; }

        public ICommand ChangeCommand { get; init; }

        public IEnumerator NewItem => newItem ??= Get().Repeat().GetEnumerator();

        private ElementViewModel Get()
        {
            return new(Data.LastOrDefault()?.Value ?? 0 + 1);
        }
    }
}
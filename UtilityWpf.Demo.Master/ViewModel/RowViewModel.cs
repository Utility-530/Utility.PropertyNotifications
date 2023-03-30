using Microsoft.Xaml.Behaviors.Core;
using Utility.Common.EventArgs;
using Utility.Enums;

namespace UtilityWpf.Demo.Master.ViewModels
{
    public class RowViewModel : Utility.WPF.Demo.Common.ViewModels.RowBaseViewModel
    {
        public RowViewModel()
        {
            ChangeCommand = new ActionCommand((a) =>
            {
                switch (a)
                {
                    case CollectionItemEventArgs { EventType: EventType.Add }:
                        if (NewItem.MoveNext())
                            Data.Add(NewItem.Current as Common.ViewModels.ElementViewModel);
                        break;

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
    }
}
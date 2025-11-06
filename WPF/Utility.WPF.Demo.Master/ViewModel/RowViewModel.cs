using Microsoft.Xaml.Behaviors.Core;
using Utility.Enums;
using Utility.EventArguments;

namespace Utility.WPF.Demo.Master.ViewModels
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
                            Data.Add(NewItem.Current as Utility.WPF.Demo.Common.ViewModels.ElementViewModel);
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
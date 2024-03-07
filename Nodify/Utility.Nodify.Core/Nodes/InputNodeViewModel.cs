//using System.Collections.ObjectModel;
//using System.Windows.Input;
//using Utility.Commands;

//namespace Utility.Nodify.Core
//{
//    public class InputNodeViewModel : NodeViewModel
//    {
//        public InputNodeViewModel()
//        {
//            AddOutputCommand = new Command(
//                () => Output.Add(new ConnectorViewModel
//                {
//                    Title = $"In {Output.Count}"
//                }),
//                () => Output.Count < 10);

//            RemoveOutputCommand = new Command(
//                () => Output.RemoveAt(Output.Count - 1),
//                () => Output.Count > 1);

//            Output.Add(new ConnectorViewModel
//            {
//                Title = $"In {Output.Count}"
//            });
//        }

//        public new RangeObservableCollection<ConnectorViewModel> Output { get; set; } = new ();

//        public ICommand AddOutputCommand { get; }
//        public ICommand RemoveOutputCommand { get; }
//    }
//}

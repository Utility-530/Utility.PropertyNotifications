////using Autofac;
//using DryIoc;
//using Utility.Nodify.Core;
//using System.Collections.Generic;
//using System.Linq;
//using System.Collections.ObjectModel;
//using IContainer = DryIoc.IContainer;
//using Utility.PropertyNotifications;

//namespace Utility.Nodify.ViewModels
//{
//    using Keys = Utility.Nodify.Operations.Keys;


//    public class DiagramsViewModel : NotifyPropertyClass
//    {
//        private readonly IContainer container;

//        public DiagramsViewModel(IContainer container)
//        {
//            this.container = container;
//        }

//        public IEnumerable<Diagram> Diagrams => container.Resolve<IEnumerable<Diagram>>(Keys.Diagrams);

//        public Diagram SelectedDiagram
//        {
//            get => container.Resolve<RangeObservableCollection<Diagram>>(Keys.SelectedDiagram).FirstOrDefault();
//            set
//            {
//                container.Resolve<RangeObservableCollection<Diagram>>(Keys.SelectedDiagram).Insert(0, value);
//                this.RaisePropertyChanged();
//            }
//        }
//    }
//}

//using Utility.Tasks;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reactive.Linq;
//using System.Reactive.Subjects;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;


//namespace Utility.Tasks.View
//{

//    public class BackgroundWorkerQueueControl : AsyncWorkerQueueControl<IWorkerItem<object>,object>
//    {
//        static BackgroundWorkerQueueControl()
//        {
//            DefaultStyleKeyProperty.OverrideMetadata(typeof(BackgroundWorkerQueueControl), new FrameworkPropertyMetadata(typeof(BackgroundWorkerQueueControl)));

//        }
        
//        public BackgroundWorkerQueueControl()
//        {
//        }

//        protected override Utility.Interfaces.IService<KeyValuePair<DynamicData.ChangeReason,IWorkerItem< object>>> NewItemsInitialise(IObservable<object> newitems)
//        {
//            return new BackgroundWorkerCommandQueue<object>(newitems.Select(_=>(WorkerArgument<object>)_));
//        }
//    }
//}

//using Utility.Tasks;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reactive.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;


//namespace Utility.Tasks.View
//{
//    public class TPLQueueControl : AsyncWorkerQueueControl<AsyncWorkerItem<object>, object>
//    {

//        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key", typeof(string), typeof(TPLQueueControl), new PropertyMetadata("Key"));
//        public static readonly DependencyProperty TaskProperty = DependencyProperty.Register("Task", typeof(string), typeof(TPLQueueControl), new PropertyMetadata("Task"));


//        public string Key
//        {
//            get { return (string)GetValue(KeyProperty); }
//            set { SetValue(KeyProperty, value); }
//        }

//        public string Task
//        {
//            get { return (string)GetValue(TaskProperty); }
//            set { SetValue(TaskProperty, value); }
//        }


//        static TPLQueueControl()
//        {
//            DefaultStyleKeyProperty.OverrideMetadata(typeof(TPLQueueControl), new FrameworkPropertyMetadata(typeof(TPLQueueControl)));
//        }



//        public TPLQueueControl()
//        {
//        }


//        protected override UtilityInterface.IService<KeyValuePair<DynamicData.ChangeReason, AsyncWorkerItem<object>>> NewItemsInitialise(IObservable<object> newitems)
//        {

//            Check(newitems);

//            var nis = NewItemSubject.Select(_ =>
//            {
//                var task = (Task<object>)_.GetType().GetProperty(Task).GetValue(_);
//                var key =  (string)_.GetType().GetProperty(Key).GetValue(_);
//                return new KeyValuePair<string, Task<object>>(key, (task));
//            });


//            var cts = new System.Threading.CancellationTokenSource();
//            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
//            return new TPLDataFlowAsyncQueue<object>(nis, cts, scheduler);

//        }


//        private void Check(IObservable<object> items)
//        {

//            items.Take(1).Subscribe(_ =>
//            {
//                var props = _.GetType().GetProperties();

//                if (!props.Select(_a => _a.Name).Contains(Task))
//                    throw new Exception(nameof(Task) + " Property has not been provided to " + nameof(TPLQueueControl));
//                if (!props.Select(_a => _a.Name).Contains(Key))
//                    throw new Exception(nameof(Key) + " Property has not been provided to " + nameof(TPLQueueControl));

//                //return _;

//            });

//        }
//    }
//}

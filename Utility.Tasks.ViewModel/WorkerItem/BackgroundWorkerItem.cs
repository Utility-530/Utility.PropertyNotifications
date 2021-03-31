using System.ComponentModel;
using System.ComponentModel.Custom.Generic;
using System.Reactive.Linq;

namespace Utility.Tasks.ViewModel
{

    public class BackgroundWorkerItem : ProgressWorkerItem
    {
        public BackgroundWorkerItem(IWorkerArgument<object> wa, int key, BackgroundWorker<IWorkerArgument<object>, object, object> backgroundWorker)
            : base(
                 progress: backgroundWorker.GetProgress().Select(_ => new ProgressChangedEventArgs(_.ProgressPercentage, _.UserState)),
                 completion: backgroundWorker.GetCompletion().Select(_ => (AsyncCompletedEventArgs)_),
                 actn: () => backgroundWorker.RunWorkerAsync(wa),
                 key: key.ToString())
        {

        }
    }


    public class BackgroundWorkerItem<T> : ProgressWorkerItem<T>
    {
        public BackgroundWorkerItem(IWorkerArgument<T> wa, int key, BackgroundWorker<IWorkerArgument<T>, T, T> backgroundWorker)
            : base(
                 progress: backgroundWorker.GetProgress(),
                 completed: backgroundWorker.GetCompletion().Select(_ => (AsyncCompletedEventArgs)_),
                 actn: () => backgroundWorker.RunWorkerAsync(wa),
                 key: key.ToString())
        {
        }
    }
}






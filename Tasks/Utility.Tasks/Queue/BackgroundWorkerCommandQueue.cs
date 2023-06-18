using System;
using System.Threading;
using System.ComponentModel.Custom.Generic;
using System.Reactive.Linq;
using UtilityEnum;
using UtilityInterface.NonGeneric;
using Utility.Tasks.Model;

namespace Utility.Tasks
{
    public record BackgroundWorkerTaskOutput(string Key, object Value) : TaskOutput(Key, Value);

    public class BackgroundWorkerInput<TArgs>
    {
        public BackgroundWorkerInput(IWorkerArgument<TArgs> workerArgument, int i, BackgroundWorker<IWorkerArgument<TArgs>, TArgs, TArgs> worker)
        {
            WorkerArgument = workerArgument;
            I = i;
            Worker = worker;
        }

        public IWorkerArgument<TArgs> WorkerArgument { get; }
        public int I { get; }
        public BackgroundWorker<IWorkerArgument<TArgs>, TArgs, TArgs> Worker { get; }
    }


    public class BackgroundWorkerCommandQueue<T> : AsyncWorkerQueue, IPlayer /*: INotifyPropertyChanged*/ //where T : new()
    {
        private readonly ManualResetEvent busy = new(true);

        static BackgroundWorker<IWorkerArgument<T>, T, T> backgroundWorker = new()
        {
            WorkerSupportsCancellation = true,
            WorkerReportsProgress = true
        };

        public BackgroundWorkerCommandQueue(IFactory<IWorkerItem, BackgroundWorkerInput<T>> factory, IObservable<IWorkerArgument<T>> mainMethod = null) :
            base(backgroundWorker.GetCompletion().Select(_ => new BackgroundWorkerTaskOutput("",_.Result)))
        {

            backgroundWorker.DoWork += new EventHandler<DoWorkEventArgs<IWorkerArgument<T>, T>>((a, b) => { new BackgroundWorkerDoWork(busy).DoWork_Handler(backgroundWorker, b); });

            int i = 0;
            mainMethod?.Subscribe(wa =>
            {
                i++;
                var creation = factory.Create(new BackgroundWorkerInput<T>(wa, i, backgroundWorker));
                Enqueue(creation);
            });

            Init(commands);
        }

        private void Init(IObservable<ProcessState> commands)
        {
            commands
            .Subscribe(command =>
            {
                if (command == ProcessState.Blocked)
                    Pause();
                //else if (command == UtilityEnum.ProcessState.Ready)
                //{
                //    _backgroundWorker.RunWorkerAsync(wa);
                //}
                else if (command == ProcessState.Running)
                    Play();
                else if (command == ProcessState.Terminated)
                    Cancel();
                else
                    throw new ArgumentOutOfRangeException("argument should be nullable bool");
            }, () => backgroundWorker.DoWork -= new EventHandler<DoWorkEventArgs<IWorkerArgument<T>, T>>((a, b) => { new BackgroundWorkerDoWork(busy).DoWork_Handler(backgroundWorker, b); }));
        }


        public void Pause()
        {
            // Block the worker
            busy.Reset();

        }

        public void Play()
        {
            // UnBlock the worker
            busy.Set();

        }

        public void Cancel()
        {
            if (backgroundWorker.IsBusy)
            {
                // Set CancellationPending property to true
                backgroundWorker.CancelAsync();
                // Unblock worker so it can see that
                busy.Set();

            }
        }
    }
}






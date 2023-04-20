using Jellyfish;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using JulMar.Windows.Collections;
using System.Windows.Input;
using Utility.Commands;
using System.Diagnostics;

namespace Utility.WPF.Demo.Collections
{
    public class ActivityLog : ViewModel
    {
        public int Id { get; set; }
        private TimeSpan _elapsed;
        private bool _success;
        private TimeSpan _removeElapsed;
        private TimeSpan _addElapsed;

        public TimeSpan AddElapsed
        {
            get { return _addElapsed; }
            set
            {
                _addElapsed = value;
                Elapsed = value;
                this.Set(ref _addElapsed, value);
            }
        }

        public TimeSpan RemoveElapsed
        {
            get { return _removeElapsed; }
            set
            {
                _removeElapsed = value;
                this.Set(ref _removeElapsed, value);
                if (Elapsed < value)
                    Elapsed = value;
            }
        }

        public TimeSpan Elapsed
        {
            get { return _elapsed; }
            set { this.Set(ref _elapsed, value); }
        }

        public bool Success
        {
            get { return _success; }
            set { this.Set(ref _success, value); }
        }
    }

    public class UnitOfWork : ViewModel
    {
        private static int _counter = 1;
        private static readonly Random RNG = new Random();
        private static readonly PropertyInfo[] AvailableColors = typeof(Colors).GetProperties();

        private int _percent;
        public int PercentComplete
        {
            get { return _percent; }
            set => this.Set(ref _percent, value);
        }

        public string Color { get; private set; }
        public int Id { get; private set; }
        public int ThreadId { get; private set; }

        public UnitOfWork()
        {
            Id = Interlocked.Increment(ref _counter);
            ThreadId = Thread.CurrentThread.ManagedThreadId;
            Color = AvailableColors[RNG.Next(AvailableColors.Length - 1)].GetValue(null, null).ToString();
        }

        public override string ToString()
        {
            return string.Format("T{0}", Id);
        }

        public void Run()
        {
            PercentComplete = 0;
            int runtime = RNG.Next(5000);
            for (int i = 0; i < 20; i++)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(runtime / 100.0));
                PercentComplete++;
            }
        }
    }


    //[ExportViewModel("Main")]
    class MainViewModel : ViewModel
    {
        private readonly CancellationTokenSource _cancellationToken = new CancellationTokenSource();
        public MTObservableCollection<UnitOfWork> TaskList { get; private set; }
        public MTObservableCollection<ActivityLog> Activity { get; private set; }

        private bool _isRunning;
        public bool IsRunning
        {
            get { return _isRunning; }
            private set { this.Set(ref _isRunning, value); }
        }

        public ICommand StartStop { get; private set; }
        public ICommand Clear { get; private set; }
        public ICommand AddOne { get; private set; }
        public ICommand AddTwenty { get; private set; }

        public MainViewModel()
        {
            TaskList = new MTObservableCollection<UnitOfWork>();
            Activity = new MTObservableCollection<ActivityLog>();
            StartStop = new Command(OnStartStopWork);
            AddOne = new Command(AddTask);
            AddTwenty = new Command(AddRange);
            Clear = new Command(() => Activity.Clear());
            IsRunning = false;
        }

        private void OnStartStopWork()
        {
            IsRunning = !IsRunning;

            if (IsRunning)
            {
                Task.Factory.StartNew(RunTasks, _cancellationToken.Token,
                    TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
            else
            {
                _cancellationToken.Cancel();
            }
        }

        private void RunTasks()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                while (TaskList.Count < 50)
                {
                    AddTask();
                }
                Thread.Sleep(100);
            }
        }

        private void AddRange()
        {
            UnitOfWork[] workItems = new UnitOfWork[20];
            ActivityLog[] logItems = new ActivityLog[20];

            for (int i = 0; i < workItems.Length; i++)
            {
                workItems[i] = new UnitOfWork();
                logItems[i] = new ActivityLog { Id = workItems[i].Id };
            }

            Stopwatch sw1 = Stopwatch.StartNew();
            TaskList.AddRange(workItems);
            Activity.AddRange(logItems);
            sw1.Stop();

            for (int i = 0; i < workItems.Length; i++)
            {
                UnitOfWork task = workItems[i];
                ActivityLog log = logItems[i];

                log.AddElapsed = sw1.Elapsed;
                Task.Factory.StartNew(task.Run)
                    .ContinueWith(t =>
                    {
                        Stopwatch sw2 = Stopwatch.StartNew();
                        bool rc = TaskList.Remove(task);
                        int index2 = TaskList.IndexOf(task);
                        log.RemoveElapsed = sw2.Elapsed;
                        log.Success = rc && index2 == -1;
                    });
            }
        }

        public void AddTask()
        {
            UnitOfWork task = new UnitOfWork();
            ActivityLog log = new ActivityLog() { Id = task.Id };

            Stopwatch sw1 = Stopwatch.StartNew();
            TaskList.Add(task);
            Activity.Add(log);
            log.AddElapsed = sw1.Elapsed;

            Task.Factory.StartNew(task.Run, TaskCreationOptions.AttachedToParent)
                .ContinueWith(t =>
                {
                    Stopwatch sw2 = Stopwatch.StartNew();
                    bool rc = TaskList.Remove(task);
                    int index2 = TaskList.IndexOf(task);
                    log.RemoveElapsed = sw2.Elapsed;
                    log.Success = rc && index2 == -1;
                });
        }
    }
}

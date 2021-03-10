using ReactiveAsyncWorker;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ReactiveAsyncWorker.Model;
using ReactiveAsyncWorker.ViewModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Threading;
using ReactiveUI;

namespace DemoApp
{
    public class SecondaryViewModel
    {


        //private const string sss = @"http://www.picture-newsletter.com/arctic/arctic-0";
        //private const string sty = @"https://upload.wikimedia.org/wikipedia/commons/e/e9/Felis_silvestris_silvestris_small_gradual_decrease_of_quality.png";
        private const string imgurImage = "https://i.imgur.com/3IWZvuT.jpg";
        private const string path = "../../Data/arctic";

        public SecondaryViewModel()
        {
            var scheduler = new SynchronizationContextScheduler(SynchronizationContext.Current);
         //   Service = new FileDownloaderQueue(scheduler);
            StartCommand = ReactiveCommand.Create(Init, outputScheduler:scheduler);
        }

        public ReactiveCommand<Unit, Unit> StartCommand { get; }

      //  public FileDownloaderQueue Service { get; }


        private void Init()
        {
            var ydt = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .StartWith(0)
                .Take(4)
                .Select(l => new FileDownload(new Uri(imgurImage), path + l + 1 + ".jpg", path + l + 1 + DateTime.Now.ToString()))
                .Subscribe(newItem =>
                {
                   // Service.OnNext(newItem);
                });

        }
    }
}

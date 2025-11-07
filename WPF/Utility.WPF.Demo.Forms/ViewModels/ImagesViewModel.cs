using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Windows.Input;
using ReactiveUI;
using Utility.Helpers.Ex;
using Utility.Models;
using Utility.WPF.Abstract;
using _ViewModel = Utility.ViewModels.ViewModel;

namespace Utility.WPF.Demo.Forms.ViewModels
{
    public class ImagesViewModel : _ViewModel
    {
        private ICommand? changeCommand;

        public ImagesViewModel(string header, IReadOnlyCollection<string> enumerable) : this(header, enumerable.Select(a => new ImageViewModel(a)).ToArray())
        {
        }

        public ImagesViewModel(string header) : this(header, Array.Empty<ImageViewModel>())
        {
        }

        public ImagesViewModel(string header, IReadOnlyCollection<ImageViewModel> collection) : base(header)
        {
            foreach (var item in collection)
            {
                Subscribe(item);
            }

            Dictionary<ImageViewModel, IDisposable> list = new();
            var obsCollection = new ObservableCollection<ImageViewModel>(collection);

            obsCollection
                .SelectNewItems<ImageViewModel>()
                .Subscribe(a =>
                {
                    var dis = a.WhenAnyValue(a => a.URL).Subscribe(a =>
                    {
                        this.RaisePropertyChanged(nameof(Children));
                    });
                    list.Add(a, dis);
                    this.RaisePropertyChanged(nameof(Children));
                });

            obsCollection
                .SelectOldItems<ImageViewModel>()
                .Subscribe(a =>
                {
                    if (list.ContainsKey(a))
                    {
                        list[a].Dispose();
                        list.Remove(a);
                    }
                    this.RaisePropertyChanged(nameof(Children));
                });
            Children = obsCollection;
            //Intialise();
        }

        public override ObservableCollection<ImageViewModel> Children { get; }

        //public System.Collections.IEnumerator NewItem { get => 0.Repeat().Select(a => new ImageViewModel(string.Empty)).GetEnumerator(); }

        public ICommand ChangeCommand => changeCommand ??= ReactiveCommand.Create<object, Unit>(Change);

        public override Property Model { get; }

        private Unit Change(object change)
        {
            if (change is CollectionItemEventArgs { Item: string item })
            {
                var ivm = new ImageViewModel(item);
                Subscribe(ivm);
                Children.Add(ivm);
            }
            return Unit.Default;
        }

        private void Subscribe(ImageViewModel ivm)
        {
            //subject
            //    .Subscribe(a =>
            //    {
            //        if (ivm.URL == a.URL)
            //        {
            //            if (a.WebUrl != null)
            //            {
            //                ivm.WebURL = a.WebUrl;
            //            }
            //            ivm.IsChecked = a.WebUrl != null;
            //        }
            //    });

            ivm
                .WhenAnyValue(a => a.URL)
                .Subscribe(a =>
                {
                    //subject1.OnNext(new WebUrlRequest(a));
                });
        }

        //ReplaySubject<WebUrlResponse> subject = new(1);
        //ReplaySubject<WebUrlRequest> subject1 = new(1);
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        //public void OnNext(WebUrlResponse value)
        //{
        //    subject.OnNext(value);
        //}

        //public IDisposable Subscribe(IObserver<WebUrlRequest> observer)
        //{
        //    return subject1.Subscribe(observer);
        //}
    }

    //public record WebUrlResponse(string URL, string? WebUrl) : WebUrlMessage(URL);
    //public record WebUrlRequest(string URL) : WebUrlMessage(URL);
    //public record WebUrlMessage(string URL);

    public class ImageViewModel : ReactiveObject
    {
        private string url;
        private string webURL;
        private bool isChecked;

        public ImageViewModel(string uRL, bool isChecked = false)
        {
            URL = uRL;
            IsChecked = isChecked;
            Guid = Guid.NewGuid();
        }

        public Guid Guid { get; }

        public string URL { get => url; set => this.RaiseAndSetIfChanged(ref url, value); }

        public string WebURL { get => webURL; set => webURL = value; }

        public bool IsChecked { get => isChecked; set => this.RaiseAndSetIfChanged(ref isChecked, value); }
    }
}
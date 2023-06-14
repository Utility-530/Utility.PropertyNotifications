using DynamicData;
using ReactiveUI;
using Suggest.FileSystem.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using Utility.FileSystem.Transfer.Demo.Infrastructure;

namespace Utility.FileSystem.Transfer.Demo
{
    public class DirectorySuggestion : ReactiveObject, IObservable<DirectorySuggestion>, IObserver<string>, IDisposable
    {

        private DirectoryInfo directoryInfo => new DirectoryInfo(Path);
        private readonly DirectoryAsyncSuggest directoryAsyncSuggest;
        private IReadOnlyCollection<DirectorySuggestion> collection;
        private ReplaySubject<DirectorySuggestion> upChangeSubject = new(1);
        private ReplaySubject<DirectoryInfo[]> downChangeSubject = new(1);
        private IDisposable disposable;

        public DirectorySuggestion(DirectoryAsyncSuggest directoryAsyncSuggest, string path)
        {
            this.Path = path;
            this.directoryAsyncSuggest = directoryAsyncSuggest;
        }

        public string Name => string.IsNullOrEmpty(Path) ? Path : directoryInfo.Name;

        public string Path { get; }

        public IReadOnlyCollection<DirectorySuggestion> Collection
        {
            get
            {
                collection ??= GetCollection();

                upChangeSubject.OnNext(this);
                //this.RaisePropertyChanged(nameof(Collection));
                return collection;
            }
        }

        private IReadOnlyCollection<DirectorySuggestion> GetCollection()
        {
            disposable = directoryAsyncSuggest
                 .SuggestAsync(Path)
                 .ToObservable()
                 .Select(a => a.Suggestions.ToObservable())
                 .Switch()
                 .ToObservableChangeSet()
                 .Transform(a => new DirectorySuggestion(directoryAsyncSuggest, a.ToString()))
                 .ObserveOn(RxApp.MainThreadScheduler)
                 .Bind(out var collection)
                 .ActOnEveryObject((dirSuggestion) =>
                 {
                     // changes propogating up the tree
                     dirSuggestion
                          .Subscribe(a =>
                          {
                              upChangeSubject.OnNext(a);
                          });

                     // changes propogating down the tree
                     downChangeSubject
                        .Subscribe(a =>
                        {
                            if (a.Any(a => a.FullName == dirSuggestion.directoryInfo.FullName))
                            {
                                dirSuggestion.OnNext(a.Last().FullName);
                            }
                        });
                 }, (a) => { });

            return collection;
        }

        public IDisposable Subscribe(IObserver<DirectorySuggestion> observer)
        {
            return upChangeSubject.Subscribe(observer);
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(string value)
        {
            if (value == null)
                return;

            if (this.Path == value)
            {
                upChangeSubject.OnNext(this);
                return;
            }

            var split = DirectoryHelper.SplitDirectory(new DirectoryInfo(value)).ToArray();

            if (split.Any(a => a.FullName == this.Path))
            {
                downChangeSubject.OnNext(split);
            }
        }

        public override string ToString()
        {
            return Path;
        }

        public void Dispose()
        {
            disposable.Dispose();
        }
    }

}

namespace BrowserHistoryDemoLib.ViewModels
{
    using BrowseHistory;
    using HistoryControlLib;
    using HistoryControlLib.Interfaces;
    using HistoryControlLib.ViewModels;
    using HistoryControlLib.ViewModels.Base;
    using ReactiveUI;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Input;

    public class NavigationViewModel : BaseViewModel
    {

        private readonly BrowseHistory<PathItem> naviHistory;

        /// <summary>
        /// Class constructor.
        /// </summary>
        public NavigationViewModel(BrowseHistory<PathItem> naviHistory)
        {
            this.naviHistory = naviHistory;

            SelectionCommand = CreateSelectionChanged(naviHistory);

            ForwardCommand = ReactiveCommand.Create<object, object>(a =>
             {
                 if (naviHistory.CanForward == false)
                     throw new Exception("fse333d");
                 naviHistory.Forward();
                 return a;
             }, naviHistory.WhenAnyValue(a => a.CanForward));

            BackwardCommand = ReactiveCommand.Create<object, object>(a =>
            {
                if (naviHistory.CanBackward == false)
                    throw new Exception("fs3e33333d");
                naviHistory.Backward();
                return a;
            }, naviHistory.WhenAnyValue(a => a.CanBackward));

            var navigateCommand = ReactiveCommand.Create<string, string>(a =>
            {

                return a;
            }, Observable.Return(true));
            NavigateCommand = navigateCommand;
            navigateCommand
                .WhereNotNull()
                .Where(a => PathHelper.IsValidPath(a))
                .DistinctUntilChanged().Subscribe(a =>
            {
                naviHistory.Navigate(new PathItem(a));
            });

            UpCommand = CreateUpCommand();

            naviHistory.PropertyChanged += NaviHistory_PropertyChanged;
        }

        private void NaviHistory_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(e.PropertyName);
        }

        public object CurrentItem => naviHistory.CurrentItem;

        public int? ForwardCount => naviHistory.ForwardCount;

        public int? BackwardCount => naviHistory.BackwardCount;


        /// <summary>
        /// Command executes when the user has selected
        /// a different item in the displayed list of items.
        /// </summary>
        public ICommand SelectionCommand { get; }

        /// <summary>
        /// Gets a command to browse forward in the available collection of items.
        /// </summary>
        public ICommand ForwardCommand { get; }

        /// <summary>
        /// Gets a command to browse backward in the available collection of items.
        /// </summary>
        public ICommand BackwardCommand { get; }

        public ICommand NavigateCommand { get; }

        /// <summary>
        /// Gets a command to browse to the parent (if any) of the current location.
        /// </summary>
        public ICommand UpCommand { get; }

        public IEnumerable Collection => naviHistory.Collection;

        public string? BackwardText => naviHistory.BackwardItem?.ToString();

        public string? ForwardText => naviHistory.ForwardItem?.ToString();


        private ICommand CreateUpCommand()
        {
            return ReactiveCommand.Create(() =>
            {
                try
                {
                    if (Directory.GetParent(naviHistory.CurrentItem.Path) is DirectoryInfo parent)
                        naviHistory.Navigate(new PathItem(parent.FullName));
                }
                catch
                {
                }
            }, Observable.Return(CanExecute()));
        }

        bool CanExecute()
        {

            if (naviHistory.CurrentItem == null)
                return false;
            try
            {
                return Directory.GetParent(naviHistory.CurrentItem.Path) != null;
            }
            catch
            {
            }
            return false;

        }
        private static ICommand CreateSelectionChanged(IBrowseHistory<PathItem> NaviHistory)
        {
            return ReactiveCommand.Create<object>(p =>
            {

                if (p is object[] paths && paths[0] is PathItem pss &&
                CreateSelectionChanged(NaviHistory.Collection, pss) is PathItem pathItem1)
                    NaviHistory.CurrentItem = pathItem1;

                else if (p is string pi &&
                   CreateSelectionChanged(NaviHistory.Collection, new PathItem(pi)) is PathItem pathItem)
                    NaviHistory.CurrentItem = pathItem;
            });

            static IEquatable<T> CreateSelectionChanged<T>(IReadOnlyCollection<IEquatable<T>> collection, IEquatable<T> item)
            {
                return collection
                  .Select(histItem => Tuple.Create(histItem, Equals(histItem, item)))
                  .FirstOrDefault(a => a.Item2)?.Item1;
            }
        }
    }

    public static class PathHelper
    {
        public static bool IsValidPath(string path)
        {
            return Directory.Exists(path) ||
                   File.Exists(path) ||
                   (IsInvalidCharacterWithin(path) == false &&
                    IsFormattedCorrectly(path));
        }


        public static bool IsFormattedCorrectly(string test)
        {
            bool result = !string.IsNullOrWhiteSpace(test);
            string
               drivePattern = /* language=regex */
                  @"^(([A-Z]:(?:\.{1,2}[\/\\]|[\/\\])?)|([\/\\]{1,2}|\.{1,2}[\/\\]))?",
               pattern = drivePattern + /* language=regex */
                         @"([^\x00-\x1A|*?\t\v\f\r\n+\/,;""'`\\:<>=[\]]+[\/\\]?)+$";
            result &= Regex.IsMatch(test, pattern, RegexOptions.ExplicitCapture);
            pattern = drivePattern + /* language=regex */
                      @"(([^\/\\. ]|[^\/. \\][\/. \\][^\/. \\]|[\/\\]$)*[^\x00-\x1A|*?\s+,;""'`:<.>=[\]])$";
            result &= Regex.IsMatch(test, pattern, RegexOptions.ExplicitCapture);
            return result;
        }

        public static bool IsInvalidCharacterWithin(string testName)
        {
            string regexString = "[" + Regex.Escape(Path.GetInvalidPathChars().ToString()) + "]";
            Regex containsABadCharacter = new Regex(regexString);
            if (containsABadCharacter.IsMatch(testName))
            {
                return false;
            }
            return true;
        }
    }
}


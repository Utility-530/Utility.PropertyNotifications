using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using Bogus;
using DynamicData;
using Utility.Helpers;

namespace Utility.WPF.Demo.Controls
{
    /// <summary>
    /// Interaction logic for ListBoxCriteria.xaml
    /// </summary>
    public partial class ListBoxCriteriaUserControl : UserControl
    {
        private ReadOnlyObservableCollection<PassFail> a;

        public ListBoxCriteriaUserControl()
        {
            InitializeComponent();

            new Faker<PassFail>()
                .RuleFor(a => a.Key, f => f.Name.FirstName())
                .RuleFor(a => a.Expired, f => f.Random.Bool())
                .GenerateLazy(10000)
                .ToObservable().Zip(Observable.Interval(TimeSpan.FromSeconds(3)), (a, b) => a)
                .ToObservableChangeSet(a => a.Key)
                .Sort(ComparerHelper.By<PassFail, string>(a => a.Key))
                .Top(10)
                .ObserveOn(SynchronizationContextScheduler.Instance)
                .Bind(out a)
                .Subscribe(ss =>
                {
                });
            passFail.ItemsSource = a;

            //new Faker<PassFail>()
            // .RuleFor(a => a.Key, f => "Bob")
            // .RuleFor(a => a.Expired, f => f.Random.Bool())
            // .GenerateLazy(10000)
            // .ToObservable().Zip(Observable.Interval(TimeSpan.FromSeconds(3)), (a, b) => a)
            // .ToObservableChangeSet(a => a.Key)
            // .Sort(new DefaultComparer<PassFail, string>(new Func<PassFail, string>(a => a.Key)))
            // .Top(10)
            // .ObserveOnDispatcher()
            // .Bind(out b)
            // .Subscribe(a =>
            // {
            // });

            //passFail2.ItemsSource = b;

            var c = new Faker<ServerFile>()
                                .RuleFor(a => a.Download, f => f.Date.Past())
                                .RuleFor(a => a.Upload, f => f.Date.Past())
                                .RuleFor(a => a.Link, f => "www.xd.com/link")
                                .Generate(20);

            file1.ItemsSource = c;
        }

        internal class PassFail
        {
            public string Key { get; set; }

            public bool Expired { get; set; }
        }

        public class ServerFile
        {
            public string Name { get; set; }

            public string Link { get; set; }

            public FileInfo File { get; set; }

            public DateTime Download { get; set; }

            public DateTime Upload { get; set; }

            public bool OutOfDate => Download < Upload;
        }
    }

    //public class DiagnosticList : InteractiveList
    //{
    //}
}
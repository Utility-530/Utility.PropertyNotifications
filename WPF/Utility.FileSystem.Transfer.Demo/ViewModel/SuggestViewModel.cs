using BrowseHistory;
using HistoryControlLib.ViewModels;
using HistoryControlLib.ViewModels.Base;
using Suggest.FileSystem.Service;
using Suggest.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Threading.Tasks;
using System;
using System.Reactive.Linq;
using DynamicData;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;
using ReactiveUI;
using Utility.FileSystem.Transfer.Demo.Infrastructure;

namespace Utility.FileSystem.Transfer.Demo
{
    public class SuggestViewModel : BaseViewModel
    {
        private readonly DirectoryAsyncSuggest suggest;
        private readonly BrowseHistory<PathItem> browseHistory;
        private string text;

        public SuggestViewModel(BrowseHistory<PathItem> browseHistory)
        {

            suggest = new DirectoryAsyncSuggest();
            this.browseHistory = browseHistory;

            var suggestion = new DirectorySuggestion(suggest, browseHistory.Collection.LastOrDefault()?.Path);
            Collection = suggestion.Collection;

            suggestion.Subscribe(ad =>
                {
                    var c = Collection;
                    Text = ad.Path;
                    DirectorySuggestion = ad;
                    this.OnPropertyChanged(nameof(DirectorySuggestion));
                });


            this.WhenAnyValue(a => a.Text)
                //.WhereNotNull()
                .Subscribe(ad =>
              {
                  //if (new DirectoryInfo(ad).Exists == false)
                  //{
                  //    return;
                  //}
                  try
                  {
                      foreach (var item in Collection)
                      {

                          item.OnNext(ad);
                      }
                  }
                  catch(Exception ex)
                  {

                  }
        
              });

        }


        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }

        public string Text
        {
            get => text;
            set
            {
                text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        public IReadOnlyCollection<DirectorySuggestion> Collection { get; }

        public IAsyncSuggest Suggest => suggest;

        public DirectorySuggestion DirectorySuggestion { get; set; }

        static string DirectorySuggestions(IReadOnlyCollection<DirectorySuggestion> suggestions)
        {
            return string.Empty;
        }
    }
}

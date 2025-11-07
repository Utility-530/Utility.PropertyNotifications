using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors.Core;
using Utility.Helpers;

namespace Utility.WPF.Demo.Common.ViewModels
{
    public abstract class ResourceDictionariesViewModel
    {
        //private bool isReadOnly;

        private readonly ActionCommand changeCommand;

        public ResourceDictionariesViewModel()
        {
            changeCommand = new ActionCommand(Change);
        }

        //public bool IsReadOnly
        //{
        //    get => isReadOnly; set => isReadOnly = value;
        //}
        public string Header { get; } = "Resource-Dictionaries ViewModel";

        public abstract IEnumerable Collection { get; }

        public ICommand ChangeCommand => changeCommand;

        private void Change()
        {
        }
    }

    public class ThemesViewModelFactory
    {
        //public ThemesViewModelFactory()
        //{
        //    var coll = Source.ThemeDictionary;

        //    Collection = CreateViewModels(coll.MergedDictionaries).ToArray();
        //}

        //public IReadOnlyCollection<TickViewModel> Collection { get; }

        public static IEnumerable<TickViewModel> CreateViewModels(IReadOnlyCollection<ResourceDictionary> coll)
        {
            foreach (var dic in coll)
            {
                //if (dic.Source == null)
                //    continue;
                var tick = new TickViewModel(Guid.NewGuid(), GetName(dic));

                yield return tick;
            }
            static string GetName(ResourceDictionary dic)
            {
                return Converter.Convert(StringHelper.Split(dic.Source.ToString(), ";component/").Last());
            }
        }
    }

    public class Source
    {
        public static ResourceDictionary? ThemeDictionary { get; } = Application.Current.Resources
                        .MergedDictionaries
                        .SingleOrDefault(d => d.Source.ToString().EndsWith("Themes.xaml"));
    }

    internal static class Converter
    {
        public static string Convert(string uri)
        {
            return uri.Remove(".xaml");
        }

        public static string ConvertBack(string value)
        {
            return value + ".xaml";
        }
    }

    public class ResourceDictionaryService : IObserver<TickViewModel>
    {
        private readonly Dictionary<ResourceDictionary, bool> dictionary;
        private ReplaySubject<TickViewModel> tickViewModel = new();

        public ResourceDictionaryService(IReadOnlyCollection<ResourceDictionary> resourceDictionaries)
        {
            dictionary = resourceDictionaries.ToDictionary(a => a, a => false);
            ReplaySubject<(ResourceDictionary, bool)> rSubject = new(1);

            UpdateMergedDictionaries(rSubject);

            var disposable = tickViewModel.Subscribe(tick =>
            {
                tick
                .Select(ad => (MatchDictionary(ad.Text), ad.NewValue))
                .Subscribe(rSubject);

                rSubject.OnNext((MatchDictionary(tick.Text), tick.IsChecked));
            });
            //rSubject.OnNext((Source.ThemeDictionary, false));
        }

        private void UpdateMergedDictionaries(ReplaySubject<(ResourceDictionary, bool)> rSubject)
        {
            rSubject
                .Subscribe(a =>
                {
                    var (dict, b) = a;
                    if (dictionary[dict] = b)
                        Application.Current.Resources.MergedDictionaries.Add(dict);
                    else
                        Application.Current.Resources.MergedDictionaries.Remove(dict);
                });
        }

        private ResourceDictionary MatchDictionary(string name)
        {
            return dictionary.Keys.Single(a => a.Source.ToString().EndsWith(Converter.ConvertBack(name)));
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(TickViewModel value)
        {
            tickViewModel.OnNext(value);
        }
    }
}
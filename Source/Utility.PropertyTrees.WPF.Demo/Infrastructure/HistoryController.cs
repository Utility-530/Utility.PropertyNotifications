using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Utility.Commands;
using Utility.Enums;
using Utility.Interfaces.NonGeneric;
using Utility.Infrastructure;
using System.Windows.Input;
using Utility.PropertyTrees.Infrastructure;
using Utility.Models;
using Utility.Collections;

namespace Utility.PropertyTrees.WPF.Demo
{
    public class HistoryController : BaseObject, IObserver
    {
        private Step enabled;

        public HistoryController()
        {
            Command = new Command<Step>(step =>
            {
                switch (step)
                {
                    case Step.None:
                        break;
                    case Step.Backward:
                        Broadcast(Enums.Playback.Back);
                        break;
                    case Step.Forward:
                        Broadcast(Enums.Playback.Forward);
                        break;
                    case Step.Walk:
                        Broadcast(Enums.Playback.Play);
                        Enabled = Step.Wait;
                        break;
                    case Step.Wait:
                        Broadcast(Enums.Playback.Pause);
                        OnNext(default);
                        break;
                    case Step.All:
                        break;
                }
            });
        }

        public override Models.Key Key => new(Guid, nameof(HistoryController), typeof(HistoryController));

        public Guid Guid => Guid.Parse("9dde99db-73b6-4cdc-974e-615ece9b4806");

        public Step Enabled { get => enabled; set => Set(ref enabled, value); }

        public ICommand Command { get; }

        private Dictionary<Enums.History, Collection> dictionary = new() { 
            { Enums.History.Future, new Collection() }, 
            { Enums.History.Present, new Collection() },
            { Enums.History.Past, new Collection() } };

        public Collection Past  => dictionary[Enums.History.Past];  
        public Collection Future => dictionary[Enums.History.Future];
        public Collection Present => dictionary[Enums.History.Present];

        public void OnNext(object value)
        {
            if (value is not ChangeSet { } changeSet)
            {
                throw new Exception("ujuj  sdsdf");
            }

            foreach (var item in changeSet)
            {
                if (item is not Change { Type: var type, Value: HistoryOrder { History: var history, Order: var order } })
                {
                    throw new Exception("22 j  sdsdf");
                }
                switch (type)
                {
                    case ChangeType.Add:
                        dictionary[history].Add(order);
                        break;
                    case ChangeType.Remove:
                        dictionary[history].Remove(order);
                        break;
                    case ChangeType.Update:
                        throw new Exception("ggv 4 sdsss");
                }
            }

            Enabled = Steps().Aggregate((x, y) => x |= y);

            IEnumerable<Step> Steps()
            {
                if (dictionary[Enums.History.Future].Count > 0)
                {
                    yield return Step.Walk;
                    yield return Step.Forward;
                }
                if (dictionary[Enums.History.Past].Count > 0)
                    yield return Step.Backward;
            }
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }


    }

    public class HeaderedList
    {
        public HeaderedList() { }

        public string Header { get; set; }

        public Collection Collection { get; } = new Collection();
    }
 
}
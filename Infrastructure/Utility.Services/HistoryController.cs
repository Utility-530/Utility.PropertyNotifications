
using System.Reactive.Linq;
using Utility.Collections;
using Utility.Models;
using h = Utility.Enums.History;
using Response = Utility.Models.Response;
using System.Reactive.Disposables;
using static System.Reactive.Linq.Observable;
using Utility.Infrastructure;

namespace Utility.Services
{
    public class History : BaseObject
    {
        private HistoryViewModel model = new();

        public History()
        {

        }

        public override Key Key => new(Guids.History, nameof(History), typeof(History));

        public override object? Model => model;

        public IObservable<HistoryResponse> OnNext(ForwardEvent forwardEvent)
        {
            return Create<HistoryResponse>(observer =>
            {
                //if (model.Future.Any() == false)
                //    yield break;
                var order = model.Future[0];
                if (model.Present.Count > 0)
                {
                    model.Past.Add(model.Present[0]);
                }
                if (model.Present.Count > 0)
                {
                    model.Present.RemoveAt(0);
                }

                model.Present.Add(order);
                model.Future.Remove(order);
                observer.OnNext(new HistoryResponse(forwardEvent));
                observer.OnCompleted();
                return Disposable.Empty;
            });
        }

        public IObservable<HistoryResponse> OnNext(BackEvent backEvent)
        {
            return Create<HistoryResponse>(observer =>
            {
                model.IsDirty = true;
                var order = model.Past[^1];
                if (model.Present.Count > 0)
                {
                    model.Future.Insert(0, model.Present[0]);
                }
                if (model.Present.Count > 0)
                {
                    model.Present.RemoveAt(0);
                }

                model.Present.Add(order);
                model.Past.Remove(order);

                observer.OnNext(new HistoryResponse(order));
                observer.OnCompleted();
                return Disposable.Empty;
            });
        }


        public void OnNext(HistoryRequest request)
        {
            //return Create<HistoryResponse>(observer =>
            // {
            if (model.IsDirty)
            {
                model.Future.Clear();
            }

            if (model.Future.Any(a => a.Equals(request)))
            {
                return;// Disposable.Empty;
            }

            model.Future.Add(request);
        }
    }


    public class HistoryViewModel : BaseViewModel
    {
        public HistoryViewModel()
        {
        }



        private readonly Dictionary<h, Collection> dictionary = new() {
            { h.Future, new Collection() },
            { h.Present, new Collection() },
            { h.Past, new Collection() } };

        public Collection Past => dictionary[h.Past];
        public Collection Future => dictionary[h.Future];
        public Collection Present => dictionary[h.Present];

        public bool IsDirty { get; internal set; }

        //public IObservable<Response> OnNext(ChangeSet changeSet)
        //{
        //    foreach (var item in changeSet)
        //    {
        //        if (item is not Change { Type: var type, Value: HistoryOrder { History: var history, Order: var order } })
        //        {
        //            throw new Exception("22 j  sdsdf");
        //        }
        //        switch (type)
        //        {
        //            case ChangeType.Add:
        //                dictionary[history].Add(order);
        //                break;
        //            case ChangeType.Remove:
        //                dictionary[history].Remove(order);
        //                break;
        //            case ChangeType.Update:
        //                throw new Exception("ggv 4 sdsss");
        //        }
        //    }
        //    var steps = Steps().ToArray();
        //    if (steps.Length > 0)
        //        Enabled = steps.Aggregate((x, y) => x |= y);

        //    return Return(new Unit());

        //    IEnumerable<Step> Steps()
        //    {
        //        if (dictionary[h.Future].Count > 0)
        //        {
        //            yield return Step.Walk;
        //            yield return Step.Forward;
        //        }
        //        if (dictionary[h.Past].Count > 0)
        //            yield return Step.Backward;
        //    }
        //}
    }

    public record PlaybackEvent(Enums.Playback Playback) : Event();
    public record BackPlaybackEvent() : PlaybackEvent(Enums.Playback.Back);
    public record ForwardPlaybackEvent() : PlaybackEvent(Enums.Playback.Forward);
    public record PlayPlaybackEvent() : PlaybackEvent(Enums.Playback.Play);
    public record PausePlaybackEvent() : PlaybackEvent(Enums.Playback.Pause);

    public record HistoryResponse(object Current) : Response(Current);

    public class HeaderedList
    {
        public HeaderedList() { }

        public string Header { get; set; }

        public Collection Collection { get; } = new Collection();
    }
}
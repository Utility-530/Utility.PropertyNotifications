using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Utility.Helpers;
using Utility.Repos;

namespace Utility.Trees.Demo.MVVM.Infrastructure
{
    public class PipeRepository : TreeRepository
    {
        Dictionary<Guid, Observable2<DateValue?>> dictionary = new();

        protected PipeRepository(string? dbDirectory = null) : base(dbDirectory)
        {
            Predicate = new StringDecisionTree(new Decision(item => (Guid)item != null) { })
                {
                    new StringDecisionTree(new Decision<Guid>(item => item == null), md=>"B"),
                    new StringDecisionTree(new Decision<Guid>(item => true){  }, md=>"A"),

                };
        }

        public override IObservable<DateValue?> Get(Guid guid)
        {
            Pipe.Instance.Queue(new QueueItem(guid));
            var observable = dictionary.Get(guid, a => new Observable2<DateValue?>());
            //Pipe.Instance.Queue(new QueueItem(guid));
            return observable;
        }

        public DecisionTree Predicate { get; set; }

        public void Select(Guid guid)
        {
            //if (item is TreeViewItem _item)
            //{
            Predicate.Reset();
            Predicate.Input = guid;
            Predicate.Evaluate();
            var observable = dictionary.Get(guid, a => new Observable2<DateValue?>());
            if (Predicate.Backput is string s)
            {
                if (s == "A")
                {
                    base.Get(guid).Subscribe(a => observable.OnNext(a));
                }
                else if (s == "B")
                {

                }
            }
            //}
            //return null;
        }

        public static PipeRepository Instance2 { get; } = new(DataPath);
    }
}

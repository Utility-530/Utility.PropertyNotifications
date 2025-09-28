using System.Collections.ObjectModel;

namespace Simple.Models
{
    public class Model
    {
        private SynchronizationContext current;

        public ObservableCollection<IChange> Changes { get; } = new();
        public List<IChange> PreChanges { get; } = new();

        public Model()
        {
            current = SynchronizationContext.Current;
        }



        public void AddChange<T>(T change) where T : IChange
        {
            PreChanges.Add(change);
            current.Post(_ =>
            {
                if (Changes.OfType<T>().LastOrDefault()?.Equals(change) == true)
                {
                    return;
                }
                PreChanges.Remove(change);
                Changes.Add(change);
            }, null);
        }

        public string String<T>() where T : IStringChange => PreChanges.Concat(Changes).ToArray().OfType<T>().LastOrDefault().Value;
        public object Object<T>() where T : IObjectChange => PreChanges.Concat(Changes).ToArray().OfType<T>().LastOrDefault().Value;

        public int Int<T>() where T : IIntChange => PreChanges.Concat(Changes).ToArray().OfType<T>().LastOrDefault().Value;

        public bool Bool<T>() where T : IBooleanChange => PreChanges.Concat(Changes).ToArray().OfType<T>().LastOrDefault().Value;

        public Dictionary<string, object> Dictionary<T>()
        {
            Dictionary<string, object> _dictionary = new Dictionary<string, object>();
            foreach (var change in Changes.ToArray().OfType<T>())
            {
                if (change is IDictionaryAddChange unac)
                {
                    _dictionary[unac.Key] = unac.Value;
                }
                if (change is IDictionaryRemoveChange re)
                {
                    _dictionary.Remove(re.Key);
                }
            }
            return _dictionary;
        }

        public static Model Instance { get; } = new Model();
    }


}

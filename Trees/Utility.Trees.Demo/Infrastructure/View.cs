using System;
using System.Linq;
using Utility.Trees;
namespace Utility.Trees.Demo
{
    //public class View : Tree, IObserver<Change<View>>
    //{
    //    private readonly IService service;


    //    //private ObservableCollection<View> children = new();
    //    private bool isExpanded;
    //    private IDisposable disposable;

    //    public View(IService service, object data, Guid key) : base(data)
    //    {
    //        this.service = service;
    //        this.Key = key;
    //    }

    //    public bool IsExpanded
    //    {
    //        get { return isExpanded; }
    //        set
    //        {
    //            if (isExpanded != value)
    //            {
    //                isExpanded = value;
    //                if (value)
    //                    disposable = service.Subscribe(Key, this);
    //                else
    //                {
    //                    disposable.Dispose();
    //                    m_items.Clear();
    //                }
    //            }
    //        }
    //    }

    //    protected override ITree CloneNode(ITree item)
    //    {
    //        return new View(this.service, item.Data, this.Key) { State = item.State };
    //    }

    //    public override bool HasItems => service.HasItems(this.Key);

    //    public void OnCompleted()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void OnError(Exception error)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void OnNext(Change<View> value)
    //    {
    //        switch (value.Type)
    //        {
    //            case ChangeType.Insert:
    //                m_items.Add(value.Value);
    //                break;
    //            case ChangeType.Remove:
    //                m_items.Remove(value.Value);
    //                break;
    //            case ChangeType.Update:

    //                if (m_items.OfType<ITree>().SingleOrDefault(a => a.Key == (value.Key as Key)?.Guid) is ITree single)
    //                    m_items.Remove(single);
    //                m_items.Add(value.Value);
    //                break;
    //            default:
    //                throw new Exception("11 rge");
    //        }
    //    }
    //}
}

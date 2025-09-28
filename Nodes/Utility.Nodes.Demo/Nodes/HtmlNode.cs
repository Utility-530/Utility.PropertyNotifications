using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Utility.Keys;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Demo
{
    internal class HtmlNode : NodeViewModel<object>
    {
        private IElement type;
        bool flag;

        public HtmlNode(IElement type)
        {
            this.type = type;
            RaisePropertyChanged(nameof(Data));
        }

        public override object Data
        {
            get
            {
                if (type != null)
                {
                    return type;
                }
                return null;
            }
        }

        public override string Key => new StringKey(type.TagName);

        public override System.IObservable<object?> GetChildren()
        {
            flag = true;
            return Observable.Create<object>(observer =>
            {
                foreach (var child in type.Children)
                    observer.OnNext(child);
                return Disposable.Empty;
            });

        }

        public override string ToString()
        {
            return type.ClassName;
        }

        public override Task<ITree> ToTree(object value)
        {
            return Task.FromResult<ITree>(new HtmlNode(value as IElement));
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }
    }
}

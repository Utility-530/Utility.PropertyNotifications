using AngleSharp.Dom;
using AngleSharp;
using AngleSharp.Html.Dom;
using Utility.Trees.Abstractions;
using Utility.Trees.Extensions.Async;
using System.Reactive.Linq;
using Utility.Nodes.Filters;
using AngleSharp.Html;
using System.IO;
using Utility.PropertyNotifications;
using Utility.PropertyDescriptors;
using Utility.Models.Trees;
using Splat;
using Utility.Interfaces.NonGeneric;
using Utility.Interfaces;
using Chronic;
using Utility.Observables;
using Utility.Observables.Generic;
using Utility.Interfaces.Generic;
using INode = Utility.Interfaces.Exs.INode;


namespace Utility.Nodes.Demo.Filters.Services
{
    internal class ParserService : Disposable
    {
        const string htmlContent = @"
<!DOCTYPE html>
<html>
<head>
<title>Page Title</title>
</head>
<body/>
</html>";

        public ParserService()
        {
            Locator.Current.GetService<IObservableIndex<INode>>()[nameof(NodeMethodFactory.BuildContentRoot)]
                .CombineLatest(Locator.Current.GetService<IObservableIndex<INode>>()[nameof(NodeMethodFactory.BuildHtmlRoot)])
                .Subscribe(nodes =>
                {
                    var (node, htmlNode) = nodes;

                    htmlNode.WithChangesTo(a => a.Data).Subscribe(data =>
                    {
                        if (data is HtmlModel stringModel)
                        {
                            AddElementByPositionAsync(node)
                            .Subscribe(html =>
                            {
                                stringModel.Set(html);
                            }).DisposeWith(this);

                            ControlsService.Instance
                            .Where(a => a.ControlEventType == ControlEventType.Refresh)
                            .Subscribe(a =>
                            {
                                stringModel.Set(AddElementByPosition(node));
                            }).DisposeWith(this);
                        }
                    }).DisposeWith(this);
                }).DisposeWith(this);
        }


        public static string AddElementByPosition(IReadOnlyTree node)
        {
            IDocument document = createDocument();

            Dictionary<IReadOnlyTree, IElement> dictionary = new();

            Utility.Trees.Extensions.Match.SelfAndDescendants(node)
                .ForEach(n =>
                {
                    document = create(dictionary, node, n, document);
                });
            return ToString(document);
        }


        public static IObservable<string> AddElementByPositionAsync(IReadOnlyTree node)
        {
            IDocument document = createDocument();
            Dictionary<IReadOnlyTree, IElement> dictionary = new();

            return Observable.Create<string>(obs =>
            {
                return node.SelfAndDescendants()
                    .Where(c => c.Type == Changes.Type.Add)
                    .Select(c => c.NewItem)
                    .Subscribe(n =>
                {
                    document = create(dictionary, node, n, document);
                    obs.OnNext(ToString(document));
                });
            });
        }

        private static IDocument createDocument()
        {
            var context = BrowsingContext.New(Configuration.Default.WithCss());
            var document = context.OpenAsync(req => req.Content(htmlContent)).Result;

            if (true)
                addStyle(document);
            return document;
        }

        private static string ToString(IDocument document)
        {
            var sw = new StringWriter();
            document.ToHtml(sw, new PrettyMarkupFormatter());
            var HTML_prettified = sw.ToString();
            return HTML_prettified;
        }


        static IDocument create(Dictionary<IReadOnlyTree, IElement> dictionary, IReadOnlyTree node, IReadOnlyTree n, IDocument document)
        {

            IElement newElement;

            if (n != node && dictionary.ContainsKey(n.Parent) == false)
            {
                return document;
            }
            if (n is Utility.Interfaces.Exs.INode _node)
            {
                if (_node.IsVisible == false)
                    return document;

                if (_node.Removed != null)
                    return document;
            }


            if (n.Data is ICollectionDescriptor collectionDescriptor)
            {
                newElement = document.CreateElement<IHtmlTableElement>();

            }
            else if (n.Data is ICollectionHeadersDescriptor headersDescriptor)
            {
                newElement = document.CreateElement<IHtmlTableRowElement>();

            }
            else if (n.Data is IHeaderDescriptor headerDescriptor)
            {
                newElement = document.CreateElement<IHtmlTableHeaderCellElement>();
                newElement.TextContent = headerDescriptor.Name;

            }
            else if (n.Parent?.Data is ICollectionDescriptor collectionItemDescriptor)
            {
                if (n.Data is IReferenceDescriptor)
                    newElement = document.CreateElement<IHtmlTableRowElement>();
                else
                    throw new Exception(" s33 sdsd");
                //newElement = document.CreateElement<IHtmlTableDataCellElement>();
            }
            else if (n.Data is IPropertiesDescriptor prsdescriptor)
            {
                dictionary[n] = dictionary[n.Parent];
                return document;
            }
            else if (n.Parent?.Data is IPropertiesDescriptor _prsdescriptor && n.Data is IValueDescriptor descriptor1)
            {
                newElement = document.CreateElement<IHtmlTableDataCellElement>();
                newElement.TextContent = (descriptor1 as IValue).Value?.ToString();
            }
            else
            {
                newElement = document.CreateElement<IHtmlDivElement>(); // Create a new element
            }

            //else if(n.Data is ICollectionItemDescriptor {  ParentType } collectionItemDescriptor)
            //{
            //    newElement = document.CreateElement<IHtmlTableDataCellElement>();

            //}



            newElement.ClassName = n.Data.ToString();
            dictionary[n] = newElement;



            if (n.Parent == null)
            {
                document.Body.AppendChild(newElement);
            }
            else if ((dictionary.TryGetValue(n.Parent, out var elem)))
            {
                elem.AppendChild(newElement);
            }
            else
            {
                throw new Exception("r g4344");
                //body.AppendChild(newElement);
            }

            if (n.Data is Utility.Interfaces.NonGeneric.IDescriptor descriptor)
            {
                var key = StyleSelector.Instance.SelectKey(n);

                //if (descriptor is ICollectionItemReferenceDescriptor)
                //{
                //}
                //else if (n.Parent?.Data is ICollectionItemReferenceDescriptor)
                //{
                //}

                if (n.Parent?.Data is IPropertiesDescriptor _prsdescriptor && n.Data is IValueDescriptor descriptor1)
                {

                }
                else if (descriptor is Utility.Interfaces.NonGeneric.IValue { Value: var value })
                {
                    var innerElement = create(value);
                    newElement.AppendChild(innerElement);
                }
                else if (n.Parent.Data is ICollectionHeadersDescriptor headersDescriptor)
                {

                }
                //else if(n.Data is IHeaderDescriptor header)
                //{
                //    var p = document.CreateElement<IHtmlParagraphElement>();
                //    newElement.AppendChild();
                //}
                else if (descriptor is IReferenceDescriptor iRef && n.Parent.Data is not ICollectionDescriptor)
                {
                    //var index = (n as IIndex).Index.Local.ToString();
                    var x = (n as ITree).Index.ToString().Split('.').Length;
                    var p = document.CreateElement("h" + x.ToString());

                    p.TextContent = iRef.Name;
                    newElement.AppendChild(p);
                }
                else if (descriptor is IPropertiesDescriptor prd && n.Parent.Data is not ICollectionDescriptor)
                {
                    //var p = document.CreateElement<IHtmlParagraphElement>();
                    //p.TextContent = iRef.Name;
                    //newElement.AppendChild(p);
                }

                else
                {

                    //var p = document.CreateElement<IHtmlParagraphElement>();
                    //p.TextContent = n.Data.ToString();
                    //newElement.AppendChild(p);
                    //body.AppendChild(newElement);
                }
            }

            return document;

            IElement create(object value)
            {
                switch (value)
                {
                    case string str:
                        var p = document.CreateElement<IHtmlParagraphElement>();
                        p.TextContent = str;
                        return p;
                    case bool b:
                        var input = document.CreateElement<IHtmlInputElement>();
                        input.IsChecked = b;
                        input.Type = "submit";
                        return input;

                }

                return document.CreateElement<IHtmlParagraphElement>();
                //throw new NotImplementedException("erew33111");
            }
        }







        static void addStyle(IDocument document)
        {
            var style = document.CreateElement<IHtmlStyleElement>();
            // note: if you have the CSS from a URL; choose IHtmlLinkElement instead
            style.TextContent = File.ReadAllText("../../../CSS/sakura.css");
            // @"#styleme { color:blue; background-color: gray; }";
            document.Head.AppendChild(style);
        }
    }
}

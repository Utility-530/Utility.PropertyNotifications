using AngleSharp.Dom;
using AngleSharp;
using AngleSharp.Html.Dom;
using Utility.Trees.Abstractions;
using Utility.Extensions;
using System.Reactive.Linq;
using Utility.Nodes.Filters;
using AngleSharp.Html;
using System.IO;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyNotifications;
using Utility.Trees.Demo.Filters;
using Utility.Descriptors;
using Utility.Models.Trees;

namespace Utility.Nodes.Demo.Filters.Services
{
    internal class ParserService
    {
        public ParserService()
        {
            NodeSource.Instance.Single(nameof(Factory.BuildContentRoot))
                .Subscribe(node =>
                {
                    NodeSource.Instance.Single(nameof(Factory.BuildHtmlRoot))
                    .Subscribe(htmlNode =>
                    {
                        if (htmlNode.Data is StringModel stringModel)
                        {
                            NodeSource.Instance.Single(nameof(Factory.BuildHtmlRenderRoot))
                            .Subscribe(_node =>
                            {
                                if (_node.Data is HtmlModel _stringModel)
                                {
                                    stringModel
                                    .WithChangesTo(a => a.Value)
                                    .Subscribe(a =>
                                    {
                                        _stringModel.Value = a;
                                    });

                                }
                            });

                            AddElementByPositionAsync(node)
                                .Subscribe(html =>
                                {
                                    stringModel.Value = html;
                                });

                        }

                    });
                });

        }

        const string htmlContent = @"
<!DOCTYPE html>
<html>
<head>
<title>Page Title</title>
</head>
<body>

<!--<h1>This is a Heading</h1>
<p>This is a paragraph.</p>-->

</body>
</html>";

        public static IObservable<string> AddElementByPositionAsync(IReadOnlyTree node)
        {
            var context = BrowsingContext.New(Configuration.Default);
            var document = context.OpenAsync(req => req.Content(htmlContent)).Result;

            return Observable.Create<string>(obs =>
            {
                Dictionary<IReadOnlyTree, IElement> dictionary = new();

    

                // Ensure we have a root element (usually the <html> element).
                IElement body = document.Body;


                // Add a new element (e.g., <div>) to the body, at the target index.
                //newElement.TextContent = $"New {tagNameToAdd} at position {targetIndex}"; // Set content for the new element

                // Get all child elements of the body.
                var childElements = body.Children;

                // Check if the target index is within range.
                return node.SelfAndDescendantsAsync()
                    .Where(c => c.Type == Changes.Type.Add)
                    .Select(c => c.NewItem)
                    .Subscribe(n =>
                {
                    IElement newElement;

                    newElement = document.CreateElement<IHtmlDivElement>(); // Create a new element

                    if (n.Data is Utility.Interfaces.NonGeneric.IValue { Value: var value } descriptor)
                    {
                        var key = StyleSelector.Instance.SelectKey(n);

                        if (descriptor is IReferenceDescriptor iRef)
                        {
                            var p = document.CreateElement<IHtmlParagraphElement>();
                            p.TextContent = iRef.Name;
                            newElement.AppendChild(p);
                        }
                        else
                        {
                            var innerElement = create(value);        
                            newElement.AppendChild(innerElement);
                        }
                    }
                    else
                    {
                        
                    }

                    if (n.Parent == null)
                    {
                        body.AppendChild(newElement);
                    }
                    else if ((dictionary.TryGetValue(n.Parent, out var elem)))
                    {
                        elem.AppendChild(newElement);
                    }
                    else
                    {
                        body.AppendChild(newElement);
                    }
                    newElement.ClassName = n.Data.ToString();
                    dictionary[n] = newElement;

                    var sw = new StringWriter();
                    document.ToHtml(sw, new PrettyMarkupFormatter());

                    var HTML_prettified = sw.ToString();
                    obs.OnNext(HTML_prettified);
                });
            });


            IElement create(object value)
            {
                switch (value)
                {
                    case string str:
                        var p = document.CreateElement<IHtmlParagraphElement>();
                        p.TextContent = str;
                        return p;

                }

                return document.CreateElement<IHtmlParagraphElement>(); 
                //throw new NotImplementedException("erew33111");
            }
        }
    }
}

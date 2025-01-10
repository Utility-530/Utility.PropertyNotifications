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

<h1>This is a Heading</h1>
<p>This is a paragraph.</p>

</body>
</html>";

        public static IObservable<string> AddElementByPositionAsync(IReadOnlyTree node)
        {
            return Observable.Create<string>(obs =>
            {
                Dictionary<IReadOnlyTree, IElement> dictionary = new();

                // Parse the HTML content into an AngleSharp document.
                var context = BrowsingContext.New(Configuration.Default);
                var document = context.OpenAsync(req => req.Content(htmlContent)).Result;

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
                    if (n.Data is Utility.Interfaces.Generic.IValue<string> { Value: var value } descriptor)
                    {
                        newElement = document.CreateElement<IHtmlDivElement>(); // Create a new element
                        var p = document.CreateElement<IHtmlParagraphElement>();
                        p.TextContent = value;
                        newElement.AppendChild(p);
                    }
                    else
                    {
                        newElement = document.CreateElement<IHtmlDivElement>(); // Create a new element
                    }

                    if (n.Parent == null)
                    {
                        body.AppendChild(newElement);
                        //Console.WriteLine("Index out of range, element appended to the end.");
                    }
                    else if ((dictionary.TryGetValue(n.Parent, out var elem)))
                    {
                        elem.AppendChild(newElement);
                        //// Insert the new element at the target index.
                        //body.InsertBefore(newElement, childElements[targetIndex]);
                        //Console.WriteLine($"Element inserted at index {targetIndex}");
                    }
                    else
                    {
                        // Append to the end if the index is out of range.
                        body.AppendChild(newElement);
                        //Console.WriteLine("Index out of range, element appended to the end.");
                    }
                    newElement.ClassName = ((IName)n).Name;
                    dictionary[n] = newElement;

                    var sw = new StringWriter();
                    document.ToHtml(sw, new PrettyMarkupFormatter());

                    var HTML_prettified = sw.ToString();
                    obs.OnNext(HTML_prettified);
                });
            });

        }
    }
}

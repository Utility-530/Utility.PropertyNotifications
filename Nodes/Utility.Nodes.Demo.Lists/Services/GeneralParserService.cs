using System.IO;
using System.Reactive.Linq;
using AngleSharp;
using AngleSharp.Css.Dom;
using AngleSharp.Dom;
using AngleSharp.Html;
using AngleSharp.Html.Dom;
using Chronic;
using Splat;
using Utility.Changes;
using Utility.Entities.Comms;
using Utility.Enums;
using Utility.Enums.Attributes;
using Utility.Enums.VisualElements;
using Utility.Interfaces;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Models.Templates;
using Utility.Nodes.Meta;
using Utility.Observables;
using Utility.Observables.Generic;
using Utility.PropertyDescriptors;
using Utility.Reactives;
using Utility.Services.Meta;
using Utility.Trees.Abstractions;
using Utility.Trees.Extensions.Async;
using Utility.WPF.Trees;

namespace Utility.Nodes.Demo.Lists.Services
{
    internal record GeneralInNodeParam() : Param<GeneralParserService>(nameof(GeneralParserService.AddElementByPositionAsync), "node");
    internal record GeneralOutStringParam() : Param<GeneralParserService>(nameof(GeneralParserService.AddElementByPositionAsync));

    internal class GeneralParserService : Disposable
    {

        public static IObservable<string> AddElementByPositionAsync(IReadOnlyTree node)
        {
            IDocument document = createDocument();
            Dictionary<IReadOnlyTree, IElement> dictionary = new();


            return node.SelfAndDescendants()
                .Select(n =>
                {
                    var _document = create(dictionary, node, n, document);
                    var text = ToString(_document);
                    return text;
                });



            static IDocument create(Dictionary<IReadOnlyTree, IElement> dictionary, IReadOnlyTree node, IReadOnlyTree n, IDocument document)
            {

                IElement treeElement = null;
                IElement styleElement = null;

                if (n is IGetParent<IReadOnlyTree> { Parent: { } parent })
                    if (n != node && dictionary.ContainsKey(parent) == false)
                    {
                        return document;
                    }
                if (n is not INodeViewModel _node)
                {
                    throw new Exception("£$DDDf3d23");
                }


                if (_node.Removed != null)
                    return document;

                if (_node.IsProliferable)
                {
                    HTML? treeMap = _node.Style == 0 ? HTML.div : UIElementExtensions.Map<HTML>(_node.Layout);
                    treeElement = document.CreateElement(treeMap.HasValue ? treeMap.ToString() : "div");
                    treeElement.ClassName = _node.Layout.ToString() + " " + _node.Name();
                }

                if (_node.Style != default && _node.Value() is { } value)
                {
                    HTML? map = _node.Style == 0 ? HTML.div : UIElementExtensions.Map<HTML>(_node.Style);
                    if (map.HasValue == false)
                        throw new Exception("DS£!!Ddf");
                    styleElement = document.CreateElement(map.Value.ToString());
                    styleElement.ClassName = _node.Style.ToString() +
                        (_node.DataTemplate != default ? " " + _node.DataTemplate?.ToString() : string.Empty) +
                        (_node.IsVisible == false ? " hidden" : string.Empty);


                    if (styleElement is IHtmlInputElement input)
                        input.Value = value?.ToString() ?? default;
                    else if (styleElement is IHtmlImageElement img)
                        img.Source = value?.ToString() ?? default;
                    else
                        styleElement.TextContent = value?.ToString() ?? default;

                }

                if (treeElement != null && styleElement != null)
                {
                    dictionary[n] = treeElement;
                    treeElement.AppendChild(styleElement);
                }
                else if (treeElement != null)

                    dictionary[n] = treeElement;

                else if (styleElement != null)
                    dictionary[n] = styleElement;
                else
                    return document;

                if (dictionary.Count == 1)
                {
                    document.Body.AppendChild(dictionary[n]);
                }
                else if ((dictionary.TryGetValue((n as IGetParent<IReadOnlyTree>).Parent, out var elem)))
                {
                    elem.AppendChild(dictionary[n]);
                }
                else
                {
                    throw new Exception("r g4344");
                    //body.AppendChild(newElement);
                }



                return document;


            }


            static IDocument createDocument()
            {
                const string htmlContent = @"
<!DOCTYPE html>
<html>
<head>
<title>Page Title</title>
</head>
<body/>
</html>";

                var context = BrowsingContext.New(Configuration.Default.WithCss());
                var document = context.OpenAsync(req => req.Content(htmlContent)).Result;

                if (true)
                    addStyle(document);
                return document;
            }


            static string ToString(IDocument document)
            {
                var sw = new StringWriter();
                document.ToHtml(sw, new PrettyMarkupFormatter());
                var HTML_prettified = sw.ToString();
                return HTML_prettified;
            }



            static void addStyle(IDocument document)
            {
                var style = document.CreateElement<IHtmlStyleElement>();
                // note: if you have the CSS from a URL; choose IHtmlLinkElement instead
                if (false)
                    style.TextContent = File.ReadAllText("../../../CSS/sakura.css");
                style.TextContent = File.ReadAllText("../../../CSS/custom.css");
                // @"#styleme { color:blue; background-color: gray; }";
                document.Head.AppendChild(style);
            }
        }
    }


}

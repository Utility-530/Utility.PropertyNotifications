using CustomModels;
using LanguageExt;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utility.Entities;
using Utility.Helpers.Reflection;
using Utility.Interfaces.Exs;
using Utility.Models;
using Utility.Models.Trees;
using Utility.PropertyNotifications;
using Utility.Services;


namespace Utility.Nodes.Demo.Lists.Services
{
    internal class NodeMethodFactory : INodeMethodFactory
    {
        //public const string controls = nameof(controls);
        public const string file_path = nameof(file_path);
        public const string html = nameof(html);
        public const string html1 = nameof(html1);
        public const string html2 = nameof(html2);
        public const string main = nameof(main);
        public const string edit = nameof(edit);

        INodeSource nodeSource = Locator.Current.GetService<INodeSource>();

        public IEnumerable<MethodInfo> Methods => this.GetType().InstantMethods();

        public IObservable<INode> BuildEbayRoot(Guid guid, Infrastructure.EbayModel value)
        {
            return nodeSource.Create(nameof(BuildEbayRoot),
                guid,
                s => new Node(s) { IsExpanded = true, Orientation = Enums.Orientation.Horizontal },
                s =>
                new Model(() => [
                    //new FilePathModel { Name = file_path },
                    new EditModel { Name = edit, Value = value },
                    new HtmlModel { Name = html },
                    new ReadOnlyStringModel(nodeAction: node=> node.DataTemplate = "HtmlEditor") { Name = html1 },
                    new ReadOnlyStringModel(nodeAction: node=> node.DataTemplate = "HtmlWebViewer") { Name = html2 },
                ],
                (node) => { },
                (parent, addition) =>
                {
                    if (addition.Data is EditModel { Value: Infrastructure.EbayModel model } editModel)
                    {
                        model.WithChangesTo(a => a.IndexPath).Subscribe(a =>
                        {
                            Locator.Current.GetService<RazorService>().OnNext(FilePath.FromFilePath(a));
                        });
                        model.WhenChanged().StartWith(default(PropertyChange)).Subscribe(a =>
                        {
                            Locator.Current.GetService<RazorService>().OnNext(new Instance(model));
                        });
                    }

                    if (addition.Data is StringModel { } stringModel)
                    {
                        Locator.Current.GetService<RazorService>().Subscribe(a => stringModel.Set(a.Output));
                    }
                    if (addition.Data is ReadOnlyStringModel { } rstringModel)
                    {
                        Locator.Current.GetService<RazorService>().Subscribe(a => rstringModel.Set(a.Output));
                    }
                })
                { Name = main });
        }
    }
}

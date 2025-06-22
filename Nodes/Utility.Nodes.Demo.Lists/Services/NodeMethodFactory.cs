using Splat;
using System.Reactive.Linq;
using System.Reflection;
using Utility.Entities;
using Utility.Helpers.Reflection;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Generic.Data;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Nodes.Demo.Lists.Entities;
using Utility.Nodes.Demo.Lists.Infrastructure;
using Utility.PropertyNotifications;
using Utility.Services;


namespace Utility.Nodes.Demo.Lists.Services
{
    internal class NodeMethodFactory : Utility.Interfaces.Generic.IEnumerableFactory<MethodInfo>
    {
        //public const string controls = nameof(controls);
        public const string file_path = nameof(file_path);
        public const string html = nameof(html);
        public const string html1 = nameof(html1);
        public const string html2 = nameof(html2);
        public const string main = nameof(main);
        public const string edit = nameof(edit);
        public const string search = nameof(search);
        public const string list = nameof(list);


        INodeSource nodeSource = Locator.Current.GetService<INodeSource>();

        public IEnumerable<MethodInfo> Create(object config) => this.GetType().InstantMethods().Where(a => a.Name != nameof(Create));

        public IObservable<INode> BuildEbayRoot(Guid guid)
        {
            return nodeSource.Create(nameof(BuildEbayRoot),
                guid,
                s => new Node(s) { IsExpanded = true, Orientation = Enums.Orientation.Horizontal },
                s =>
                new Models.Model(() => [
                    //new FilePathModel { Name = file_path },
                    new SearchModel() { Name = search },
                    new ListModel(typeof(EbayModel)) { Name = list },
                    new EditModel { Name = edit },
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
                        Locator.Current.GetService<SelectionService>().Subscribe(a => editModel.Set(a.Value));
                    }
                    if (addition.Data is ListModel { } listModel)
                    {
                        Locator.Current.GetService<CollectionCreationService>()
                        .Subscribe(a =>
                        {
                            listModel.Collection = a.Collection;
                        });
                        listModel.WithChangesTo(a => a.Value).Select(a => new SelectionInput(a)).Subscribe(Locator.Current.GetService<SelectionService>());
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

        public IObservable<INode> BuildUserProfileRoot(Guid guid)
        {
            return nodeSource.Create(nameof(BuildUserProfileRoot),
                guid,
                s => new Node(s) { IsExpanded = true, Orientation = Enums.Orientation.Horizontal },
                s =>
                new Models.Model(() => [
                     new SearchModel() { Name = html1 },
                     new ListModel(typeof(UserProfileModel)) { Name = list },
                     new EditModel { Name = edit },
                ],
                (node) => { node.ItemsPanelTemplate = "Stack"; },
                (parent, addition) =>
                {
                    if (addition.Data is EditModel { } editModel)
                    {
                        Locator.Current.GetService<SelectionService>().Subscribe(a => { editModel.Value = a.Value; editModel.RaisePropertyChanged(nameof(EditModel.Value)); });
                    }

                    if (addition.Data is SearchModel { } stringModel)
                    {
                        stringModel
                        .WhenReceivedFrom(a => a.Value, includeNulls: false)
                        .Subscribe(a =>
                        {
                            Locator.Current.GetService<FilterService>().OnNext(new Filter(a));
                        });
                        stringModel
                        .WithChangesTo(a => a.Value, includeNulls: false)
                        .Subscribe(a =>
                        {
                            Locator.Current.GetService<FilterService>().OnNext(new Filter(a));
                        });
                    }

                    if (addition.Data is ListModel { } listModel)
                    {
                        Locator.Current.GetService<CollectionViewService>()
                        .Subscribe(a =>
                        {
                            listModel.Collection = a.Collection;
                        });
                        listModel
                        .WhenReceivedFrom(a => a.Add, includeNulls: false)
                        .Subscribe(a =>
                        {
                            Locator.Current.GetService<CollectionCreationService>().OnNext(new Changes.Change(a, null, Changes.Type.Add));
                        });
                        listModel
                        .WhenReceivedFrom(a => a.Remove, includeNulls: false)
                        .Subscribe(a =>
                        {
                            Locator.Current.GetService<CollectionCreationService>().OnNext(new Changes.Change(a, null, Changes.Type.Remove));
                        });

                        listModel
                        .WhenReceivedFrom(a => a.Value, includeNulls: false)
                        .Select(a => new SelectionInput(a))
                        .Subscribe(Locator.Current.GetService<SelectionService>());
                    }
                })
                { Name = main });
        }

        public IObservable<INode> BuildDefaultRoot(Guid guid, IId<Guid> value)
        {
            return nodeSource.Create(nameof(BuildDefaultRoot),
                guid,
                s => new Node(s) { IsExpanded = true, Orientation = Enums.Orientation.Horizontal },
                s =>
                new Model(() => [
                    new ListModel(value.GetType()){Name = list},
                    new EditModel { Name = edit, Value = value },

                ],
                (node) => { },
                (parent, addition) =>
                {

                })
                { Name = main });
        }

    }
}

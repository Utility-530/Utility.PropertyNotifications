using Splat;
using Utility.Entities.Comms;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Nodes.Meta;
using Utility.PropertyNotifications;
using Utility.Repos;
using Utility.Structs;
using Utility.Extensions;
using Utility.Nodes.Demo.Filters.Services;
using System.Reactive.Linq;

namespace Utility.Nodes.Demo.Editor
{
    internal partial class NodeMethodFactory : EnumerableMethodFactory
    {
        private Guid guid = Guid.Parse("D4F6C8E2-3B7A-4F2E-9F1A-8C6D8E2B3A7C");
        private Guid subGuid = Guid.Parse("A1B2C3D4-E5F6-4789-ABCD-EF0123456789");
        private Guid f = Guid.Parse("12345678-90AB-CDEF-1234-567890ABCDEF");

        public const string Refresh = nameof(Refresh);
        public const string Run = nameof(Run);
        public const string Save = nameof(Save);
        public const string Save_Filters = nameof(Save_Filters);
        public const string Clear = nameof(Clear);
        public const string New = nameof(New);
        public const string Expand = nameof(Expand);
        public const string Collapse = nameof(Collapse);
        public const string Search = nameof(Search);
        public const string Next = nameof(Next);
        public const string Load = nameof(Load);
        public const string Select = nameof(Select);
        public const string Cancel = nameof(Cancel);
        public const string Master = nameof(Master);
        public const string Slave = nameof(Slave);
        public const string Commands = nameof(Commands);

        public IObservable<INodeViewModel> BuildContainer()
        {
            return nodeSource.Create(
                nameof(BuildContainer),
                subGuid,
               s =>

                new Model(() =>
                [
                    new Model(() =>
                    [
                        new DataFilesModel { Name = nameof(DataFilesModel), DataTemplate = "MasterTemplate" },
                        new Model(() => [new CommandModel<SaveEvent> { Name = Save }, new CommandModel<RefreshEvent> { Name = Refresh }, new CommandModel<RunEvent> { Name = Run }],
                        attach: n => { n.IsExpanded = true; n.Orientation = Enums.Orientation.Horizontal; })
                        { Name = Commands,
                        IsExpanded = true}
                    ], addition:a=>
                    {

                        if(a is DataFilesModel dfm)
                        {
                            dfm.WhenReceivedFrom(a => a.Current, includeNulls: false)
                            .OfType<DataFileModel>()
                            .Observe<ComboServiceInputParam, DataFileModel>();
                        }
                    })
                    {
                        Name = Master,
                        IsExpanded = true
                    },
                new Model(attach:a=>
                {
                    a.ReactTo<ComboServiceOutputParam, DataFileModel>(setAction: (a) =>
                    {
                        var repo = new TreeRepository(a.FilePath);
                        var nodeSource = new NodeEngine(repo);
                        nodeSource.Create(a.Alias, a.Guid, s => new Model() { Name = s })
                        .Subscribe(ca =>
                        {
                            a.Add(ca);
                        });
                    });
                })
                {
                    Name = Slave,
                    IsExpanded = true
                }])
                {
                    IsExpanded = true,
                    Name = s
                })
                ;
        }
    }
}
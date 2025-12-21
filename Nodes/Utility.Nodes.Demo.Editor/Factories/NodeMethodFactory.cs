using System.Reactive.Linq;
using System.Runtime.InteropServices;
using Optional;
using Utility.Changes;
using Utility.Entities.Comms;
using Utility.Extensions;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Nodes.Demo.Filters.Services;
using Utility.Nodes.Meta;
using Utility.PropertyNotifications;
using Utility.ServiceLocation;

namespace Utility.Nodes.Demo.Editor
{
    public class BuildAttribute : Attribute
    {
        public BuildAttribute(string guid, string? serviceConstructor = null)
        {
        }
    }

    internal partial class NodeMethodFactory : EnumerableMethodFactory
    {
        //private Guid guid = Guid.Parse("D4F6C8E2-3B7A-4F2E-9F1A-8C6D8E2B3A7C");
        private const string subGuid = "A1B2C3D4-E5F6-4789-ABCD-EF0123456789";
        private const string fileGuid = "12345678-90AB-CDEF-1234-567890ABCDEF";

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
        public const string Settings = nameof(Settings);
        public const string controls = nameof(controls);
        public const string Files = nameof(Files);
        public const string list = nameof(list);

        public static readonly Guid settingsRootGuid = Guid.Parse("a743505d-128c-466f-afdb-d7dd97e37a08");
        public static readonly Guid listRootGuid = Guid.Parse("7ed21df8-d54c-44e1-8d97-7fcc2591e6c1");

        [Utility.Attributes.GuidAttribute(subGuid)]
        public Model BuildContainer()
        {
            return
                new Model(() =>
                [
                    new Model(() =>
                    [
                        new Model(proliferation: () => new Model<string>()
                        {
                            Name = "File",
                            Guid = Guid.Parse(fileGuid),
                            DataTemplate ="StringTemplate",
                            SelectedItemTemplate="StringTemplate"
                        })
                        {
                            Name = nameof(Files),
                            IsProliferable = true,
                            DataTemplate = "InvisibleTemplate",
                            IsExpanded = true
                        },
                        new Model(() => [new CommandModel<SaveEvent> { Name = Save }, new CommandModel<RefreshEvent> { Name = Refresh }, new CommandModel<RunEvent> { Name = Run }],
                        attach: n => {
                            n.Orientation = Enums.Orientation.Horizontal; })
                        {
                            Name = Commands,
                            DataTemplate = "InvisibleTemplate",
                            IsExpanded = true
                        }
                        ], addition:a=>
                    {
                        if(a is Model { Name:  nameof(Files)} dfm)
                        {
                            dfm.WhenReceivedFrom(a => a.Current, includeNulls: false)
                            .OfType<Model<string>>()
                            .Observe<EngineServiceInputParam, Model<string>>();
                        }
                    })
                    {
                        Name = Master,
                        DataTemplate = "InvisibleTemplate",
                        IsExpanded = true
                    },
                    new Model(attach: attach =>
                    {
                        attach.ReactTo<ComboServiceOutputParam, Change<INodeViewModel>>(setAction: change =>
                        {
                            switch(change.Type)
                            {
                                case Utility.Changes.Type.Add:
                                    if(change.Value is NodeViewModel _nvm)
                                    {
                                        if(attach.Contains(_nvm) == false)
                                            attach.Add(_nvm);
                                    }
                                    break;
                                case Utility.Changes.Type.Remove:
                                    if(change.OldValue is NodeViewModel nvm)
                                    {
                                        attach.Remove(nvm);
                                    }
                                    break;
                                case Utility.Changes.Type.Reset:
                                    attach.Clear();
                                    break;
                            }
                        });
                    })
                    {
                        Name = Slave,
                        IsProliferable = true,
                        IsExpanded = true
                    }])
                {
                    Orientation = Enums.Orientation.Vertical,
                    IsExpanded = true,
                    Key = subGuid,
                    DataTemplate = "InvisibleTemplate"
                };
        }
    }
}
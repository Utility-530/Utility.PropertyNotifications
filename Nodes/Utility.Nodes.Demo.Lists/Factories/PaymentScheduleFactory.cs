
using System.Collections;
using System.Reactive.Linq;
using MimeKit;
using NodaTime.Extensions;
using Utility.Entities;
using Utility.Enums;
using Utility.Extensions;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Models;
using Utility.Models.Templates;
using Utility.Models.Trees;
using Utility.Nodes.Demo.Lists.Infrastructure;
using Utility.Nodes.Demo.Lists.Services;
using Utility.Nodes.Meta;
using Utility.PropertyNotifications;
using Utility.ServiceLocation;
using Utility.Services;
using Utility.Services.Meta;
using SFTemplates = Utility.Nodes.WPF.Templates.SyncFusion.Templates;

namespace Utility.Nodes.Demo.Lists.Factories
{
    internal partial class NodeMethodFactory : EnumerableMethodFactory
    {
        public INodeViewModel BuildPaymentScheduleRoot()
        {
            var guid = Guid.Parse(MetaDataFactory.paymentScheduleGuid);
            buildNetwork(guid);

            return
                new Model(() => [
                    new Model{ Name = "send", DataTemplate = Templates.ActionTemplate },
                    new ListModel(MetaDataFactory.personType) { Name = list, DataTemplate =  SFTemplates.SFGridTemplate},
                    new Model{ Name = "amount", DataTemplate = Templates.MoneyTemplate },
                    new Model{ Name = "dayOfWeek", DataTemplate = Templates.EnumTemplate, Value = DayOfWeek.Monday },
                    new Model<string>() { Name = summary, DataTemplate = Templates.HtmlWebViewer }
                ],
                (addition) =>
                {
                    if (addition is ListModel { } listModel)
                    {
                        listModel.ReactTo<ListCollectionViewReturnParam>(setAction: (a) => listModel.Collection = (IEnumerable)a, guid: guid);
                        listModel.Observe<SelectionParam>(guid);
                    }

                    if (addition is Model { Name: "send" } sendModel)
                    {
                        Observable.Create<MimeMessage>(observer =>
                        {
                            MimeMessage param = null;

                            Globals.Resolver
                            .Resolve<IServiceResolver>(guid.ToString())
                            .ReactTo<MimeMessageReturnParam, MimeMessage>(a => param = a);

                            return sendModel
                            .WhenReceivedFrom(a => a.Value)
                            .Where(a => a is true)
                            .Subscribe(a =>
                            {
                                if (param != null)
                                    observer.OnNext(param);
                            });
                        }).Observe<MimeMessageInputParam, MimeMessage>(guid: guid);
                    }
                    if (addition is Model { Name: "amount" } amountModel)
                    {
                        amountModel.Observe<AmountInGBPInputParam>(guid: guid);
                    }
                    if (addition is Model { Name: "dayOfWeek" } dayOfWeek)
                    {
                        dayOfWeek.Observe<DayOfWeekInputParam>(guid: guid);
                    }
                    if (addition is Model<string> { Name: summary } searchModel)
                    {
                        searchModel.ReactTo<ScheduleOutputParam>(setAction: a =>
                        {
                            searchModel.Value = EmailReminderService.ToBody((Schedule)a);
                            searchModel.RaisePropertyChanged(nameof(Model.Value));
                        }, guid: guid);
                    }
                },
                attach: (node) => { node.IsExpanded = true; node.Orientation = Orientation.Vertical; })
                { Name = main, Guid = guid };

            static void buildNetwork(Guid guid)
            {
                var serviceResolver = Globals.Resolver.Resolve<IServiceResolver>(guid.ToString());
                serviceResolver.Connect<ScheduleOutputParam, ScheduleInputParam>();
                //serviceResolver.Connect<MimeMessageReturnParam, MimeMessageInputParam>();
                serviceResolver.Connect<ListInstanceReturnParam, ListInParam>();
                serviceResolver.Connect<ListInstanceReturnParam, PeopleInputParam>();
                serviceResolver.Connect<ListInstanceReturnParam, PeopleInputCreatePaymentParam>();
            }
        }
    }
}
using Splat;
using System.Collections;
using System.Reactive.Linq;
using Utility.Entities;
using Utility.Interfaces.Reactive.NonGeneric;
using Utility.Models;
using Utility.Reactives;
using Utility.Services.Meta;

namespace Utility.Nodes.Demo.Lists.Services
{
    public record ValueListInputParam() : Param<AssetValueService>(nameof(AssetValueService.Execute), "list");
    public record ValueListReturnParam() : Param<AssetValueService>(nameof(AssetValueService.Execute));

    public class AssetValueService
    {
        public IObservable<decimal> Execute(IList list)
        {
            decimal totalValue = 0;

            return Observable.Create<decimal>(observer =>
            {
                return list.AndChanges<Asset>(includeInitial: false).Subscribe(async a =>
                {
                    foreach (var item in a)
                    {
                        if (item.Type == Changes.Type.Add)
                        {
                            if (item.Value is { AssetType: AssetType.PreciousMetals, Name: { } name, Count: { } count, Quantity: { } quantity, Quality: { } quality, QuantityUnit: { } quantityUnit, End: var end } asset)
                            {
                                double price = 0;
                                var service = Locator.Current.GetService<Utility.API.Services.GoldApi>() ?? throw new Exception("S D££D");
                                var task = await service.Task;
                                if (name == Utility.API.Services.GoldApi.Gold)
                                {
                                    price = (double)(quantityUnit switch { Utility.API.Services.GoldApi.GramUnit => service.GoldPricePerGram, Utility.API.Services.GoldApi.OunceUnit => service.GoldPricePerOunce });
                                }
                                if (name == Utility.API.Services.GoldApi.Silver)
                                {
                                    price = (double)(quantityUnit switch { Utility.API.Services.GoldApi.GramUnit => service.SilverPricePerGram, Utility.API.Services.GoldApi.OunceUnit => service.SilverPricePerOunce });
                                }
                                asset.Value = (decimal)(count * quality / 1000d * quantity * price);
                                asset.RaisePropertyChanged(nameof(Asset.Value));
                                asset.Gain ??= asset.Value - asset.Cost;
                                if (end == null)
                                    totalValue += asset.Value;
                            }
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                    observer.OnNext(totalValue);
                });
            });
        }
    }
}
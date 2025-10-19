using Splat;
using System.Collections;
using Utility.Entities;
using Utility.Models;
using Utility.Services.Meta;

namespace Utility.Nodes.Demo.Lists.Services
{
    public record ValueListInputParam() : Param<AssetValueService>(nameof(AssetValueService.Execute), "list");
    public record ValueListReturnParam() : Param<AssetValueService>(nameof(AssetValueService.Execute));


    public class AssetValueService
    {
        public async Task<decimal> Execute(IList list)
        {
            decimal totalValue = 0;
            foreach (var item in list)
            {
                if (item is Asset { AssetType: AssetType.PreciousMetals, Name: { } name, Count: { } count, Quantity: { } quantity, Quality: { } quality, QuantityUnit: { } quantityUnit, End: var end } asset)
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
            return totalValue;
        }
    }

}

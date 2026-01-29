using Humanizer;
using SQLite;
using System.Collections;
using System.Windows.Documents;
using System.Windows.Forms;
using Utility.API;
using Utility.Entities;
using Utility.Enums;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Services.Meta;

namespace Utility.Nodes.Demo.Lists.Services
{
    public record SumBalanceInputParam() : Param<SumAmountService>(nameof(SumBalanceService.Sum), "list");

    public record SumBalanceReturnParam() : Param<SumAmountService>(nameof(SumBalanceService.Sum));

    public class SumBalanceService
    {
        public static string Sum(IList list) => list.OfType<IGetBalance>().Aggregate(0m, (a, b) => a + b.Balance).ToString();
    }
}
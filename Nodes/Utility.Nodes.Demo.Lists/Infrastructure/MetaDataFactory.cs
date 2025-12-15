using System.Transactions;
using ICSharpCode.WpfDesign;
using Utility.API.Services;
using Utility.Entities;
using Utility.Enums;
using Utility.Interfaces.Generic;
using Utility.Meta;
using static Utility.API.Services.Coinbase;
using Transaction = Utility.Entities.Transaction;

namespace Utility.Nodes.Demo.Lists.Infrastructure
{
    internal class MetaDataFactory : IFactory<EntityMetaData>, IEnumerableFactory<EntityMetaData>
    {
        public const string loanGuid = "a762c86f-2e02-4b41-afa9-028a9c4fff1b";
        public const string auctionItemGuid = "a223440a-a171-4989-91f3-a11fbea82546";
        public const string userProfileGuid = "d17c5de2-7836-4c02-958c-eb1de974f474";
        public const string transactionGuid = "6a63ecd3-5b1c-4e6d-b789-2135b6529388";
        public const string assetGuid = "f2a11e92-d1fe-4272-ae36-3a45b4a5571d";
        public const string coinbaseTransactionGuid = "4ec4c118-243e-47ca-a1a5-739bde9204eb";
        public const string paymentScheduleGuid = "5e71d381-f60c-491e-919a-0d9eae220cad";

        public static readonly Type loanType = typeof(Loan);
        public static readonly Type auctionItemType = typeof(AuctionItem);
        public static readonly Type userProfileType = typeof(UserProfile);
        public static readonly Type transactionType = typeof(Transaction);
        public static readonly Type personType = typeof(Person);
        public static readonly Type assetType = typeof(Asset);
        public static readonly Type coinbaseTransactionType = typeof(CoinbaseTransaction);

        Lazy<EntityMetaData[]> lazy = new Lazy<EntityMetaData[]>(() => MetaData.ToArray());

        static readonly IEnumerable<EntityMetaData> MetaData =
            [
                new EntityMetaData
                    {
                        Guid = Guid.Parse(loanGuid),
                        Type = loanType,
                        TransformationMethod = nameof(Factories.NodeMethodFactory.BuildCreditCardRoot),
                        Index = 6,
                        Properties =
                        [
                        new PropertyMetaData
                            {
                                Name = nameof(Loan.Name),
                                ColumnWidth = 200,
                            },
                            new PropertyMetaData
                            {
                                Name = nameof(Loan.Amount),
                                DataType = DataType.Money,
                            },
                            new PropertyMetaData
                            {
                                Name = nameof(Loan.Interest),
                                DataType = DataType.Percentage,
                            },
                            //new PropertyMetaData
                            //{
                            //    Name = nameof(Loan.FutureInterest),
                            //    DataType = DataType.Percentage,

                            //},
                            new PropertyMetaData
                            {
                                Name = nameof(Loan.Limit),
                                DataType = DataType.Money,
                            },
                            new PropertyMetaData
                            {
                                Name = nameof(Loan.InitialFee),
                                DataType = DataType.Money,
                            },
                            ]
                    },
            new EntityMetaData
                    {
                        Guid = Guid.Parse(auctionItemGuid),
                        Type = auctionItemType,
                        TransformationMethod = nameof(Factories.NodeMethodFactory.BuildEbayRoot),
                        Index = 3,
                        Properties =
                        [

                        ]
            },
            new EntityMetaData
            {
                Guid = Guid.Parse(userProfileGuid),
                Type = userProfileType,
                TransformationMethod = nameof(Factories.NodeMethodFactory.BuildUserProfileRoot),
                Index = 1,
                Properties =
                [
                    new PropertyMetaData
                            {
                                Name = nameof(UserProfile.Group),
                                ColumnWidth = 120,
                            },
                            new PropertyMetaData
                            {
                                Name = nameof(UserProfile.Class),
                                ColumnWidth = 120,
                            },
                            new PropertyMetaData
                            {
                                Name = nameof(UserProfile.Name),
                                ColumnWidth = 120,
                            },
                            new PropertyMetaData
                            {
                                Name = nameof(UserProfile.UserName),
                                ColumnWidth = 120,
                            },
                            new PropertyMetaData
                            {
                                Name = nameof(UserProfile.Password),
                                ColumnWidth = 120,
                            },
                            new PropertyMetaData
                            {
                                Name = nameof(UserProfile.AddDate),
                                IsReadOnly = true,
                            },
                ]
            },
            new EntityMetaData
            {
                Guid = Guid.Parse(transactionGuid),
                Type = transactionType,
                TransformationMethod = nameof(Factories.NodeMethodFactory.BuildTransactionRoot),
                Index = 1,
            },
            new EntityMetaData
            {
                Guid = Guid.Parse(assetGuid),
                Type = assetType,
                TransformationMethod = nameof(Factories.NodeMethodFactory.BuildAssetRoot),
                Index = 9,
            },
            new EntityMetaData
            {
                Guid = Guid.Parse(coinbaseTransactionGuid),
                Type = coinbaseTransactionType,
                TransformationMethod = nameof(Factories.NodeMethodFactory.BuildCoinbaseTransactionRoot),
                Index = 10,
            },
            new EntityMetaData
            {
                Guid = Guid.Parse(paymentScheduleGuid),
                Type =personType,
                TransformationMethod = nameof(Factories.NodeMethodFactory.BuildPaymentScheduleRoot),
                Index = 11,
            }
        ];

        public EntityMetaData Create(object config)
        {
            if (config is Type type)
            {
                return lazy.Value.FirstOrDefault(x => x.Type == type) ?? throw new Exception("No match 3fvvd44");
            }
            throw new ArgumentException("Invalid config type", nameof(config));
        }

        IEnumerable<EntityMetaData> IEnumerableFactory<EntityMetaData>.Create(object config)
        {
            return lazy.Value;
        }

        public static MetaDataFactory Instance { get; } = new();
    }
}
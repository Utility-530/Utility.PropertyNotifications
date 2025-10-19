using System.Transactions;
using Utility.API.Services;
using Utility.Entities;
using Utility.Enums;
using Utility.Interfaces.Generic;
using Utility.Meta;
using static Utility.API.Services.Coinbase;
using Transaction = Utility.Entities.Transaction;

namespace Utility.Nodes.Demo.Lists.Infrastructure
{
    internal class MetaDataFactory : IFactory<EntityMetaData>
    {
        public EntityMetaData Create(object config)
        {
            var type = config as Type;
            {
                if (type == typeof(Loan))
                {
                    return new EntityMetaData
                    {
                        Guid = Guid.Parse("a762c86f-2e02-4b41-afa9-028a9c4fff1b"),
                        Type = typeof(Loan),
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
                    };
                }
                else if (type == typeof(AuctionItem))
                    {
                        return new EntityMetaData
                        {
                            Guid = Guid.Parse("a223440a-a171-4989-91f3-a11fbea82546"),
                            Type = typeof(AuctionItem),
                            TransformationMethod = nameof(Factories.NodeMethodFactory.BuildEbayRoot),
                            Index = 3,
                            Properties =
                            [

                            ]
                        };
                    }

                else if (type == typeof(UserProfile))
                    {
                        return new EntityMetaData
                        {
                            Guid = Guid.Parse("d17c5de2-7836-4c02-958c-eb1de974f474"),
                            Type = typeof(UserProfile),
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
                        };
                    }
                else if (type == typeof(Transaction))
                    {
                        return new EntityMetaData
                        {
                            Guid = Guid.Parse("6a63ecd3-5b1c-4e6d-b789-2135b6529388"),
                            Type = typeof(Transaction),
                            TransformationMethod = nameof(Factories.NodeMethodFactory.BuildTransactionRoot),
                            Index = 1,
                        };
                    }
                else if (type== typeof(Asset))
                    {
                        return new EntityMetaData
                        {
                            Guid = Guid.Parse("f2a11e92-d1fe-4272-ae36-3a45b4a5571d"),
                            Type = typeof(Asset),
                            TransformationMethod = nameof(Factories.NodeMethodFactory.BuildAssetRoot),
                            Index = 9,
                        };
                    }
                else if(type == typeof(CoinbaseTransaction))
                    {
                        return new EntityMetaData
                        {
                            Guid = Guid.Parse("4ec4c118-243e-47ca-a1a5-739bde9204eb"),
                            Type = typeof(CoinbaseTransaction),
                            TransformationMethod = nameof(Factories.NodeMethodFactory.BuildCoinbaseTransactionRoot),
                            Index = 10,
                        };
                    }
                }

                throw new Exception("No match 3fvvd44");

            }
        }
    }
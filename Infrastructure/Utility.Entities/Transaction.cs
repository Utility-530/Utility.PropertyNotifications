using System.ComponentModel;
using Utility.Interfaces.Generic.Data;
using Utility.Interfaces.NonGeneric;

namespace Utility.Entities
{
    //29/12/2017, DEB,'11-08-52,00864878,SAINSBURYS S/MKTS,7.65,,3993.32
    public class Transaction : Entity, IEquatable<Transaction>, IGetBalance, IId<Guid>
    {
        public DateTime Date { get; set; }

        [Attributes.Ignore]
        public TransactionType Type { get; set; }

        [Attributes.Ignore]
        public int SortCode { get; set; }

        [Attributes.Ignore]
        public int AccountNumber { get; set; }

        public string Description { get; set; }

        [Attributes.Ignore]
        public int? DebitAmount { get; set; }

        [Attributes.Ignore]
        public int? CreditAmount { get; set; }

        [Attributes.Ignore]
        public int Balance { get; set; }

        public decimal DebitAmount00 => DebitAmount.HasValue ? DebitAmount.Value / 100m : 0;

        public decimal CreditAmount00 => CreditAmount.HasValue ? CreditAmount.Value / 100m : 0;

        public decimal Balance00 => Balance / 100m;

        //    [SQLite.Ignore]

        public string TypeDescription => TransactionTypeMapper.Instance.Dictionary[Type];

        public string Source { get; set; }

        public bool Equals(Transaction other)
        {
            return other != null &&
                   Date == other.Date &&
                   Type == other.Type &&
                   SortCode == other.SortCode &&
                   AccountNumber == other.AccountNumber &&
                   Description == other.Description &&
                   DebitAmount == other.DebitAmount &&
                   CreditAmount == other.CreditAmount &&
                   Balance == other.Balance;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Date, Type, SortCode, AccountNumber, Description, DebitAmount, CreditAmount, Balance);
        }

        public override bool Equals(object other)
        {
            return Equals(other as Transaction);
        }
    }

    public enum TransactionType
    {
        [Description("None")]
        None = 0,

        [Description("Bank Giro Credit")]
        BGC,

        [Description("Bonus BP")]
        BNS,

        [Description("Bill Payment")]
        BP,

        [Description("Correction")]
        CPT,

        [Description("Charge")]
        CHG,

        [Description("Cheque")]
        CHQ,

        [Description("Commission")]
        COR,

        [Description("Bank Giro Credit")]
        CSH,

        [Description("Direct Debit")]
        DD,

        [Description("Debit Card")]
        DEB,

        [Description("Faster Payment Charge")]
        FPI,

        [Description("Faster Payment Incoming")]
        FPO,

        [Description("Standing Order")]
        SO,

        [Description("Payment")]
        PAY,

        [Description("Term Deposit Net Interest")]
        TFR,

        [Description("Deposit")]
        DEP
    }

    public class TransactionTypeMapper
    {
        private static readonly TransactionTypeMapper mapper = new TransactionTypeMapper();

        private TransactionTypeMapper()
        {
            Dictionary = Utility.Helpers.EnumHelper.SelectAllValuesAndDescriptions<TransactionType>().ToDictionary(a => a.Value, a => a.Description);
        }

        public Dictionary<TransactionType, string> Dictionary { get; }

        public static TransactionTypeMapper Instance => mapper;
    }
}
using System;

namespace Utility.API.Entities
{
    public class MetalPrice
    {
        public Guid Guid { get; set; }
        public long Timestamp { get; set; }
        public string Metal { get; set; }
        public string Currency { get; set; }
        public string Exchange { get; set; }
        public string Symbol { get; set; }
        public decimal PrevClosePrice { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal LowPrice { get; set; }
        public decimal HighPrice { get; set; }
        public long OpenTime { get; set; }
        public decimal Price { get; set; }
        public decimal Ch { get; set; }
        public decimal Chp { get; set; }
        public decimal Ask { get; set; }
        public decimal Bid { get; set; }
        public decimal PriceGram24k { get; set; }
        public decimal PriceGram22k { get; set; }
        public decimal PriceGram21k { get; set; }
        public decimal PriceGram20k { get; set; }
        public decimal PriceGram18k { get; set; }
        public decimal PriceGram16k { get; set; }
        public decimal PriceGram14k { get; set; }
        public decimal PriceGram10k { get; set; }
    }
}

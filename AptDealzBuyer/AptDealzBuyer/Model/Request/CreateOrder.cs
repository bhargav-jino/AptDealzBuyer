using Newtonsoft.Json;

namespace AptDealzBuyer.Model.Request
{
    public class CreateOrder
    {
        [JsonProperty("quoteId")]
        public string QuoteId { get; set; }

        [JsonProperty("paidAmount")]
        public decimal PaidAmount { get; set; }

        //[JsonProperty("paymentStatus")]
        //public int PaymentStatus { get; set; }
    }
}

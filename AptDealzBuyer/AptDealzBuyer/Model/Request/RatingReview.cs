using Newtonsoft.Json;

namespace AptDealzBuyer.Model.Request
{
    public class RatingReview
    {
        [JsonProperty("reviewForOrderId")]
        public string ReviewForOrderId { get; set; }

        [JsonProperty("reviewedForSellerId")]
        public string ReviewedForSellerId { get; set; }

        [JsonProperty("sellerRating")]
        public decimal SellerRating { get; set; }
    }
}

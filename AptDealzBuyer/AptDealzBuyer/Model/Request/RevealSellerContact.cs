using Newtonsoft.Json;

namespace AptDealzBuyer.Model.Request
{
    public class RevealSellerContact
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("sellerId")]
        public string SellerId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("quoteId")]
        public string QuoteId { get; set; }

        [JsonProperty("paymentStatus")]
        public int PaymentStatus { get; set; }

        [JsonProperty("razorPayPaymentId")]
        public string RazorPayPaymentId { get; set; }

        [JsonProperty("razorPayOrderId")]
        public string RazorPayOrderId { get; set; }
    }
}

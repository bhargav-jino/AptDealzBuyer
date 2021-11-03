using Newtonsoft.Json;

namespace AptDealzBuyer.Model.Request
{
    public class OrderPayment
    {
        [JsonProperty("orderId")]
        public string OrderId { get; set; }

        [JsonProperty("paymentStatus")]
        public int PaymentStatus { get; set; }

        [JsonProperty("razorPayOrderId")]
        public string RazorPayOrderId { get; set; }

        [JsonProperty("razorPayPaymentId")]
        public string RazorPayPaymentId { get; set; }

    }
}

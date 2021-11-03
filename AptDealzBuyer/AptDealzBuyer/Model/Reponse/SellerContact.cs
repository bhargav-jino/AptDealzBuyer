using Newtonsoft.Json;

namespace AptDealzBuyer.Model.Reponse
{
    public class SellerContact
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
    }
}
using Newtonsoft.Json;

namespace AptDealzBuyer.Model.Reponse
{
    public class BuyerContact
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("buyerId")]
        public string BuyerId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }
    }
}

using Newtonsoft.Json;

namespace AptDealzBuyer.Model.Reponse
{
    public class ShippingAddressDetails
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("building")]
        public string Building { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("pinCode")]
        public string PinCode { get; set; }

        [JsonProperty("landmark")]
        public string Landmark { get; set; }

        [JsonProperty("country")]
        public int? Country { get; set; }
    }
}

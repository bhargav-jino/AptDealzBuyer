using Newtonsoft.Json;
using System;

namespace AptDealzBuyer.Model.Request
{
    public class BuyerDetails
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("profilePhoto")]
        public string ProfilePhoto { get; set; }

        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("building")]
        public string Building { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("countryId")]
        public int? CountryId { get; set; }

        [JsonProperty("pinCode")]
        public string PinCode { get; set; }

        [JsonProperty("landmark")]
        public string Landmark { get; set; }

        [JsonProperty("buyerId")]
        public string BuyerId { get; set; }

        [JsonProperty("buyerNo")]
        public string BuyerNo { get; set; }

        [JsonProperty("registeredOn")]
        public DateTime RegisteredOn { get; set; }

        [JsonProperty("nationality")]
        public string Nationality { get; set; }
    }
}

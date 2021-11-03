using Newtonsoft.Json;

namespace AptDealzBuyer.Model.Request
{
    public class Register
    {
        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("firebaseVerificationId")]
        public string FirebaseVerificationId { get; set; }

        [JsonProperty("latitude")]
        public int Latitude { get; set; }

        [JsonProperty("longitude")]
        public int Longitude { get; set; }
    }

}

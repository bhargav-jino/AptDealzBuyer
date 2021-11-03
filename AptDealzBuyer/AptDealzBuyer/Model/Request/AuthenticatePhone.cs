using Newtonsoft.Json;

namespace AptDealzBuyer.Model.Request
{
    public class AuthenticatePhone
    {
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("firebaseVerificationId")]
        public string FirebaseVerificationId { get; set; }

        [JsonProperty("fcm_Token")]
        public string FcmToken { get; set; }
    }
}

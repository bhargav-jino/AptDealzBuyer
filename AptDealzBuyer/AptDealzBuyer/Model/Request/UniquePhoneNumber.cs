using Newtonsoft.Json;

namespace AptDealzBuyer.Model.Request
{
    public class UniquePhoneNumber
    {
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }
    }
}

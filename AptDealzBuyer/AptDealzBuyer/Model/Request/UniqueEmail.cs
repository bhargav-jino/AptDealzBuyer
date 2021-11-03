using Newtonsoft.Json;

namespace AptDealzBuyer.Model.Request
{
    public class UniqueEmail
    {
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}

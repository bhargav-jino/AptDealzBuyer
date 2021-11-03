using Newtonsoft.Json;

namespace AptDealzBuyer.Model.Reponse
{
    public class Affiliations
    {
        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}

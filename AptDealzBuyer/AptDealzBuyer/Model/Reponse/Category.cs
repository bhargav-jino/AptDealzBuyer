using Newtonsoft.Json;

namespace AptDealzBuyer.Model.Reponse
{
    public class Category
    {
        [JsonProperty("categoryId")]
        public string CategoryId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}

using Newtonsoft.Json;
using System.Collections.Generic;

namespace AptDealzBuyer.Model.Reponse
{
    public class ResponsePincode
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public List<PostOffice> PostOffice { get; set; }
    }

    public class PostOffice
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Description")]
        public object Description { get; set; }

        [JsonProperty("BranchType")]
        public string BranchType { get; set; }

        [JsonProperty("DeliveryStatus")]
        public string DeliveryStatus { get; set; }

        [JsonProperty("Circle")]
        public string Circle { get; set; }

        [JsonProperty("District")]
        public string District { get; set; }

        [JsonProperty("Division")]
        public string Division { get; set; }

        [JsonProperty("Region")]
        public string Region { get; set; }

        [JsonProperty("Block")]
        public string Block { get; set; }

        [JsonProperty("State")]
        public string State { get; set; }

        [JsonProperty("Country")]
        public string Country { get; set; }

        [JsonProperty("Pincode")]
        public string Pincode { get; set; }
    }
}

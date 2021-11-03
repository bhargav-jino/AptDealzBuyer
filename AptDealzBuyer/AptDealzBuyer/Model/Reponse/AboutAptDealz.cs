using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AptDealzBuyer.Model.Reponse
{
    public class AboutAptDealz
    {
        [JsonProperty("about")]
        public string About { get; set; }

        [JsonProperty("contactAddressLine1")]
        public string ContactAddressLine1 { get; set; }

        [JsonProperty("contactAddressLine2")]
        public string ContactAddressLine2 { get; set; }

        [JsonProperty("contactAddressPincode")]
        public string ContactAddressPincode { get; set; }

        [JsonProperty("contactAddressEmail")]
        public string ContactAddressEmail { get; set; }

        [JsonProperty("contactAddressPhone")]
        public string ContactAddressPhone { get; set; }
    }

}

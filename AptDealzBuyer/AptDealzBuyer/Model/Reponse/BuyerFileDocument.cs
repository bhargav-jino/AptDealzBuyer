using AptDealzBuyer.Utility;
using Newtonsoft.Json;
using System.IO;

namespace AptDealzBuyer.Model.Reponse
{
    public class BuyerFileDocument
    {
        [JsonProperty("fileUri")]
        public string FileUri { get; set; }

        [JsonProperty("relativePath")]
        public string RelativePath { get; set; }
    }
}

using Newtonsoft.Json;
using System;

namespace AptDealzBuyer.Model.Request
{
    public class GrievanceRespons
    {
        [JsonProperty("grievanceResponseId")]
        public string GrievanceResponseId { get; set; }

        [JsonProperty("grievanceId")]
        public string GrievanceId { get; set; }

        [JsonProperty("responseFromUserName")]
        public string ResponseFromUserName { get; set; }

        [JsonProperty("responseFromUserProfileImage")]
        public string ResponseFromUserProfileImage { get; set; }

        [JsonProperty("response")]
        public string Response { get; set; }

        [JsonProperty("responseFromUserId")]
        public string ResponseFromUserId { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

        #region [ Extra Properties ]     
        [JsonIgnore]
        public bool IsContact { get; set; } = true;

        [JsonIgnore]
        public bool IsUser { get; set; } = false;
        #endregion
    }
}

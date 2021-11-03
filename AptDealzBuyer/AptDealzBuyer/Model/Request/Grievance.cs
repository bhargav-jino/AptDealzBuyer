using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AptDealzBuyer.Model.Request
{
    public class Grievance
    {
        [JsonProperty("grievanceNo")]
        public string GrievanceNo { get; set; }

        [JsonProperty("requirementTitle")]
        public string RequirementTitle { get; set; }

        [JsonProperty("grievanceId")]
        public string GrievanceId { get; set; }

        [JsonProperty("grievanceFromUserId")]
        public string GrievanceFromUserId { get; set; }

        [JsonProperty("orderId")]
        public string OrderId { get; set; }

        [JsonProperty("orderNo")]
        public string OrderNo { get; set; }

        [JsonProperty("grievanceType")]
        public int GrievanceType { get; set; }

        [JsonProperty("grievanceTypeDescr")]
        public string GrievanceTypeDescr { get; set; }

        [JsonProperty("grievanceFromUserName")]
        public string GrievanceFromUserName { get; set; }

        [JsonProperty("issueDescription")]
        public string IssueDescription { get; set; }

        [JsonProperty("preferredSolution")]
        public string PreferredSolution { get; set; }

        [JsonProperty("enableResponseFromUser")]
        public bool EnableResponseFromUser { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("statusDescr")]
        public string StatusDescr { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }

        [JsonProperty("sellerName")]
        public string SellerName { get; set; }

        [JsonProperty("buyerName")]
        public string BuyerName { get; set; }

        [JsonProperty("orderDate")]
        public DateTime OrderDate { get; set; }

        [JsonProperty("documents")]
        public List<string> Documents { get; set; }

        [JsonProperty("grievanceResponses")]
        public List<GrievanceRespons> GrievanceResponses { get; set; }
    }
}

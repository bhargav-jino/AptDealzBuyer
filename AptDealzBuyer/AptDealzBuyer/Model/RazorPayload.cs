using Newtonsoft.Json;
using System.Collections.Generic;

namespace AptDealzBuyer.Model
{
    public class RazorPayload
    {
        public long amount { get; set; }
        public string currency { get; set; }
        public string receipt { get; set; }
        public int payment_capture { get; set; }

        [JsonIgnore]
        public string email { get; set; }
        [JsonIgnore]
        public string contact { get; set; }
    }

    public class RazorResponse
    {
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public string Signature { get; set; }
        public bool isPaid { get; set; }
        public string OrderNo { get; set; }


        public string id { get; set; }
        public string entity { get; set; }
        public int amount { get; set; }
        public int amount_paid { get; set; }
        public int amount_due { get; set; }
        public string currency { get; set; }
        public string receipt { get; set; }
        public object offer_id { get; set; }
        public string status { get; set; }
        public int attempts { get; set; }
        public List<object> notes { get; set; }
        public int created_at { get; set; }
    }
}

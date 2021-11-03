using Newtonsoft.Json;
using System;

namespace AptDealzBuyer.Model.Reponse
{
    public class ReceivedQuote
    {
        [JsonProperty("quoteNo")]
        public string QuoteNo { get; set; }

        [JsonProperty("quoteId")]
        public string QuoteId { get; set; }

        [JsonProperty("sellerId")]
        public string SellerId { get; set; }

        [JsonProperty("sellerName")]
        public string SellerName { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("receivedDate")]
        public DateTime ReceivedDate { get; set; }

        [JsonProperty("validityDate")]
        public DateTime ValidityDate { get; set; }

        [JsonProperty("validity")]
        public int Validity { get; set; }

        [JsonIgnore]
        public string Days
        {
            get
            {
                DateTime startdate;
                DateTime enddate;
                TimeSpan remaindate;
                startdate = DateTime.Now.Date;
                enddate = ValidityDate.Date;
                remaindate = enddate - startdate;
                if (remaindate != null && ValidityDate.Date >= DateTime.Today)
                {
                    return $"Validity : {remaindate.TotalDays} " + (remaindate.TotalDays > 1 ? "Days" : "Day");
                }
                else if (ValidityDate.Date < DateTime.Today)
                {
                    return $"Validity : Expired";
                }
                else
                {
                    return $"Validity : {0} day";
                }
            }
        }
    }

}

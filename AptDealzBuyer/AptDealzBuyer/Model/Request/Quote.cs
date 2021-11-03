using AptDealzBuyer.Model.Reponse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AptDealzBuyer.Model.Request
{
    public class Quote
    {
        [JsonProperty("quoteId")]
        public string QuoteId { get; set; }

        [JsonProperty("quoteNo")]
        public string QuoteNo { get; set; }

        [JsonProperty("requirementId")]
        public string RequirementId { get; set; }

        [JsonProperty("requirementNo")]
        public string RequirementNo { get; set; }

        [JsonProperty("unitPrice")]
        public decimal UnitPrice { get; set; }

        [JsonProperty("handlingCharges")]
        public decimal HandlingCharges { get; set; }

        [JsonProperty("shippingCharges")]
        public decimal ShippingCharges { get; set; }

        [JsonProperty("insuranceCharges")]
        public decimal InsuranceCharges { get; set; }

        [JsonProperty("shippingPinCode")]
        public string ShippingPinCode { get; set; }

        [JsonProperty("validityDate")]
        public DateTime ValidityDate { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("comments")]
        public string Comments { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("paymentStatus")]
        public string PaymentStatus { get; set; }

        [JsonProperty("sellerId")]
        public string SellerId { get; set; }

        [JsonProperty("buyerId")]
        public string BuyerId { get; set; }

        [JsonProperty("requestedQuantity")]
        public decimal RequestedQuantity { get; set; }

        [JsonProperty("netAmount")]
        public decimal NetAmount { get; set; }

        [JsonProperty("totalQuoteAmount")]
        public decimal TotalQuoteAmount { get; set; }

        [JsonProperty("requirementTitle")]
        public string RequirementTitle { get; set; }

        [JsonProperty("category")]
        public object Category { get; set; }

        [JsonProperty("subCategories")]
        public List<object> SubCategories { get; set; }

        [JsonProperty("preferredSource")]
        public string PreferredSource { get; set; }

        [JsonProperty("totalPriceEstimation")]
        public decimal TotalPriceEstimation { get; set; }

        [JsonProperty("expectedDeliveryDate")]
        public DateTime ExpectedDeliveryDate { get; set; }

        [JsonProperty("productImage")]
        public string ProductImage { get; set; }

        [JsonProperty("needInsuranceCoverage")]
        public bool NeedInsuranceCoverage { get; set; }

        [JsonProperty("isBuyerContactRevealed")]
        public bool IsBuyerContactRevealed { get; set; }

        [JsonProperty("isSellerContactRevealed")]
        public bool IsSellerContactRevealed { get; set; }

        [JsonProperty("sellerContact")]
        public SellerContact SellerContact { get; set; }

        [JsonProperty("buyerContact")]
        public BuyerContact BuyerContact { get; set; }

        [JsonProperty("sellerName")]
        public string SellerName { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("validity")]
        public int Validity { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }


        #region [ Extra Properties ]
        [JsonIgnore]
        public string TotalAmount
        {
            get
            {
                //double amount;
                //amount = UnitPrice + HandlingCharges + ShippingCharges + InsuranceCharges;
                return $"Rs {TotalQuoteAmount}";
            }
        }

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
        #endregion
    }
}
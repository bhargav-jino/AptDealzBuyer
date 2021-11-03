using AptDealzBuyer.Model.Reponse;
using AptDealzBuyer.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;

namespace AptDealzBuyer.Model.Request
{
    public class Requirement : INotifyPropertyChanged
    {
        [JsonProperty("requirementId")]
        public string RequirementId { get; set; }

        [JsonProperty("requirementNo")]
        public string RequirementNo { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("buyerName")]
        public string BuyerName { get; set; }

        [JsonProperty("buyerNo")]
        public string BuyerNo { get; set; }

        [JsonProperty("productImage")]
        public string ProductImage { get; set; }

        [JsonProperty("productDescription")]
        public string ProductDescription { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("totalPriceEstimation")]
        public decimal TotalPriceEstimation { get; set; }

        [JsonProperty("preferInIndiaProducts")]
        public bool PreferInIndiaProducts { get; set; }

        [JsonProperty("pickupProductDirectly")]
        public bool PickupProductDirectly { get; set; }

        [JsonProperty("deliveryLocationPinCode")]
        public string DeliveryLocationPinCode { get; set; }

        [JsonProperty("preferredSourceOfSupply")]
        public object PreferredSourceOfSupply { get; set; }

        [JsonProperty("expectedDeliveryDate")]
        public DateTime ExpectedDeliveryDate { get; set; }

        [JsonProperty("expectedDeliveryDateStr")]
        public string ExpectedDeliveryDateStr { get; set; }

        [JsonProperty("billingAddressName")]
        public string BillingAddressName { get; set; }

        [JsonProperty("billingAddressBuilding")]
        public string BillingAddressBuilding { get; set; }

        [JsonProperty("billingAddressStreet")]
        public string BillingAddressStreet { get; set; }

        [JsonProperty("billingAddressCity")]
        public string BillingAddressCity { get; set; }

        [JsonProperty("billingAddressState")]
        public string BillingAddressState { get; set; }

        [JsonProperty("billingAddressPinCode")]
        public string BillingAddressPinCode { get; set; }

        [JsonProperty("shippingAddressName")]
        public string ShippingAddressName { get; set; }

        [JsonProperty("shippingAddressBuilding")]
        public string ShippingAddressBuilding { get; set; }

        [JsonProperty("shippingAddressStreet")]
        public string ShippingAddressStreet { get; set; }

        [JsonProperty("shippingAddressCity")]
        public string ShippingAddressCity { get; set; }

        [JsonProperty("shippingAddressState")]
        public string ShippingAddressState { get; set; }

        [JsonProperty("shippingAddressPinCode")]
        public string ShippingAddressPinCode { get; set; }

        [JsonProperty("shippingAddressLandmark")]
        public string ShippingAddressLandmark { get; set; }

        [JsonProperty("needInsuranceCoverage")]
        public bool NeedInsuranceCoverage { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("subCategories")]
        public List<string> SubCategories { get; set; }

        [JsonProperty("statusEnum")]
        public int StatusEnum { get; set; }

        [JsonProperty("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("canRevealContact")]
        public bool CanRevealContact { get; set; }

        [JsonProperty("receivedQuotes")]
        public List<ReceivedQuote> ReceivedQuotes { get; set; }

        [JsonProperty("buyerContact")]
        public object BuyerContact { get; set; }

        [JsonProperty("isBuyerContactRevealed")]
        public bool IsBuyerContactRevealed { get; set; }

        [JsonProperty("userProfile")]
        public object UserProfile { get; set; }

        [JsonProperty("requirementCategories")]
        public object RequirementCategories { get; set; }

        [JsonProperty("createdBy")]
        public object CreatedBy { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }

        [JsonProperty("lastModifiedBy")]
        public object LastModifiedBy { get; set; }

        [JsonProperty("lastModified")]
        public object LastModified { get; set; }

        [JsonProperty("quotes")]
        public int Quotes { get; set; }


        #region [ Extra Properties ]
        [JsonIgnore]
        private string _ArrowImage { get; set; } = Constraints.Img_ArrowRight;

        [JsonIgnore]
        public string ArrowImage
        {
            get { return _ArrowImage; }
            set { _ArrowImage = value; PropertyChangedEventArgs("ArrowImage"); }
        }

        [JsonIgnore]
        private double _NameFont { get; set; } = 13;
        [JsonIgnore]
        public double NameFont
        {
            get { return _NameFont; }
            set { _NameFont = value; PropertyChangedEventArgs("NameFont"); }
        }

        [JsonIgnore]
        private bool _MoreDetail { get; set; } = false;
        [JsonIgnore]
        public bool MoreDetail
        {
            get { return _MoreDetail; }
            set { _MoreDetail = value; PropertyChangedEventArgs("MoreDetail"); }
        }

        [JsonIgnore]
        private bool _HideDetail { get; set; } = true;
        [JsonIgnore]
        public bool HideDetail
        {
            get { return _HideDetail; }
            set { _HideDetail = value; PropertyChangedEventArgs("HideDetail"); }
        }

        [JsonIgnore]
        private Color _GridBg { get; set; } = Color.Transparent;
        [JsonIgnore]
        public Color GridBg
        {
            get { return _GridBg; }
            set { _GridBg = value; PropertyChangedEventArgs("GridBg"); }
        }

        [JsonIgnore]
        public string TotalQuotes
        {
            get
            {
                string quotes;
                if (Quotes > 1)
                {
                    quotes = Quotes + " Quotes";
                }
                else
                {
                    quotes = Quotes + " Quote";
                }
                return quotes;
            }
        }

        [JsonIgnore]
        public Color StatusColor
        {
            get
            {
                if (Status == RequirementStatus.Completed.ToString())
                {
                    return (Color)App.Current.Resources["appColor1"];
                }
                else if (Status == RequirementStatus.Rejected.ToString())
                {
                    return (Color)App.Current.Resources["appColor2"];
                }
                else if (Status == RequirementStatus.Inactive.ToString())
                {
                    return (Color)App.Current.Resources["appColor5"];
                }
                else
                {
                    return (Color)App.Current.Resources["appColor4"];
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void PropertyChangedEventArgs(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

}

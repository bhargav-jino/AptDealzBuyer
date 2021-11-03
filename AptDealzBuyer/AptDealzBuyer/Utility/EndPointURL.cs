namespace AptDealzBuyer.Utility
{
    public class EndPointURL
    {
        #region [ Register API ]
        public const string IsUniquePhoneNumber = "api/v{0}/BuyerAuth/IsUniquePhoneNumber";
        public const string IsUniqueEmail = "api/v{0}/BuyerAuth/IsUniqueEmail";
        public const string Register = "api/v{0}/BuyerAuth/Register";
        #endregion

        #region [ Authentication API ]
        public const string CheckPhoneNumberExists = "api/v{0}/BuyerAuth/CheckPhoneNumberExists?phoneNumber={1}";
        public const string BuyerAuthenticateEmail = "api/v{0}/BuyerAuth/authenticate/email";
        public const string BuyerAuthenticatePhone = "api/v{0}/BuyerAuth/authenticate/phone";
        public const string SendOtpByEmail = "api/v{0}/BuyerAuth/SendOtpByEmail";
        public const string RefreshToken = "api/Account/refresh-token";
        public const string SendOtp = "api/Account/SendOtp/Email/{0}";
        public const string Logout = "api/Account/logout";
        #endregion

        #region [ Profile API ]
        public const string DeactivateUser = "api/Account/DeactivateUser";
        public const string GetUserProfileByEmail = "api/v{0}/BuyerAuth/GetUserProfile/Email";
        public const string GetMyProfileData = "api/v{0}/BuyerManagement/GetMyProfileData";
        public const string SaveProfile = "api/v{0}/BuyerManagement/Update";
        public const string Country = "api/v{0}/Country/Get";
        public const string FileUpload = "api/FileUpload";
        public const string GetPincodeInfo = "api/IndianPincode/GetPincodeInfo/{0}";
        #endregion

        #region [ Category API ]
        public const string Category = "api/v{0}/Category/Get";
        public const string SubCategory = "api/v{0}/SubCategory/Get?CategoryId={1}";
        #endregion

        #region [ Requirement API ]
        public const string CreateRequirement = "api/v{0}/Requirement/Create";
        public const string GetRequirement = "api/v{0}/Requirement/Get?PageNumber={1}&PageSize={2}";
        public const string GetRequirementById = "api/v{0}/Requirement/Get/{1}";
        public const string GetAllMyActiveRequirements = "api/v{0}/Requirement/GetAllMyActiveRequirements";
        public const string GetMyPreviousRequirements = "api/v{0}/Requirement/GetMyPreviousRequirements";
        public const string DeleteRequirement = "api/v{0}/Requirement/Delete/{1}";
        public const string CancelRequirement = "api/v{0}/Requirement/CancelRequirement?RequirementId={1}";
        public const string UpdateStatusRequirement = "api/v{0}/Requirement/UpdateStatus";
        #endregion

        #region [ Quote API ]
        public const string GetQuoteById = "api/v{0}/Quote/Get/{1}";
        public const string GetQuotes = "api/v{0}/Quote/Get?RequirementId={1}";
        public const string AcceptQuote = "api/v{0}/Quote/AcceptQuote?quoteId={1}";
        public const string RejectQuote = "api/v{0}/Quote/RejectQuote?quoteId={1}";
        public const string RevealSellerContact = "api/v{0}/Quote/RevealSellerContact";
        #endregion

        #region [ Order API ]
        public const string GetOrderDetailsForBuyer = "api/v{0}/Order/GetOrderDetailsForBuyer/{1}";
        public const string GetOrdersForBuyer = "api/v{0}/Order/GetOrdersForBuyer";
        public const string CancelOrder = "api/v{0}/Order/CancelOrder/{1}";
        public const string CreateOrder = "api/v{0}/Order/Create";
        public const string OrderPayment = "api/v{0}/Order/Create/Payment";
        public const string GetShippedOrdersForBuyer = "api/v{0}/Order/GetShippedOrdersForBuyer";
        public const string GenerateQRCodeImageForBuyerApp = "api​/v{0}​/Order​/GenerateQRCodeImageForBuyerApp​/{1}";
        public const string ConfirmDeliveryFromBuyer = "api/v{0}/Order/ConfirmDeliveryFromBuyer";
        #endregion

        #region [ RateAndReview API ]
        public const string ReviewSeller = "api/v{0}/RateAndReview/ReviewSeller";
        public const string ReviewSellerProduct = "api/v{0}/RateAndReview/ReviewSellerProduct";
        #endregion

        #region [ Grievance API ]
        public const string GetAllGrievancesByMe = "api/v{0}/Grievance//GetAllGrievancesByBuyer";
        public const string GetGrievancesDetailsForBuyer = "api/v{0}/Grievance/GetGrievancesDetailsForBuyer/{1}";
        public const string CreateGrievanceFromBuyer = "api/v{0}/Grievance/CreateGrievanceFromBuyer";
        public const string SubmitGrievanceResponseFromBuyer = "api/v{0}/Grievance/SubmitGrievanceResponseFromBuyer";
        #endregion

        #region [ Notification API ]
        public const string GetAllNotificationsForUser = "api/v{0}/Notifications/GetAllNotificationsForUser";
        public const string GetNotificationsCountForUser = "api/v{0}/Notifications/GetNotificationsCountForUser";
        public const string SetUserNoficiationAsRead = "api/v{0}/Notifications/SetUserNoficiationAsRead/{1}";
        public const string SetUserNoficiationAsReadAndDelete = "/api/v{0}/Notifications/SetUserNoficiationAsReadAndDelete/{1}";
        #endregion

        #region [ SupportChat API ]
        public const string GetAllMyChat = "api/v{0}/SupportChat/GetAllMyChat";
        public const string SendChatSupportMessage = "api/v{0}/SupportChat/SendChatSupportMessage";
        #endregion

        #region [ Affiliations API ]
        public const string GetAllAffiliations = "api/v{0}/Affiliations/GetAllAffiliations";
        #endregion


        #region MyRegion
        public const string GetAmountToBePaidToRevealBuyerContact = "api​/v{0}​/Requirement​/GetAmountToBePaidToRevealBuyerContact";
        public const string GetPrivacyPolicyTermsAndConditions = "api/v{0}/AppSettings/GetPrivacyPolicyTermsAndConditions";
        public const string GetFAQ = "api/v{0}/AppSettings/GetFAQ";
        public const string GetAppPromoBar = "api/v{0}/AppSettings/GetAppPromoBar";
        public const string AboutAptdealzBuyerApp = "api/v{0}/AppSettings/AboutAptdealzBuyerApp";
        #endregion

    }
}



using Newtonsoft.Json;

namespace AptDealzBuyer.Model.Reponse
{
    public class NotificationData
    {
        [JsonProperty("notificationId")]
        public string NotificationId { get; set; }

        [JsonProperty("parentKeyId")]
        public string ParentKeyId { get; set; }

        [JsonProperty("navigationScreen")]
        public int NavigationScreen { get; set; }

        [JsonProperty("navigationScreenEnumDescr")]
        public string NavigationScreenEnumDescr { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("notifiedForUserName")]
        public string NotifiedForUserName { get; set; }

        [JsonProperty("notificationForUserId")]
        public string NotificationForUserId { get; set; }

        [JsonProperty("isRead")]
        public bool IsRead { get; set; }
    }
}

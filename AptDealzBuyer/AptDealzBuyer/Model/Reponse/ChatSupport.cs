using Newtonsoft.Json;
using System;

namespace AptDealzBuyer.Model.Reponse
{
    public class ChatSupport
    {
        [JsonProperty("chatMessageId")]
        public string ChatMessageId { get; set; }

        [JsonProperty("chatId")]
        public string ChatId { get; set; }

        [JsonProperty("isMessageFromSupportTeam")]
        public bool IsMessageFromSupportTeam { get; set; }

        [JsonProperty("chatMessageFromUserName")]
        public string ChatMessageFromUserName { get; set; }

        [JsonProperty("chatMessageFromUserProfileImage")]
        public string ChatMessageFromUserProfileImage { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("chatMessageFromUserId")]
        public string ChatMessageFromUserId { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("createdDateStr")]
        public string CreatedDateStr { get; set; }

        #region [ Extra Properties ]     
        [JsonIgnore]
        public bool IsContact { get; set; } = true;

        [JsonIgnore]
        public bool IsUser { get; set; } = false;
        #endregion
    }
}

//public class ChatSupport : INotifyPropertyChanged
//{
//    public event PropertyChangedEventHandler PropertyChanged;

//    protected void OnPropertyChanged(string propertyName)
//    {
//        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//    }

//    private string _ChatMessageId { get; set; }

//    [JsonProperty("chatMessageId")]
//    public string ChatMessageId
//    {
//        get { return _ChatMessageId; }
//        set
//        {
//            _ChatMessageId = value;
//            OnPropertyChanged("ChatMessageId");
//        }
//    }


//    private string _ChatId { get; set; }
//    [JsonProperty("chatId")]
//    public string ChatId
//    {
//        get { return _ChatId; }
//        set
//        {
//            _ChatId = value;
//            OnPropertyChanged("ChatId");
//        }
//    }


//    private bool _IsMessageFromSupportTeam { get; set; }
//    [JsonProperty("isMessageFromSupportTeam")]
//    public bool IsMessageFromSupportTeam
//    {
//        get { return _IsMessageFromSupportTeam; }
//        set
//        {
//            _IsMessageFromSupportTeam = value;
//            OnPropertyChanged("IsMessageFromSupportTeam");
//        }
//    }

//    private string _ChatMessageFromUserName { get; set; }
//    [JsonProperty("chatMessageFromUserName")]
//    public string ChatMessageFromUserName
//    {
//        get { return _ChatMessageFromUserName; }
//        set
//        {
//            _ChatMessageFromUserName = value;
//            OnPropertyChanged("ChatMessageFromUserName");
//        }
//    }

//    private string _ChatMessageFromUserProfileImage { get; set; }
//    [JsonProperty("chatMessageFromUserProfileImage")]
//    public string ChatMessageFromUserProfileImage
//    {
//        get { return _ChatMessageFromUserProfileImage; }
//        set
//        {
//            _ChatMessageFromUserProfileImage = value;
//            OnPropertyChanged("ChatMessageFromUserProfileImage");
//        }
//    }

//    private string _Message { get; set; }
//    [JsonProperty("message")]
//    public string Message
//    {
//        get { return _Message; }
//        set
//        {
//            _Message = value;
//            OnPropertyChanged("Message");
//        }
//    }

//    private string _ChatMessageFromUserId { get; set; }
//    [JsonProperty("chatMessageFromUserId")]
//    public string ChatMessageFromUserId
//    {
//        get { return _ChatMessageFromUserId; }
//        set
//        {
//            _ChatMessageFromUserId = value;
//            OnPropertyChanged("ChatMessageFromUserId");
//        }
//    }

//    private DateTime _CreatedDate { get; set; }
//    [JsonProperty("createdDate")]
//    public DateTime CreatedDate
//    {
//        get { return _CreatedDate; }
//        set
//        {
//            _CreatedDate = value;
//            OnPropertyChanged("CreatedDate");
//        }
//    }

//    private string _CreatedDateStr { get; set; }
//    [JsonProperty("createdDateStr")]
//    public string CreatedDateStr
//    {
//        get { return _CreatedDateStr; }
//        set
//        {
//            _CreatedDateStr = value;
//            OnPropertyChanged("CreatedDateStr");
//        }
//    }

//    #region [ Extra Properties ]     
//    private bool _IsContact { get; set; } = true;
//    [JsonIgnore]
//    public bool IsContact
//    {
//        get { return _IsContact; }
//        set
//        {
//            _IsContact = value;
//            OnPropertyChanged("IsContact");
//        }
//    }
//    private bool _IsUser { get; set; } = false;
//    [JsonIgnore]
//    public bool IsUser
//    {
//        get { return _IsUser; }
//        set
//        {
//            _IsUser = value;
//            OnPropertyChanged("IsUser");
//        }
//    }
//}

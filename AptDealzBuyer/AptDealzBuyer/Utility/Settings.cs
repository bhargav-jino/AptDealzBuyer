using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System.ComponentModel;

namespace AptDealzBuyer.Utility
{
    public class UserInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public string emailAddress;
        public string EmailAddress
        {
            get { return EmailAddress; }
            set
            {
                emailAddress = value;
                PropertyChanged(this, new PropertyChangedEventArgs("EmailAddress"));
                Settings.EmailAddress = value;
            }
        }

        public string _fcm_token;
        public string fcm_token
        {
            get { return fcm_token; }
            set
            {
                _fcm_token = value;
                PropertyChanged(this, new PropertyChangedEventArgs("fcm_token"));
                Settings.fcm_token = value;
            }
        }

        public string _userToken;
        public string UserToken
        {
            get { return UserToken; }
            set
            {
                _userToken = value;
                PropertyChanged(this, new PropertyChangedEventArgs("UserToken"));
                Settings.UserToken = value;
            }
        }

        public string userId;
        public string UserId
        {
            get { return UserId; }
            set
            {
                userId = value;
                PropertyChanged(this, new PropertyChangedEventArgs("UserId"));
                Settings.UserId = value;
            }
        }

        public string phoneAuthToken;
        public string PhoneAuthToken
        {
            get { return PhoneAuthToken; }
            set
            {
                phoneAuthToken = value;
                PropertyChanged(this, new PropertyChangedEventArgs("PhoneAuthToken"));
                Settings.PhoneAuthToken = value;
            }
        }

        public string refreshToken;
        public string RefreshToken
        {
            get { return RefreshToken; }
            set
            {
                refreshToken = value;
                PropertyChanged(this, new PropertyChangedEventArgs("RefreshToken"));
                Settings.RefreshToken = value;
            }
        }

        public string loginTrackingKey;
        public string LoginTrackingKey
        {
            get { return LoginTrackingKey; }
            set
            {
                loginTrackingKey = value;
                PropertyChanged(this, new PropertyChangedEventArgs("LoginTrackingKey"));
                Settings.LoginTrackingKey = value;
            }
        }

        public bool isViewWelcomeScreen = true;
        public bool IsViewWelcomeScreen
        {
            get { return IsViewWelcomeScreen; }
            set
            {
                isViewWelcomeScreen = value;
                Settings.IsViewWelcomeScreen = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsViewWelcomeScreen"));
            }
        }
    }

    public class Settings
    {
        private static ISettings AppSettings
        {
            get { return CrossSettings.Current; }
        }

        private const string EmailAddressKey = "email_address_key";
        private const string fcm_tokenKey = "fcm_token_key";
        private const string UserTokenKey = "userToken_key";
        private const string UserIdKey = "userId_key";
        private const string PhoneAuthTokenKey = "phoneAuthToken_key";
        private const string RefreshTokenKey = "refreshToken_key";
        private const string LoginTrackingKey_Key = "loginTrackingKey_key";
        private const string IsViewWelcomeScreen_Key = "IsViewWelcomeScreen_Key";

        private static readonly string SettingsDefault = string.Empty;
        private static readonly bool SettingsBoolDefault = true;

        public static string EmailAddress
        {
            get { return AppSettings.GetValueOrDefault(EmailAddressKey, SettingsDefault); }
            set { AppSettings.AddOrUpdateValue(EmailAddressKey, value); }
        }

        public static string fcm_token
        {
            get { return AppSettings.GetValueOrDefault(fcm_tokenKey, SettingsDefault); }
            set { AppSettings.AddOrUpdateValue(fcm_tokenKey, value); }
        }

        public static string UserToken
        {
            get { return AppSettings.GetValueOrDefault(UserTokenKey, SettingsDefault); }
            set { AppSettings.AddOrUpdateValue(UserTokenKey, value); }
        }

        public static string UserId
        {
            get { return AppSettings.GetValueOrDefault(UserIdKey, SettingsDefault); }
            set { AppSettings.AddOrUpdateValue(UserIdKey, value); }
        }

        public static string PhoneAuthToken
        {
            get { return AppSettings.GetValueOrDefault(PhoneAuthTokenKey, SettingsDefault); }
            set { AppSettings.AddOrUpdateValue(PhoneAuthTokenKey, value); }
        }

        public static string RefreshToken
        {
            get { return AppSettings.GetValueOrDefault(RefreshTokenKey, SettingsDefault); }
            set { AppSettings.AddOrUpdateValue(RefreshTokenKey, value); }
        }

        public static string LoginTrackingKey
        {
            get { return AppSettings.GetValueOrDefault(LoginTrackingKey_Key, SettingsDefault); }
            set { AppSettings.AddOrUpdateValue(LoginTrackingKey_Key, value); }
        }

        public static bool IsViewWelcomeScreen
        {
            get { return AppSettings.GetValueOrDefault(IsViewWelcomeScreen_Key, SettingsBoolDefault); }
            set { AppSettings.AddOrUpdateValue(IsViewWelcomeScreen_Key, value); }
        }
    }
}

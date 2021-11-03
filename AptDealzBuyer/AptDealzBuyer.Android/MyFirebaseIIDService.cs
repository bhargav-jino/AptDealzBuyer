using Android.App;
using Android.Content;
using Firebase.Iid;

namespace AptDealzBuyer.Droid
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyFirebaseIIDService : FirebaseInstanceIdService
    {
        const string TAG = "MyFirebaseIIDService";
        public override void OnTokenRefresh()
        {
            Utility.Settings.fcm_token = FirebaseInstanceId.Instance.Token;

        }

        void SendRegistrationToServer(string token)
        {
            // send this token to server
        }
    }
}
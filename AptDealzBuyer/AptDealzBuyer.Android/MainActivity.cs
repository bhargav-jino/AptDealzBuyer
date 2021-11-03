using Acr.UserDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using AptDealzBuyer.Droid.DependencService;
using AptDealzBuyer.Interfaces;
using AptDealzBuyer.Model;
using AptDealzBuyer.Utility;
using Com.Razorpay;
using DLToolkit.Forms.Controls;
using FFImageLoading.Forms.Platform;
using Firebase;
using Newtonsoft.Json;
using Org.Json;
using Plugin.FirebasePushNotification;
using Plugin.Permissions;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AptDealzBuyer.Droid
{
    [Activity(Label = "Aptdealz", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IPaymentResultWithDataListener
    {
        #region [ Properties ]
        public string MerchantName = "AptDealz";
        public string paymentColor = "#006027";
        public bool isReveal = false;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;

            FirebaseApp.InitializeApp(this);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            FirebasePushNotificationManager.ProcessIntent(this, Intent);
            //CreateNotificationFromIntent(Intent);
            Xamarin.Forms.DependencyService.Register<IFirebaseAuthenticator, FirebaseAuthenticator>();

            CachedImageRenderer.Init(true);
            FlowListView.Init();
            UserDialogs.Init(this);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            Rg.Plugins.Popup.Popup.Init(this);

            CameraPermission();
           // GetPermission();

            LoadApplication(new App());

            MessagingCenter.Subscribe<RazorPayload>(this, Constraints.RP_PayNow, (payload) =>
             {
                 isReveal = false;
                 string username = Constraints.RP_UserName;
                 string password = Constraints.RP_Password;

                 PayViaRazor(payload, username, password);
             });

            MessagingCenter.Subscribe<RazorPayload>(this, Constraints.RP_RevealPayNow, (payload) =>
            {
                isReveal = true;

                string username = Constraints.RP_UserName;
                string password = Constraints.RP_Password;
                PayViaRazor(payload, username, password);
            });
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
            }
            else
            {
                // Do something if there are not any pages in the `PopupStack`
            }
        }

        #region [ Firebase ]
        protected override void OnNewIntent(Intent intent)
        {
            FirebasePushNotificationManager.ProcessIntent(this, intent);
        }
        #endregion

        #region [ RazorPay ]
        public void OnPaymentError(int p0, string p1, PaymentData p2)
        {
            RazorResponse mRazorResponse = new RazorResponse()
            {
                PaymentId = p2.PaymentId,
                OrderId = p2.OrderId,
                Signature = p2.Signature,
                isPaid = false
            };

            if (isReveal)
                MessagingCenter.Send<RazorResponse>(mRazorResponse, Constraints.RP_PaidRevealResponse);
            else
                MessagingCenter.Send<RazorResponse>(mRazorResponse, Constraints.RP_PaidResponse);
        }

        public void OnPaymentSuccess(string p0, PaymentData p1)
        {
            RazorResponse mRazorResponse = new RazorResponse()
            {
                PaymentId = p1.PaymentId,
                OrderId = p1.OrderId,
                Signature = p1.Signature,
                isPaid = true
            };

            if (isReveal)
                MessagingCenter.Send<RazorResponse>(mRazorResponse, Constraints.RP_PaidRevealResponse);
            else
                MessagingCenter.Send<RazorResponse>(mRazorResponse, Constraints.RP_PaidResponse);
        }

        public async void PayViaRazor(RazorPayload payload, string username, string password)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.razorpay.com/v1/orders"))
                    {
                        var plainTextBytes = Encoding.UTF8.GetBytes($"{username}:{password}");
                        var basicAuthKey = Convert.ToBase64String(plainTextBytes);

                        request.Headers.TryAddWithoutValidation("Authorization", $"Basic {basicAuthKey}");

                        string jsonData = JsonConvert.SerializeObject(payload);
                        request.Content = new StringContent(jsonData);
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                        var response = await httpClient.SendAsync(request);
                        string jsonResp = await response.Content.ReadAsStringAsync();
                        RazorResponse mRazorResponse = JsonConvert.DeserializeObject<RazorResponse>(jsonResp);

                        if (!string.IsNullOrEmpty(mRazorResponse.id))
                        {
                            CheckoutRazorPay(mRazorResponse.id, username, payload);
                        }
                        else
                        {
                            var mOrderRequest = JsonConvert.DeserializeObject<OrderRequest>(jsonResp);
                            if (mOrderRequest != null && mOrderRequest.error != null)
                                Toast.MakeText(this, mOrderRequest.error.description, ToastLength.Short).Show();
                            else
                                Toast.MakeText(this, "Payment Error", ToastLength.Short).Show();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Exception-Payment: " + ex.Message, ToastLength.Short).Show();
            }
        }

        public void CheckoutRazorPay(string orderId, string username, RazorPayload payload)
        {
            // checkout
            try
            {
                Checkout checkout = new Checkout();
                checkout.SetImage(0);
                checkout.SetKeyID(username);

                JSONObject options = new JSONObject();

                options.Put("name", MerchantName); //Merchant Name
                options.Put("description", $"Order Id. {payload.receipt}");
                options.Put("image", "https://s3.amazonaws.com/rzp-mobile/images/rzp.png");
                options.Put("order_id", orderId); //from response of step 3.
                options.Put("theme.color", paymentColor);
                options.Put("currency", payload.currency);
                options.Put("amount", payload.amount); //pass amount in currency subunits 2000 
                options.Put("prefill.email", payload.email);
                options.Put("prefill.contact", payload.contact);

                checkout.Open(this, options);
            }
            catch (Exception ex)
            {
                //Toast.MakeText(this, "Exception-CheckoutRazorPay: " + ex.Message, ToastLength.Short).Show();
            }
        }
        #endregion

        #region [ Permissions ]
        public async Task CameraPermission()
        {
            try
            {
                string version = Android.OS.Build.VERSION.Release; //Android Version No like 4.4.4 etc... 
                var mver = string.Format("{0:0.00}", version.Substring(0, 3));
                double ver = Convert.ToDouble(mver);
                if (ver >= 5.0)
                {
                    var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Camera);
                    if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                    {
                        var results = await CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Camera);
                        //Best practice to always check that the key exists
                        if (results.ContainsKey(Plugin.Permissions.Abstractions.Permission.Camera))
                        {
                            status = results[Plugin.Permissions.Abstractions.Permission.Camera];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }
        }

        public void GetPermission()
        {
            //var name = Android.OS.Build.VERSION.SdkInt;     //Android Version Name like Kitkate etc... 
            //string version = Android.OS.Build.VERSION.Release;    //Android Version No like 4.4.4 etc... 

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
            {
                const string AccessFineLocationpermission = Manifest.Permission.AccessFineLocation;
                const string AccessCoarseLocationpermission = Manifest.Permission.AccessCoarseLocation;
                const string AccessLocationExtraCommandspermission = Manifest.Permission.AccessLocationExtraCommands;
                const string AccessMockLocationpermission = Manifest.Permission.AccessMockLocation;
                const string AccessNetworkStatepermission = Manifest.Permission.AccessNetworkState;
                const string ChangeWifiStatepermission = Manifest.Permission.ChangeWifiState;
                const string Internetpermission = Manifest.Permission.Internet;
                const string Camerapermission = Manifest.Permission.Camera;
                const string ReadExternalStoragepermission = Manifest.Permission.ReadExternalStorage;
                const string WriteExternalStoragepermission = Manifest.Permission.WriteExternalStorage;
                const string CallPhonepermission = Manifest.Permission.CallPhone;
                const string ReadContactspermission = Manifest.Permission.ReadContacts;
                const string WriteContactspermission = Manifest.Permission.WriteContacts;
                const string ReadCallLogpermission = Manifest.Permission.ReadCallLog;

                if (CheckSelfPermission(AccessFineLocationpermission) != (int)Android.Content.PM.Permission.Granted
                   || CheckSelfPermission(AccessCoarseLocationpermission) != (int)Android.Content.PM.Permission.Granted
                   || CheckSelfPermission(AccessLocationExtraCommandspermission) != (int)Android.Content.PM.Permission.Granted
                   || CheckSelfPermission(AccessMockLocationpermission) != (int)Android.Content.PM.Permission.Granted
                   || CheckSelfPermission(AccessNetworkStatepermission) != (int)Android.Content.PM.Permission.Granted
                   || CheckSelfPermission(ChangeWifiStatepermission) != (int)Android.Content.PM.Permission.Granted
                   || CheckSelfPermission(Internetpermission) != (int)Android.Content.PM.Permission.Granted
                   || CheckSelfPermission(Camerapermission) != (int)Android.Content.PM.Permission.Granted
                   || CheckSelfPermission(ReadExternalStoragepermission) != (int)Android.Content.PM.Permission.Granted
                   || CheckSelfPermission(WriteExternalStoragepermission) != (int)Android.Content.PM.Permission.Granted
                   || CheckSelfPermission(CallPhonepermission) != (int)Android.Content.PM.Permission.Granted
                   || CheckSelfPermission(ReadContactspermission) != (int)Android.Content.PM.Permission.Granted
                   || CheckSelfPermission(WriteContactspermission) != (int)Android.Content.PM.Permission.Granted
                   || CheckSelfPermission(ReadCallLogpermission) != (int)Android.Content.PM.Permission.Granted
                   )
                {
                    RequestPermissions(new string[]  {
                        Manifest.Permission.AccessFineLocation,
                        Manifest.Permission.AccessCoarseLocation,
                        Manifest.Permission.AccessLocationExtraCommands,
                        Manifest.Permission.AccessMockLocation,
                        Manifest.Permission.AccessNetworkState,
                        Manifest.Permission.ChangeWifiState,
                        Manifest.Permission.Internet,
                        Manifest.Permission.Camera,
                        Manifest.Permission.ReadExternalStorage,
                        Manifest.Permission.WriteExternalStorage,
                        Manifest.Permission.CallPhone,
                        Manifest.Permission.ReadContacts,
                        Manifest.Permission.WriteContacts,
                        Manifest.Permission.ReadCallLog,
                    },
                101);
                }
            }
        }
        #endregion
    }

    #region [ RazorPay Helper Class ]
    public class Metadata
    {
    }

    public class Error
    {
        public string code { get; set; }
        public string description { get; set; }
        public string source { get; set; }
        public string step { get; set; }
        public string reason { get; set; }
        public Metadata metadata { get; set; }
        public string field { get; set; }
    }

    public class OrderRequest
    {
        public Error error { get; set; }
    }
    #endregion
}
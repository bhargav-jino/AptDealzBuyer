using AptDealzBuyer.Interfaces;
using AptDealzBuyer.iOS.Service;
using DLToolkit.Forms.Controls;
using FFImageLoading.Forms.Platform;
using Foundation;
using Plugin.FirebasePushNotification;
using System;
using System.Collections.Generic;
using UIKit;
using UserNotifications;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AptDealzBuyer.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            try
            {
                global::Xamarin.Forms.Forms.Init();

                CachedImageRenderer.Init();
                FlowListView.Init();
                ZXing.Net.Mobile.Forms.iOS.Platform.Init();
                Rg.Plugins.Popup.Popup.Init();
                CarouselView.FormsPlugin.iOS.CarouselViewRenderer.Init();
                Firebase.Core.App.Configure();

                Plugin.LocalNotification.NotificationCenter.AskPermission();
                
                LoadApplication(new App());

                FirebasePushNotificationManager.Initialize(options, new NotificationUserCategory[]
                {
                     new NotificationUserCategory("message",new List<NotificationUserAction>
                     {
                         new NotificationUserAction("Reply","Reply",NotificationActionType.Foreground)
                     }),
                     new NotificationUserCategory("request",new List<NotificationUserAction>
                     {
                         new NotificationUserAction("Accept","Accept"),
                         new NotificationUserAction("Reject","Reject",NotificationActionType.Destructive)
                     })
                });

                FirebasePushNotificationManager.Initialize(options, true);
                DependencyService.Register<IFirebaseAuthenticator, FirebaseAuthenticator>();

                // Added by BK 10-14-2021
                FirebasePushNotificationManager.CurrentNotificationPresentationOption = UNNotificationPresentationOptions.Alert | UNNotificationPresentationOptions.Badge;
                UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate();

                return base.FinishedLaunching(app, options);
            }
            catch (Exception ex)
            {
                App.Current.MainPage.DisplayAlert("Exception-FinishedLaunching", ex.Message, "Ok");
                throw;
            }
        }

        /// <summary>
        /// Code Added By BK 10-13-2021
        /// </summary>
        /// <param name="application"></param>
        /// <param name="deviceToken"></param>
        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            try
            {
                FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken);
            }
            catch (Exception ex)
            {
                App.Current.MainPage.DisplayAlert("Exception-RegisteredForRemoteNotifications", ex.Message, "Ok");
            }
        }

        /// <summary>
        /// Code Added By BK 10-13-2021
        /// </summary>
        /// <param name="application"></param>
        /// <param name="error"></param>
        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            try
            {
                FirebasePushNotificationManager.RemoteNotificationRegistrationFailed(error);
            }
            catch (Exception ex)
            {
                App.Current.MainPage.DisplayAlert("Exception-FailedToRegisterForRemoteNotifications", ex.Message, "Ok");
            }
        }

        /// <summary>
        /// Code Added By BK 10-13-2021
        /// </summary>
        /// <param name="application"></param>
        /// <param name="userInfo"></param>
        /// <param name="completionHandler"></param>
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            try
            {
                FirebasePushNotificationManager.DidReceiveMessage(userInfo);
                completionHandler(UIBackgroundFetchResult.NewData);
                ProcessNotification(userInfo);
            }
            catch (Exception ex)
            {
                App.Current.MainPage.DisplayAlert("Exception-DidReceiveRemoteNotification", ex.Message, "Ok");
            }
        }

        /// <summary>
        /// Code Added By BK 10-14-2021
        /// </summary>
        /// <param name="application"></param>
        /// <param name="userInfo"></param>
        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            if (userInfo == null)
                return;

            ProcessNotification(userInfo);
        }

        /// <summary>
        /// Code Added By BK 10-14-2021
        /// </summary>
        /// <param name="options"></param>
        void ProcessNotification(NSDictionary options)
        {
            try
            {
                if (options != null && options.ContainsKey(new NSString("aps")))
                {
                    NSDictionary aps = options.ObjectForKey(new NSString("aps")) as NSDictionary;
                    string body = string.Empty;
                    string title = AppInfo.Name;
                    if (aps.ContainsKey(new NSString("alert")))
                    {
                        body = (aps[new NSString("alert")] as NSString).ToString();
                    }

                    if (!string.IsNullOrEmpty(body))
                    {
                        App.PushNotificationForiOS(title, body);
                    }
                }
            }
            catch (System.Exception ex)
            {
                App.Current.MainPage.DisplayAlert("ProcessNotification", ex.Message, "Ok");
            }
        }

        /// <summary>
        /// Code Added By BK 10-14-2021
        /// </summary>
        /// <param name="uiApplication"></param>
        public override void WillEnterForeground(UIApplication uiApplication)
        {
            Plugin.LocalNotification.NotificationCenter.ResetApplicationIconBadgeNumber(uiApplication);
        }

    }
}

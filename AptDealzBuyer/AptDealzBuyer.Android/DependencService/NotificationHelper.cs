using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Support.V4.App;
using AptDealzBuyer.Droid.DependencService;
using AptDealzBuyer.Utility;
using System;
using AndroidApp = Android.App.Application;

[assembly: Xamarin.Forms.Dependency(typeof(NotificationHelper))]

namespace AptDealzBuyer.Droid.DependencService
{
    public class NotificationHelper : INotificationHelper
    {
        const string channelId = "default";
        const string channelName = "Default";
        const string channelDescription = "The default channel for notifications.";
        const int pendingIntentId = 0;

        public const string TitleKey = "title";
        public const string MessageKey = "message";

        bool channelInitialized;
        int messageId = -1;
        NotificationManager manager;

        public event EventHandler NotificationReceived;
        //        Android.Net.Uri mute = Android.Net.Uri.Parse(Application.Context.PackageName + "/" + Resource.Raw.s);
        //        Android.Net.Uri unmute = Android.Net.Uri.Parse(Application.Context.PackageName + "/" + Resource.Raw.note);
        /// <summary>
        /// Initializes Required Properties for Notification in Android.
        /// </summary>
        /// <returns></returns>
        public void Initialize()
        {
            CreateNotificationChannel();
        }

        /// <summary>
        /// Schedules Notification to be sent.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public int ScheduleNotification(string title, string message)
        {
            if (!channelInitialized)
            {
                CreateNotificationChannel();
            }

            messageId++;

            Intent intent = new Intent(AndroidApp.Context, typeof(MainActivity));
            intent.PutExtra(TitleKey, title);
            intent.PutExtra(MessageKey, message);

            PendingIntent pendingIntent = PendingIntent.GetActivity(AndroidApp.Context, pendingIntentId, intent, PendingIntentFlags.OneShot);

            NotificationCompat.Builder builder = new NotificationCompat.Builder(AndroidApp.Context, channelId)
                .SetContentIntent(pendingIntent)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetSmallIcon(Resource.Drawable.iconLogo);
            var notification = builder.Build();
            manager.Notify(messageId, notification);

            return messageId;
        }

        /// <summary>
        /// Initialized NotificationEventArgs and invokes Notification Received.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public void ReceiveNotification(string title, string message)
        {
            var args = new NotificationEventArgs()
            {
                Title = title,
                Message = message,
            };
            NotificationReceived?.Invoke(null, args);
        }

        /// <summary>
        /// Creates Notification Channel for Android.
        /// </summary>
        void CreateNotificationChannel()
        {
            var alarmAttributes = new AudioAttributes.Builder()
                    .SetContentType(AudioContentType.Sonification)
                    .SetUsage(AudioUsageKind.Notification).Build();

            manager = (NotificationManager)AndroidApp.Context.GetSystemService(AndroidApp.NotificationService);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelNameJava = new Java.Lang.String(channelName);
                var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.Default)
                {
                    Description = channelDescription,
                };
                manager.CreateNotificationChannel(channel);
            }

            channelInitialized = true;
        }
    }
}
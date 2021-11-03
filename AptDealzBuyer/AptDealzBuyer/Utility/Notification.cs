using AptDealzBuyer.Constants;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AptDealzBuyer.Utility
{
    public static class Notification
    {
        /// <summary>
        /// Show Notification method invokes local notification for iOS and Android
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public static void ShowNotification(string title, string message)
        {
            var isEnable = Preferences.Get(AppKeys.Notification, true);
            if (isEnable)
            {
                INotificationHelper notificationManager;
                notificationManager = DependencyService.Get<INotificationHelper>();
                notificationManager.Initialize();
                notificationManager.ScheduleNotification(title, message);
            }
        }
    }

    public interface INotificationHelper
    {
        event EventHandler NotificationReceived;

        void Initialize();

        int ScheduleNotification(string title, string message);

        void ReceiveNotification(string title, string message);
    }

    public class NotificationEventArgs : EventArgs
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }
}

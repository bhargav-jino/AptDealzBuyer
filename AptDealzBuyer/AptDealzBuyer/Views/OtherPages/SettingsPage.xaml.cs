using AptDealzBuyer.Utility;
using AptDealzBuyer.Views.DashboardPages;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AptDealzBuyer.Views.OtherPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        #region [ Ctor ]
        public SettingsPage()
        {
            InitializeComponent();

            MessagingCenter.Unsubscribe<string>(this, Constraints.Str_NotificationCount); MessagingCenter.Subscribe<string>(this, Constraints.Str_NotificationCount, (count) =>
            {
                if (!Common.EmptyFiels(Common.NotificationCount))
                {
                    lblNotificationCount.Text = count;
                    frmNotification.IsVisible = true;
                }
                else
                {
                    frmNotification.IsVisible = false;
                    lblNotificationCount.Text = string.Empty;
                }
            });
        }
        #endregion

        #region [ Methods ]
        public void Dispose()
        {
            GC.Collect();
            GC.SuppressFinalize(this);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Dispose();
        }
        #endregion

        #region [ Events ]
        private void BtnLogo_Clicked(object sender, EventArgs e)
        {
            Common.MasterData.Detail = new NavigationPage(new MainTabbedPages.MainTabbedPage("Home"));
        }

        private async void ImgNotification_Tapped(object sender, EventArgs e)
        {
            var Tab = (Grid)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    await Navigation.PushAsync(new NotificationPage());
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("SettingsPage/ImgNotification_Tapped: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }

        private void ImgQuestion_Tapped(object sender, EventArgs e)
        {
            Common.MasterData.Detail = new NavigationPage(new MainTabbedPages.MainTabbedPage("FAQHelp"));
        }

        private void ImgMenu_Tapped(object sender, EventArgs e)
        {

        }

        private async void ImgBack_Tapped(object sender, EventArgs e)
        {
            Common.BindAnimation(imageButton: ImgBack);
            await Navigation.PopAsync();
        }

        private void BtnLight_Clicked(object sender, EventArgs e)
        {

        }

        private void BtnDark_Clicked(object sender, EventArgs e)
        {

        }

        private void pkAlertTone_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void BtnAlertTone_Clicked(object sender, EventArgs e)
        {
            pkAlertTone.Focus();
        }

        private void BtnMuteNotifications_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (BtnMuteNotifications.Source.ToString().Replace("File: ", "") == Constraints.Img_SwitchOff)
                {
                    BtnMuteNotifications.Source = Constraints.Img_SwitchOn;
                }
                else
                {
                    BtnMuteNotifications.Source = Constraints.Img_SwitchOff;
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("SettingsPage/BtnMuteNotifications_Clicked: " + ex.Message);
            }
        }

        private void Picker_Unfocused(object sender, FocusEventArgs e)
        {

        }
        #endregion
    }
}
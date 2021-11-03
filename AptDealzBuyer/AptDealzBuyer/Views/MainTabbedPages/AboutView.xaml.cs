using Acr.UserDialogs;
using AptDealzBuyer.API;
using AptDealzBuyer.Model.Reponse;
using AptDealzBuyer.Utility;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AptDealzBuyer.Views.MainTabbedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutView : ContentView
    {
        #region [ Constructor ]
        public AboutView()
        {
            try
            {
                InitializeComponent();
                BindAboutApzdealz();
                MessagingCenter.Unsubscribe<string>(this, Constraints.Str_NotificationCount);
                MessagingCenter.Subscribe<string>(this, Constraints.Str_NotificationCount, (count) =>
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
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("AboutView/Ctor: " + ex.Message);
            }
        }
        #endregion

        async void BindAboutApzdealz()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Loading...");
                AppSettingsAPI appSettingsAPI = new AppSettingsAPI();
                var mResponse = await appSettingsAPI.AboutAptdealzBuyerApp();
                UserDialogs.Instance.HideLoading();

                if (mResponse != null && mResponse.Succeeded)
                {
                    var jObject = (Newtonsoft.Json.Linq.JObject)mResponse.Data;
                    if (jObject != null)
                    {
                        var mAboutAptDealz = jObject.ToObject<AboutAptDealz>();
                        if (mAboutAptDealz != null)
                        {
                            lblAbout.Text = mAboutAptDealz.About;
                            lblAddress1.Text = mAboutAptDealz.ContactAddressLine1;
                            lblAddress2.Text = mAboutAptDealz.ContactAddressLine2;
                            lblPincode.Text = "PIN - " + mAboutAptDealz.ContactAddressPincode;
                            lblEmail.Text = "Email : " + mAboutAptDealz.ContactAddressEmail;
                            lblPhoneNo.Text = "Phone : " + mAboutAptDealz.ContactAddressPhone;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        #region [ Events ]
        private void ImgMenu_Tapped(object sender, EventArgs e)
        {
            Common.BindAnimation(image: ImgMenu);
            //Common.OpenMenu();
        }

        private async void ImgNotification_Tapped(object sender, EventArgs e)
        {
            var Tab = (Grid)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    await Navigation.PushAsync(new DashboardPages.NotificationPage());
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("AboutView/ImgNotification_Tapped: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }

        private void ImgQuestion_Tapped(object sender, EventArgs e)
        {
            Common.MasterData.Detail = new NavigationPage(new MainTabbedPages.MainTabbedPage(Constraints.Str_FAQHelp));
        }

        private void ImgBack_Tapped(object sender, EventArgs e)
        {
            Common.BindAnimation(imageButton: ImgBack);
            Common.MasterData.Detail = new NavigationPage(new MainTabbedPages.MainTabbedPage(Constraints.Str_Home));
        }

        private void BtnLogo_Clicked(object sender, EventArgs e)
        {
            Common.MasterData.Detail = new NavigationPage(new MainTabbedPages.MainTabbedPage(Constraints.Str_Home));
        }
        #endregion
    }
}
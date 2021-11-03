using Acr.UserDialogs;
using AptDealzBuyer.API;
using AptDealzBuyer.Interfaces;
using AptDealzBuyer.Model;
using AptDealzBuyer.Model.Reponse;
using AptDealzBuyer.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AptDealzBuyer.Views.SplashScreen
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class WelcomePage : ContentPage
    {
        #region [ Constructor ]
        public WelcomePage()
        {
            InitializeComponent();
            Settings.IsViewWelcomeScreen = false;
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            GetAppPromoBar();
        }

        private async void GetAppPromoBar()
        {
            try
            {
                Indicators.ItemsSource = null;

                List<AppPromo> mAppPromos = new List<AppPromo>();
                AppSettingsAPI appSettingsAPI = new AppSettingsAPI();
                UserDialogs.Instance.ShowLoading("Loading...");
                var mResponse = await appSettingsAPI.GetAppPromoBar();
                UserDialogs.Instance.HideLoading();
                if (mResponse != null && mResponse.Succeeded)
                {
                    JArray result = (JArray)mResponse.Data;
                    if (result != null)
                    {
                        mAppPromos = result.ToObject<List<AppPromo>>();
                        if (mAppPromos != null && mAppPromos.Count > 0)
                        {
                            Indicators.ItemsSource = cvWelcome.ItemsSource = mAppPromos.ToList();
                            lblNoRecord.IsVisible = false;
                        }
                        else
                        {
                            lblNoRecord.IsVisible = true;
                        }
                    }
                }
                else
                {
                    if (mResponse != null && !Common.EmptyFiels(mResponse.Message))
                        lblNoRecord.Text = mResponse.Message;
                    else
                        lblNoRecord.Text = Constraints.Something_Wrong;
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();
                Common.DisplayErrorMessage("WelcomePage/GetAppPromoBar: " + ex.Message);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            try
            {
                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        var result = await DisplayAlert(Constraints.Alert, Constraints.DoYouWantToExit, Constraints.Yes, Constraints.No);
                        if (result)
                        {
                            Xamarin.Forms.DependencyService.Get<ICloseAppOnBackButton>().CloseApp();
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("WelcomePage/OnBackButtonPressed: " + ex.Message);
            }
            return true;
        }

        #endregion

        #region [ Events ]  
        private async void BtnLogin_Clicked(object sender, EventArgs e)
        {
            var Tab = (Button)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    Common.BindAnimation(button: BtnLogin);
                    await Navigation.PushAsync(new Login.LoginPage());
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("WelcomePage/BtnLogin_Clicked: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }
        #endregion
    }
}
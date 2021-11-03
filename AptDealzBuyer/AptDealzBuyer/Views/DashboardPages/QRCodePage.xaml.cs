using Acr.UserDialogs;
using AptDealzBuyer.API;
using AptDealzBuyer.Model.Reponse;
using AptDealzBuyer.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AptDealzBuyer.Views.DashboardPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QRCodePage : ContentPage
    {
        private string OrderId;

        public QRCodePage(string OrderId = "")
        {
            InitializeComponent();
            this.OrderId = OrderId;

        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await GenerateQRCodeImageForBuyerApp(OrderId);
        }

        public async Task GenerateQRCodeImageForBuyerApp(string OrderId)
        {
            try
            {
                OrderAPI mOrderAPI = new OrderAPI();
                UserDialogs.Instance.ShowLoading(Constraints.Loading);
                var mResponse = await mOrderAPI.GenerateQRCodeImageForBuyerApp(OrderId);
                UserDialogs.Instance.HideLoading();
                if (mResponse != null && mResponse.Succeeded)
                {
                    string imageBase64 = (string)mResponse.Data;
                    if (!Common.EmptyFiels(imageBase64))
                    {
                        lblNotificationCount.IsVisible = false;
                        imgQRCode.IsVisible = true;
                        imgQRCode.Source = Xamarin.Forms.ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(imageBase64)));
                    }
                }
                else if(mResponse != null && !mResponse.Succeeded)
                {
                    imgQRCode.IsVisible = false;
                    lblQRCode.IsVisible = true;
                    lblQRCode.Text = mResponse.Message;
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("QRCodePage/GenerateQRCodeImageForBuyerApp: " + ex.Message);
            }
        }

        private void BtnLogo_Clicked(object sender, EventArgs e)
        {
            Common.MasterData.Detail = new NavigationPage(new MainTabbedPages.MainTabbedPage(Constraints.Str_Home));
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
                    Common.DisplayErrorMessage("QRCodePage/ImgNotification_Tapped: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }

        private void ImgQuestion_Tapped(object sender, EventArgs e)
        {
            try
            {
                Common.MasterData.Detail = new NavigationPage(new MainTabbedPages.MainTabbedPage(Constraints.Str_FAQHelp));
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("QRCodePage/ImgQuestion_Tapped: " + ex.Message);
            }
        }

        private void ImgMenu_Tapped(object sender, EventArgs e)
        {
            try
            {
                Common.BindAnimation(image: ImgMenu);
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("QRCodePage/ImgMenu_Tapped: " + ex.Message);
            }
        }

        private async void ImgBack_Tapped(object sender, EventArgs e)
        {
            try
            {
                Common.BindAnimation(imageButton: ImgBack);
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("QRCodePage/ImgBack_Tapped: " + ex.Message);
            }
        }
    }
}
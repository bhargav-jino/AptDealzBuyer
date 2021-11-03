using AptDealzBuyer.Repository;
using AptDealzBuyer.Utility;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AptDealzBuyer.Views.OtherPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeactivateAccountPage : ContentPage
    {
        #region [ Constructor ]
        public DeactivateAccountPage()
        {
            InitializeComponent();

            if (DeviceInfo.Platform == DevicePlatform.Android)
                txtReason.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);

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
                    Common.DisplayErrorMessage("DeactivateAccountPage/ImgNotification_Tapped: " + ex.Message);
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

        private async void ImgBack_Tapped(object sender, EventArgs e)
        {
            Common.BindAnimation(imageButton: ImgBack);
            await Navigation.PopAsync();
        }

        private async void BtnDeactivation_Clicked(object sender, EventArgs e)
        {
            var Tab = (Button)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    Common.BindAnimation(button: BtnDeactivation);
                    if (!Common.EmptyFiels(txtReason.Text))
                    {
                        BoxReason.BackgroundColor = (Color)App.Current.Resources["appColor8"];
                        await DependencyService.Get<IProfileRepository>().DeactivateAccount();
                    }
                    else
                    {
                        BoxReason.BackgroundColor = (Color)App.Current.Resources["appColor3"];
                        Common.DisplayErrorMessage(Constraints.Required_Reason);
                    }
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("DeactivateAccountPage/BtnDeactivation_Clicked: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }

        private void BtnLogo_Clicked(object sender, EventArgs e)
        {
            Common.MasterData.Detail = new NavigationPage(new MainTabbedPages.MainTabbedPage(Constraints.Str_Home));
        }
        #endregion
    }
}
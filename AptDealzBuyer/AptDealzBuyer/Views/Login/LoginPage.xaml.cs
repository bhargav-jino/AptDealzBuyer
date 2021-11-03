using Acr.UserDialogs;
using AptDealzBuyer.API;
using AptDealzBuyer.Interfaces;
using AptDealzBuyer.Model.Reponse;
using AptDealzBuyer.Utility;
using AptDealzBuyer.Views.MasterData;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AptDealzBuyer.Views.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        #region [ Objects ]
        private bool isEmail = false;
        #endregion

        #region [ Constructor ]
        public LoginPage()
        {
            InitializeComponent();
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
            BindProperties();
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
                Common.DisplayErrorMessage("LoginPage/OnBackButtonPressed: " + ex.Message);
            }
            return true;
        }

        private void BindProperties()
        {
            try
            {
                if (!Common.EmptyFiels(Settings.EmailAddress))
                    txtUserAuth.Text = Settings.EmailAddress;
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("LoginPage/BindProperties: " + ex.Message);
            }
        }

        private bool Validations()
        {
            bool isValid = false;
            try
            {
                if (Common.EmptyFiels(txtUserAuth.Text))
                {
                    BoxUserAuth.BackgroundColor = (Color)App.Current.Resources["appColor3"];
                    Common.DisplayErrorMessage(Constraints.Required_Email_Phone);
                }
                else if (txtUserAuth.Text.Contains("@") || txtUserAuth.Text.Contains("."))
                {
                    if (!txtUserAuth.Text.IsValidEmail())
                    {
                        Common.DisplayErrorMessage(Constraints.InValid_Email);
                    }
                    else
                    {
                        isEmail = true;
                        isValid = true;
                    }
                }
                else if (!txtUserAuth.Text.IsValidPhone())
                {
                    Common.DisplayErrorMessage(Constraints.InValid_PhoneNumber);
                }
                else
                {
                    isEmail = false;
                    isValid = true;
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("LoginPage/Validations: " + ex.Message);
            }
            return isValid;
        }

        private void FieldsTrim()
        {
            txtUserAuth.Text = txtUserAuth.Text.Trim();
        }

        private async Task AuthenticateUser()
        {
            try
            {
                if (Validations())
                {
                    FieldsTrim();
                    AuthenticationAPI authenticationAPI = new AuthenticationAPI();
                    if (isEmail)
                    {
                        UserDialogs.Instance.ShowLoading(Constraints.Loading);
                        var mResponse = await authenticationAPI.SendOtpByEmail(txtUserAuth.Text);
                        if (mResponse != null && mResponse.Succeeded)
                        {
                            Common.DisplaySuccessMessage(mResponse.Message);
                            await Navigation.PushAsync(new Views.Login.EnterOtpPage(txtUserAuth.Text));
                            txtUserAuth.Text = string.Empty;
                        }
                        else
                        {
                            if (mResponse != null)
                                Common.DisplayErrorMessage(mResponse.Message);
                            else
                                Common.DisplayErrorMessage(Constraints.Something_Wrong);
                        }
                    }
                    else
                    {
                        UserDialogs.Instance.ShowLoading(Constraints.Loading);
                        var mResponse = await authenticationAPI.CheckPhoneNumberExists(txtUserAuth.Text);
                        if (mResponse != null)
                        {
                            bool isValidNumber = (bool)mResponse.Data;
                            if (isValidNumber && mResponse != null)
                            {
                                var result = await Xamarin.Forms.DependencyService.Get<IFirebaseAuthenticator>().SendOtpCodeAsync(txtUserAuth.Text);
                                var keyValue = result.FirstOrDefault();

                                if (keyValue.Key)
                                {
                                    if (keyValue.Value == Constraints.OTPSent)
                                    {
                                        await Navigation.PushAsync(new Views.Login.EnterOtpPage(txtUserAuth.Text, false));
                                        txtUserAuth.Text = string.Empty;
                                    }
                                    else
                                    {
                                        Settings.PhoneAuthToken = keyValue.Value;

                                        var mLogin = FillLogin();
                                        mResponse = await authenticationAPI.BuyerAuthPhone(mLogin);
                                        NavigateToDashboard(mResponse);
                                    }
                                }
                                else if (!string.IsNullOrEmpty(keyValue.Value))
                                {
                                    Common.DisplayErrorMessage(keyValue.Value);
                                }
                                else
                                {
                                    Common.DisplayErrorMessage(Constraints.CouldNotSentOTP);
                                }
                            }
                            else
                            {
                                if (mResponse != null)
                                    Common.DisplayErrorMessage(mResponse.Message);
                                else
                                    Common.DisplayErrorMessage(Constraints.Something_Wrong);
                            }
                        }
                        else
                        {
                            Common.DisplayErrorMessage(Constraints.Something_Wrong);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("LoginPage/AuthenticateUser: " + ex.Message);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private Model.Request.AuthenticatePhone FillLogin()
        {
            Model.Request.AuthenticatePhone mLogin = new Model.Request.AuthenticatePhone();
            try
            {
                mLogin.PhoneNumber = txtUserAuth.Text;
                if (!Common.EmptyFiels(Settings.fcm_token))
                {
                    mLogin.FcmToken = Settings.fcm_token;
                }
                if (!Common.EmptyFiels(Settings.PhoneAuthToken))
                {
                    mLogin.FirebaseVerificationId = Settings.PhoneAuthToken;
                }
                else
                {
                    Common.DisplayErrorMessage(Constraints.Something_Wrong);
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
            return mLogin;
        }

        private void NavigateToDashboard(Response mResponse)
        {
            try
            {
                if (mResponse != null && mResponse.Succeeded)
                {
                    var jObject = (Newtonsoft.Json.Linq.JObject)mResponse.Data;
                    if (jObject != null)
                    {
                        var mBuyer = jObject.ToObject<Model.Request.Buyer>();
                        if (mBuyer != null)
                        {
                            Settings.UserId = mBuyer.Id;
                            Settings.RefreshToken = mBuyer.RefreshToken;
                            Settings.LoginTrackingKey = mBuyer.LoginTrackingKey == "00000000-0000-0000-0000-000000000000" ? Settings.LoginTrackingKey : mBuyer.LoginTrackingKey;
                            Common.Token = mBuyer.JwToken;
                            Settings.UserToken = mBuyer.JwToken;

                            App.Current.MainPage = new MasterDataPage();
                        }
                    }
                }
                else
                {
                    if (mResponse != null)
                        Common.DisplayErrorMessage(mResponse.Message);
                    else
                        Common.DisplayErrorMessage(Constraints.Something_Wrong);
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("LoginPage/NavigateToDashboard: " + ex.Message);
            }
        }
        #endregion

        #region [ Events ]  
        private async void StkSignup_Tapped(object sender, EventArgs e)
        {
            var Tab = (StackLayout)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    await Navigation.PushAsync(new Login.SignupPage());
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("LoginPage/StkSignup_Tapped: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }

        }

        private async void BtnGetOtp_Clicked(object sender, EventArgs e)
        {
            var Tab = (Button)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    Common.BindAnimation(button: BtnGetOtp);
                    await AuthenticateUser();
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("LoginPage/BtnGetOtp_Clicked: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }

        }

        private void txtUserAuth_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = (Extention.ExtEntry)sender;
            if (!Common.EmptyFiels(entry.Text))
            {
                BoxUserAuth.BackgroundColor = (Color)App.Current.Resources["appColor8"];
            }
        }
        #endregion
    }
}
using Acr.UserDialogs;
using AptDealzBuyer.API;
using AptDealzBuyer.Interfaces;
using AptDealzBuyer.Model.Reponse;
using AptDealzBuyer.Model.Request;
using AptDealzBuyer.Utility;
using AptDealzBuyer.Views.MasterData;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AptDealzBuyer.Views.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EnterOtpPage : ContentPage
    {
        #region [ Objects ]
        private string UserAuth;
        private bool isEmail;
        private bool IsRegister;
        private string OTPString;
        private Register mRegister;
        #endregion

        #region [ Constructor ]
        public EnterOtpPage(string UserAuth, bool isEmail = true)
        {
            InitializeComponent();
            this.UserAuth = UserAuth;
            this.isEmail = isEmail;
            ResendButtonEnable();
        }

        public EnterOtpPage(Register register)
        {
            InitializeComponent();
            mRegister = register;
            this.IsRegister = true;
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

        private void ResendButtonEnable()
        {
            try
            {
                BtnResentOtp.IsEnabled = false;
                int i = 120;

                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    BtnResentOtp.Text = i + " sec";
                    if (i == 0)
                    {
                        BtnResentOtp.IsEnabled = true;
                        BtnResentOtp.Text = "Resend OTP";
                        return false;
                    }
                    i--;
                    return true;
                });
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("EnterOtpPage/ResendButtonEnable: " + ex.Message);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private bool Validations()
        {
            bool isValid = false;
            try
            {
                if (Common.EmptyFiels(TxtOtpOne.Text)
                  || Common.EmptyFiels(TxtOtpTwo.Text)
                  || Common.EmptyFiels(TxtOtpThree.Text)
                  || Common.EmptyFiels(TxtOtpFour.Text)
                  || Common.EmptyFiels(TxtOtpFive.Text)
                  || Common.EmptyFiels(TxtOtpSix.Text))
                {
                    Common.DisplayErrorMessage(Constraints.Required_VerificationCode);
                }
                else
                {
                    isValid = true;
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("EnterOtpPage/Validations: " + ex.Message);
            }
            return isValid;
        }

        private async void RegisterBuyer()
        {
            try
            {
                mRegister.FirebaseVerificationId = Settings.PhoneAuthToken;
                RegisterAPI registerAPI = new RegisterAPI();
                var mResponse = await registerAPI.Register(mRegister);
                if (mResponse != null && mResponse.Succeeded)
                {
                    App.Current.MainPage = new NavigationPage(new Views.Login.LoginPage());
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
                Common.DisplayErrorMessage("EnterOtpPage/RegisterBuyer: " + ex.Message);
            }
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
                        var mBuyer = jObject.ToObject<Buyer>();
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
                Common.DisplayErrorMessage("EnterOtpPage/NavigateToDashboard: " + ex.Message);
            }
        }

        private Model.Request.AuthenticatePhone FillPhoneAuthentication()
        {
            Model.Request.AuthenticatePhone mAuthenticatePhone = new Model.Request.AuthenticatePhone();
            try
            {
                mAuthenticatePhone.PhoneNumber = UserAuth;
                if (!Common.EmptyFiels(Settings.fcm_token))
                {
                    mAuthenticatePhone.FcmToken = Settings.fcm_token;
                }
                if (!Common.EmptyFiels(Settings.PhoneAuthToken))
                {
                    mAuthenticatePhone.FirebaseVerificationId = Settings.PhoneAuthToken;
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
            return mAuthenticatePhone;
        }

        private Model.Request.AuthenticateEmail FillEmailAuthentication()
        {
            Model.Request.AuthenticateEmail mAuthenticateEmail = new Model.Request.AuthenticateEmail();
            try
            {
                mAuthenticateEmail.Email = UserAuth;
                OTPString = TxtOtpOne.Text + TxtOtpTwo.Text + TxtOtpThree.Text + TxtOtpFour.Text + TxtOtpFive.Text + TxtOtpSix.Text;
                mAuthenticateEmail.Otp = OTPString;
                if (!Common.EmptyFiels(Settings.fcm_token))
                {
                    mAuthenticateEmail.FcmToken = Settings.fcm_token;
                }
            }
            catch (Exception)
            {
                return null;
            }
            return mAuthenticateEmail;
        }

        private async Task SubmitOTP()
        {
            try
            {
                if (Validations())
                {
                    UserDialogs.Instance.ShowLoading(Constraints.Loading);
                    AuthenticationAPI authenticationAPI = new AuthenticationAPI();

                    Response mResponse = new Response();
                    if (!this.isEmail)
                    {
                        OTPString = TxtOtpOne.Text + TxtOtpTwo.Text + TxtOtpThree.Text + TxtOtpFour.Text + TxtOtpFive.Text + TxtOtpSix.Text;

                        var token = await Xamarin.Forms.DependencyService.Get<IFirebaseAuthenticator>().VerifyOtpCodeAsync(OTPString);
                        if (!Common.EmptyFiels(token))
                        {
                            Settings.PhoneAuthToken = token;
                            if (IsRegister)
                            {
                                RegisterBuyer();
                            }
                            else
                            {
                                var mLogin = FillPhoneAuthentication();
                                mResponse = await authenticationAPI.BuyerAuthPhone(mLogin);
                                NavigateToDashboard(mResponse);
                            }
                        }
                        else
                        {
                            Common.DisplayErrorMessage(Constraints.InValid_OTP);
                        }
                    }
                    else
                    {
                        if (IsRegister)
                        {
                            RegisterBuyer();
                        }
                        else
                        {
                            var mLogin = FillEmailAuthentication();
                            mResponse = await authenticationAPI.BuyerAuthEmail(mLogin);
                            NavigateToDashboard(mResponse);
                        }
                    }
                }
                else
                {
                    Common.DisplayErrorMessage(Constraints.Required_VerificationCode);
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("EnterOtpPage/SubmitOTP: " + ex.Message);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private async Task ResentOTP()
        {
            try
            {
                OTPString = string.Empty;
                TxtOtpOne.Text = string.Empty;
                TxtOtpTwo.Text = string.Empty;
                TxtOtpThree.Text = string.Empty;
                TxtOtpFour.Text = string.Empty;
                TxtOtpFive.Text = string.Empty;
                TxtOtpSix.Text = string.Empty;

                AuthenticationAPI authenticationAPI = new AuthenticationAPI();
                UserDialogs.Instance.ShowLoading(Constraints.Loading);
                if (this.isEmail)
                {
                    var mResponse = await authenticationAPI.SendOtpByEmail(UserAuth);
                    if (mResponse != null && mResponse.Succeeded)
                    {
                        Common.DisplaySuccessMessage(mResponse.Message);
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
                    var result = await Xamarin.Forms.DependencyService.Get<IFirebaseAuthenticator>().SendOtpCodeAsync(UserAuth);
                    var keyValue = result.FirstOrDefault();

                    if (!keyValue.Key)
                    {
                        Common.DisplayErrorMessage("Could not send Verification Code to the given number!");
                    }
                    else
                    {
                        if (keyValue.Value != Constraints.OTPSent)
                        {
                            Settings.PhoneAuthToken = keyValue.Value;

                            var mLogin = FillPhoneAuthentication();
                            var mResponse = await authenticationAPI.BuyerAuthPhone(mLogin);
                            NavigateToDashboard(mResponse);
                        }
                    }
                }

                ResendButtonEnable();
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("EnterOtpPage/ResentOTP: " + ex.Message);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }
        #endregion

        #region [ Events ]
        private async void ImgBack_Tapped(object sender, EventArgs e)
        {
            Common.BindAnimation(image: ImgBack);
            await Navigation.PopAsync();
        }

        private async void BtnSubmit_Tapped(object sender, EventArgs e)
        {
            var Tab = (Button)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    Common.BindAnimation(button: BtnSubmit);
                    await SubmitOTP();
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("EnterOtpPage/BtnSubmit_Tapped: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }

        }

        private async void BtnResentOtp_Tapped(object sender, EventArgs e)
        {
            await ResentOTP();
        }

        private void TxtOtpOne_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Common.EmptyFiels(TxtOtpOne.Text))
                TxtOtpTwo.Focus();
        }

        private void TxtOtpTwo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Common.EmptyFiels(TxtOtpTwo.Text))
                TxtOtpThree.Focus();
            else
                TxtOtpOne.Focus();
        }

        private void TxtOtpThree_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Common.EmptyFiels(TxtOtpThree.Text))
                TxtOtpFour.Focus();
            else
                TxtOtpTwo.Focus();
        }

        private void TxtOtpFour_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Common.EmptyFiels(TxtOtpFour.Text))
                TxtOtpFive.Focus();
            else
                TxtOtpThree.Focus();
        }

        private void TxtOtpFive_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Common.EmptyFiels(TxtOtpFive.Text))
                TxtOtpSix.Focus();
            else
                TxtOtpFour.Focus();
        }

        private void TxtOtpSix_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Common.EmptyFiels(TxtOtpSix.Text))
            {
                TxtOtpSix.Unfocus();
                BtnSubmit.BackgroundColor = (Color)App.Current.Resources["appColor1"];
            }
            else
                TxtOtpFive.Focus();
        }
        #endregion
    }
}
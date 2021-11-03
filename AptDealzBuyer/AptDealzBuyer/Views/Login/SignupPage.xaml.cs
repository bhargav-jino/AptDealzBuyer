using Acr.UserDialogs;
using AptDealzBuyer.API;
using AptDealzBuyer.Interfaces;
using AptDealzBuyer.Model.Reponse;
using AptDealzBuyer.Model.Request;
using AptDealzBuyer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AptDealzBuyer.Views.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignupPage : ContentPage
    {
        #region [ Objects ]
        private bool isChecked = false;
        #endregion

        #region [ Constructor ]
        public SignupPage()
        {
            InitializeComponent();

            if (DeviceInfo.Platform == DevicePlatform.Android)
                txtFullName.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);
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

            if (isChecked)
                imgCheck.Source = Constraints.Img_CheckBoxChecked;
            else
                imgCheck.Source = Constraints.Img_CheckBoxUnChecked;
        }

        private bool Validations()
        {
            bool isValid = false;
            try
            {
                if (Common.EmptyFiels(txtFullName.Text) || Common.EmptyFiels(txtEmailAddress.Text) || Common.EmptyFiels(txtPhoneNumber.Text))
                {
                    RequiredFields();
                    isValid = false;
                }

                if (Common.EmptyFiels(txtFullName.Text))
                {
                    Common.DisplayErrorMessage(Constraints.Required_FullName);
                }
                else if (Common.EmptyFiels(txtEmailAddress.Text))
                {
                    Common.DisplayErrorMessage(Constraints.Required_Email);
                }
                else if (!txtEmailAddress.Text.IsValidEmail())
                {
                    Common.DisplayErrorMessage(Constraints.InValid_Email);
                }
                else if (Common.EmptyFiels(txtPhoneNumber.Text))
                {
                    Common.DisplayErrorMessage(Constraints.Required_PhoneNumber);
                }
                else if (!txtPhoneNumber.Text.IsValidPhone())
                {
                    Common.DisplayErrorMessage(Constraints.InValid_PhoneNumber);
                }
                else if (!isChecked)
                {
                    Common.DisplayErrorMessage(Constraints.Agree_T_C);
                }
                else
                {
                    isValid = true;
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("SignupPage/Validations: " + ex.Message);
            }
            return isValid;
        }

        private void RequiredFields()
        {
            try
            {
                if (Common.EmptyFiels(txtFullName.Text))
                {
                    BoxFullName.BackgroundColor = (Color)App.Current.Resources["appColor3"];
                }

                if (Common.EmptyFiels(txtEmailAddress.Text))
                {
                    BoxEmailAddress.BackgroundColor = (Color)App.Current.Resources["appColor3"];
                }

                if (Common.EmptyFiels(txtPhoneNumber.Text))
                {
                    BoxPhoneNumber.BackgroundColor = (Color)App.Current.Resources["appColor3"];
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("SignupPage/RequiredFields: " + ex.Message);
            }
        }

        private void FieldsTrim()
        {
            try
            {
                txtFullName.Text = txtFullName.Text.Trim();
                txtEmailAddress.Text = txtEmailAddress.Text.Trim();
                txtPhoneNumber.Text = txtPhoneNumber.Text.Trim();
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("SignupPage/FieldsTrim: " + ex.Message);
            }
        }

        private Model.Request.Register FillRegister()
        {
            Register mRegister = new Register();
            try
            {
                mRegister.FullName = txtFullName.Text;
                mRegister.Email = txtEmailAddress.Text;
                mRegister.PhoneNumber = txtPhoneNumber.Text;
                mRegister.FirebaseVerificationId = Settings.PhoneAuthToken;
                mRegister.Latitude = App.latitude;
                mRegister.Longitude = App.longitude;
            }
            catch (Exception)
            {
                return null;
            }

            return mRegister;
        }

        private async Task RegisterUser()
        {
            try
            {
                if (Validations())
                {
                    FieldsTrim();
                    RegisterAPI registerAPI = new RegisterAPI();
                    UniquePhoneNumber mUniquePhoneNumber = new UniquePhoneNumber();
                    mUniquePhoneNumber.PhoneNumber = txtPhoneNumber.Text;

                    UniqueEmail mUniqueEmail = new UniqueEmail();
                    mUniqueEmail.Email = txtEmailAddress.Text;

                    var mRegister = FillRegister();
                    UserDialogs.Instance.ShowLoading(Constraints.Loading);
                    Response mResponse = new Response();
                    mResponse = await registerAPI.IsUniqueEmail(mUniqueEmail);

                    if (mResponse != null && mResponse.Succeeded)
                    {
                        var UniqueEmail = (bool)mResponse.Data;
                        if (UniqueEmail)
                        {
                            mResponse = await registerAPI.IsUniquePhoneNumber(mUniquePhoneNumber);
                            var UniquePhone = (bool)mResponse.Data;
                            if (UniquePhone)
                            {
                                var keyValuePairs = await SendOTP(mRegister.PhoneNumber);
                                var keyValue = keyValuePairs.FirstOrDefault();
                                if (keyValue.Key)
                                {
                                    if (keyValue.Value == Constraints.OTPSent)
                                    {
                                        var _verificationId = Xamarin.Forms.DependencyService.Get<IFirebaseAuthenticator>()._verificationId;
                                        await Navigation.PushAsync(new Views.Login.EnterOtpPage(mRegister));
                                    }
                                    else
                                    {
                                        //Common.DisplayWarningMessage(keyValue.Value);                                       
                                        Settings.PhoneAuthToken = keyValue.Value;

                                        mRegister.FirebaseVerificationId = Settings.PhoneAuthToken;
                                        mResponse = await registerAPI.Register(mRegister);
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
                            if (mResponse != null)
                                Common.DisplayErrorMessage(mResponse.Message);
                            else
                                Common.DisplayErrorMessage(Constraints.Something_Wrong);
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
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("SignupPage/RegisterUser: " + ex.Message);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private async Task<Dictionary<bool, string>> SendOTP(string phoneNumber)
        {
            Dictionary<bool, string> keyValuePairs = new Dictionary<bool, string>();
            try
            {
                keyValuePairs = await Xamarin.Forms.DependencyService.Get<IFirebaseAuthenticator>().SendOtpCodeAsync(phoneNumber);
                var keyValue = keyValuePairs.FirstOrDefault();

                if (!keyValue.Key)
                {
                    var isTryAgain = await UserDialogs.Instance.ConfirmAsync(Constraints.CouldNotSentOTP, "Verification", "Try Again", "Goto Login");
                    if (isTryAgain)
                        return await SendOTP(phoneNumber);
                    else
                        return keyValuePairs;
                }
                else
                {
                    //await DisplayAlert("OTP", "OTP send to your Phone number " + phoneNumber, "Ok");
                    return keyValuePairs;
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("SignupPage/SendOTP: " + ex.Message);
                return keyValuePairs;
            }
        }
        #endregion

        #region [ Events ]
        private async void ImgBack_Tapped(object sender, EventArgs e)
        {
            Common.BindAnimation(image: ImgBack);
            await Navigation.PopAsync();
        }

        private async void StkLogin_Tapped(object sender, EventArgs e)
        {
            var Tab = (StackLayout)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    await Navigation.PopAsync();
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("SignupPage/StkLogin_Tapped: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }

        private void StkAgreeTC_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (imgCheck.Source.ToString().Replace("File: ", "") == Constraints.Img_CheckBoxChecked)
                {
                    isChecked = false;
                    imgCheck.Source = Constraints.Img_CheckBoxUnChecked;
                }
                else
                {
                    isChecked = true;
                    imgCheck.Source = Constraints.Img_CheckBoxChecked;
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("SignupPage/StkAgreeTC_Tapped: " + ex.Message);
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
                    await RegisterUser();
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("SignupPage/GetOtp_Tapped: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }

        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            try
            {
                var entry = (Extention.ExtEntry)sender;
                if (!Common.EmptyFiels(entry.Text))
                {
                    if (entry.ClassId == "FullName")
                    {
                        BoxFullName.BackgroundColor = (Color)App.Current.Resources["appColor8"];
                    }
                    else if (entry.ClassId == "Email")
                    {
                        BoxEmailAddress.BackgroundColor = (Color)App.Current.Resources["appColor8"];
                    }
                    else if (entry.ClassId == "PhoneNumber")
                    {
                        BoxPhoneNumber.BackgroundColor = (Color)App.Current.Resources["appColor8"];
                    }
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("SignupPage/Entry_Unfocused: " + ex.Message);
            }
        }
        #endregion
    }
}
using Acr.UserDialogs;
using AptDealzBuyer.API;
using AptDealzBuyer.Model.Reponse;
using AptDealzBuyer.Repository;
using AptDealzBuyer.Utility;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AptDealzBuyer.Services
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        public async Task<bool> RefreshToken()
        {
            bool result = false;

            AuthenticationAPI authenticationAPI = new AuthenticationAPI();
            try
            {
                UserDialogs.Instance.ShowLoading(Constraints.Loading);
                var mResponse = await authenticationAPI.RefreshToken(Settings.RefreshToken);
                if (mResponse != null && mResponse.Succeeded)
                {
                    var jObject = (Newtonsoft.Json.Linq.JObject)mResponse.Data;
                    if (jObject != null)
                    {
                        var mBuyer = jObject.ToObject<Model.Request.Buyer>();
                        if (mBuyer != null)
                        {
                            Settings.UserId = mBuyer.Id;
                            Settings.UserToken = mBuyer.JwToken;
                            Common.Token = mBuyer.JwToken;
                            Settings.RefreshToken = mBuyer.RefreshToken;
                            Settings.LoginTrackingKey = mBuyer.LoginTrackingKey == "00000000-0000-0000-0000-000000000000" ? Settings.LoginTrackingKey : mBuyer.LoginTrackingKey;
                            result = true;
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
            catch (System.Exception ex)
            {
                Common.DisplayErrorMessage("AuthenticationRepository/RefreshToken: " + ex.Message);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
            return result;
        }

        public async Task DoLogout()
        {
            try
            {
                var isClose = await App.Current.MainPage.DisplayAlert(Constraints.Logout, Constraints.AreYouSureWantLogout, Constraints.Yes, Constraints.No);
                if (isClose)
                {
                    AuthenticationAPI authenticationAPI = new AuthenticationAPI();
                    UserDialogs.Instance.ShowLoading(Constraints.Loading);
                    var mResponse = await authenticationAPI.Logout(Settings.RefreshToken, Settings.LoginTrackingKey);
                    if (mResponse != null && mResponse.Succeeded)
                    {
                        //Common.DisplaySuccessMessage(mResponse.Message);
                    }
                    else
                    {
                        if (mResponse != null && !mResponse.Message.Contains("TrackingKey"))
                            Common.DisplayErrorMessage(mResponse.Message);
                    }
                    MessagingCenter.Unsubscribe<string>(this, Constraints.Str_NotificationCount);
                    Common.ClearAllData();
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("AccountView/DoLogout: " + ex.Message);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        public async Task<Response> APIResponse(HttpResponseMessage httpResponseMessage)
        {
            Response mResponse = new Response();
            var responseJson = await httpResponseMessage.Content.ReadAsStringAsync();

            if (httpResponseMessage != null)
            {
                if (!Common.EmptyFiels(responseJson))
                {
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        mResponse = JsonConvert.DeserializeObject<Response>(responseJson);
                    }
                    else if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        var errorString = JsonConvert.DeserializeObject<string>(responseJson);
                        if (errorString == Constraints.Session_Expired)
                        {
                            mResponse.Message = Constraints.Session_Expired;
                            MessagingCenter.Unsubscribe<string>(this, Constraints.Str_NotificationCount);
                            Common.ClearAllData();
                        }
                    }
                    else if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                    {
                        mResponse.Message = Constraints.ServiceUnavailable;
                        MessagingCenter.Unsubscribe<string>(this, Constraints.Str_NotificationCount);
                        Common.ClearAllData();
                    }
                    else if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        if (responseJson.Contains(Constraints.Str_Duplicate))
                        {
                            mResponse = JsonConvert.DeserializeObject<Response>(responseJson);
                        }
                        else
                        {
                            try
                            {
                                mResponse = JsonConvert.DeserializeObject<Response>(responseJson);
                            }
                            catch (Exception)
                            {
                                mResponse.Message = Constraints.Something_Wrong_Server;
                            }
                            MessagingCenter.Unsubscribe<string>(this, Constraints.Str_NotificationCount);
                        }
                    }
                    else if (responseJson.Contains(Constraints.Str_AccountDeactivated) && httpResponseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        if (Common.mBuyerDetail != null && !Common.EmptyFiels(Common.mBuyerDetail.FullName))
                            mResponse.Message = "Hey " + Common.mBuyerDetail.FullName + ", your account is deactivated.Please contact customer support.";
                        else
                            mResponse.Message = "Hey, your account is deactivated.Please contact customer support.";

                        MessagingCenter.Unsubscribe<string>(this, Constraints.Str_NotificationCount);
                        Common.ClearAllData();
                    }
                    else
                    {
                        if (responseJson.Contains(Constraints.Str_TokenExpired) || httpResponseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            var isRefresh = await DependencyService.Get<IAuthenticationRepository>().RefreshToken();
                            if (!isRefresh)
                            {
                                mResponse.Message = Constraints.Session_Expired;
                                MessagingCenter.Unsubscribe<string>(this, Constraints.Str_NotificationCount);
                                Common.ClearAllData();
                            }
                        }
                        else
                        {
                            mResponse = JsonConvert.DeserializeObject<Response>(responseJson);
                        }
                    }
                }
                else
                {
                    mResponse.Succeeded = false;
                    mResponse.Message = Constraints.Something_Wrong;
                }
            }
            else
            {
                mResponse.Succeeded = false;
                mResponse.Message = Constraints.Something_Wrong;
            }
            return mResponse;
        }
    }
}

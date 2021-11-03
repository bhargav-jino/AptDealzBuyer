using AptDealzBuyer.Model.Reponse;
using AptDealzBuyer.Repository;
using AptDealzBuyer.Utility;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace AptDealzBuyer.API
{
    public class NotificationAPI
    {
        #region [ GET ]
        public async Task<Response> GetAllNotificationsForUser()
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        string url = string.Format(EndPointURL.GetAllNotificationsForUser, (int)App.Current.Resources["Version"]);
                        var response = await hcf.GetAsync(url);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(response);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await GetAllNotificationsForUser();
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("NotificationAPI/GetAllNotificationsForUser: " + ex.Message);
            }
            return mResponse;
        }

        public async Task<Response> GetNotificationsCountForUser()
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        string url = string.Format(EndPointURL.GetNotificationsCountForUser, (int)App.Current.Resources["Version"]);
                        var respons = await hcf.GetAsync(url);
                        var responseJson = await respons.Content.ReadAsStringAsync();
                        if (respons.IsSuccessStatusCode)
                        {
                            mResponse = JsonConvert.DeserializeObject<Response>(responseJson);
                        }
                        else if (respons.StatusCode == System.Net.HttpStatusCode.Forbidden)
                        {
                            var errorString = JsonConvert.DeserializeObject<string>(responseJson);
                            if (errorString == Constraints.Session_Expired)
                            {
                                mResponse.Message = Constraints.Session_Expired;
                                MessagingCenter.Unsubscribe<string>(this, Constraints.Str_NotificationCount);
                                Common.ClearAllData();
                            }
                        }
                        else if (respons.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                        {
                            mResponse.Message = Constraints.ServiceUnavailable;
                            MessagingCenter.Unsubscribe<string>(this, Constraints.Str_NotificationCount);
                            Common.ClearAllData();
                        }
                        else if (respons.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                        {
                            if (responseJson.Contains(Constraints.Str_Duplicate))
                            {
                                mResponse = JsonConvert.DeserializeObject<Response>(responseJson);
                            }
                            else
                            {
                                mResponse.Message = Constraints.Something_Wrong_Server;
                                MessagingCenter.Unsubscribe<string>(this, Constraints.Str_NotificationCount);
                                Common.ClearAllData();
                            }
                        }
                        else if (responseJson.Contains(Constraints.Str_AccountDeactivated) && respons.StatusCode == System.Net.HttpStatusCode.Unauthorized)
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
                            if (responseJson.Contains(Constraints.Str_TokenExpired) || respons.StatusCode == System.Net.HttpStatusCode.Unauthorized)
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
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await GetNotificationsCountForUser();
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("NotificationAPI/GetNotificationsCountForUser: " + ex.Message);
            }
            return mResponse;
        }
        #endregion

        #region [ POST ]
        public async Task<Response> SetUserNoficiationAsRead(string NotificationId)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    string requestJson = "{\"NotificationId\":\"" + NotificationId + "\"}";
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        string url = string.Format(EndPointURL.SetUserNoficiationAsRead, (int)App.Current.Resources["Version"], NotificationId);
                        var responseHttp = await hcf.PostAsync(url, requestJson);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(responseHttp);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await SetUserNoficiationAsRead(NotificationId);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("NotificationAPI/SetUserNoficiationAsRead: " + ex.Message);
            }
            return mResponse;
        }

        public async Task<Response> SetUserNoficiationAsReadAndDelete(string NotificationId)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    string requestJson = "";
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        string url = string.Format(EndPointURL.SetUserNoficiationAsReadAndDelete, (int)App.Current.Resources["Version"], NotificationId);
                        var response = await hcf.PostAsync(url, requestJson);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(response);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await SetUserNoficiationAsReadAndDelete(NotificationId);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("NotificationAPI/SetUserNoficiationAsReadAndDelete: " + ex.Message);
            }
            return mResponse;
        }
        #endregion
    }
}

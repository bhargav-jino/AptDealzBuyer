using AptDealzBuyer.Model.Reponse;
using AptDealzBuyer.Model.Request;
using AptDealzBuyer.Repository;
using AptDealzBuyer.Utility;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AptDealzBuyer.API
{
    public class ProfileAPI
    {
        #region [ GET ]
        public async Task<Response> GetMyProfileData()
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        string url = string.Format(EndPointURL.GetMyProfileData, (int)App.Current.Resources["Version"]);
                        var response = await hcf.GetAsync(url);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(response);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await GetMyProfileData();
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("ProfileAPI/GetMyProfileData: " + ex.Message);
            }
            return mResponse;
        }

        public async Task<List<Country>> GetCountry()
        {
            List<Country> mCountries = new List<Country>();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    using (var hcf = new HttpClientFactory())
                    {
                        string url = string.Format(EndPointURL.Country, (int)App.Current.Resources["Version"]);
                        var response = await hcf.GetAsync(url);
                        var responseJson = await response.Content.ReadAsStringAsync();
                        if (response.IsSuccessStatusCode)
                        {
                            mCountries = JsonConvert.DeserializeObject<List<Country>>(responseJson);
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                        {
                            var errorString = JsonConvert.DeserializeObject<string>(responseJson);
                            if (errorString == Constraints.Session_Expired)
                            {
                                mCountries = null;
                                MessagingCenter.Unsubscribe<string>(this, Constraints.Str_NotificationCount);
                                Common.ClearAllData();
                            }
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable
                                || response.StatusCode == System.Net.HttpStatusCode.InternalServerError
                                || responseJson.Contains(Constraints.Str_AccountDeactivated) && response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            mCountries = null;
                            MessagingCenter.Unsubscribe<string>(this, Constraints.Str_NotificationCount);
                            Common.ClearAllData();
                        }
                        else
                        {
                            mCountries = null;
                        }
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await GetCountry();
                    }
                }
            }
            catch (Exception ex)
            {
                mCountries = null;
                Common.DisplayErrorMessage("ProfileAPI/GetCountry: " + ex.Message);
            }
            return mCountries;
        }

        public async Task<Response> GetPincodeInfo(string PinCode)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    using (var hcf = new HttpClientFactory())
                    {
                        string url = string.Format(EndPointURL.GetPincodeInfo, PinCode);
                        var response = await hcf.GetAsync(url);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(response);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await GetPincodeInfo(PinCode);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("ProfileAPI/GetPincodeInfo: " + ex.Message);
            }
            return mResponse;
        }
        #endregion

        #region [ POST ]
        public async Task<Response> DeactiviateUser(string userId)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    string requestJson = "{\"userId\":\"" + userId + "\"}";
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        string url = EndPointURL.DeactivateUser; //sent UserId
                        var response = await hcf.PostAsync(url, requestJson);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(response);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await DeactiviateUser(userId);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("ProfileAPI/DeactiviateUser: " + ex.Message);
            }
            return mResponse;
        }

        public async Task<Response> FileUpload(FileUpload mFileUpload)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    var requestJson = JsonConvert.SerializeObject(mFileUpload);
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        string url = EndPointURL.FileUpload;
                        var response = await hcf.PostAsync(url, requestJson);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(response);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await FileUpload(mFileUpload);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("ProfileAPI/FileUpload: " + ex.Message);
            }
            return mResponse;
        }
        #endregion

        #region [ PUT ]
        public async Task<Response> SaveProfile(BuyerDetails mBuyerDetails)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    var requestJson = JsonConvert.SerializeObject(mBuyerDetails);
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        string url = string.Format(EndPointURL.SaveProfile, (int)App.Current.Resources["Version"]);
                        var response = await hcf.PutAsync(url, requestJson);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(response);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await SaveProfile(mBuyerDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("ProfileAPI/SaveProfile: " + ex.Message);
            }
            return mResponse;
        }
        #endregion
    }
}

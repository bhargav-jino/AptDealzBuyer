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
    public class RegisterAPI
    {
        #region [ POST ]
        public async Task<Response> IsUniquePhoneNumber(Model.Request.UniquePhoneNumber mUniquePhoneNumber)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    var requestJson = JsonConvert.SerializeObject(mUniquePhoneNumber);
                    using (var hcf = new HttpClientFactory())
                    {
                        string url = string.Format(EndPointURL.IsUniquePhoneNumber, (int)App.Current.Resources["Version"]);
                        var response = await hcf.PostAsync(url, requestJson);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(response);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await IsUniquePhoneNumber(mUniquePhoneNumber);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("RegisterAPI/IsUniquePhoneNumber: " + ex.Message);
            }
            return mResponse;
        }

        public async Task<Response> IsUniqueEmail(Model.Request.UniqueEmail mUniqueEmail)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    var requestJson = JsonConvert.SerializeObject(mUniqueEmail);
                    using (var hcf = new HttpClientFactory())
                    {
                        string url = string.Format(EndPointURL.IsUniqueEmail, (int)App.Current.Resources["Version"]);
                        var response = await hcf.PostAsync(url, requestJson);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(response);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await IsUniqueEmail(mUniqueEmail);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("RegisterAPI/IsUniqueEmail: " + ex.Message);
            }
            return mResponse;
        }

        public async Task<Response> Register(Model.Request.Register mRequestRegister)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    var requestJson = JsonConvert.SerializeObject(mRequestRegister);
                    using (var hcf = new HttpClientFactory())
                    {
                        string url = string.Format(EndPointURL.Register, (int)App.Current.Resources["Version"]);
                        var response = await hcf.PostAsync(url, requestJson);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(response);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await Register(mRequestRegister);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("RegisterAPI/Register: " + ex.Message);
            }
            return mResponse;
        }
        #endregion
    }
}

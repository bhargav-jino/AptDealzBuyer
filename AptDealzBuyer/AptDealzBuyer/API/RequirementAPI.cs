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
    public class RequirementAPI
    {
        #region [ GET ]
        public async Task<Response> GetRequirementById(string RequirmentId)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        string url = string.Format(EndPointURL.GetRequirementById, (int)App.Current.Resources["Version"], RequirmentId);
                        var response = await hcf.GetAsync(url);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(response);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await GetRequirementById(RequirmentId);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("RequirementAPI/GetRequirementById: " + ex.Message);
            }
            return mResponse;
        }

        public async Task<Response> GetAllMyActiveRequirements(string SortBy = "", string Title = "", bool? IsAscending = null, int PageNumber = 1, int PageSize = 10)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        string url = string.Format(EndPointURL.GetAllMyActiveRequirements + "?PageNumber={1}&PageSize={2}", (int)App.Current.Resources["Version"], PageNumber, PageSize);

                        if (!Common.EmptyFiels(Title))
                            url += "&Title=" + Title;
                        if (!Common.EmptyFiels(SortBy))
                            url += "&SortBy=" + SortBy;
                        if (IsAscending.HasValue)
                            url += "&IsAscending=" + IsAscending.Value;

                        var response = await hcf.GetAsync(url);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(response);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await GetAllMyActiveRequirements(SortBy, Title, IsAscending, PageNumber, PageSize);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("RequirementAPI/GetActiveRequirements: " + ex.Message);
            }
            return mResponse;
        }

        public async Task<Response> GetMyPreviousRequirements(string SortBy = "", string Title = "", bool? IsAscending = null, int PageNumber = 1, int PageSize = 10)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        string url = string.Format(EndPointURL.GetMyPreviousRequirements + "?PageNumber={1}&PageSize={2}", (int)App.Current.Resources["Version"], PageNumber, PageSize);

                        if (!Common.EmptyFiels(Title))
                            url += "&Title=" + Title;
                        if (!Common.EmptyFiels(SortBy))
                            url += "&SortBy=" + SortBy;
                        if (IsAscending.HasValue)
                            url += "&IsAscending=" + IsAscending.Value;

                        var response = await hcf.GetAsync(url);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(response);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await GetMyPreviousRequirements(SortBy, Title, IsAscending, PageNumber, PageSize);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("RequirementAPI/GetPreviousRequirements: " + ex.Message);
            }
            return mResponse;
        }
        #endregion

        #region [ POST ]
        public async Task<Response> CreateRequirement(Model.Request.Requirement mRequirement)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    var requestJson = JsonConvert.SerializeObject(mRequirement);
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        string url = string.Format(EndPointURL.CreateRequirement, (int)App.Current.Resources["Version"]);
                        var response = await hcf.PostAsync(url, requestJson);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(response);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await CreateRequirement(mRequirement);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("RequirementAPI/CreateRequirement: " + ex.Message);
            }
            return mResponse;
        }

        public async Task<Response> CancelRequirement(string requirementId)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    string requestJson = "{\"requirementId\":\"" + requirementId + "\"}";
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        string url = string.Format(EndPointURL.CancelRequirement, (int)App.Current.Resources["Version"], requirementId);
                        var response = await hcf.PostAsync(url, requestJson);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(response);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await CancelRequirement(requirementId);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("RequirementAPI/CancelRequirement: " + ex.Message);
            }
            return mResponse;
        }

        public async Task<Response> GetAmountToBePaidToRevealBuyerContact(string requirementId)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    string requestJson = "{\"requirementId\":\"" + requirementId + "\"}";
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        //string url = string.Format(EndPointURL.GetAmountToBePaidToRevealBuyerContact, (int)App.Current.Resources["Version"]);

                        string url = "https://aptdealzapidev.azurewebsites.net/api/v1/Requirement/GetAmountToBePaidToRevealBuyerContact";
                        var response = await hcf.PostAsync(url, requestJson);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(response);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await GetAmountToBePaidToRevealBuyerContact(requirementId);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("RequirementAPI/CancelRequirement: " + ex.Message);
            }
            return mResponse;
        }


        #endregion

        #region [ DELETE ]

        public async Task<Response> DeleteRequirement(string id)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        string url = string.Format(EndPointURL.DeleteRequirement, (int)App.Current.Resources["Version"], id);
                        var response = await hcf.DeleteAsync(url);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(response);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await DeleteRequirement(id);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("RequirementAPI/DeleteRequirement: " + ex.Message);
            }
            return mResponse;
        }
        #endregion

        #region [ PUT ]
        public async Task<Response> UpdateStatusRequirement(string requirementId, int status)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    string requestJson = "{\"requirementId\":\"" + requirementId + "\",\"status\":\"" + status + "\"}";
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        string url = string.Format(EndPointURL.UpdateStatusRequirement, (int)App.Current.Resources["Version"]);
                        var response = await hcf.PutAsync(url, requestJson);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(response);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await UpdateStatusRequirement(requirementId, status);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("RequirementAPI/UpdateStatusRequirement: " + ex.Message);
            }
            return mResponse;
        }
        #endregion
    }
}

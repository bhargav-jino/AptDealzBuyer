using AptDealzBuyer.Model.Reponse;
using AptDealzBuyer.Model.Request;
using AptDealzBuyer.Repository;
using AptDealzBuyer.Utility;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AptDealzBuyer.API
{
    public class GrievanceAPI
    {
        #region [ GET ]
        public async Task<Response> GetAllGrievances(int? Status = null, string Title = "", string SortBy = "", bool? IsAscending = null, int PageNumber = 1, int PageSize = 10)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        string url = string.Format(EndPointURL.GetAllGrievancesByMe + "?PageNumber={1}&PageSize={2}", (int)App.Current.Resources["Version"], PageNumber, PageSize);

                        if (Status.HasValue)
                            url += "&Status=" + Status;
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
                        await GetAllGrievances();
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("GrievanceAPI/GetAllGrievances: " + ex.Message);
            }
            return mResponse;
        }

        public async Task<Response> GetGrievancesDetailsForBuyer(string GrievanceId)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        string url = string.Format(EndPointURL.GetGrievancesDetailsForBuyer, (int)App.Current.Resources["Version"], GrievanceId);
                        var response = await hcf.GetAsync(url);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(response);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await GetGrievancesDetailsForBuyer(GrievanceId);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("GrievanceAPI/GetGrievancesDetailsForBuyer: " + ex.Message);
            }
            return mResponse;
        }
        #endregion

        #region [ POST ]
        public async Task<Response> CreateGrievanceFromBuyer(RaiseGrievance mRaiseGrievance)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    var requestJson = JsonConvert.SerializeObject(mRaiseGrievance);
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        string url = string.Format(EndPointURL.CreateGrievanceFromBuyer, (int)App.Current.Resources["Version"]);
                        var response = await hcf.PostAsync(url, requestJson);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(response);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await CreateGrievanceFromBuyer(mRaiseGrievance);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("GrievanceAPI/CreateGrievanceFromBuyer: " + ex.Message);
            }
            return mResponse;
        }

        public async Task<Response> SubmitGrievanceResponseFromBuyer(string grievanceId, string response)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    string requestJson = "{\"grievanceId\":\"" + grievanceId + "\",\"response\":\"" + response + "\"}";
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        string url = string.Format(EndPointURL.SubmitGrievanceResponseFromBuyer, (int)App.Current.Resources["Version"]);
                        var responseHttp = await hcf.PostAsync(url, requestJson);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(responseHttp);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await SubmitGrievanceResponseFromBuyer(grievanceId, response);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("GrievanceAPI/SubmitGrievanceResponseFromBuyer: " + ex.Message);
            }
            return mResponse;
        }
        #endregion
    }
}

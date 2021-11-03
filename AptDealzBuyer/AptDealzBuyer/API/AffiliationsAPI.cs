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
    public class AffiliationsAPI
    {
        #region [ GET ]
        public async Task<Response> GetAllAffiliations()
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    using (var hcf = new HttpClientFactory())
                    {
                        string url = string.Format(EndPointURL.GetAllAffiliations, (int)App.Current.Resources["Version"]);
                        var response = await hcf.GetAsync(url);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(response);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await GetAllAffiliations();
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("AffiliationsAPI/GetAllAffiliations: " + ex.Message);
            }
            return mResponse;
        }
        #endregion
    }
}

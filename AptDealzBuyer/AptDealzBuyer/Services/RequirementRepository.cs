using Acr.UserDialogs;
using AptDealzBuyer.API;
using AptDealzBuyer.Model.Request;
using AptDealzBuyer.Repository;
using AptDealzBuyer.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace AptDealzBuyer.Services
{
    public class RequirementRepository : IRequirementRepository
    {
        RequirementAPI requirementAPI = new RequirementAPI();

        public async Task<Requirement> GetRequirementById(string RequirmentId)
        {
            Requirement mRequirement = new Requirement();
            try
            {
                UserDialogs.Instance.ShowLoading(Constraints.Loading);
                var mResponse = await requirementAPI.GetRequirementById(RequirmentId);
                if (mResponse != null && mResponse.Succeeded)
                {
                    var jObject = (JObject)mResponse.Data;
                    if (jObject != null)
                    {
                        mRequirement = jObject.ToObject<Requirement>();
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
                Common.DisplayErrorMessage("RequirementRepository/GetRequirementById: " + ex.Message);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
            return mRequirement;
        }

        public async Task<bool> DeleteRequirement(string requirmentId)
        {
            bool hasDelete = false;
            try
            {
                var isDelete = await App.Current.MainPage.DisplayAlert(Constraints.Alert, Constraints.AreYouSureWantDeleteReq, Constraints.Yes, Constraints.No);
                if (isDelete)
                {
                    RequirementAPI requirementAPI = new RequirementAPI();
                    UserDialogs.Instance.ShowLoading(Constraints.Loading);

                    var mResponse = await requirementAPI.DeleteRequirement(requirmentId);
                    if (mResponse != null && mResponse.Succeeded)
                    {
                        hasDelete = true;
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
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("RequirementRepository/DeleteRequirement: " + ex.Message);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
            return hasDelete;
        }
    }
}

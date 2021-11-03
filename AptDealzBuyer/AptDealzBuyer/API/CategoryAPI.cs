using AptDealzBuyer.Model.Reponse;
using AptDealzBuyer.Utility;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AptDealzBuyer.API
{
    public class CategoryAPI
    {
        #region [ GET ]
        public async Task<List<Category>> GetCategory()
        {
            List<Category> mCategory = new List<Category>();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    using (var hcf = new HttpClientFactory())
                    {
                        string url = string.Format(EndPointURL.Category, (int)App.Current.Resources["Version"]);
                        var response = await hcf.GetAsync(url);
                        var responseJson = await response.Content.ReadAsStringAsync();
                        if (response.IsSuccessStatusCode)
                        {
                            mCategory = JsonConvert.DeserializeObject<List<Category>>(responseJson);
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                        {
                            var errorString = JsonConvert.DeserializeObject<string>(responseJson);
                            if (errorString == Constraints.Session_Expired)
                            {
                                mCategory = null;
                                MessagingCenter.Unsubscribe<string>(this, Constraints.Str_NotificationCount);
                                Common.ClearAllData();
                            }
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable
                                || response.StatusCode == System.Net.HttpStatusCode.InternalServerError
                                || responseJson.Contains(Constraints.Str_AccountDeactivated) && response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            mCategory = null;
                            MessagingCenter.Unsubscribe<string>(this, Constraints.Str_NotificationCount);
                            Common.ClearAllData();
                        }
                        else
                        {
                            mCategory = null;
                        }
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await GetCategory();
                    }
                }
            }
            catch (Exception ex)
            {
                mCategory = null;
                Common.DisplayErrorMessage("CategoryAPI/GetCategory: " + ex.Message);
            }
            return mCategory;
        }

        public async Task<List<SubCategory>> GetSubCategory(string categoryId)
        {
            List<SubCategory> mSubCategory = new List<SubCategory>();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    using (var hcf = new HttpClientFactory())
                    {
                        string url = string.Format(EndPointURL.SubCategory, (int)App.Current.Resources["Version"], categoryId);
                        var response = await hcf.GetAsync(url);
                        var responseJson = await response.Content.ReadAsStringAsync();
                        if (response.IsSuccessStatusCode)
                        {
                            mSubCategory = JsonConvert.DeserializeObject<List<SubCategory>>(responseJson);
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                        {
                            var errorString = JsonConvert.DeserializeObject<string>(responseJson);
                            if (errorString == Constraints.Session_Expired)
                            {
                                mSubCategory = null;
                                MessagingCenter.Unsubscribe<string>(this, Constraints.Str_NotificationCount);
                                Common.ClearAllData();
                            }
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable
                                || response.StatusCode == System.Net.HttpStatusCode.InternalServerError
                                || responseJson.Contains(Constraints.Str_AccountDeactivated) && response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            mSubCategory = null;
                            MessagingCenter.Unsubscribe<string>(this, Constraints.Str_NotificationCount);
                            Common.ClearAllData();
                        }
                        else
                        {
                            mSubCategory = null;
                        }
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await GetSubCategory(categoryId);
                    }
                }
            }
            catch (Exception ex)
            {
                mSubCategory = null;
                Common.DisplayErrorMessage("CategoryAPI/GetSubCategory: " + ex.Message);
            }
            return mSubCategory;
        }
        #endregion
    }
}

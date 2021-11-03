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
    class OrderAPI
    {
        #region [ GET ]
        public async Task<Response> GetOrdersForBuyer(int? Status = null, string Title = "", string SortBy = "", bool? IsAscending = null, int PageNumber = 1, int PageSize = 10)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        string url = string.Format(EndPointURL.GetOrdersForBuyer + "?PageNumber={1}&PageSize={2}", (int)App.Current.Resources["Version"], PageNumber, PageSize);

                        if (Status > 0)
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
                        await GetOrdersForBuyer(Status, Title, SortBy, IsAscending, PageNumber, PageSize);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("OrderAPI/GetOrdersForBuyer: " + ex.Message);
            }
            return mResponse;
        }

        public async Task<Response> GetOrderDetailsForBuyer(string orderId)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        string url = string.Format(EndPointURL.GetOrderDetailsForBuyer, (int)App.Current.Resources["Version"], orderId);
                        var response = await hcf.GetAsync(url);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(response);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await GetOrderDetailsForBuyer(orderId);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("OrderAPI/GetOrderDetailsForBuyer: " + ex.Message);
            }
            return mResponse;
        }

        public async Task<Response> GetShippedOrdersForBuyer(string Title = "", string SortBy = "", bool? IsAscending = null, int PageNumber = 1, int PageSize = 10)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        string url = string.Format(EndPointURL.GetShippedOrdersForBuyer + "?PageNumber={1}&PageSize={2}", (int)App.Current.Resources["Version"], PageNumber, PageSize);

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
                        await GetShippedOrdersForBuyer(Title, SortBy, IsAscending, PageNumber, PageSize);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("OrderAPI/GetShippedOrdersForBuyer: " + ex.Message);
            }
            return mResponse;
        }

        public async Task<Response> GenerateQRCodeImageForBuyerApp(string orderId)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        string url = "https://aptdealzapidev.azurewebsites.net/api/v1/Order/GenerateQRCodeImageForBuyerApp/" + orderId;
                        //string url = string.Format(EndPointURL.GenerateQRCodeImageForBuyerApp, (int)App.Current.Resources["Version"], orderId);
                        var response = await hcf.client.GetAsync(url);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(response);
                        if (response.IsSuccessStatusCode)
                        {
                            if (mResponse != null && mResponse.Succeeded)
                            {
                                var contentStream = await response.Content.ReadAsStreamAsync();
                                mResponse.Succeeded = true;
                                byte[] buffer = ImageConvertion.streamToByteArray(contentStream);
                                contentStream.Read(buffer, 0, (int)contentStream.Length);
                                mResponse.Data = Convert.ToBase64String(buffer);
                            }
                        }
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await GenerateQRCodeImageForBuyerApp(orderId);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("OrderAPI/GenerateQRCodeImageForBuyerApp: " + ex.Message);
            }
            return mResponse;
        }
        #endregion

        #region [ POST ]
        public async Task<Response> CreateOrder(CreateOrder mCreateOrder)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    var requestJson = JsonConvert.SerializeObject(mCreateOrder);
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        string url = string.Format(EndPointURL.CreateOrder, (int)App.Current.Resources["Version"]);
                        var response = await hcf.PostAsync(url, requestJson);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(response);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await CreateOrder(mCreateOrder);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("OrderAPI/CreateOrder: " + ex.Message);
            }
            return mResponse;
        }

        public async Task<Response> CreateOrderPayment(OrderPayment mOrderPayment)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    var requestJson = JsonConvert.SerializeObject(mOrderPayment);
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        string url = string.Format(EndPointURL.OrderPayment, (int)App.Current.Resources["Version"]);
                        var response = await hcf.PostAsync(url, requestJson);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(response);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await CreateOrderPayment(mOrderPayment);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("OrderAPI/CreateOrderPayment: " + ex.Message);
            }
            return mResponse;
        }


        #endregion

        #region [ PUT ]
        public async Task<Response> CancelOrder(string orderId)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    string requestJson = "{\"orderId\":\"" + orderId + "\"}";
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        string url = string.Format(EndPointURL.CancelOrder, (int)App.Current.Resources["Version"], orderId);
                        var response = await hcf.PutAsync(url, requestJson);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(response);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await CancelOrder(orderId);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("OrderAPI/CancelOrder: " + ex.Message);
            }
            return mResponse;
        }

        public async Task<Response> ConfirmDeliveryFromBuyer(string orderId)
        {
            Response mResponse = new Response();
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    string requestJson = "{\"orderId\":\"" + orderId + "\"}";
                    using (var hcf = new HttpClientFactory(token: Common.Token))
                    {
                        string url = string.Format(EndPointURL.ConfirmDeliveryFromBuyer, (int)App.Current.Resources["Version"], orderId);
                        var response = await hcf.PutAsync(url, requestJson);
                        mResponse = await DependencyService.Get<IAuthenticationRepository>().APIResponse(response);
                    }
                }
                else
                {
                    if (await Common.InternetConnection())
                    {
                        await ConfirmDeliveryFromBuyer(orderId);
                    }
                }
            }
            catch (Exception ex)
            {
                mResponse.Succeeded = false;
                mResponse.Message = ex.Message;
                Common.DisplayErrorMessage("OrderAPI/ConfirmDeliveryFromBuyer: " + ex.Message);
            }
            return mResponse;
        }
        #endregion
    }
}

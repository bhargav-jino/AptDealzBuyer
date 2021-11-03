using AptDealzBuyer.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AptDealzBuyer.Utility
{
    public class RazorPayUtility
    {
        public async Task<List<string>> PayViaRazor(RazorPayload payload, RequestPayLoad mRequestPayLoad, string username, string password)
        {
            List<string> datas = new List<string>();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.razorpay.com/v1/orders"))
                    {
                        var plainTextBytes = Encoding.UTF8.GetBytes($"{username}:{password}");
                        var basicAuthKey = Convert.ToBase64String(plainTextBytes);

                        request.Headers.TryAddWithoutValidation("Authorization", $"Basic {basicAuthKey}");

                        string jsonData = JsonConvert.SerializeObject(payload);
                        request.Content = new StringContent(jsonData);
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                        var response = await httpClient.SendAsync(request);
                        string jsonResp = await response.Content.ReadAsStringAsync();
                        RazorResponse mRazorResponse = JsonConvert.DeserializeObject<RazorResponse>(jsonResp);

                        if (!string.IsNullOrEmpty(mRazorResponse.id))
                        {
                            mRequestPayLoad.reference_id = mRazorResponse.id;
                            var url = await CreatePaymentLink(mRequestPayLoad, username, password);

                            datas.Add(url);
                            datas.Add(mRazorResponse.id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return datas;
        }

        private async Task<string> CreatePaymentLink(RequestPayLoad payload, string username, string password)
        {
            string url = "";
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.razorpay.com/v1/payment_links"))
                    {
                        var plainTextBytes = Encoding.UTF8.GetBytes($"{username}:{password}");
                        var basicAuthKey = Convert.ToBase64String(plainTextBytes);

                        request.Headers.TryAddWithoutValidation("Authorization", $"Basic {basicAuthKey}");

                        string jsonData = JsonConvert.SerializeObject(payload);
                        request.Content = new StringContent(jsonData);
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                        var response = await httpClient.SendAsync(request);
                        string jsonResp = await response.Content.ReadAsStringAsync();
                        ResponsePayLoad mResponsePayLoad = JsonConvert.DeserializeObject<ResponsePayLoad>(jsonResp);

                        if (mResponsePayLoad != null && !string.IsNullOrEmpty(mResponsePayLoad.id))
                        {
                            url = mResponsePayLoad.short_url;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return url;
        }
    }

    public class Customer
    {
        public string name { get; set; }
        public string contact { get; set; }
        public string email { get; set; }
    }

    public class Notify
    {
        public bool sms { get; set; }
        public bool email { get; set; }
    }

    public class Notes
    {
        public string policy_name { get; set; }
    }

    public class RequestPayLoad
    {
        public long amount { get; set; }
        public string currency { get; set; }
        public bool accept_partial { get; set; }
        public long first_min_partial_amount { get; set; }
        public int expire_by { get; set; }
        public string reference_id { get; set; }
        public string description { get; set; }
        public Customer customer { get; set; }
        public Notify notify { get; set; }
        public bool reminder_enable { get; set; }
        public Notes notes { get; set; }
        public string callback_url { get; set; }
        public string callback_method { get; set; }
    }

    public class ResponsePayLoad
    {
        public bool accept_partial { get; set; }
        public int amount { get; set; }
        public int amount_paid { get; set; }
        public string callback_method { get; set; }
        public string callback_url { get; set; }
        public int cancelled_at { get; set; }
        public int created_at { get; set; }
        public string currency { get; set; }
        public Customer customer { get; set; }
        public string description { get; set; }
        public int expire_by { get; set; }
        public int expired_at { get; set; }
        public int first_min_partial_amount { get; set; }
        public string id { get; set; }
        public Notes notes { get; set; }
        public Notify notify { get; set; }
        public object payments { get; set; }
        public string reference_id { get; set; }
        public bool reminder_enable { get; set; }
        public List<object> reminders { get; set; }
        public string short_url { get; set; }
        public string source { get; set; }
        public string source_id { get; set; }
        public string status { get; set; }
        public int updated_at { get; set; }
        public string user_id { get; set; }
    }
}

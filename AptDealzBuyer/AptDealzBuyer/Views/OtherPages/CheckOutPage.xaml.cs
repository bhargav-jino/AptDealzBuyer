using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AptDealzBuyer.Views.OtherPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckOutPage : ContentPage
    {
        public event EventHandler PaidEvent;
        private bool isSuccess = false;

        public CheckOutPage(string url)
        {
            InitializeComponent();

            wbView.Source = new Uri(url);
            wbView.Navigating += WbView_Navigating;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (!isSuccess)
            {
                if (PaidEvent != null)
                    PaidEvent(null, EventArgs.Empty);
            }
        }

        private void WbView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            try
            {
                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                var data = e.Url;
                if (data.StartsWith("https://purple-field-04c774300.azurestaticapps.net/login"))
                {
                    isSuccess = true;
                    var dataResponse = data.Replace("https://purple-field-04c774300.azurestaticapps.net/login?", "");
                    if (!string.IsNullOrEmpty(dataResponse))
                    {
                        var datas = dataResponse.Split('&');
                        if (datas != null && datas.Count() > 0)
                        {
                            foreach (var item in datas)
                            {
                                var items = item.Split('=');
                                keyValuePairs.Add(items[0], items[1]);
                            }
                        }
                    }
                }

                if (isSuccess)
                {
                    Navigation.PopAsync();
                    if (PaidEvent != null)
                        PaidEvent(keyValuePairs, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }
        }

    }
}
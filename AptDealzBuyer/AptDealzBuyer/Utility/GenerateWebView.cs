using AptDealzBuyer.Interfaces;
using AptDealzBuyer.Views.PopupPages;
using Rg.Plugins.Popup.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AptDealzBuyer.Utility
{
    public class GenerateWebView
    {
        private static string base64File = string.Empty;
        private static string extension = string.Empty;
        private static string filename = string.Empty;
        public static async Task GenerateView(string url)
        {
            try
            {
                if (!Common.EmptyFiels(url))
                {
                    url = url.Replace((string)App.Current.Resources["BaseURL"], "");
                }

                var fullUrl = (string)App.Current.Resources["BaseURL"] + url;
                base64File = ImageConvertion.ConvertImageURLToBase64(fullUrl);
                extension = Path.GetExtension(url).ToLower();
                filename = Path.GetFileName(url).ToLower();

                if (!Common.EmptyFiels(base64File))
                {
                    // Doc pdf Excel PPT
                    if (extension.ToLower() == ".doc" || extension.ToLower() == ".docx" ||
                        extension.ToLower() == ".pdf" || extension.ToLower() == ".xls" ||
                        extension.ToLower() == ".xlsx" || extension.ToLower() == ".ppt" ||
                        extension.ToLower() == ".pptx")
                    {
                        OpenView(base64File, filename);
                    }
                    else
                    {
                        var successPopup = new DisplayDocumentPopup(base64File, extension);
                        await PopupNavigation.Instance.PushAsync(successPopup);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("GenerateWebView/GenerateView: " + ex.Message);
            }
        }

        public static async void OpenView(string base64, string fileNane)
        {
            try
            {
                var path = DependencyService.Get<IHtmlToPDF>().SaveFiles(fileNane, Convert.FromBase64String(base64));
                await Launcher.OpenAsync(new OpenFileRequest()
                {
                    File = new ReadOnlyFile(path)
                });
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("GenerateWebView/OpenView: " + ex.Message);
            }
        }
    }
}

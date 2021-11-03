using AptDealzBuyer.Utility;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AptDealzBuyer.Views.PopupPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayDocumentPopup : PopupPage
    {
        #region [ Objects ]      
        public event EventHandler isRefresh;

        private string Base64File = string.Empty;
        private string FileExtension = string.Empty;
        #endregion

        #region [ Constructor ]
        public DisplayDocumentPopup(string base64File, string fileExtension)
        {
            InitializeComponent();

            Base64File = base64File;
            FileExtension = fileExtension;
        }
        #endregion

        #region [ Method ]
        protected override bool OnBackgroundClicked()
        {
            base.OnBackgroundClicked();
            return false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BindAttachmentOnUI();
        }

        public async void BindAttachmentOnUI()
        {
            try
            {
                if (FileExtension.ToLower() == ".jpg" ||
                    FileExtension.ToLower() == ".jpeg" ||
                    FileExtension.ToLower() == ".png")
                {
                    BindImageBase64(Base64File);
                }
                else if (FileExtension.ToLower() == ".mp4" ||
                         FileExtension.ToLower() == ".3gp" ||
                         FileExtension.ToLower() == ".mkv" ||
                         FileExtension.ToLower() == ".avi" ||
                         FileExtension.ToLower() == ".flv" ||
                         FileExtension.ToLower() == ".webm")
                {
                    BindVideoBase64(Base64File);
                }
                else if (FileExtension.ToLower() == ".mp3" ||
                         FileExtension.ToLower() == ".wav" ||
                         FileExtension.ToLower() == ".aac" ||
                         FileExtension.ToLower() == ".acc" ||
                         FileExtension.ToLower() == ".ogg")
                {
                    BindAudioBase64(Base64File);
                }
                else if (FileExtension.ToLower() == ".txt")
                {
                    BindTextBase64(Base64File);
                }
                else
                {
                    await DisplayAlert("Alert!", "Something wrong with the file.", "OK");
                }
            }
            catch (Exception ex)
            {
                //await Common.ExMessage("OpenCamera", "AttachmentsPage", ex);
            }
        }

        private void BindImageBase64(string base64)
        {
            try
            {
                string html = "<html><head><meta name='viewport' content='width=device-width, initial-scale=1.0'></head><body><img style='width: 100%;'  src='data:image/*;base64," + base64 + "' ></img></body></html>";
                WbView.Source = new HtmlWebViewSource()
                {
                    Html = html
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void BindVideoBase64(string base64)
        {
            try
            {
                string html = "<html><head><meta name='viewport' content='width=device-width, initial-scale=1.0'></head><body><video autoplay controls src='data:video/mp4;base64," + base64 + "' style='max-width: 100%;'></video></body></html>";
                WbView.Source = new HtmlWebViewSource()
                {
                    Html = html
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void BindAudioBase64(string base64)
        {
            try
            {
                string html = "<html><head><meta name='viewport' content='width=device-width, initial-scale=1.0'></head><body><audio controls style='width: 100%;'  src='data:audio/mp3;base64," + base64 + "' ></audio></body></html>";
                WbView.Source = new HtmlWebViewSource()
                {
                    Html = html
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void BindTextBase64(string base64)
        {
            try
            {
                string html = "<html><head><meta name='viewport' content='width=device-width, initial-scale=1.0'></head><body><embed style='width: 100%;height: 100%;'  src='data:text/plain;base64," + base64 + "' ></embed></body></html>";
                WbView.Source = new HtmlWebViewSource()
                {
                    Html = html
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region [ Event ]
        private void ImgClose_Clicked(object sender, EventArgs e)
        {
            try
            {
                isRefresh?.Invoke(true, EventArgs.Empty);
                PopupNavigation.Instance.PopAsync();
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("ShowImagePopup/ImgClose_Clicked: " + ex.Message);
            }
        }
        #endregion
    }
}
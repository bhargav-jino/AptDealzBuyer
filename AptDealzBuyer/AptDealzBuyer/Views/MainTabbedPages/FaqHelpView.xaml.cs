using Acr.UserDialogs;
using AptDealzBuyer.API;
using AptDealzBuyer.Model.Reponse;
using AptDealzBuyer.Utility;
using AptDealzBuyer.Views.DashboardPages;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AptDealzBuyer.Views.MainTabbedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FaqHelpView : ContentView
    {
        #region [ Objects ]       
        List<FAQResponse> mFAQResponses = new List<FAQResponse>();
        #endregion

        #region [ Constructor ]
        public FaqHelpView()
        {
            try
            {
                InitializeComponent();
                BindFAQ();

                MessagingCenter.Unsubscribe<string>(this, Constraints.Str_NotificationCount); MessagingCenter.Subscribe<string>(this, Constraints.Str_NotificationCount, (count) =>
                {
                    if (!Common.EmptyFiels(Common.NotificationCount))
                    {
                        lblNotificationCount.Text = count;
                        frmNotification.IsVisible = true;
                    }
                    else
                    {
                        frmNotification.IsVisible = false;
                        lblNotificationCount.Text = string.Empty;
                    }
                });
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("FaqHelpView/Ctor: " + ex.Message);
            }
        }
        #endregion

        #region [ Methods ]
        private async void BindFAQ()
        {
            try
            {
                lstFaq.ItemsSource = null;
                AppSettingsAPI appSettingsAPI = new AppSettingsAPI();
                UserDialogs.Instance.ShowLoading("Loading...");
                var mResponse = await appSettingsAPI.GetFAQ();
                UserDialogs.Instance.HideLoading();

                if (mResponse != null && mResponse.Succeeded)
                {
                    JArray result = (JArray)mResponse.Data;
                    if (result != null)
                    {
                        //txtMessage.Text = string.Empty;
                        mFAQResponses = result.ToObject<List<FAQResponse>>();
                    }
                }

                if (mFAQResponses != null && mFAQResponses.Count > 0)
                {
                    lstFaq.ItemsSource = mFAQResponses.ToList();
                }
                else
                {
                }


                //    FaqMs = new List<FaqM>()
                //{
                //    new FaqM{ FaqTitle="How do I post requirement?", FaqDesc="Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text."},
                //    new FaqM{ FaqTitle="How do I view the quotes receive", FaqDesc="Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text."},
                //    new FaqM{ FaqTitle="Do I have to pay to submit requirement", FaqDesc="Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text."},
                //    new FaqM{ FaqTitle="How long will my requirement be", FaqDesc="Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text."}
                //};
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("FaqHelpView/BindFaq: " + ex.Message);
            }
        }
        #endregion

        #region [ Events ]
        private void ImgMenu_Tapped(object sender, EventArgs e)
        {
            Common.BindAnimation(image: ImgMenu);
            //Common.OpenMenu();
        }

        private async void ImgNotification_Tapped(object sender, EventArgs e)
        {
            var Tab = (Grid)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    await Navigation.PushAsync(new NotificationPage());
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("FaqHelpView/ImgNotification_Tapped: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }

        private void ImgQuestion_Tapped(object sender, EventArgs e)
        {
            Common.MasterData.Detail = new NavigationPage(new MainTabbedPages.MainTabbedPage(Constraints.Str_FAQHelp));
        }

        private void ImgBack_Tapped(object sender, EventArgs e)
        {
            Common.BindAnimation(imageButton: ImgBack);
            Common.MasterData.Detail = new NavigationPage(new MainTabbedPages.MainTabbedPage(Constraints.Str_Home));
        }

        private void ImgExpand_Tapped(object sender, EventArgs e)
        {
            try
            {
                var imgExp = (Grid)sender;
                var viewCell = (ViewCell)imgExp.Parent.Parent;
                if (viewCell != null)
                {
                    viewCell.ForceUpdateSize();
                }
                var faqModel = imgExp.BindingContext as FAQResponse;
                if (faqModel != null && faqModel.ArrowImage == Constraints.Img_GreenArrowDown)
                {
                    faqModel.ArrowImage = Constraints.Img_GreenArrowUp;
                    faqModel.ShowFaqDesc = true;
                }
                else
                {
                    faqModel.ArrowImage = Constraints.Img_GreenArrowDown;
                    faqModel.ShowFaqDesc = false;
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("FaqHelpView/ImgExpand_Tapped: " + ex.Message);
            }
        }

        private void BtnLogo_Clicked(object sender, EventArgs e)
        {
            Common.MasterData.Detail = new NavigationPage(new MainTabbedPages.MainTabbedPage(Constraints.Str_Home));
        }
        #endregion

        private void lstFaq_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            lstFaq.SelectedItem = null;
        }
    }
}
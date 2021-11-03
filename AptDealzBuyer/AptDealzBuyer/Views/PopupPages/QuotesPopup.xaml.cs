using AptDealzBuyer.Utility;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using Xamarin.Forms.Xaml;

namespace AptDealzBuyer.Views.PopupPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuotesPopup : PopupPage
    {
        #region [ Objects ]      
        public event EventHandler isRefresh;
        private string type = string.Empty;
        #endregion
        public QuotesPopup(string filterType)
        {
            InitializeComponent();
            type = filterType;
        }
        protected override bool OnBackgroundClicked()
        {
            base.OnBackgroundClicked();
            return false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BindSource(type);
        }

        private void BindSource(string viewSource)
        {
            try
            {
                if (!string.IsNullOrEmpty(viewSource))
                {
                    if (viewSource == Constraints.Str_ReceivedDate)
                    {
                        ClearSource();
                        imgFirstType.Source = Constraints.Img_RadioSelected;
                    }
                    else if (viewSource == Constraints.Str_Amount)
                    {
                        ClearSource();
                        imgSecondType.Source = Constraints.Img_RadioSelected;
                    }
                    else
                    {
                        ClearSource();
                        imgFirstType.Source = Constraints.Img_RadioSelected;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("QuotesPopup/BindSource: " + ex.Message);
            }
        }

        private void ClearSource()
        {
            imgFirstType.Source = Constraints.Img_RedioUnSelected;
            imgSecondType.Source = Constraints.Img_RedioUnSelected;
        }
        private void StkFirstType_Tapped(object sender, EventArgs e)
        {
            try
            {
                BindSource(Constraints.Str_ReceivedDate);
                isRefresh?.Invoke(Constraints.Str_ReceivedDate, null);
                PopupNavigation.Instance.PopAsync();
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("QuotesPopup/StkFirstType_Tapped: " + ex.Message);
            }
        }

        private void StkSecondType_Tapped(object sender, EventArgs e)
        {
            try
            {
                BindSource(Constraints.Str_Amount);
                isRefresh?.Invoke(Constraints.Str_Amount, null);
                PopupNavigation.Instance.PopAsync();
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("QuotesPopup/StkSecondType_Tapped: " + ex.Message);
            }
        }
    }
}
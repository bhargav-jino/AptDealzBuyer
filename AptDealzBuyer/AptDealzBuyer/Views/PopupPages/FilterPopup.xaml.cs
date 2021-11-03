using AptDealzBuyer.Utility;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using Xamarin.Forms.Xaml;

namespace AptDealzBuyer.Views.PopupPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilterPopup : PopupPage
    {
        #region [ Objects ]      
        public event EventHandler isRefresh;
        private string PageName;
        #endregion

        #region [ Constructor ]       
        public FilterPopup(string SortBy, string SortPageName)
        {
            InitializeComponent();
            PageName = SortPageName;
            BindSource(SortBy);
        }
        #endregion

        #region [ Methos ]
        protected override bool OnBackgroundClicked()
        {
            base.OnBackgroundClicked();
            return false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BindLabel();
        }

        private void BindLabel()
        {
            try
            {
                if (PageName == Constraints.Str_Active)
                {
                    StkThirdType.IsVisible = true;
                    StkFourType.IsVisible = true;
                    lblFirstType.Text = Utility.SortByField.ID.ToString();
                    lblSecondType.Text = Utility.SortByField.Date.ToString();
                    lblThirdType.Text = Utility.SortByField.Quotes.ToString();
                    lblFourType.Text = Utility.SortByField.TotalPriceEstimation.ToString().ToCamelCase();
                }
                else if (PageName == Constraints.Str_Order)
                {
                    StkThirdType.IsVisible = true;
                    StkFourType.IsVisible = false;
                    lblFirstType.Text = Utility.SortByField.ID.ToString();
                    lblSecondType.Text = Utility.SortByField.Date.ToString();
                    lblThirdType.Text = Utility.SortByField.TotalPriceEstimation.ToString().ToCamelCase();
                }
                else if (PageName == Constraints.Str_Grievances)
                {
                    StkFourType.IsVisible = false;
                    StkThirdType.IsVisible = false;
                    lblFirstType.Text = Utility.SortByField.ID.ToString();
                    lblSecondType.Text = Utility.SortByField.Date.ToString();
                }
                else
                {
                    StkFourType.IsVisible = true;
                    StkThirdType.IsVisible = true;
                    lblFirstType.Text = Utility.SortByField.ID.ToString();
                    lblSecondType.Text = Utility.SortByField.Date.ToString();
                    lblThirdType.Text = Utility.SortByField.Quotes.ToString();
                    lblFourType.Text = Utility.SortByField.TotalPriceEstimation.ToString().ToCamelCase();
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("FilterPopup/BindLabel: " + ex.Message);
            }
        }

        private void BindSource(string viewSource)
        {
            try
            {
                if (!string.IsNullOrEmpty(viewSource))
                {
                    if (viewSource == SortByField.ID.ToString())
                    {
                        ClearSource();
                        imgFirstType.Source = Constraints.Img_RadioSelected;
                    }
                    else if (viewSource == SortByField.Date.ToString())
                    {
                        ClearSource();
                        imgSecondType.Source = Constraints.Img_RadioSelected;
                    }
                    else if (viewSource == SortByField.Quotes.ToString() || viewSource == SortByField.Amount.ToString())
                    {
                        ClearSource();
                        imgThirdType.Source = Constraints.Img_RadioSelected;
                    }
                    else if (viewSource == SortByField.TotalPriceEstimation.ToString())
                    {
                        ClearSource();
                        imgFourType.Source = Constraints.Img_RadioSelected;
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
                Common.DisplayErrorMessage("FilterPopup/BindSource: " + ex.Message);
            }
        }

        private void ClearSource()
        {
            imgFirstType.Source = Constraints.Img_RedioUnSelected;
            imgSecondType.Source = Constraints.Img_RedioUnSelected;
            imgThirdType.Source = Constraints.Img_RedioUnSelected;
            imgFourType.Source = Constraints.Img_RedioUnSelected;
        }
        #endregion

        #region [ Events ]
        private void StkFirstType_Tapped(object sender, EventArgs e)
        {
            try
            {
                BindSource(SortByField.ID.ToString());
                isRefresh?.Invoke(SortByField.ID.ToString(), null);
                PopupNavigation.Instance.PopAsync();
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("FilterPopup/StkFirstType_Tapped: " + ex.Message);
            }
        }

        private void StkSecondType_Tapped(object sender, EventArgs e)
        {
            try
            {
                BindSource(SortByField.Date.ToString());
                isRefresh?.Invoke(SortByField.Date.ToString(), null);
                PopupNavigation.Instance.PopAsync();
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("FilterPopup/StkSecondType_Tapped: " + ex.Message);
            }
        }

        private void StkThirdType_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (PageName == Constraints.Str_Active)
                {
                    BindSource(SortByField.Quotes.ToString());
                    isRefresh?.Invoke(SortByField.Quotes.ToString(), null);
                }
                else
                {
                    BindSource(SortByField.Amount.ToString());
                    isRefresh?.Invoke(SortByField.Amount.ToString(), null);
                }
                PopupNavigation.Instance.PopAsync();
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("FilterPopup/StkThirdType_Tapped: " + ex.Message);
            }
        }

        private void StkFourType_Tapped(object sender, EventArgs e)
        {
            try
            {
                BindSource(SortByField.TotalPriceEstimation.ToString());
                isRefresh?.Invoke(SortByField.TotalPriceEstimation.ToString(), null);
                PopupNavigation.Instance.PopAsync();
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("FilterPopup/StkFourType_Tapped: " + ex.Message);
            }
        }
        #endregion
    }
}
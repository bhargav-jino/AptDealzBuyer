using Acr.UserDialogs;
using AptDealzBuyer.API;
using AptDealzBuyer.Model.Reponse;
using AptDealzBuyer.Model.Request;
using AptDealzBuyer.Repository;
using AptDealzBuyer.Utility;
using AptDealzBuyer.Views.PopupPages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AptDealzBuyer.Views.DashboardPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewRequirememntPage : ContentPage
    {
        #region [ Objects ]
        private string ReqType = string.Empty;
        private string RequirementId;
        private string ProductImageUrl = string.Empty;
        private string filterBy = Constraints.Str_ReceivedDate;
        private bool isAssending = false;

        Requirement mRequirement;
        private List<string> subcaregories;
        private List<ReceivedQuote> mQuoteList;
        #endregion

        #region [ Constructor ]
        public ViewRequirememntPage(string RequirementId, string ReqType = Constraints.Str_Active)
        {
            try
            {
                InitializeComponent();
                this.ReqType = ReqType;
                this.RequirementId = RequirementId;
                mRequirement = new Requirement();
                mQuoteList = new List<ReceivedQuote>();

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
                Common.DisplayErrorMessage("ViewRequirememntPage/Ctor: " + ex.Message);
            }
        }
        #endregion

        #region [ Methods ]
        public void Dispose()
        {
            GC.Collect();
            GC.SuppressFinalize(this);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Dispose();
        }

        protected override void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                BindGrids();
                GetRequirementDetails();
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("ViewRequirememntPage/OnAppearing: " + ex.Message);
            }
        }

        private void BindGrids()
        {
            if (ReqType == Constraints.Str_Active)
            {
                grdPrevReq.IsVisible = false;
                grdActiveReq.IsVisible = true;

            }
            else
            {
                grdActiveReq.IsVisible = false;
                grdPrevReq.IsVisible = true;
            }
        }

        private async Task GetRequirementDetails()
        {
            try
            {
                mRequirement = await DependencyService.Get<IRequirementRepository>().GetRequirementById(RequirementId);
                if (mRequirement != null)
                {
                    if (!Common.EmptyFiels(mRequirement.ProductImage))
                    {
                        imgProductImage.Source = mRequirement.ProductImage;
                        ProductImageUrl = mRequirement.ProductImage;
                    }
                    else
                    {
                        imgProductImage.Source = Constraints.Img_ProductBanner;
                    }

                    lblDescription.Text = mRequirement.ProductDescription;

                    if (!Common.EmptyFiels(mRequirement.Title))
                    {
                        lblTitle.Text = mRequirement.Title;
                    }
                    if (!Common.EmptyFiels(mRequirement.RequirementNo))
                    {
                        lblRequirementId.Text = mRequirement.RequirementNo;
                    }
                    if (!Common.EmptyFiels(mRequirement.Category))
                    {
                        lblCategory.Text = mRequirement.Category;
                    }
                    if (mRequirement.SubCategories != null)
                    {
                        subcaregories = mRequirement.SubCategories;
                        lblSubCategory.Text = string.Join(",", subcaregories);
                    }
                    if (!Common.EmptyFiels(mRequirement.Quantity.ToString()))
                    {
                        lblQuantity.Text = mRequirement.Quantity + " " + mRequirement.Unit;
                    }
                    if (!Common.EmptyFiels(mRequirement.TotalPriceEstimation.ToString()))
                    {
                        lblEstimatePrice.Text = "Rs " + mRequirement.TotalPriceEstimation.ToString();
                    }
                    if (mRequirement.ExpectedDeliveryDate != null && mRequirement.ExpectedDeliveryDate != DateTime.MinValue)
                    {
                        lblDeliveryDateValue.Text = mRequirement.ExpectedDeliveryDate.ToString(Constraints.Str_DateFormate);
                    }
                    if (!Common.EmptyFiels(mRequirement.DeliveryLocationPinCode))
                    {
                        lblLocPinCode.Text = mRequirement.DeliveryLocationPinCode;
                    }
                    if (!Common.EmptyFiels((string)mRequirement.PreferredSourceOfSupply))
                    {
                        lblPreferredSource.Text = (string)mRequirement.PreferredSourceOfSupply;
                    }

                    if (mRequirement.NeedInsuranceCoverage)
                    {
                        lblNeedInsurance.Text = Constraints.Str_Right;
                    }
                    else
                    {
                        lblNeedInsurance.Text = Constraints.Str_Wrong;
                    }

                    if (mRequirement.PreferInIndiaProducts)
                    {
                        lblPreferInIndiaProducts.Text = Constraints.Str_Right;
                    }
                    else
                    {
                        lblPreferInIndiaProducts.Text = Constraints.Str_Wrong;
                    }

                    if (mRequirement.PickupProductDirectly)
                    {
                        lblDeliveryDate.Text = Constraints.Str_ExpectedPickupDate;
                        lblPreferSeller.Text = Constraints.Str_Right;
                    }
                    else
                    {
                        lblDeliveryDate.Text = Constraints.Str_ExpectedDeliveryDate;
                        lblPreferSeller.Text = Constraints.Str_Wrong;
                    }

                    //List of Quote
                    mQuoteList = mRequirement.ReceivedQuotes;
                    GetListOfQuotes(filterBy, false);

                    #region [ Address ]
                    lblBillingAddress.Text = mRequirement.BillingAddressName + "\n"
                               + mRequirement.BillingAddressBuilding + "\n"
                               + mRequirement.BillingAddressStreet + "\n"
                               + mRequirement.BillingAddressCity + "\n"
                               + mRequirement.BillingAddressState + "-" + mRequirement.BillingAddressPinCode;

                    if (!Common.EmptyFiels(mRequirement.ShippingAddressName) || !Common.EmptyFiels(mRequirement.ShippingAddressBuilding)
                      || !Common.EmptyFiels(mRequirement.ShippingAddressStreet) || !Common.EmptyFiels(mRequirement.ShippingAddressCity)
                      || !Common.EmptyFiels(mRequirement.ShippingAddressPinCode) || !Common.EmptyFiels(mRequirement.ShippingAddressLandmark)
                      )
                    {
                        lblShippingAddress.Text = mRequirement.ShippingAddressName + "\n"
                            + mRequirement.ShippingAddressBuilding + "\n"
                            + mRequirement.ShippingAddressStreet + "\n"
                            + mRequirement.ShippingAddressCity + "\n"
                            + mRequirement.ShippingAddressState + "-" + mRequirement.ShippingAddressPinCode + "\n"
                            + mRequirement.ShippingAddressLandmark;
                        GrdShippingAddress.IsVisible = true;
                    }
                    else
                    {
                        GrdShippingAddress.IsVisible = false;
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("ViewRequirememntPage/GetRequirementDetails: " + ex.Message);
            }
        }

        private void GetListOfQuotes(string filterBy, bool sortBy)
        {
            try
            {
                if (mQuoteList != null && mQuoteList.Count > 0)
                {
                    var AccepteedQuote = mQuoteList.Where(x => x.Status == QuoteStatus.Accepted.ToString());
                    if (ReqType == Constraints.Str_Active)
                    {
                        lblNoRecord.IsVisible = false;
                        lstQoutes.IsVisible = true;
                        FrmSortBy.IsVisible = true;
                        if (filterBy == Constraints.Str_Amount && sortBy)
                        {
                            lstQoutes.ItemsSource = mQuoteList.OrderBy(x => x.Amount).ToList();
                        }
                        else if (filterBy == Constraints.Str_Amount && !sortBy)
                        {
                            lstQoutes.ItemsSource = mQuoteList.OrderByDescending(x => x.Amount).ToList();
                        }
                        else if (filterBy == Constraints.Str_ReceivedDate && sortBy)
                        {
                            lstQoutes.ItemsSource = mQuoteList.OrderBy(x => x.ReceivedDate).ToList();
                        }
                        else
                        {
                            lstQoutes.ItemsSource = mQuoteList.OrderByDescending(x => x.ReceivedDate).ToList();
                        }
                        lstQoutes.HeightRequest = mQuoteList.Count * 100;
                    }
                    else
                    {
                        var mAcceptedQuoteList = AccepteedQuote.ToList();
                        if (mAcceptedQuoteList != null && mAcceptedQuoteList.Count > 0)
                        {
                            lblAcceptedQoutesNoRecord.IsVisible = false;
                            lstAcceptedQoutes.IsVisible = true;
                            lstAcceptedQoutes.ItemsSource = mAcceptedQuoteList.ToList();
                            lstAcceptedQoutes.HeightRequest = mAcceptedQuoteList.Count * 100;
                        }
                        else
                        {
                            lblAcceptedQoutesNoRecord.IsVisible = true;
                            lstAcceptedQoutes.IsVisible = false;
                            lstAcceptedQoutes.ItemsSource = null;
                            lstAcceptedQoutes.HeightRequest = 50;
                        }
                    }
                }
                else
                {
                    if (ReqType == Constraints.Str_Active)
                    {
                        lblNoRecord.IsVisible = true;
                        FrmSortBy.IsVisible = false;
                        FrmFilterBy.IsVisible = false;
                        lstQoutes.IsVisible = false;
                        lstQoutes.ItemsSource = null;
                        lstQoutes.HeightRequest = 50;
                    }
                    else
                    {
                        lblAcceptedQoutesNoRecord.IsVisible = true;
                        lstAcceptedQoutes.IsVisible = false;
                        lstAcceptedQoutes.ItemsSource = null;
                        lstAcceptedQoutes.HeightRequest = 50;

                    }
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("ViewRequirememntPage/GetListOfQuotes: " + ex.Message);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private async Task DeleteRequirement(string requirmentId)
        {
            try
            {
                var isDelete = await DependencyService.Get<IRequirementRepository>().DeleteRequirement(requirmentId);
                if (isDelete)
                {
                    Common.MasterData.Detail = new NavigationPage(new MainTabbedPages.MainTabbedPage(Constraints.Str_Requirements, isNavigate: true));
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("ViewRequirememntPage/DeleteRequirement: " + ex.Message);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }
        #endregion

        #region [ Events ]
        #region [ Header Navigation ]
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
                    Common.DisplayErrorMessage("ViewRequirememntPage/ImgNotification_Tapped: " + ex.Message);
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

        private async void ImgBack_Tapped(object sender, EventArgs e)
        {
            Common.BindAnimation(imageButton: ImgBack);
            await Navigation.PopAsync();
        }

        private void ImgMenu_Tapped(object sender, EventArgs e)
        {
            Common.BindAnimation(image: ImgMenu);
            //Common.OpenMenu();
        }

        private void BtnLogo_Clicked(object sender, EventArgs e)
        {
            Common.MasterData.Detail = new NavigationPage(new MainTabbedPages.MainTabbedPage(Constraints.Str_Home));
        }
        #endregion

        private async void BtnDeleteRequirement_Tapped(object sender, EventArgs e)
        {
            var Tab = (Button)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    Common.BindAnimation(button: BtnDeleteRequirement);
                    await DeleteRequirement(RequirementId);
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("ViewRequirememntPage/BtnDeleteRequirement: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }

        }

        private void FrmSortBy_Tapped(object sender, EventArgs e)
        {
            if (ImgSort.Source.ToString().Replace("File: ", "") == Constraints.Img_SortASC)
            {
                ImgSort.Source = Constraints.Img_SortDSC;
                isAssending = false;
            }
            else
            {
                ImgSort.Source = Constraints.Img_SortASC;
                isAssending = true;
            }
            GetListOfQuotes(filterBy, isAssending);
        }

        private async void FrmFilter_Tapped(object sender, EventArgs e)
        {
            var Tab = (Frame)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    var sortby = new QuotesPopup(filterBy);
                    sortby.isRefresh += (s1, e1) =>
                    {
                        string result = s1.ToString();
                        if (!Common.EmptyFiels(result))
                        {
                            filterBy = result;
                            lblFilterBy.Text = filterBy;

                            GetListOfQuotes(filterBy, isAssending);
                        }
                    };
                    await PopupNavigation.Instance.PushAsync(sortby);
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("ViewRequirememntPage/FrmFilter_Tapped: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }

        private void CopyString_Tapped(object sender, EventArgs e)
        {
            try
            {
                string message = Constraints.CopiedRequirementId;
                Common.CopyText(lblRequirementId, message);
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("ViewRequirememntPage/CopyString_Tapped: " + ex.Message);
            }
        }

        #region [ Listing ]
        private async void GrdViewQuote_Tapped(object sender, EventArgs e)
        {
            var GridTab = (Grid)sender;
            if (GridTab.IsEnabled)
            {
                try
                {
                    GridTab.IsEnabled = false;
                    var mQuote = GridTab.BindingContext as ReceivedQuote;
                    await Navigation.PushAsync(new QuoteDetailsPage(mQuote.QuoteId));
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("ViewRequirememntPage/GrdViewQuote_Tapped: " + ex.Message);
                }
                finally
                {
                    GridTab.IsEnabled = true;
                }
            }
        }

        private async void RefreshView_Refreshing(object sender, EventArgs e)
        {
            try
            {
                rfView.IsRefreshing = true;
                BindGrids();
                await GetRequirementDetails();
                rfView.IsRefreshing = false;
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("ViewRequirememntPage/RefreshView_Refreshing: " + ex.Message);
            }
        }

        private void lstQoutes_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            lstQoutes.SelectedItem = null;
        }

        private void lstAcceptedQoutes_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            lstAcceptedQoutes.SelectedItem = null;
        }
        #endregion

        private async void FrmProductImage_Tapped(object sender, EventArgs e)
        {
            var Tab = (Frame)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    if (!Common.EmptyFiels(ProductImageUrl))
                    {
                        var base64File = ImageConvertion.ConvertImageURLToBase64(ProductImageUrl);
                        string extension = Path.GetExtension(ProductImageUrl).ToLower();

                        var successPopup = new DisplayDocumentPopup(base64File, extension);
                        await PopupNavigation.Instance.PushAsync(successPopup);
                    }
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("ViewRequirememntPage/FrmProductImage_Tapped: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }
        #endregion
    }
}
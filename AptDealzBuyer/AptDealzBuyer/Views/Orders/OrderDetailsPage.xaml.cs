using Acr.UserDialogs;
using AptDealzBuyer.API;
using AptDealzBuyer.Model.Reponse;
using AptDealzBuyer.Model.Request;
using AptDealzBuyer.Repository;
using AptDealzBuyer.Utility;
using AptDealzBuyer.Views.DashboardPages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AptDealzBuyer.Views.Orders
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrderDetailsPage : ContentPage
    {
        #region [ Objects ]
        private Order mOrder;
        private string OrderId;
        RateAndReviewAPI rateAndReviewAPI;
        #endregion

        #region [ Constructor ]
        public OrderDetailsPage(string orderId)
        {
            try
            {
                InitializeComponent();
                mOrder = new Order();
                rateAndReviewAPI = new RateAndReviewAPI();
                OrderId = orderId;

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
                Common.DisplayErrorMessage("OrderDetailsPage/Ctor: " + ex.Message);
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
            base.OnAppearing();
            GetOrderDetails();
        }

        private void BindSellerAddress(SellerAddressDetails mSellerAddress)
        {
            try
            {
                if (mSellerAddress != null &&
                       (!Common.EmptyFiels(mSellerAddress.Building) || !Common.EmptyFiels(mSellerAddress.Street) ||
                        !Common.EmptyFiels(mSellerAddress.City) || !Common.EmptyFiels(mSellerAddress.State) ||
                        !Common.EmptyFiels(mSellerAddress.PinCode) || !Common.EmptyFiels(mSellerAddress.Landmark) ||
                        !Common.EmptyFiels(mSellerAddress.Country)))
                {
                    FrmSellerAddress.IsVisible = true;
                    List<string> addresses = new List<string>();

                    if (!Common.EmptyFiels(mSellerAddress.Building))
                    {
                        addresses.Add(mSellerAddress.Building);
                    }

                    if (!Common.EmptyFiels(mSellerAddress.Street) && !Common.EmptyFiels(mSellerAddress.City))
                    {
                        addresses.Add(mSellerAddress.Street + ", " + mSellerAddress.City);
                    }
                    else
                    {
                        if (!Common.EmptyFiels(mSellerAddress.Street))
                        {
                            addresses.Add(mSellerAddress.Street);
                        }
                        if (!Common.EmptyFiels(mSellerAddress.City))
                        {
                            addresses.Add(mSellerAddress.City);
                        }
                    }

                    if (!Common.EmptyFiels(mSellerAddress.State) && !Common.EmptyFiels(mSellerAddress.PinCode))
                    {
                        addresses.Add(mSellerAddress.State + " " + mSellerAddress.PinCode);
                    }
                    else
                    {
                        if (!Common.EmptyFiels(mSellerAddress.State))
                        {
                            addresses.Add(mSellerAddress.State);
                        }
                        if (!Common.EmptyFiels(mSellerAddress.PinCode))
                        {
                            addresses.Add(mSellerAddress.PinCode);
                        }
                    }

                    if (!Common.EmptyFiels(mSellerAddress.Landmark))
                    {
                        addresses.Add(mSellerAddress.Landmark + ", " + mSellerAddress.Country);
                    }
                    else
                    {
                        if (!Common.EmptyFiels(mSellerAddress.Landmark))
                        {
                            addresses.Add(mSellerAddress.Landmark);
                        }
                        if (!Common.EmptyFiels(mSellerAddress.Country))
                        {
                            addresses.Add(mSellerAddress.Country);
                        }
                    }

                    if (addresses != null && addresses.Count > 0)
                    {
                        lblSellerAddress.Text = string.Join(Environment.NewLine, addresses);
                    }
                }
                else
                {
                    //HideAddress
                    FrmSellerAddress.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("OrderDetailsPage/BindSellerAddress: " + ex.Message);
            }
        }

        private void BindShippingAddress(ShippingAddressDetails mShippingAddress)
        {
            try
            {
                if (mShippingAddress != null && (mShippingAddress.Country > 0 ||
                      !Common.EmptyFiels(mShippingAddress.Building) || !Common.EmptyFiels(mShippingAddress.Street) ||
                      !Common.EmptyFiels(mShippingAddress.City) || !Common.EmptyFiels(mShippingAddress.PinCode) ||
                      !Common.EmptyFiels(mShippingAddress.Landmark) || !Common.EmptyFiels(mShippingAddress.State)))
                {
                    FrmShippingAddress.IsVisible = true;
                    List<string> addresses = new List<string>();

                    if (!Common.EmptyFiels(mShippingAddress.Building))
                    {
                        addresses.Add(mShippingAddress.Building);
                    }

                    if (!Common.EmptyFiels(mShippingAddress.Street) && !Common.EmptyFiels(mShippingAddress.City))
                    {
                        addresses.Add(mShippingAddress.Street + ", " + mShippingAddress.City);
                    }
                    else
                    {
                        if (!Common.EmptyFiels(mShippingAddress.Street))
                        {
                            addresses.Add(mShippingAddress.Street);
                        }
                        if (!Common.EmptyFiels(mShippingAddress.City))
                        {
                            addresses.Add(mShippingAddress.City);
                        }
                    }

                    if (!Common.EmptyFiels(mShippingAddress.State) && !Common.EmptyFiels(mShippingAddress.PinCode))
                    {
                        addresses.Add(mShippingAddress.State + " " + mShippingAddress.PinCode);
                    }
                    else
                    {
                        if (!Common.EmptyFiels(mShippingAddress.State))
                        {
                            addresses.Add(mShippingAddress.State);
                        }
                        if (!Common.EmptyFiels(mShippingAddress.PinCode))
                        {
                            addresses.Add(mShippingAddress.PinCode);
                        }
                    }

                    if (!Common.EmptyFiels(mShippingAddress.Landmark))
                    {
                        addresses.Add(mShippingAddress.Landmark);
                    }

                    if (mShippingAddress.Country.HasValue)
                    {
                        addresses.Add(mShippingAddress.Country.Value.ToString());
                    }


                    if (addresses != null && addresses.Count > 0)
                    {
                        lblShippingAddress.Text = string.Join(Environment.NewLine, addresses);
                    }
                }
                else
                {
                    //HideAddress
                    FrmShippingAddress.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("OrderDetailsPage/BindShippingAddress: " + ex.Message);
            }
        }

        private void BindOrderStatus(int Status, bool DeliveryConfirmation, bool OrderCancelAllowed)
        {
            try
            {
                if ((Status == (int)OrderStatus.Shipped || Status == (int)OrderStatus.Delivered ||
                    Status == (int)OrderStatus.Completed) && (DeliveryConfirmation == false))
                {
                    //Visible only ConfirmDelivery Button
                    GrdTrackOrderAndRaiseGrvInMainGrid.IsVisible = false;
                    BtnShowQrCodeInMainGrid.IsVisible = false;
                    GrdScanQRCodeImageInMainGrid.IsVisible = false;

                    GrdAllSubGrid.IsVisible = true;
                    BtnConfirmDeliveryInSubGrid.IsVisible = true;
                    GrdRateAndReportInSubGrid.IsVisible = false;
                    GrdWarningAndRaiseComplainInSubGrid.IsVisible = false;
                    GrdCancelOrderInSubGrid.IsVisible = false;

                    if (true)
                    {
                        BtnRaiseGrievance.IsVisible = false;
                    }
                    else
                    { BtnRaiseGrievance.IsVisible = true; }
                }
                else if (Status == (int)OrderStatus.Pending || Status == (int)OrderStatus.Accepted ||
                    Status == (int)OrderStatus.Delivered || Status == (int)OrderStatus.CancelledFromBuyer)
                {
                    //Nothing to show
                    GrdTrackOrderAndRaiseGrvInMainGrid.IsVisible = false;
                    GrdScanQRCodeImageInMainGrid.IsVisible = false;
                    BtnShowQrCodeInMainGrid.IsVisible = false;

                    GrdAllSubGrid.IsVisible = false;
                }
                else if (Status == (int)OrderStatus.ReadyForPickup)
                {
                    //visible Warning Msg,Raise Complain button ,Qr Code image   
                    GrdTrackOrderAndRaiseGrvInMainGrid.IsVisible = false;
                    BtnShowQrCodeInMainGrid.IsVisible = true;

                    GrdAllSubGrid.IsVisible = true;
                    GrdWarningAndRaiseComplainInSubGrid.IsVisible = true;
                    BtnConfirmDeliveryInSubGrid.IsVisible = false;
                    GrdRateAndReportInSubGrid.IsVisible = false;
                    GrdCancelOrderInSubGrid.IsVisible = false;
                }
                else if (Status == (int)OrderStatus.Shipped)
                {
                    //visible Track order and Raise grv buttons
                    GrdTrackOrderAndRaiseGrvInMainGrid.IsVisible = true;
                    BtnShowQrCodeInMainGrid.IsVisible = false;
                    GrdScanQRCodeImageInMainGrid.IsVisible = false;

                    GrdAllSubGrid.IsVisible = false;
                }
                else if (Status == (int)OrderStatus.Completed)
                {
                    //Visible Rating / Repost Req and Raise Grv Buttons   
                    GrdTrackOrderAndRaiseGrvInMainGrid.IsVisible = false;
                    BtnShowQrCodeInMainGrid.IsVisible = false;
                    GrdScanQRCodeImageInMainGrid.IsVisible = false;

                    GrdAllSubGrid.IsVisible = true;
                    GrdRateAndReportInSubGrid.IsVisible = true;
                    GrdCancelOrderInSubGrid.IsVisible = false;
                    GrdWarningAndRaiseComplainInSubGrid.IsVisible = false;
                    BtnConfirmDeliveryInSubGrid.IsVisible = false;

                    if (true)
                    {
                        BtnRaiseGrievance.IsVisible = false;
                    }
                    else
                    { BtnRaiseGrievance.IsVisible = true; }
                }

                if (OrderCancelAllowed)
                {
                    GrdCancelOrderInSubGrid.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("OrderDetailsPage/BindOrderStatus: " + ex.Message);
            }
        }

        private async Task GetOrderDetails()
        {
            try
            {
                mOrder = await DependencyService.Get<IOrderRepository>().GetOrderDetails(OrderId);
                if (mOrder != null)
                {
                    #region [ Details ]
                    if (mOrder.PickupProductDirectly)
                        lblPinCodeTitle.Text = Constraints.Str_ProductPickupPINCode;
                    else
                        lblPinCodeTitle.Text = Constraints.Str_ShippingPINCode;

                    lblOrderId.Text = mOrder.OrderNo;
                    lblRequirementId.Text = mOrder.RequirementNo;
                    lblRequirementTitle.Text = mOrder.Title;
                    lblQuoteRefNo.Text = mOrder.QuoteNo;
                    lblBuyerName.Text = mOrder.BuyerContact.Name;
                    if (mOrder.SellerContact != null && !Common.EmptyFiels(mOrder.SellerContact.SellerId) && !Common.EmptyFiels(mOrder.SellerContact.UserId))
                    {
                        StkSellerName.IsVisible = true;
                        StkSellerPhoneNo.IsVisible = true;
                        StkSellerEmail.IsVisible = true;
                        lblContactSellerLabel.IsVisible = true;
                        lblSellerContact.IsVisible = true;

                        lblSellerName.Text = mOrder.SellerContact.Name;
                        lblSellerPNumber.Text = mOrder.SellerContact.PhoneNumber;
                        lblSellerEmail.Text = mOrder.SellerContact.Email;
                        lblSellerContact.Text = mOrder.SellerContact.PhoneNumber;
                    }
                    else
                    {
                        StkSellerName.IsVisible = false;
                        StkSellerPhoneNo.IsVisible = false;
                        StkSellerEmail.IsVisible = false;
                        lblContactSellerLabel.IsVisible = false;
                        lblSellerContact.IsVisible = false;

                        lblSellerName.Text = string.Empty;
                        lblSellerPNumber.Text = string.Empty;
                        lblSellerEmail.Text = string.Empty;
                        lblSellerContact.Text = string.Empty;
                    }

                    if (!Common.EmptyFiels(mOrder.ShippingPincode))
                    {
                        StkShippingPINCode.IsVisible = true;
                        lblShippingPINCode.Text = mOrder.ShippingPincode;
                    }
                    else
                    {
                        StkShippingPINCode.IsVisible = false;
                        lblShippingPINCode.Text = string.Empty;
                    }

                    lblRequestedQuantity.Text = mOrder.RequestedQuantity + " " + mOrder.Unit;
                    lblUnitPrice.Text = "Rs " + mOrder.UnitPrice;
                    lblNetAmount.Text = "Rs " + mOrder.NetAmount;
                    lblHandlingCharges.Text = "Rs " + mOrder.HandlingCharges;
                    lblShippingCharges.Text = "Rs " + mOrder.ShippingCharges;
                    lblInsuranceCharges.Text = "Rs " + mOrder.InsuranceCharges;
                    lblOriginProduct.Text = mOrder.Country;
                    lblInvoiceNo.Text = mOrder.OrderNo;
                    lblTotalAmount.Text = "Rs " + mOrder.TotalAmount;
                    lblExpectedDate.Text = mOrder.ExpectedDelivery.ToString(Constraints.Str_DateFormate);

                    lblOrderStatus.Text = mOrder.OrderStatusDescr;
                    #endregion

                    #region [ Address ]
                    BindSellerAddress(mOrder.SellerAddressDetails);
                    BindShippingAddress(mOrder.ShippingAddressDetails);
                    #endregion

                    #region [ Status ]
                    BindOrderStatus(mOrder.OrderStatus, mOrder.IsDeliveryConfirmedFromBuyer, mOrder.IsOrderCancelAllowed);
                    #endregion
                }

            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("OrderDetailsPage/GetOrderDetails: " + ex.Message);
            }
        }

        private async Task CancelOrder()
        {
            try
            {
                await DependencyService.Get<IOrderRepository>().CancelOrder(OrderId);
                await GetOrderDetails();
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("OrderDetailsPage/CancelOrder: " + ex.Message);
            }
        }

        private async Task ConfirmDelivery()
        {
            try
            {
                await DependencyService.Get<IOrderRepository>().ConfirmDelivery(OrderId);
                await GetOrderDetails();
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("OrderDetailsPage/ConfirmDelivery: " + ex.Message);
            }
        }

        private async Task RatingReview(bool isSellerRating)
        {
            try
            {
                UserDialogs.Instance.ShowLoading(Constraints.Loading);
                Response mResponse = new Response();
                var mRatingReview = new RatingReview();
                mRatingReview.ReviewForOrderId = OrderId;
                mRatingReview.ReviewedForSellerId = mOrder.SellerContact.UserId;

                if (isSellerRating)
                {
                    mRatingReview.SellerRating = (decimal)rvSeller.Value;
                    mResponse = await rateAndReviewAPI.ReviewSeller(mRatingReview);
                }
                else
                {
                    mRatingReview.SellerRating = (decimal)rvProduct.Value;
                    mResponse = await rateAndReviewAPI.ReviewSellerProduct(mRatingReview);
                }

                if (mResponse != null && mResponse.Succeeded)
                {
                    Common.DisplaySuccessMessage(mResponse.Message);
                    await GetOrderDetails();
                }
                else
                {
                    if (mResponse != null && !Common.EmptyFiels(mResponse.Message))
                        Common.DisplayErrorMessage(mResponse.Message);
                    else
                        Common.DisplayErrorMessage(Constraints.Something_Wrong);
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("OrderDetailsPage/GetOrderDetails: " + ex.Message);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }

        }

        private async Task ShowQRCodeImage()
        {
            try
            {
                string imageBase64 = await DependencyService.Get<IOrderRepository>().GenerateQRCodeImage(OrderId);
                if (!Common.EmptyFiels(imageBase64))
                {
                    BtnShowQrCodeInMainGrid.IsVisible = false;
                    GrdScanQRCodeImageInMainGrid.IsVisible = true;
                    ImgQRCode.Source = Xamarin.Forms.ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(imageBase64)));
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("OrderDetailsPage/ShowQRCodeImage: " + ex.Message);
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
                    await Navigation.PushAsync(new DashboardPages.NotificationPage());
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("OrderDetailsPage/ImgNotification_Tapped: " + ex.Message);
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

        private void BtnLogo_Clicked(object sender, EventArgs e)
        {
            Common.MasterData.Detail = new NavigationPage(new MainTabbedPages.MainTabbedPage(Constraints.Str_Home));
        }

        private async void RefreshView_Refreshing(object sender, EventArgs e)
        {
            try
            {
                rfView.IsRefreshing = true;
                await GetOrderDetails();
                rfView.IsRefreshing = false;
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("OrderDetailsPage/RefreshView_Refreshing: " + ex.Message);
            }
        }

        private async void BtnRateSeller_Clicked(object sender, EventArgs e)
        {
            var Tab = (Button)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    Common.BindAnimation(button: BtnRateSeller);
                    await RatingReview(true);
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("OrderDetailsPage/BtnRateSeller_Clicked: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }

        private async void BtnRateProduct_Clicked(object sender, EventArgs e)
        {
            var Tab = (Button)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    Common.BindAnimation(button: BtnRateProduct);
                    await RatingReview(false);
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("OrderDetailsPage/BtnRateProduct_Clicked: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }

        private void BtnRateAptDealz_Clicked(object sender, EventArgs e)
        {

            var Tab = (Button)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    Common.BindAnimation(button: BtnRateAptDealz);
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("OrderDetailsPage/BtnRateAptDealz_Clicked: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }

        private async void BtnRepostReqt_Clicked(object sender, EventArgs e)
        {
            var Tab = (Button)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    Common.BindAnimation(button: BtnRepostReqt);
                    await Navigation.PushAsync(new DashboardPages.PostNewRequiremntPage(mOrder.RequirementId));
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("OrderDetailsPage/BtnRepostReqt_Clicked: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }

        private async void BtnRaiseGrievance_Tapped(object sender, EventArgs e)
        {
            var Tab = (Button)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    Common.BindAnimation(button: BtnRaiseGrievance);
                    await Navigation.PushAsync(new Orders.RaiseGrievancePage(OrderId));
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("OrderDetailsPage/BtnRaiseGrievance_Tapped: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }

        private async void BtnCancelOrder_Tapped(object sender, EventArgs e)
        {
            var Tab = (Button)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    Common.BindAnimation(button: BtnCancelOrder);
                    await CancelOrder();
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("OrderDetailsPage/BtnCancelOrder_Tapped: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }

        }

        private void BtnTrackOrder_Tapped(object sender, EventArgs e)
        {
            var Tab = (Button)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    Common.BindAnimation(button: BtnTrackOrder);
                    if (mOrder != null && mOrder.TrackingLink != null && mOrder.TrackingLink.Length > 10)
                    {
                        Xamarin.Essentials.Launcher.OpenAsync(new Uri(mOrder.TrackingLink));
                    }
                    else
                    {
                        Common.DisplayErrorMessage("Invalid tracking URL");
                    }
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("OrderDetailsPage/BtnTrack_Tapped: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }

        private async void BtnConfirmDelivery_Clicked(object sender, EventArgs e)
        {
            var Tab = (Button)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    Common.BindAnimation(button: BtnConfirmDeliveryInSubGrid);
                    await ConfirmDelivery();
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("OrderDetailsPage/BtnConfirmDelivery: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }

        private async void BtnShowQrCode_Clicked(object sender, EventArgs e)
        {
            var Tab = (Button)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    Common.BindAnimation(button: BtnShowQrCodeInMainGrid);
                    await ShowQRCodeImage();
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("OrderDetailsPage/BtnShowQrCode_Clicked: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }

        private async void CopyString_Tapped(object sender, EventArgs e)
        {
            var stackLayout = (StackLayout)sender;
            if (stackLayout.IsEnabled)
            {
                try
                {
                    stackLayout.IsEnabled = false;
                    if (!Common.EmptyFiels(stackLayout.ClassId))
                    {
                        if (stackLayout.ClassId == "OrderId")
                        {
                            string message = Constraints.CopiedOrderId;
                            Common.CopyText(lblOrderId, message);
                        }
                        else if (stackLayout.ClassId == "RequirementId")
                        {
                            if (!Common.EmptyFiels(mOrder.RequirementId))
                            {
                                string message = Constraints.CopiedRequirementId;
                                Common.CopyText(lblRequirementId, message);
                                await Navigation.PushAsync(new ViewRequirememntPage(mOrder.RequirementId));
                            }
                        }
                        else if (stackLayout.ClassId == "QuoteRefNo")
                        {
                            if (!Common.EmptyFiels(mOrder.QuoteId))
                            {
                                string message = Constraints.CopiedQuoteRefNo;
                                Common.CopyText(lblQuoteRefNo, message);
                                await Navigation.PushAsync(new QuoteDetailsPage(mOrder.QuoteId));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("OrderDetailsPage/CopyString_Tapped: " + ex.Message);
                }
                finally
                {
                    stackLayout.IsEnabled = true;
                }
            }

        }
        #endregion
    }
}
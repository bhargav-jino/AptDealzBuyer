using Acr.UserDialogs;
using AptDealzBuyer.API;
using AptDealzBuyer.Interfaces;
using AptDealzBuyer.Model;
using AptDealzBuyer.Model.Reponse;
using AptDealzBuyer.Model.Request;
using AptDealzBuyer.Repository;
using AptDealzBuyer.Utility;
using AptDealzBuyer.Views.OtherPages;
using Newtonsoft.Json.Linq;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AptDealzBuyer.Views.DashboardPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuoteDetailsPage : ContentPage
    {
        #region [ Objects ]
        private QuoteAPI quoteAPI;
        private OrderAPI orderAPI;
        private Quote mQuote;

        private string QuoteId;
        #endregion

        #region [ Constructor ]
        public QuoteDetailsPage(string quoteId)
        {
            try
            {
                InitializeComponent();
                quoteAPI = new QuoteAPI();
                orderAPI = new OrderAPI();
                QuoteId = quoteId;
                mQuote = new Quote();

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
                Common.DisplayErrorMessage("QuoteDetailsPage/Ctor: " + ex.Message);
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
            GetQuoteDetails();
        }

        private void BindRevealedContact(Requirement mRequirement, SellerContact mSellerContact, bool ContactRevealed)
        {
            if (mRequirement.Status != RequirementStatus.Active.ToString())
            {
                BtnAcceptQuote.IsVisible = false;
                BtnRevealContact.IsVisible = false;
                lblSellerContact.IsVisible = true;
                if (mSellerContact != null)
                {
                    lblSellerContact.Text = mSellerContact.PhoneNumber;
                }
                else
                {
                    lblSellerContact.Text = Constraints.Str_NotRevealContact;
                }
            }
            else
            {
                BtnAcceptQuote.IsVisible = true;
                if (ContactRevealed)
                {
                    lblSellerContact.IsVisible = true;
                    BtnRevealContact.IsVisible = false;

                    if (mSellerContact != null)
                        lblSellerContact.Text = mSellerContact.PhoneNumber;
                    else
                        lblSellerContact.Text = Constraints.Str_NotRevealContact;
                }
                else
                {
                    lblSellerContact.IsVisible = false;
                    BtnRevealContact.IsVisible = true;
                    BtnRevealContact.Text = Constraints.Str_RevealContact;
                }
            }
        }

        private async Task GetQuoteDetails()
        {
            try
            {
                mQuote = await DependencyService.Get<IQuoteRepository>().GetQuoteById(QuoteId);
                if (mQuote != null)
                {
                    lblRequirementId.Text = mQuote.RequirementNo;
                    lblQuoteRefNo.Text = mQuote.QuoteNo;
                    lblBuyerId.Text = mQuote.BuyerId;
                    lblSellerId.Text = mQuote.SellerId;
                    lblUnitPrice.Text = "Rs " + mQuote.UnitPrice;
                    lblCountryOrigin.Text = mQuote.Country;
                    lblTotalAmount.Text = "Rs " + mQuote.TotalQuoteAmount;


                    if (!Common.EmptyFiels(mQuote.ShippingPinCode))
                    {
                        lblShippingPINCode.Text = mQuote.ShippingPinCode;
                    }
                    if (!Common.EmptyFiels(mQuote.RequestedQuantity.ToString()))
                    {
                        lblRequestedQuantity.Text = mQuote.RequestedQuantity + " " + mQuote.Unit;
                    }
                    if (!Common.EmptyFiels(mQuote.NetAmount.ToString()))
                    {
                        lblNetAmount.Text = "Rs " + mQuote.NetAmount;
                    }
                    if (!Common.EmptyFiels(mQuote.HandlingCharges.ToString()))
                    {
                        lblHandlingCharges.Text = "Rs " + mQuote.HandlingCharges;
                    }
                    if (!Common.EmptyFiels(mQuote.ShippingCharges.ToString()))
                    {
                        lblShippingCharges.Text = "Rs " + mQuote.ShippingCharges;
                    }
                    if (!Common.EmptyFiels(mQuote.InsuranceCharges.ToString()))
                    {
                        lblInsuranceCharges.Text = "Rs " + mQuote.InsuranceCharges;
                    }
                    if (!Common.EmptyFiels(mQuote.Comments))
                    {
                        lblComments.Text = mQuote.Comments;
                    }



                    if (mQuote.Days.Contains("Expired"))
                    {

                        lblDate.Text = mQuote.ValidityDate.Date.ToString(Constraints.Str_DateFormate) + " ( Expired )";
                    }
                    else
                    {
                        lblDate.Text = mQuote.ValidityDate.Date.ToString(Constraints.Str_DateFormate);
                    }

                    if (mQuote.Status == QuoteStatus.Accepted.ToString() || mQuote.Days.Contains("Expired"))
                    {
                        BtnAcceptQuote.IsEnabled = false;
                    }
                    else
                    {
                        BtnAcceptQuote.IsEnabled = true;
                    }

                    var mRequirement = await GetRequirementsDetails();
                    if (mRequirement != null)
                    {
                        if (mRequirement.PickupProductDirectly)
                        {
                            lblShippingPinCode.Text = Constraints.Str_ProductPickupPINCode;
                        }
                        else
                        {
                            lblShippingPinCode.Text = Constraints.Str_ShippingPINCode;
                        }
                    }

                    BindRevealedContact(mRequirement, mQuote.SellerContact, mQuote.IsSellerContactRevealed);
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("QuoteDetailsPage/BindQuoteDetails: " + ex.Message);
            }
        }

        private async Task<Requirement> GetRequirementsDetails()
        {
            var mRequirement = await DependencyService.Get<IRequirementRepository>().GetRequirementById(mQuote.RequirementId);
            return mRequirement;
        }

        private async Task QuotePayment(bool isRevealContact)
        {
            try
            {
                string message = string.Empty;
                long revealRs = 0;
                if (isRevealContact)
                {
                    decimal amount = 0;

                    RequirementAPI requirementAPI = new RequirementAPI();
                    var mResponse = await requirementAPI.GetAmountToBePaidToRevealBuyerContact(mQuote.RequirementId);
                    if (mResponse != null && mResponse.Succeeded)
                    {
                        var jObject = (JObject)mResponse.Data;
                        if (jObject != null)
                        {
                            var mAmount = jObject.ToObject<Data>();
                            if (mAmount != null)
                                amount = mAmount.amount;

                            revealRs = (long)App.Current.Resources["RevealContact"];
                            long.TryParse(amount.ToString(), out revealRs);

                            //revealRs = (long)App.Current.Resources["RevealContact"];
                            message = "You need to pay Rs " + revealRs + " to reveal the Seller contact information. Do you wish to continue making payment?";
                        }
                    }
                    else
                    {
                        if (mResponse != null)
                            Common.DisplayErrorMessage(mResponse.Message);
                        else
                            Common.DisplayErrorMessage(Constraints.Something_Wrong);

                        return;
                    }
                }
                else
                {
                    message = "Make a payment of Rs " + mQuote.TotalQuoteAmount + " to Accept Quote";
                }

                var contactPopup = new PopupPages.PaymentPopup(message);
                contactPopup.isRefresh += async (s1, e1) =>
                {
                    bool isPay = (bool)s1;
                    if (isPay)
                    {
                        if (isRevealContact)
                        {
                            await RevealSellerContact(revealRs);
                        }
                        else
                        {
                            //Accept Quote
                            PaidQuote();
                        }
                    }
                };
                await PopupNavigation.Instance.PushAsync(contactPopup);
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("QuoteDetailsPage/QuotePayment: " + ex.Message);
            }
        }

        #region [ AcceptQuote ]
        /// <summary>
        ///  [ Create Order | Payment Process | Update Order Payment Status | Accept Quote ]
        /// </summary>
        private async void PaidQuote()
        {
            try
            {
                var mOrder = await CreateOrder();
                if (mOrder != null && mOrder.IsSuccess)
                {
                    OpenRazorPayQuote(mOrder);
                }
                else
                {
                    if (mOrder.OrderErrorMessage.Contains("duplicate"))
                    {
                        OpenRazorPayQuote(mOrder);
                    }
                    else
                    {
                        Common.DisplayErrorMessage(mOrder.OrderErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("QuoteDetailsPage/PaidQuote: " + ex.Message);
            }
        }


        /// <summary>
        /// [ Step 1 - CreateOrder ]
        /// </summary>
        private async Task<Order> CreateOrder()
        {
            Order mOrder = new Order();
            try
            {
                CreateOrder mCreateOrder = new CreateOrder();
                mCreateOrder.QuoteId = mQuote.QuoteId;
                mCreateOrder.PaidAmount = mQuote.TotalQuoteAmount;

                UserDialogs.Instance.ShowLoading(Constraints.Loading);
                var mResponse = await orderAPI.CreateOrder(mCreateOrder);
                if (mResponse != null && mResponse.Succeeded)
                {
                    var jObject = (JObject)mResponse.Data;
                    if (jObject != null)
                    {
                        mOrder = jObject.ToObject<Order>();
                        mOrder.OrderSuccessMessage = mResponse.Message;
                        mOrder.IsSuccess = true;
                    }
                }
                else
                {
                    if (mResponse != null && !Common.EmptyFiels(mResponse.Message))
                        mOrder.OrderErrorMessage = mResponse.Message;
                    else
                        mOrder.OrderErrorMessage = Constraints.Something_Wrong;

                    mOrder.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("QuoteDetailsPage/CreateOrder: " + ex.Message);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
            return mOrder;
        }

        public async Task<Response> OrderPayment(RazorResponse mRazorResponse)
        {
            Response mResponse = new Response();
            try
            {
                OrderPayment mOrderPayment = new OrderPayment();
                mOrderPayment.OrderId = mRazorResponse.OrderNo;
                mOrderPayment.RazorPayPaymentId = mRazorResponse.PaymentId;
                mOrderPayment.RazorPayOrderId = mRazorResponse.OrderId;
                mOrderPayment.PaymentStatus = mRazorResponse.isPaid ? (int)Utility.PaymentStatus.Success : (int)Utility.PaymentStatus.Failed;

                UserDialogs.Instance.ShowLoading(Constraints.Loading);
                mResponse = await orderAPI.CreateOrderPayment(mOrderPayment);
                if (mResponse != null && mResponse.Succeeded)
                {
                }
                else
                {
                    if (mResponse != null)
                    {
                        if (!Common.EmptyFiels(mResponse.Message))
                            Common.DisplayErrorMessage(mResponse.Message);
                        else
                        {
                            var jObject = (JObject)mResponse.Data;
                            if (jObject != null)
                            {
                                var mError = jObject.ToObject<ErrorResponse>();
                                if (mError != null)
                                {
                                    Common.DisplayErrorMessage(mError.title);
                                }
                            }
                        }
                    }
                    else
                        Common.DisplayErrorMessage(Constraints.Something_Wrong);
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("OrderRepository/OrderPayment: " + ex.Message);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
            return mResponse;
        }

        /// <summary>
        /// [ Step 2 - Payment Process with orderId ]
        /// </summary>
        /// <param name="mOrder"></param>
        private async void OpenRazorPayQuote(Order mOrder)
        {
            try
            {
                RazorPayload payload = new RazorPayload();
                payload.amount = Convert.ToInt64(mQuote.TotalQuoteAmount * 100);
                payload.currency = (string)App.Current.Resources["Currency"];
                payload.receipt = mOrder.OrderNo; // Order NO

                if (Common.mBuyerDetail != null && !Common.EmptyFiels(Common.mBuyerDetail.Email) && !Common.EmptyFiels(Common.mBuyerDetail.PhoneNumber))
                {
                    payload.email = Common.mBuyerDetail.Email;
                    payload.contact = Common.mBuyerDetail.PhoneNumber;

                    if (DeviceInfo.Platform == DevicePlatform.Android)
                    {
                        MessagingCenter.Send<RazorPayload>(payload, Constraints.RP_PayNow);
                        MessagingCenter.Subscribe<RazorResponse>(this, Constraints.RP_PaidResponse, async (razorResponse) =>
                        {
                            // Make Order
                            MakeOrder(razorResponse, mOrder);
                            MessagingCenter.Unsubscribe<RazorResponse>(this, Constraints.RP_PaidResponse);
                        });
                    }
                    else
                    {
                        RequestPayLoad mPayLoad = new RequestPayLoad()
                        {
                            amount = payload.amount,
                            currency = payload.currency,
                            accept_partial = false,
                            description = mOrder.Title,
                            customer = new Customer()
                            {
                                contact = Common.mBuyerDetail.PhoneNumber,
                                email = Common.mBuyerDetail.Email,
                                name = Common.mBuyerDetail.FullName
                            },
                            callback_method = "get",
                            callback_url = "https://purple-field-04c774300.azurestaticapps.net/login",
                        };
                        RazorPayUtility razorPayUtility = new RazorPayUtility();
                        var urls = await razorPayUtility.PayViaRazor(payload, mPayLoad, Constraints.RP_UserName, Constraints.RP_Password);
                        if (urls != null && urls.Count > 0)
                        {
                            var url = urls.FirstOrDefault();
                            var orderId = urls.LastOrDefault();
                            var checkoutPage = new CheckOutPage(url);
                            checkoutPage.PaidEvent += (s1, e1) =>
                            {
                                MessagingCenter.Unsubscribe<RazorResponse>(this, Constraints.RP_PaidRevealResponse);
                                MessagingCenter.Unsubscribe<RazorResponse>(this, Constraints.RP_PaidResponse);
                                RazorResponse razorResponse = new RazorResponse();
                                var keyValuePairs = (Dictionary<string, string>)s1;
                                if (keyValuePairs != null)
                                {
                                    razorResponse.isPaid = true;
                                    razorResponse.PaymentId = keyValuePairs.Where(x => x.Key == "razorpay_payment_id").FirstOrDefault().Value;
                                    razorResponse.OrderId = keyValuePairs.Where(x => x.Key == "razorpay_payment_link_reference_id").FirstOrDefault().Value;
                                    razorResponse.Signature = keyValuePairs.Where(x => x.Key == "razorpay_signature").FirstOrDefault().Value;
                                }
                                else
                                {
                                    razorResponse.isPaid = false;
                                    razorResponse.OrderId = orderId;
                                }
                                MakeOrder(razorResponse, mOrder);
                            };
                            await Navigation.PushAsync(checkoutPage);
                        }
                    }
                }
                else
                {
                    Common.DisplayErrorMessage(Constraints.Something_Wrong);
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("QuoteDetailsPage/OpenRazorPayQuote: " + ex.Message);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private async void MakeOrder(RazorResponse razorResponse, Order mOrder)
        {
            try
            {
                razorResponse.OrderNo = mOrder.OrderId;
                var mResponse = await OrderPayment(razorResponse);
                if (mResponse != null && mResponse.Succeeded)
                {
                    if (razorResponse != null && razorResponse.isPaid)
                    {
                        Common.DisplaySuccessMessage("Payment Successfully");

                        //await AcceptQuote(mOrder);
                        //if (mResponse != null && mResponse.Succeeded)
                        //{
                        //    if (!Common.EmptyFiels(mResponse.Message))
                        //    {
                        //        Common.DisplaySuccessMessage(mResponse.Message);
                        //    }
                        //}
                        //else
                        //{
                        //    if (mResponse != null)
                        //    {
                        //        if (!Common.EmptyFiels(mResponse.Message))
                        //        {
                        //            Common.DisplayErrorMessage(mResponse.Message);
                        //        }
                        //    }
                        //    else
                        //        Common.DisplayErrorMessage(Constraints.Something_Wrong);
                        //}

                        OnAppearing();
                    }
                    else
                    {
                        string message = "Payment failed ";

                        if (Common.EmptyFiels(razorResponse.OrderId))
                            message += "OrderId: " + razorResponse.OrderId + " ";

                        if (Common.EmptyFiels(razorResponse.PaymentId))
                            message += "PaymentId: " + razorResponse.PaymentId + " ";

                        var contactPopup = new PopupPages.SuccessPopup(message, false);
                        await PopupNavigation.Instance.PushAsync(contactPopup);
                    }
                }
                else
                {
                    if (mResponse != null)
                    {
                        if (!Common.EmptyFiels(mResponse.Message))
                        {
                            Common.DisplayErrorMessage(mResponse.Message);
                        }
                    }
                    else
                        Common.DisplayErrorMessage(Constraints.Something_Wrong);
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("QuoteDetailsPage/MakeOrder: " + ex.Message);
            }
        }

        /// <summary>
        /// [ Step 3 - Update Status of quote ]
        /// </summary>
        /// <returns></returns>
        private async Task AcceptQuote(string orderSuccessMessage)
        {
            try
            {
                UserDialogs.Instance.ShowLoading(Constraints.Loading);
                var mResponse = await quoteAPI.AcceptQuote(QuoteId);
                if (mResponse != null && mResponse.Succeeded)
                {
                    if (Common.EmptyFiels(orderSuccessMessage))
                    {
                        Common.DisplaySuccessMessage(orderSuccessMessage);
                    }
                }
                else
                {
                    if (mResponse != null)
                        Common.DisplayErrorMessage(mResponse.Message);
                    else
                        Common.DisplayErrorMessage(Constraints.Something_Wrong);
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("QuoteDetailsPage/AcceptQuote: " + ex.Message);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }
        #endregion

        private async Task RevealSellerContact(long revealRs)
        {
            try
            {
                RazorPayload payload = new RazorPayload();
                long amount = (long)App.Current.Resources["RevealContact"];
                payload.amount = revealRs * 100;
                payload.currency = (string)App.Current.Resources["Currency"];
                payload.receipt = QuoteId; // quoteid
                payload.email = Common.mBuyerDetail.Email;
                payload.contact = Common.mBuyerDetail.PhoneNumber;

                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    MessagingCenter.Send<RazorPayload>(payload, Constraints.RP_RevealPayNow);
                    MessagingCenter.Subscribe<RazorResponse>(this, Constraints.RP_PaidRevealResponse, async (razorResponse) =>
                     {
                         if (razorResponse != null && !razorResponse.isPaid)
                         {
                             string message = "Payment failed ";

                             if (!Common.EmptyFiels(razorResponse.OrderId))
                                 message += "OrderId: " + razorResponse.OrderId + " ";

                             if (!Common.EmptyFiels(razorResponse.PaymentId))
                                 message += "PaymentId: " + razorResponse.PaymentId + " ";

                             if (message != null)
                                 Common.DisplayErrorMessage(message);
                         }


                         RevealSellerContact mRevealSellerContact = new RevealSellerContact();
                         mRevealSellerContact.QuoteId = QuoteId;
                         mRevealSellerContact.PaymentStatus = razorResponse.isPaid ? (int)RevealContactStatus.Success : (int)RevealContactStatus.Failure;
                         mRevealSellerContact.RazorPayOrderId = razorResponse.OrderId;
                         mRevealSellerContact.RazorPayPaymentId = razorResponse.PaymentId;

                         BtnRevealContact.Text = await DependencyService.Get<IQuoteRepository>().RevealContact(mRevealSellerContact);

                         MessagingCenter.Unsubscribe<RazorResponse>(this, Constraints.RP_PaidRevealResponse);
                     });
                }
                else
                {
                    RequestPayLoad mPayLoad = new RequestPayLoad()
                    {
                        amount = payload.amount,
                        currency = payload.currency,
                        accept_partial = false,
                        description = Constraints.Str_RevealContact,
                        customer = new Customer()
                        {
                            contact = Common.mBuyerDetail.PhoneNumber,
                            email = Common.mBuyerDetail.Email,
                            name = Common.mBuyerDetail.FullName
                        },
                        callback_method = "get",
                        callback_url = "https://purple-field-04c774300.azurestaticapps.net/login",
                    };
                    RazorPayUtility razorPayUtility = new RazorPayUtility();
                    var urls = await razorPayUtility.PayViaRazor(payload, mPayLoad, Constraints.RP_UserName, Constraints.RP_Password);
                    if (urls != null && urls.Count > 0)
                    {
                        var url = urls.FirstOrDefault();
                        var orderId = urls.LastOrDefault();
                        var checkoutPage = new CheckOutPage(url);
                        checkoutPage.PaidEvent += async (s1, e1) =>
                        {
                            MessagingCenter.Unsubscribe<RazorResponse>(this, Constraints.RP_PaidRevealResponse);
                            MessagingCenter.Unsubscribe<RazorResponse>(this, Constraints.RP_PaidResponse);
                            RazorResponse razorResponse = new RazorResponse();
                            var keyValuePairs = (Dictionary<string, string>)s1;
                            if (keyValuePairs != null)
                            {
                                razorResponse.isPaid = true;
                                razorResponse.PaymentId = keyValuePairs.Where(x => x.Key == "razorpay_payment_id").FirstOrDefault().Value;
                                razorResponse.OrderId = keyValuePairs.Where(x => x.Key == "razorpay_payment_link_reference_id").FirstOrDefault().Value;
                                razorResponse.Signature = keyValuePairs.Where(x => x.Key == "razorpay_signature").FirstOrDefault().Value;
                            }
                            else
                            {
                                razorResponse.isPaid = false;
                                razorResponse.OrderId = orderId;
                            }
                            RevealSellerContact mRevealSellerContact = new RevealSellerContact();
                            mRevealSellerContact.QuoteId = QuoteId;
                            mRevealSellerContact.PaymentStatus = razorResponse.isPaid ? (int)RevealContactStatus.Success : (int)RevealContactStatus.Failure;
                            mRevealSellerContact.RazorPayOrderId = razorResponse.OrderId;
                            mRevealSellerContact.RazorPayPaymentId = razorResponse.PaymentId;
                            BtnRevealContact.Text = await DependencyService.Get<IQuoteRepository>().RevealContact(mRevealSellerContact);
                        };
                        await Navigation.PushAsync(checkoutPage);
                    }
                }

            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("QuoteDetailsPage/RevealSellerContact: " + ex.Message);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
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
                    Common.DisplayErrorMessage("QuoteDetailsPage/ImgNotification_Tapped: " + ex.Message);
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

        private async void BtnRevealContact_Clicked(object sender, EventArgs e)
        {
            var Tab = (Button)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    Common.BindAnimation(button: BtnRevealContact);
                    if (BtnRevealContact.Text == Constraints.Str_RevealContact)
                    {
                        await QuotePayment(true);
                    }
                    else
                    {
                        DependencyService.Get<IDialer>().Dial(App.Current.Resources["CountryCode"] + BtnRevealContact.Text);
                    }
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("QuoteDetailsPage/BtnRevealContact_Clicked: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }

        private async void BtnAcceptQuote_Clicked(object sender, EventArgs e)
        {
            var Tab = (Button)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    Common.BindAnimation(button: BtnAcceptQuote);
                    await QuotePayment(false);
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("QuoteDetailsPage/BtnAcceptQuote_Clicked: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }

        }

        private async void BtnBackToQuote_Clicked(object sender, EventArgs e)
        {
            var Tab = (Button)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    Common.BindAnimation(button: BtnBackToQuote);
                    await Navigation.PopAsync();
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("QuoteDetailsPage/BtnBackToQuote_Clicked: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
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
                await GetQuoteDetails();
                rfView.IsRefreshing = false;
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("QuoteDetailsPage/RefreshView_Refreshing: " + ex.Message);
            }
        }

        private void CopyString_Tapped(object sender, EventArgs e)
        {
            try
            {
                var stackLayout = (StackLayout)sender;
                if (!Common.EmptyFiels(stackLayout.ClassId))
                {
                    if (stackLayout.ClassId == "RequirementId")
                    {
                        string message = Constraints.CopiedRequirementId;
                        Common.CopyText(lblRequirementId, message);
                    }
                    else if (stackLayout.ClassId == "QuoteRefNo")
                    {
                        string message = Constraints.CopiedQuoteRefNo;
                        Common.CopyText(lblQuoteRefNo, message);
                    }
                    else if (stackLayout.ClassId == "BuyerId")
                    {
                        string message = Constraints.CopiedBuyerId;
                        Common.CopyText(lblBuyerId, message);
                    }
                    else if (stackLayout.ClassId == "SellerId")
                    {
                        string message = Constraints.CopiedSellerId;
                        Common.CopyText(lblSellerId, message);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("QuoteDetailsPage/CopyString_Tapped: " + ex.Message);
            }
        }
        #endregion
    }
}
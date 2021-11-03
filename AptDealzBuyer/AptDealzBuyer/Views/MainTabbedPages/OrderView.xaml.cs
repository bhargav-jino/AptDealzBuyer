using Acr.UserDialogs;
using AptDealzBuyer.API;
using AptDealzBuyer.Model.Reponse;
using AptDealzBuyer.Repository;
using AptDealzBuyer.Utility;
using AptDealzBuyer.Views.PopupPages;
using Newtonsoft.Json.Linq;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AptDealzBuyer.Views.MainTabbedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrderView : ContentView
    {
        #region [ Objects ]        
        public List<Order> mOrders;
        private string filterBy = SortByField.Date.ToString();
        private string title = string.Empty;
        private int? statusBy = null;
        private bool? isAssending = false;
        private bool isGrievance = false;
        private readonly int pageSize = 10;
        private int pageNo;
        #endregion

        #region [ Constructor ]
        public OrderView(bool isGrievance = false)
        {
            try
            {
                InitializeComponent();
                mOrders = new List<Order>();
                pageNo = 1;
                this.isGrievance = isGrievance;
                GetOrders(statusBy, title, filterBy, isAssending);

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
                Common.DisplayErrorMessage("OrderView/Ctor: " + ex.Message);
            }
        }
        #endregion

        #region [ Methods ]
        private async void GetOrders(int? StatusBy = null, string Title = "", string FilterBy = "", bool? SortBy = null, bool isLoader = true)
        {
            try
            {
                OrderAPI orderAPI = new OrderAPI();
                if (isLoader)
                {
                    UserDialogs.Instance.ShowLoading(Constraints.Loading);
                }
                var mResponse = await orderAPI.GetOrdersForBuyer(StatusBy, Title, FilterBy, SortBy, pageNo, pageSize);
                if (mResponse != null && mResponse.Succeeded)
                {
                    JArray result = (JArray)mResponse.Data;
                    var orders = result.ToObject<List<Order>>();
                    if (pageNo == 1)
                    {
                        mOrders.Clear();
                    }

                    foreach (var mOrder in orders)
                    {
                        if (isGrievance)
                        {
                            lblHeader.Text = "Raise Grievances";

                            mOrder.IsSelectGrievance = true;
                            mOrder.OrderActionVisibility = false;
                            mOrder.OrderTrackVisibility = false;
                        }
                        else
                        {
                            mOrder.IsSelectGrievance = false;
                            if (Common.EmptyFiels(mOrder.OrderAction))
                                mOrder.OrderActionVisibility = false;
                            else
                                mOrder.OrderActionVisibility = true;

                            if (mOrder.OrderStatus >= (int)OrderStatus.ReadyForPickup && !Common.EmptyFiels(mOrder.TrackingLink))
                                mOrder.OrderTrackVisibility = true;
                            else
                                mOrder.OrderTrackVisibility = false;
                        }

                        if (mOrders.Where(x => x.OrderId == mOrder.OrderId).Count() == 0)
                            mOrders.Add(mOrder);
                    }
                    BindList(mOrders);
                }
                else
                {
                    lstOrders.IsVisible = false;
                    lblNoRecord.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("OrderView/GetOrders: " + ex.Message);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private void BindList(List<Order> mOrderList)
        {
            try
            {
                if (mOrderList != null && mOrderList.Count > 0)
                {
                    lstOrders.IsVisible = true;
                    lblNoRecord.IsVisible = false;
                    lstOrders.ItemsSource = mOrderList.ToList();
                }
                else
                {
                    lstOrders.IsVisible = false;
                    lblNoRecord.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("OrderView/BindList: " + ex.Message);
            }
        }

        private async Task ShowQRCodeImage(string OrderId)
        {
            try
            {
                string imageBase64 = await DependencyService.Get<IOrderRepository>().GenerateQRCodeImage(OrderId);
                if (!Common.EmptyFiels(imageBase64))
                {
                    //ImgQRCode.Source = Xamarin.Forms.ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(imageBase64)));
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("OrderDetailsPage/ShowQRCodeImage: " + ex.Message);
            }
        }
        #endregion

        #region [ Events ]

        #region [ Header Navigation ]
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
                    Common.DisplayErrorMessage("OrderView/ImgNotification_Tapped: " + ex.Message);
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
            try
            {
                Common.BindAnimation(imageButton: ImgBack);
                if (isGrievance)
                    await Navigation.PopAsync();
                else
                    Common.MasterData.Detail = new NavigationPage(new MainTabbedPages.MainTabbedPage(Constraints.Str_Home));
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("OrderView/ImgBack_Tapped: " + ex.Message);
            }
        }

        private void BtnLogo_Clicked(object sender, EventArgs e)
        {
            Common.MasterData.Detail = new NavigationPage(new MainTabbedPages.MainTabbedPage(Constraints.Str_Home));
        }
        #endregion

        #region [ Filteration ]
        private async void FrmStatus_Tapped(object sender, EventArgs e)
        {
            var Tab = (Frame)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    var statusPopup = new OrderStatusPopup(statusBy);
                    statusPopup.isRefresh += (s1, e1) =>
                    {
                        string result = s1.ToString();
                        if (!Common.EmptyFiels(result))
                        {
                            lblStatus.Text = result.ToCamelCase();
                            statusBy = Common.GetOrderStatus(result);
                            pageNo = 1;
                            mOrders.Clear();
                            GetOrders(statusBy, title, filterBy, isAssending);
                        }
                    };
                    await PopupNavigation.Instance.PushAsync(statusPopup);
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("OrderView/FrmStatusBy_Tapped: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }

        private void FrmSortBy_Tapped(object sender, EventArgs e)
        {
            try
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

                pageNo = 1;
                mOrders.Clear();
                GetOrders(statusBy, title, filterBy, isAssending);
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("OrderView/FrmSortBy_Tapped: " + ex.Message);
            }
        }

        private async void FrmFilterBy_Tapped(object sender, EventArgs e)
        {
            var Tab = (Frame)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    var sortby = new FilterPopup(filterBy, Constraints.Str_Order);
                    sortby.isRefresh += (s1, e1) =>
                    {
                        string result = s1.ToString();
                        if (!Common.EmptyFiels(result))
                        {
                            filterBy = result;
                            lblFilterBy.Text = filterBy;
                            pageNo = 1;
                            mOrders.Clear();
                            GetOrders(statusBy, title, filterBy, isAssending);
                        }
                    };
                    await PopupNavigation.Instance.PushAsync(sortby);
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("OrderView/FrmFilter_Tapped: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }

        private void entrSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                pageNo = 1;
                if (!Common.EmptyFiels(entrSearch.Text))
                {
                    GetOrders(statusBy, entrSearch.Text, filterBy, isAssending, false);
                }
                else
                {
                    pageNo = 1;
                    mOrders.Clear();
                    GetOrders(statusBy, title, filterBy, isAssending);
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("OrderView/entrSearch_TextChanged: " + ex.Message);
            }

        }

        private void BtnClose_Clicked(object sender, EventArgs e)
        {
            entrSearch.Text = string.Empty;
            BindList(mOrders);
        }
        #endregion

        private async void GrdViewOrderDetails_Tapped(object sender, EventArgs e)
        {
            var GridExp = (Grid)sender;
            if (GridExp.IsEnabled)
            {
                try
                {
                    GridExp.IsEnabled = false;
                    var mOrder = GridExp.BindingContext as Order;
                    if (mOrder != null)
                    {
                        await Navigation.PushAsync(new Orders.OrderDetailsPage(mOrder.OrderId));
                    }
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("OrderView/GrdViewOrderDetails: " + ex.Message);
                }
                finally
                {
                    GridExp.IsEnabled = true;
                }
            }
        }

        private async void BtnOrderAction_Tapped(object sender, EventArgs e)
        {
            var ButtonExp = (Button)sender;
            var order = ButtonExp.BindingContext as Order;

            if (ButtonExp.IsEnabled)
            {
                try
                {
                    ButtonExp.IsEnabled = false;
                    var mOrder = ButtonExp.BindingContext as Order;
                    if (mOrder.OrderAction == "Repost")
                    {
                        await Navigation.PushAsync(new DashboardPages.PostNewRequiremntPage(mOrder.RequirementId));
                    }
                    else if (mOrder.OrderAction == "Show QR Code")
                    {
                        if (order != null && !Common.EmptyFiels(order.OrderId))
                        {
                            //await ShowQRCodeImage(order.OrderId);
                            await Navigation.PushAsync(new DashboardPages.QRCodePage(mOrder.OrderId));
                        }
                    }
                    else
                    {
                        //perform Action
                    }
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("OrderView/BtnOrderAction_Tapped: " + ex.Message);
                }
                finally
                {
                    ButtonExp.IsEnabled = true;
                }
            }


        }

        private void BtnTrack_Tapped(object sender, EventArgs e)
        {
            var Tab = (Button)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    var mOrder = Tab.BindingContext as Order;
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
                    Common.DisplayErrorMessage("OrderView/BtnTrack_Tapped: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }

        private void lstOrders_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            try
            {
                if (this.mOrders.Count < 10)
                    return;
                if (this.mOrders.Count == 0)
                    return;

                var lastrequirement = this.mOrders[this.mOrders.Count - 1];
                var lastAppearing = (Order)e.Item;
                if (lastAppearing != null)
                {
                    if (lastrequirement == lastAppearing)
                    {
                        var totalAspectedRow = pageSize * pageNo;
                        pageNo += 1;

                        if (this.mOrders.Count() >= totalAspectedRow)
                        {
                            GetOrders(statusBy, title, filterBy, isAssending, false);
                        }
                    }
                    else
                    {
                        UserDialogs.Instance.HideLoading();
                    }
                }
                else
                {
                    UserDialogs.Instance.HideLoading();
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("OrderView/ItemAppearing: " + ex.Message);
                UserDialogs.Instance.HideLoading();
            }
        }

        private void lstOrders_Refreshing(object sender, EventArgs e)
        {
            try
            {
                lstOrders.IsRefreshing = true;
                pageNo = 1;
                mOrders.Clear();
                GetOrders(statusBy, title, filterBy, isAssending);
                lstOrders.IsRefreshing = false;
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("OrderView/Refreshing: " + ex.Message);
            }
        }

        private async void BtnSelect_Tapped(object sender, EventArgs e)
        {
            var ButtonExp = (Button)sender;
            if (ButtonExp.IsEnabled)
            {
                try
                {
                    ButtonExp.IsEnabled = false;
                    var mOrder = ButtonExp.BindingContext as Order;
                    await Navigation.PushAsync(new Orders.RaiseGrievancePage(mOrder.OrderId));
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("OrderView/BtnSelect_Tapped: " + ex.Message);
                }
                finally
                {
                    ButtonExp.IsEnabled = true;
                }
            }
        }
        #endregion

        private void lstOrders_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            lstOrders.SelectedItem = null;
        }
    }
}
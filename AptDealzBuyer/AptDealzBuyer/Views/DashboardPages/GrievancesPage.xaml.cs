using Acr.UserDialogs;
using AptDealzBuyer.API;
using AptDealzBuyer.Model.Request;
using AptDealzBuyer.Utility;
using AptDealzBuyer.Views.PopupPages;
using Newtonsoft.Json.Linq;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AptDealzBuyer.Views.DashboardPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GrievancesPage : ContentPage
    {
        #region [ Objects ]
        public List<Grievance> mGrievance;
        private string filterBy = SortByField.Date.ToString();
        private string title = string.Empty;
        private int? statusBy = null;
        private bool? isAssending = false;
        private readonly int pageSize = 10;
        private int pageNo;
        #endregion

        #region [ Constructor ]
        public GrievancesPage()
        {
            try
            {
                InitializeComponent();
                mGrievance = new List<Grievance>();
                pageNo = 1;
                GetGrievance(statusBy, title, filterBy, isAssending);

                MessagingCenter.Unsubscribe<string>(this, Constraints.Str_NotificationCount);
                MessagingCenter.Subscribe<string>(this, Constraints.Str_NotificationCount, (count) =>
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
                Common.DisplayErrorMessage("GrievancesPage/Ctor: " + ex.Message);
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

        private async void GetGrievance(int? StatusBy = null, string Title = "", string FilterBy = "", bool? SortBy = null, bool isLoader = true)
        {
            try
            {
                GrievanceAPI grievanceAPI = new GrievanceAPI();
                if (isLoader)
                {
                    UserDialogs.Instance.ShowLoading(Constraints.Loading);
                }
                var mResponse = await grievanceAPI.GetAllGrievances(StatusBy, Title, FilterBy, SortBy, pageNo, pageSize);
                if (mResponse != null && mResponse.Succeeded)
                {
                    JArray result = (JArray)mResponse.Data;
                    var orders = result.ToObject<List<Grievance>>();
                    if (pageNo == 1)
                    {
                        mGrievance.Clear();
                    }

                    foreach (var mGrievances in orders)
                    {
                        if (mGrievance.Where(x => x.GrievanceId == mGrievances.GrievanceId).Count() == 0)
                            mGrievance.Add(mGrievances);
                    }
                    BindList(mGrievance);
                }
                else
                {
                    lstGrievance.IsVisible = false;
                    lblNoRecord.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("GrievancesPage/GetGrievance: " + ex.Message);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private void BindList(List<Grievance> mGrievanceList)
        {
            try
            {
                if (mGrievanceList != null && mGrievanceList.Count > 0)
                {
                    lstGrievance.IsVisible = true;
                    lblNoRecord.IsVisible = false;
                    lstGrievance.ItemsSource = mGrievanceList.ToList();
                }
                else
                {
                    lstGrievance.IsVisible = false;
                    lblNoRecord.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("GrievancesPage/BindList: " + ex.Message);
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
                    Common.DisplayErrorMessage("GrievancesPage/ImgNotification_Tapped: " + ex.Message);
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
        #endregion

        #region [ Filtering ]
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
                mGrievance.Clear();
                GetGrievance(statusBy, title, filterBy, isAssending);
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("GrievancesPage/FrmSortBy_Tapped: " + ex.Message);
            }
        }

        private async void FrmStatus_Tapped(object sender, EventArgs e)
        {
            var Tab = (Frame)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    var statusPopup = new StatusPopup(statusBy);
                    statusPopup.isRefresh += (s1, e1) =>
                    {
                        string result = s1.ToString();
                        if (!Common.EmptyFiels(result))
                        {
                            lblStatus.Text = result;
                            statusBy = Common.GetGrievanceStatus(result);
                            pageNo = 1;
                            mGrievance.Clear();
                            GetGrievance(statusBy, title, filterBy, isAssending);
                        }
                    };
                    await PopupNavigation.Instance.PushAsync(statusPopup);
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("GrievancesPage/FrmStatusBy_Tapped: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
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
                    var sortby = new FilterPopup(filterBy, Constraints.Str_Grievances);
                    sortby.isRefresh += (s1, e1) =>
                    {
                        string result = s1.ToString();
                        if (!Common.EmptyFiels(result))
                        {
                            filterBy = result;
                            lblFilterBy.Text = result;
                            pageNo = 1;
                            mGrievance.Clear();
                            GetGrievance(statusBy, title, filterBy, isAssending);
                        }
                    };
                    await PopupNavigation.Instance.PushAsync(sortby);
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("GrievancesPage/FrmFilterBy_Tapped: " + ex.Message);
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
                    GetGrievance(statusBy, entrSearch.Text, filterBy, isAssending, false);
                }
                else
                {
                    pageNo = 1;
                    mGrievance.Clear();
                    GetGrievance(statusBy, title, filterBy, isAssending);
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("GrievancesPage/entrSearch_TextChanged: " + ex.Message);
            }

        }

        private void BtnClose_Clicked(object sender, EventArgs e)
        {
            entrSearch.Text = string.Empty;
            BindList(mGrievance);
        }
        #endregion

        private async void FrmAdd_Tapped(object sender, EventArgs e)
        {
            var Tab = (Frame)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    await Navigation.PushAsync(new MainTabbedPages.MainTabbedPage(Constraints.Str_RaiseGrievances, isNavigate: true));
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("GrievancesPage/FrmAdd_Tapped: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }

        #region [ Listing ]
        private async void GrdViewGrievances_Tapped(object sender, EventArgs e)
        {
            var Tab = (Grid)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    var mGrievance = Tab.BindingContext as Grievance;
                    await Navigation.PushAsync(new GrievanceDetailsPage(mGrievance.GrievanceId));
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("GrievancesPage/GrdViewGrievances_Tapped: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }

        private void lstGrievance_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            try
            {
                if (this.mGrievance.Count < 10)
                    return;
                if (this.mGrievance.Count == 0)
                    return;

                var lastrequirement = this.mGrievance[this.mGrievance.Count - 1];
                var lastAppearing = (Grievance)e.Item;
                if (lastAppearing != null)
                {
                    if (lastrequirement == lastAppearing)
                    {
                        var totalAspectedRow = pageSize * pageNo;
                        pageNo += 1;

                        if (this.mGrievance.Count() >= totalAspectedRow)
                        {
                            GetGrievance(statusBy, title, filterBy, isAssending, false);
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
                Common.DisplayErrorMessage("GrievancesPage/ItemAppearing: " + ex.Message);
                UserDialogs.Instance.HideLoading();
            }
        }

        private void lstGrievance_Refreshing(object sender, EventArgs e)
        {
            try
            {
                lstGrievance.IsRefreshing = true;
                pageNo = 1;
                mGrievance.Clear();
                GetGrievance(statusBy, title, filterBy, isAssending);
                lstGrievance.IsRefreshing = false;
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("GrievancesPage/Refreshing: " + ex.Message);
            }
        }

        private void lstGrievance_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            lstGrievance.SelectedItem = null;
        }
        #endregion

        #endregion
    }
}
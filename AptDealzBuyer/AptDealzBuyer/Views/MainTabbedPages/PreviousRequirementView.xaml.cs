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

namespace AptDealzBuyer.Views.MainTabbedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PreviousRequirementView : ContentView
    {
        #region [ Objects ]        
        private List<Requirement> mRequirements;
        private string filterBy = SortByField.Date.ToString();
        private string title = string.Empty;
        private bool? isAscending = false;
        private readonly int pageSize = 10;
        private int pageNo;
        #endregion

        #region [ Constructor ]
        public PreviousRequirementView()
        {
            try
            {
                InitializeComponent();
                mRequirements = new List<Requirement>();
                pageNo = 1;
                GetPreviousRequirements(filterBy, title, isAscending);

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
                Common.DisplayErrorMessage("PreviousRequirementView/Ctor: " + ex.Message);
            }
        }
        #endregion

        #region [ Methods ]
        private async void GetPreviousRequirements(string FilterBy = "", string Title = "", bool? SortBy = null, bool isLoader = true)
        {
            try
            {
                RequirementAPI requirementAPI = new RequirementAPI();
                if (isLoader)
                {
                    UserDialogs.Instance.ShowLoading(Constraints.Loading);
                }

                var mResponse = await requirementAPI.GetMyPreviousRequirements(FilterBy, Title, SortBy, pageNo, pageSize);
                if (mResponse != null && mResponse.Succeeded)
                {
                    JArray result = (JArray)mResponse.Data;
                    var requirements = result.ToObject<List<Requirement>>();
                    if (pageNo == 1)
                    {
                        mRequirements.Clear();
                    }

                    foreach (var mRequirement in requirements)
                    {
                        if (mRequirements.Where(x => x.RequirementId == mRequirement.RequirementId).Count() == 0)
                            mRequirements.Add(mRequirement);
                    }
                    BindList(mRequirements);
                }
                else
                {
                    lstRequirements.IsVisible = false;
                    lblNoRecord.IsVisible = true;
                    //if (mResponse != null && mResponse.Message != null)
                    //{
                    //    lblNoRecord.Text = mResponse.Message;
                    //}
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("PreviousRequirementView/GetRequirements: " + ex.Message);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private void BindList(List<Requirement> mRequirementList)
        {
            try
            {
                if (mRequirementList != null && mRequirementList.Count > 0)
                {
                    lstRequirements.IsVisible = true;
                    lblNoRecord.IsVisible = false;
                    lstRequirements.ItemsSource = mRequirementList.ToList();
                }
                else
                {
                    lstRequirements.IsVisible = false;
                    lblNoRecord.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("PreviousRequirementView/BindList: " + ex.Message);
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
                    Common.DisplayErrorMessage("PreviousRequirementView/ImgNotification_Tapped: " + ex.Message);
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

        private void FrmSortBy_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (ImgSort.Source.ToString().Replace("File: ", "") == Constraints.Img_SortASC)
                {
                    ImgSort.Source = Constraints.Img_SortDSC;
                    isAscending = false;
                }
                else
                {
                    ImgSort.Source = Constraints.Img_SortASC;
                    isAscending = true;
                }

                pageNo = 1;
                mRequirements.Clear();
                GetPreviousRequirements(filterBy, title, isAscending);
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("PreviousRequirementView/FrmSortBy_Tapped: " + ex.Message);
            }
        }

        private void ImgExpand_Tapped(object sender, EventArgs e)
        {
            try
            {
                var imgExp = (ImageButton)sender;
                var viewCell = (ViewCell)imgExp.Parent.Parent.Parent;
                if (viewCell != null)
                {
                    viewCell.ForceUpdateSize();
                }
                var mRequirement = imgExp.BindingContext as Requirement;
                if (mRequirement != null && mRequirement.ArrowImage == Constraints.Img_ArrowRight)
                {
                    mRequirement.ArrowImage = Constraints.Img_ArrowDown;
                    mRequirement.GridBg = (Color)App.Current.Resources["appColor8"];
                    mRequirement.MoreDetail = true;
                }
                else
                {
                    mRequirement.ArrowImage = Constraints.Img_ArrowRight;
                    mRequirement.GridBg = Color.Transparent;
                    mRequirement.MoreDetail = false;
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("PreviousRequirementView/ImgExpand_Tapped: " + ex.Message);
            }
        }

        private async void GrdViewPrevRequirement_Tapped(object sender, EventArgs e)
        {
            var GridExp = (Grid)sender;
            if (GridExp.IsEnabled)
            {
                try
                {
                    GridExp.IsEnabled = false;
                    var mRequirement = GridExp.BindingContext as Requirement;
                    await Navigation.PushAsync(new DashboardPages.ViewRequirememntPage(mRequirement.RequirementId, "previous"));
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("PreviousRequirementView/GrdViewPrevRequirement_Tapped: " + ex.Message);
                }
                finally
                {
                    GridExp.IsEnabled = true;
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
                    GetPreviousRequirements(filterBy, entrSearch.Text, isAscending, false);
                }
                else
                {
                    pageNo = 1;
                    mRequirements.Clear();
                    GetPreviousRequirements(filterBy, title, isAscending);
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("PreviousRequirementView/entrSearch_TextChanged: " + ex.Message);
            }
        }

        private void BtnClose_Clicked(object sender, EventArgs e)
        {
            entrSearch.Text = string.Empty;
            BindList(mRequirements);
        }

        private void lstRequirements_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            try
            {
                if (this.mRequirements.Count < 10)
                    return;
                if (this.mRequirements.Count == 0)
                    return;

                var lastrequirement = this.mRequirements[this.mRequirements.Count - 1];
                var lastAppearing = (Requirement)e.Item;
                if (lastAppearing != null)
                {
                    if (lastrequirement == lastAppearing)
                    {
                        var totalAspectedRow = pageSize * pageNo;
                        pageNo += 1;

                        if (this.mRequirements.Count() >= totalAspectedRow)
                        {
                            GetPreviousRequirements(filterBy, title, isAscending, false);
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
                Common.DisplayErrorMessage("PreviousRequirementView/ItemAppearing: " + ex.Message);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private void lstRequirements_Refreshing(object sender, EventArgs e)
        {
            try
            {
                lstRequirements.IsRefreshing = true;
                pageNo = 1;
                mRequirements.Clear();
                GetPreviousRequirements(filterBy, title, isAscending);
                lstRequirements.IsRefreshing = false;
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("PreviousRequirementView/Refreshing: " + ex.Message);
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
                    var sortby = new FilterPopup(filterBy, Constraints.Str_Active);
                    sortby.isRefresh += (s1, e1) =>
                    {
                        string result = s1.ToString();
                        if (!Common.EmptyFiels(result))
                        {
                            filterBy = result;
                            if (filterBy == SortByField.ID.ToString())
                            {
                                lblFilterBy.Text = filterBy;
                            }
                            else
                            {
                                lblFilterBy.Text = filterBy.ToCamelCase();
                            }
                            pageNo = 1;
                            mRequirements.Clear();
                            GetPreviousRequirements(filterBy, title, isAscending);
                        }
                    };
                    await PopupNavigation.Instance.PushAsync(sortby);
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("PreviousRequirementView/FrmFilterBy_Tapped: " + ex.Message);
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

        private void lstRequirements_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            lstRequirements.SelectedItem = null;
        }
        #endregion
    }
}
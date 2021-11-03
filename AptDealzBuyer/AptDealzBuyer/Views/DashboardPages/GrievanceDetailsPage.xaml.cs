using Acr.UserDialogs;
using AptDealzBuyer.Interfaces;
using AptDealzBuyer.Model.Request;
using AptDealzBuyer.Repository;
using AptDealzBuyer.Utility;
using Rg.Plugins.Popup.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AptDealzBuyer.Views.DashboardPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GrievanceDetailsPage : ContentPage
    {
        #region [ Objects ]
        private Grievance mGrievance;
        private string GrievanceId = string.Empty;
        #endregion

        #region [ Constructor ]
        public GrievanceDetailsPage(string GrievanceId)
        {
            try
            {
                InitializeComponent();
                this.GrievanceId = GrievanceId;

                if (DeviceInfo.Platform == DevicePlatform.Android)
                    txtMessage.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);

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
                Common.DisplayErrorMessage("GrievanceDetailsPage/Ctor: " + ex.Message);
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
            GetGrievancesDetails();
        }

        private async Task GetGrievancesDetails()
        {
            try
            {
                mGrievance = await DependencyService.Get<IGrievanceRepository>().GetGrievancesDetails(GrievanceId);
                if (mGrievance != null)
                {
                    lblGrievanceId.Text = mGrievance.GrievanceNo;
                    lblOrderId.Text = mGrievance.OrderNo;
                    lblOrderDate.Text = mGrievance.OrderDate.ToString(Constraints.Str_DateFormate);
                    lblGrievanceDate.Text = mGrievance.Created.ToString(Constraints.Str_DateFormate);
                    lblSellerName.Text = mGrievance.SellerName;

                    string GrievanceTypeDescr = "";
                    if (mGrievance.GrievanceType > -1)
                    {
                        switch (mGrievance.GrievanceType)
                        {
                            case (int)GrievancesType.Order_Related:
                                GrievanceTypeDescr = GrievancesType.Order_Related.ToString().Replace("_", " ");
                                break;
                            case (int)GrievancesType.Delayed_Delivery:
                                GrievanceTypeDescr = GrievancesType.Delayed_Delivery.ToString().Replace("_", " ");
                                break;
                            case (int)GrievancesType.Payment_Related:
                                GrievanceTypeDescr = GrievancesType.Payment_Related.ToString().Replace("_", " ");
                                break;
                            case (int)GrievancesType.Manufacture_Defect:
                                GrievanceTypeDescr = GrievancesType.Manufacture_Defect.ToString().Replace("_", " ");
                                break;
                            case (int)GrievancesType.Incomplete_Product_Delivery:
                                GrievanceTypeDescr = GrievancesType.Incomplete_Product_Delivery.ToString().Replace("_", " ");
                                break;
                            case (int)GrievancesType.Wrong_Order:
                                GrievanceTypeDescr = GrievancesType.Wrong_Order.ToString().Replace("_", " ");
                                break;
                            default:
                                GrievanceTypeDescr = GrievancesType.Order_Related.ToString().Replace("_", " ");
                                break;
                        }
                    }

                    lblGrievanceType.Text = GrievanceTypeDescr.ToCamelCase();
                    lblStatus.Text = mGrievance.StatusDescr.ToCamelCase();
                    lblSolution.Text = mGrievance.PreferredSolution;

                    if (Common.EmptyFiels(mGrievance.IssueDescription))
                    {
                        lblDescription.Text = "No description found";
                    }
                    else
                    {
                        lblDescription.Text = mGrievance.IssueDescription;
                    }

                    if (mGrievance.GrievanceResponses != null && mGrievance.GrievanceResponses.Count > 0)
                    {
                        foreach (var grievanceResponses in mGrievance.GrievanceResponses)
                        {
                            if (grievanceResponses.ResponseFromUserId != Settings.UserId)
                            {
                                //User Data
                                grievanceResponses.IsContact = false;
                                grievanceResponses.IsUser = true;
                            }

                            string baseURL = (string)App.Current.Resources["BaseURL"];
                            grievanceResponses.ResponseFromUserProfileImage = baseURL + grievanceResponses.ResponseFromUserProfileImage.Replace(baseURL, "");
                        }
                        lstResponse.IsVisible = true;
                        lblNoRecord.IsVisible = false;
                        lstResponse.ItemsSource = mGrievance.GrievanceResponses.ToList();
                        lstResponse.HeightRequest = mGrievance.GrievanceResponses.Count * 95; //50+10+10+20+extra 5
                    }
                    else
                    {
                        lstResponse.ItemsSource = null;
                        lstResponse.IsVisible = false;
                        lblNoRecord.IsVisible = true;
                    }

                    AttachDocumentList();
                    GrdMessage.IsVisible = mGrievance.EnableResponseFromUser;
                    if (mGrievance.EnableResponseFromUser)
                    {
                        if (mGrievance.Status == (int)GrievancesStatus.Closed)
                        {
                            GrdMessage.IsVisible = false;
                        }
                        else
                        {
                            GrdMessage.IsVisible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("GrievanceDetailsPage/GetGrievancesDetails: " + ex.Message);
            }
        }

        private async Task SubmitGrievanceResponse()
        {
            try
            {
                if (!Common.EmptyFiels(txtMessage.Text))
                {
                    await DependencyService.Get<IGrievanceRepository>().SubmitGrievanceResponse(GrievanceId, txtMessage.Text);
                    txtMessage.Text = string.Empty;
                    await GetGrievancesDetails();
                }
                else
                {
                    BoxMessage.BackgroundColor = (Color)App.Current.Resources["appColor3"];
                    Common.DisplayErrorMessage(Constraints.Required_Response);
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("GrievanceDetailsPage/SubmitGrievanceResponse: " + ex.Message);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }

        }

        private void AttachDocumentList()
        {
            try
            {
                if (mGrievance.Documents != null && mGrievance.Documents.Count > 0)
                {
                    lblAttachDocument.IsVisible = false;
                    lstDocument.ItemsSource = mGrievance.Documents.ToList();
                    lstDocument.IsVisible = true;
                }
                else
                {
                    lblAttachDocument.IsVisible = true;
                    lstDocument.ItemsSource = null;
                    lstDocument.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("GrievanceDetailsPage/AttachDocumentList: " + ex.Message);
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
                    Common.DisplayErrorMessage("GrievanceDetailsPage/ImgNotification_Tapped: " + ex.Message);
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

        private async void BtnSubmit_Tapped(object sender, EventArgs e)
        {
            var Tab = (Button)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    Common.BindAnimation(button: BtnSubmit);
                    await SubmitGrievanceResponse();
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("GrievanceDetailsPage/BtnSubmit_Tapped: " + ex.Message);
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
            rfView.IsRefreshing = true;
            await GetGrievancesDetails();
            rfView.IsRefreshing = false;
        }

        private void TxtMessage_Unfocused(object sender, FocusEventArgs e)
        {
            if (!Common.EmptyFiels(txtMessage.Text))
            {
                BoxMessage.BackgroundColor = (Color)App.Current.Resources["appColor8"];
            }
        }

        private void CopyString_Tapped(object sender, EventArgs e)
        {
            try
            {
                var stackLayout = (StackLayout)sender;
                if (!Common.EmptyFiels(stackLayout.ClassId))
                {
                    if (stackLayout.ClassId == "GrievanceId")
                    {
                        string message = Constraints.CopiedGrievanceId;
                        Common.CopyText(lblGrievanceId, message);
                    }
                    else if (stackLayout.ClassId == "OrderId")
                    {
                        string message = Constraints.CopiedOrderId;
                        Common.CopyText(lblOrderId, message);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("GrievanceDetailsPage/CopyString_Tapped: " + ex.Message);
            }
        }

        private void lstResponse_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            lstResponse.SelectedItem = null;
        }

        private async void ImgDocument_Clicked(object sender, EventArgs e)
        {
            var imgButton = (ImageButton)sender;
            if (imgButton.IsEnabled)
            {
                try
                {
                    imgButton.IsEnabled = false;
                    var url = imgButton.BindingContext as string;
                    await GenerateWebView.GenerateView(url);
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("GrievancesPage/ImgDocument_Clicked: " + ex.Message);
                }
                finally
                {
                    imgButton.IsEnabled = true;
                }
            }
        }
        #endregion
    }
}
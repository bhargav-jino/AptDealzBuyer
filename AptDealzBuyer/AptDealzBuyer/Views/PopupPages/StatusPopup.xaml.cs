using AptDealzBuyer.Utility;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using Xamarin.Forms.Xaml;

namespace AptDealzBuyer.Views.PopupPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatusPopup : PopupPage
    {
        #region [ Objects ]
        public event EventHandler isRefresh;
        #endregion

        #region Constructor
        public StatusPopup(int? StatusBy)
        {
            InitializeComponent();
            BindSource(StatusBy);
        }
        #endregion

        #region Methods
        protected override void OnAppearing()
        {
            base.OnAppearing();
            BindLabel();
        }

        protected override bool OnBackgroundClicked()
        {
            base.OnBackgroundClicked();
            return false;
        }

        private void BindLabel()
        {
            try
            {
                lblFirstType.Text = GrievancesStatus.Pending.ToString();
                lblSecondType.Text = GrievancesStatus.Open.ToString();
                lblThirdType.Text = GrievancesStatus.Closed.ToString();
                lblFourType.Text = GrievancesStatus.All.ToString();
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("StatusPopup/BindLabel: " + ex.Message);
            }
        }

        private void BindSource(int? viewSource)
        {
            try
            {
                if (viewSource != null)
                {
                    if (viewSource == (int)GrievancesStatus.Pending)
                    {
                        ClearSource();
                        imgFirstType.Source = Constraints.Img_RadioSelected;
                    }
                    else if (viewSource == (int)GrievancesStatus.Open)
                    {
                        ClearSource();
                        imgSecondType.Source = Constraints.Img_RadioSelected;
                    }
                    else if (viewSource == (int)GrievancesStatus.Closed)
                    {
                        ClearSource();
                        imgThirdType.Source = Constraints.Img_RadioSelected;
                    }
                    else if (viewSource == (int)GrievancesStatus.All)
                    {
                        ClearSource();
                        imgFourType.Source = Constraints.Img_RadioSelected;
                    }
                    else
                    {
                        ClearSource();
                        imgFourType.Source = Constraints.Img_RadioSelected;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("StatusPopup/BindSource: " + ex.Message);
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

        #region Events
        private void StkFirstType_Tapped(object sender, EventArgs e)
        {
            try
            {
                BindSource((int)GrievancesStatus.Pending);
                isRefresh?.Invoke(GrievancesStatus.Pending.ToString(), null);
                PopupNavigation.Instance.PopAsync();
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("StatusPopup/StkFirstType_Tapped: " + ex.Message);
            }
        }

        private void StkSecondType_Tapped(object sender, EventArgs e)
        {
            try
            {
                BindSource((int)GrievancesStatus.Open);
                isRefresh?.Invoke(GrievancesStatus.Open.ToString(), null);
                PopupNavigation.Instance.PopAsync();
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("StatusPopup/StkSecondType_Tapped: " + ex.Message);
            }
        }

        private void StkThirdType_Tapped(object sender, EventArgs e)
        {
            try
            {
                BindSource((int)GrievancesStatus.Closed);
                isRefresh?.Invoke(GrievancesStatus.Closed.ToString(), null);
                PopupNavigation.Instance.PopAsync();
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("StatusPopup/StkThirdType_Tapped: " + ex.Message);
            }
        }

        private void StkFourType_Tapped(object sender, EventArgs e)
        {
            try
            {
                BindSource((int)GrievancesStatus.All);
                isRefresh?.Invoke(GrievancesStatus.All.ToString(), null);
                PopupNavigation.Instance.PopAsync();
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("StatusPopup/StkFourType_Tapped: " + ex.Message);
            }
        }
        #endregion
    }
}
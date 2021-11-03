using Acr.UserDialogs;
using AptDealzBuyer.API;
using AptDealzBuyer.Extention;
using AptDealzBuyer.Model.Reponse;
using AptDealzBuyer.Model.Request;
using AptDealzBuyer.Repository;
using AptDealzBuyer.Utility;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AptDealzBuyer.Views.DashboardPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PostNewRequiremntPage : ContentPage
    {
        #region [ Objects ]     
        private BuyerDetails mBuyerDetail;
        private List<Category> mCategories;
        private List<SubCategory> mSubCategories;
        private List<string> selectedSubCategory;

        private string relativePath = string.Empty;
        private bool isIndiaProducts = false;
        private bool isPickupProductFromSeller = false;
        private bool isNeedInsuranceCoverage = false;
        private string RequirementId;
        private Requirement mRequirement;
        private string subCategoryName;
        private string ErrorMessage = string.Empty;
        #endregion

        #region [ Constructor ]
        public PostNewRequiremntPage(string RequirementId = "")
        {
            try
            {
                InitializeComponent();

                this.RequirementId = RequirementId;
                BindProperties();
                this.mRequirement = new Requirement();

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
                Common.DisplayErrorMessage("PostNewRequiremntPage/Ctor: " + ex.Message);
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

        #region [ Get / Bind Data ]
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await GetRequirementDetails();
        }

        private async Task GetRequirementDetails()
        {
            try
            {
                if (Common.EmptyFiels(RequirementId))
                    return;

                UserDialogs.Instance.ShowLoading(Constraints.Loading);
                mRequirement = await DependencyService.Get<IRequirementRepository>().GetRequirementById(RequirementId);
                if (mRequirement != null && !Common.EmptyFiels(mRequirement.RequirementId))
                {

                    if (!Common.EmptyFiels(mRequirement.Title))
                    {
                        txtTitle.Text = mRequirement.Title;
                    }

                    if (!Common.EmptyFiels(mRequirement.Category))
                    {
                        pkCategory.SelectedItem = mRequirement.Category;

                    }
                    if (mRequirement.SubCategories != null)
                    {
                        selectedSubCategory = mRequirement.SubCategories;
                        subCategoryName = mRequirement.SubCategories.FirstOrDefault();
                        //SubCategory.Text = string.Join(",", selectedSubCategory);
                    }

                    if (!Common.EmptyFiels(mRequirement.ProductImage))
                    {
                        ImgProductImage.Source = mRequirement.ProductImage;
                    }
                    else
                    {
                        ImgProductImage.Source = Constraints.Img_ProductBanner;
                    }

                    txtDescription.Text = mRequirement.ProductDescription;

                    if (!Common.EmptyFiels(mRequirement.Quantity.ToString()))
                    {
                        txtQuantity.Text = mRequirement.Quantity.ToString();
                    }

                    if (!Common.EmptyFiels(mRequirement.Unit))
                    {
                        pkQuantityUnits.SelectedItem = mRequirement.Unit;
                    }

                    if (!Common.EmptyFiels(mRequirement.TotalPriceEstimation.ToString()))
                    {
                        txtEstimation.Text = mRequirement.TotalPriceEstimation.ToString();
                    }

                    if (mRequirement.PreferInIndiaProducts)
                    {
                        isIndiaProducts = true;
                        imgPrefer.Source = Constraints.Img_CheckBoxChecked;
                        lblDeliveryDate.Text = Constraints.Str_ExpectedPickupDate;
                    }
                    else
                    {
                        lblDeliveryDate.Text = Constraints.Str_ExpectedDeliveryDate;
                    }

                    if (mRequirement.PickupProductDirectly)
                    {
                        isPickupProductFromSeller = true;
                        imgPickup.Source = Constraints.Img_CheckBoxChecked;
                    }

                    if (mRequirement.NeedInsuranceCoverage)
                    {
                        isNeedInsuranceCoverage = true;
                        imgInsCoverage.Source = Constraints.Img_CheckBoxChecked;
                    }

                    if (!Common.EmptyFiels(mRequirement.DeliveryLocationPinCode))
                    {
                        txtLocationPinCode.Text = mRequirement.DeliveryLocationPinCode;
                    }

                    #region [ Address ]

                    txtName.Text = mRequirement.BillingAddressName;
                    txtBuilding.Text = mRequirement.BillingAddressBuilding;
                    txtStreet.Text = mRequirement.BillingAddressStreet;
                    txtCity.Text = mRequirement.BillingAddressCity;
                    txtState.Text = mRequirement.BillingAddressState;
                    txtPinCode.Text = mRequirement.BillingAddressPinCode;

                    if (!Common.EmptyFiels(mRequirement.ShippingAddressName) || !Common.EmptyFiels(mRequirement.ShippingAddressBuilding)
                      || !Common.EmptyFiels(mRequirement.ShippingAddressStreet) || !Common.EmptyFiels(mRequirement.ShippingAddressCity)
                      || !Common.EmptyFiels(mRequirement.ShippingAddressPinCode) || !Common.EmptyFiels(mRequirement.ShippingAddressLandmark)
                      )
                    {
                        txtSAName.Text = mRequirement.ShippingAddressName;
                        txtSABuilding.Text = mRequirement.ShippingAddressBuilding;
                        txtSAStreet.Text = mRequirement.ShippingAddressStreet;
                        txtSACity.Text = mRequirement.ShippingAddressCity;
                        txtSAState.Text = mRequirement.ShippingAddressState;
                        txtSAPinCode.Text = mRequirement.ShippingAddressPinCode;
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("ViewRequirememntPage/GetRequirementDetails: " + ex.Message);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private void BindProperties()
        {
            try
            {
                mCategories = new List<Category>();
                mSubCategories = new List<SubCategory>();
                selectedSubCategory = new List<string>();
                BindBillingAddress();
                BindCategories();
                CapitalizeWord();
                dpExpectedDeliveryDate.NullableDate = null;
                dpExpectedDeliveryDate.MinimumDate = DateTime.Today;
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("PostNewRequiremntPage/BindProperties: " + ex.Message);
            }
        }

        private void CapitalizeWord()
        {
            try
            {
                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    txtTitle.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);
                    txtDescription.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);
                    txtSourceSupply.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);

                    txtName.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);
                    txtBuilding.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);
                    txtStreet.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);
                    txtCity.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);
                    txtState.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);

                    txtSAName.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);
                    txtSABuilding.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);
                    txtSAStreet.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);
                    txtSACity.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);
                    txtSAState.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);
                    txtSALandmark.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("PostNewRequiremntPage/CapitalizeWord: " + ex.Message);
            }
        }

        private async void BindBillingAddress()
        {
            try
            {
                ProfileAPI profileAPI = new ProfileAPI();
                UserDialogs.Instance.ShowLoading(Constraints.Loading);

                var mResponse = await profileAPI.GetMyProfileData();
                if (mResponse != null && mResponse.Succeeded)
                {
                    var jObject = (Newtonsoft.Json.Linq.JObject)mResponse.Data;
                    if (jObject != null)
                    {
                        mBuyerDetail = jObject.ToObject<Model.Request.BuyerDetails>();
                        if (mBuyerDetail != null)
                        {
                            txtName.Text = mBuyerDetail.FullName;
                            txtBuilding.Text = mBuyerDetail.Building;
                            txtStreet.Text = mBuyerDetail.Street;
                            txtCity.Text = mBuyerDetail.City;
                            txtState.Text = mBuyerDetail.State;
                            txtPinCode.Text = mBuyerDetail.PinCode;
                        }
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
                Common.DisplayErrorMessage("PostNewRequiremntPage/GetProfile: " + ex.Message);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private async void BindCategories()
        {
            try
            {
                mCategories = await DependencyService.Get<IProfileRepository>().GetCategory();
                if (mCategories != null || mCategories.Count > 0)
                {
                    pkCategory.ItemsSource = mCategories.Select(x => x.Name).ToList();
                }

                if (pkCategory.SelectedItem != null)
                {
                    var categoryId = mCategories.Where(x => x.Name == pkCategory.SelectedItem.ToString()).FirstOrDefault()?.CategoryId;
                    GetSubCategoryByCategoryId(categoryId);
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("PostNewRequiremntPage/BindList: " + ex.Message);
            }
        }

        private async void GetSubCategoryByCategoryId(string categoryId)
        {
            try
            {
                mSubCategories = await DependencyService.Get<IProfileRepository>().GetSubCategory(categoryId);
                pkSubCategory.ItemsSource = mSubCategories.Select(x => x.Name).ToList();
                if (!Common.EmptyFiels(subCategoryName))
                {
                    pkSubCategory.SelectedItem = subCategoryName;
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("PostNewRequiremntPage/GetSubCategoryByCategoryId: " + ex.Message);
            }
        }
        #endregion

        #region [ Validations ]
        private bool Validations()
        {
            bool isValid = false;
            try
            {
                if (Common.EmptyFiels(txtTitle.Text) || Common.EmptyFiels(txtDescription.Text)
                    || pkCategory.SelectedIndex == -1 || pkSubCategory.SelectedIndex == -1
                    || Common.EmptyFiels(txtQuantity.Text) || Common.EmptyFiels(txtName.Text)
                    || Common.EmptyFiels(txtBuilding.Text) || Common.EmptyFiels(txtStreet.Text)
                    || Common.EmptyFiels(txtCity.Text) || Common.EmptyFiels(txtState.Text)
                    || Common.EmptyFiels(txtPinCode.Text) || pkQuantityUnits.SelectedIndex == -1
                    || Common.EmptyFiels(txtEstimation.Text))
                {
                    RequiredFields();
                    isValid = false;
                }

                if (Common.EmptyFiels(txtTitle.Text))
                {
                    Common.DisplayErrorMessage(Constraints.Required_Title);
                }
                else if (pkCategory.SelectedIndex == -1)
                {
                    Common.DisplayErrorMessage(Constraints.Required_Category);
                }
                else if (pkSubCategory.SelectedIndex == -1)
                {
                    Common.DisplayErrorMessage(Constraints.Required_SubCategory);
                }
                else if (Common.EmptyFiels(txtDescription.Text))
                {
                    Common.DisplayErrorMessage(Constraints.Required_Description);
                }
                else if (Common.EmptyFiels(txtQuantity.Text))
                {
                    Common.DisplayErrorMessage(Constraints.Required_Quantity);
                }
                else if (pkQuantityUnits.SelectedIndex == -1)
                {
                    Common.DisplayErrorMessage(Constraints.Required_QuantityUnits);
                }
                else if (Common.EmptyFiels(txtEstimation.Text))
                {
                    Common.DisplayErrorMessage(Constraints.Required_PriceEstimation);
                }
                else if (Common.EmptyFiels(txtLocationPinCode.Text) && !isPickupProductFromSeller)
                {
                    Common.DisplayErrorMessage(Constraints.Required_Delivery_PinCode);
                }
                else if (Common.EmptyFiels(txtName.Text))
                {
                    Common.DisplayErrorMessage(Constraints.Required_Billing_Name);
                }
                else if (Common.EmptyFiels(txtBuilding.Text))
                {
                    Common.DisplayErrorMessage(Constraints.Required_Billing_Building);
                }
                else if (Common.EmptyFiels(txtStreet.Text))
                {
                    Common.DisplayErrorMessage(Constraints.Required_Billing_Street);
                }
                else if (Common.EmptyFiels(txtCity.Text))
                {
                    Common.DisplayErrorMessage(Constraints.Required_Billing_City);
                }
                else if (Common.EmptyFiels(txtState.Text))
                {
                    Common.DisplayErrorMessage(Constraints.Required_Billing_State);
                }
                else if (Common.EmptyFiels(txtPinCode.Text))
                {
                    Common.DisplayErrorMessage(Constraints.Required_Billing_PinCode);
                }
                else
                {
                    isValid = true;
                }

                if (isValid && !isPickupProductFromSeller)
                {
                    if (Common.EmptyFiels(txtSAName.Text) || Common.EmptyFiels(txtSABuilding.Text)
                        || Common.EmptyFiels(txtSAStreet.Text) || Common.EmptyFiels(txtSACity.Text)
                        || Common.EmptyFiels(txtSAState.Text) || Common.EmptyFiels(txtSAPinCode.Text))
                    {
                        return RequiredShippingAddressFields();
                    }
                }
                else
                {
                    if (!isPickupProductFromSeller)
                        EmptyRedShippingAddress();
                }

            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("PostNewRequiremntPage/Validations: " + ex.Message);
            }
            return isValid;
        }

        private void RequiredFields()
        {
            try
            {
                if (Common.EmptyFiels(txtTitle.Text))
                {
                    BoxTitle.BackgroundColor = (Color)App.Current.Resources["appColor3"];
                }

                if (pkCategory.SelectedIndex == -1)
                {
                    BoxCategory.BackgroundColor = (Color)App.Current.Resources["appColor3"];
                }

                if (pkSubCategory.SelectedIndex == -1)
                {
                    BoxSubCategory.BackgroundColor = (Color)App.Current.Resources["appColor3"];
                }

                if (Common.EmptyFiels(txtDescription.Text))
                {
                    BoxDescription.BackgroundColor = (Color)App.Current.Resources["appColor3"];
                }

                if (Common.EmptyFiels(txtQuantity.Text))
                {
                    BoxQuantity.BackgroundColor = (Color)App.Current.Resources["appColor3"];
                }

                if (pkQuantityUnits.SelectedIndex == -1)
                {
                    BoxQuantityUnits.BackgroundColor = (Color)App.Current.Resources["appColor3"];
                }

                if (Common.EmptyFiels(txtEstimation.Text))
                {
                    BoxPriceEstimation.BackgroundColor = (Color)App.Current.Resources["appColor3"];
                }

                if (Common.EmptyFiels(txtLocationPinCode.Text) && !isPickupProductFromSeller)
                {
                    BoxLocationPinCode.BackgroundColor = (Color)App.Current.Resources["appColor3"];
                }

                if (Common.EmptyFiels(txtName.Text))
                {
                    BoxName.BackgroundColor = (Color)App.Current.Resources["appColor3"];
                }

                if (Common.EmptyFiels(txtBuilding.Text))
                {
                    BoxBuilding.BackgroundColor = (Color)App.Current.Resources["appColor3"];
                }

                if (Common.EmptyFiels(txtStreet.Text))
                {
                    BoxStreet.BackgroundColor = (Color)App.Current.Resources["appColor3"];
                }

                if (Common.EmptyFiels(txtCity.Text))
                {
                    BoxCity.BackgroundColor = (Color)App.Current.Resources["appColor3"];
                }

                if (Common.EmptyFiels(txtState.Text))
                {
                    BoxState.BackgroundColor = (Color)App.Current.Resources["appColor3"];
                }

                if (Common.EmptyFiels(txtPinCode.Text))
                {
                    BoxPinCode.BackgroundColor = (Color)App.Current.Resources["appColor3"];
                }

            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("PostNewRequiremntPage/RequiredFields: " + ex.Message);
            }
        }

        private void EmptyRedShippingAddress()
        {
            if (Common.EmptyFiels(txtSAName.Text))
            {
                BoxSAName.BackgroundColor = (Color)App.Current.Resources["appColor3"];
            }

            if (Common.EmptyFiels(txtSABuilding.Text))
            {
                BoxSABuilding.BackgroundColor = (Color)App.Current.Resources["appColor3"];
            }

            if (Common.EmptyFiels(txtSAStreet.Text))
            {
                BoxSAStreet.BackgroundColor = (Color)App.Current.Resources["appColor3"];
            }

            if (Common.EmptyFiels(txtSACity.Text))
            {
                BoxSACity.BackgroundColor = (Color)App.Current.Resources["appColor3"];
            }
            if (Common.EmptyFiels(txtSAState.Text))
            {
                BoxSAState.BackgroundColor = (Color)App.Current.Resources["appColor3"];
            }

            if (Common.EmptyFiels(txtSAPinCode.Text))
            {
                BoxSAPinCode.BackgroundColor = (Color)App.Current.Resources["appColor3"];
            }
        }

        private void EmptyGrayShippingAddress()
        {
            BoxSAName.BackgroundColor = (Color)App.Current.Resources["appColor8"];
            BoxSABuilding.BackgroundColor = (Color)App.Current.Resources["appColor8"];
            BoxSAStreet.BackgroundColor = (Color)App.Current.Resources["appColor8"];
            BoxSACity.BackgroundColor = (Color)App.Current.Resources["appColor8"];
            BoxSAPinCode.BackgroundColor = (Color)App.Current.Resources["appColor8"];
        }

        private bool RequiredShippingAddressFields()
        {
            bool isValid = false;

            EmptyRedShippingAddress();

            if (Common.EmptyFiels(txtSAName.Text))
            {
                Common.DisplayErrorMessage(Constraints.Required_Shipping_Name);
            }
            else if (Common.EmptyFiels(txtSABuilding.Text))
            {
                Common.DisplayErrorMessage(Constraints.Required_Shipping_Building);
            }
            else if (Common.EmptyFiels(txtSAStreet.Text))
            {
                Common.DisplayErrorMessage(Constraints.Required_Shipping_Street);
            }
            else if (Common.EmptyFiels(txtSACity.Text))
            {
                Common.DisplayErrorMessage(Constraints.Required_Shipping_City);
            }
            else if (Common.EmptyFiels(txtSAState.Text))
            {
                Common.DisplayErrorMessage(Constraints.Required_Shipping_State);
            }
            else if (Common.EmptyFiels(txtSAPinCode.Text))
            {
                Common.DisplayErrorMessage(Constraints.Required_Shipping_PinCode);
            }
            else
            {
                isValid = true;
            }

            return isValid;
        }

        private void UnfocussedFields(Entry entry = null, Editor editor = null, Picker picker = null)
        {
            try
            {
                if (entry != null)
                {
                    if (entry.ClassId == "Title")
                    {
                        BoxTitle.BackgroundColor = (Color)App.Current.Resources["appColor8"];
                    }
                    else if (entry.ClassId == "Quantity")
                    {
                        BoxQuantity.BackgroundColor = (Color)App.Current.Resources["appColor8"];
                    }
                    else if (entry.ClassId == "LocationPinCode")
                    {
                        BoxLocationPinCode.BackgroundColor = (Color)App.Current.Resources["appColor8"];
                    }
                    else if (entry.ClassId == "PriceEstimation")
                    {
                        BoxPriceEstimation.BackgroundColor = (Color)App.Current.Resources["appColor8"];
                    }
                    else if (entry.ClassId == "Name")
                    {
                        BoxName.BackgroundColor = (Color)App.Current.Resources["appColor8"];
                    }
                    else if (entry.ClassId == "Building")
                    {
                        BoxBuilding.BackgroundColor = (Color)App.Current.Resources["appColor8"];
                    }
                    else if (entry.ClassId == "Street")
                    {
                        BoxStreet.BackgroundColor = (Color)App.Current.Resources["appColor8"];
                    }
                    else if (entry.ClassId == "City")
                    {
                        BoxCity.BackgroundColor = (Color)App.Current.Resources["appColor8"];
                    }
                    else if (entry.ClassId == "State")
                    {
                        BoxState.BackgroundColor = (Color)App.Current.Resources["appColor8"];
                    }
                    else if (entry.ClassId == "PinCode")
                    {
                        BoxPinCode.BackgroundColor = (Color)App.Current.Resources["appColor8"];
                    }
                    else if (entry.ClassId == "SAName")
                    {
                        BoxSAName.BackgroundColor = (Color)App.Current.Resources["appColor8"];
                    }
                    else if (entry.ClassId == "SABuilding")
                    {
                        BoxSABuilding.BackgroundColor = (Color)App.Current.Resources["appColor8"];
                    }
                    else if (entry.ClassId == "SAStreet")
                    {
                        BoxSAStreet.BackgroundColor = (Color)App.Current.Resources["appColor8"];
                    }
                    else if (entry.ClassId == "SACity")
                    {
                        BoxSACity.BackgroundColor = (Color)App.Current.Resources["appColor8"];
                    }
                    else if (entry.ClassId == "SAState")
                    {
                        BoxSAState.BackgroundColor = (Color)App.Current.Resources["appColor8"];
                    }
                    else if (entry.ClassId == "SAPinCode")
                    {
                        BoxSAPinCode.BackgroundColor = (Color)App.Current.Resources["appColor8"];
                    }
                }

                if (editor != null)
                {
                    if (editor.ClassId == "Description")
                    {
                        BoxDescription.BackgroundColor = (Color)App.Current.Resources["appColor8"];
                    }
                }

                if (picker != null)
                {
                    if (picker.ClassId == "Category")
                    {
                        BoxCategory.BackgroundColor = (Color)App.Current.Resources["appColor8"];
                    }
                    else if (picker.ClassId == "SubCategory")
                    {
                        BoxSubCategory.BackgroundColor = (Color)App.Current.Resources["appColor8"];
                    }
                    else if (picker.ClassId == "QuantityUnits")
                    {
                        BoxQuantityUnits.BackgroundColor = (Color)App.Current.Resources["appColor8"];
                    }
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("PostNewRequiremntPage/UnfocussedFields: " + ex.Message);
            }
        }

        private async Task<bool> PinCodeValidation(string PinCode, BoxView boxView, string PincodeName)
        {
            bool isValid = false;
            try
            {
                if (!Common.EmptyFiels(PinCode))
                {
                    PinCode = PinCode.Trim();
                    isValid = await DependencyService.Get<IProfileRepository>().ValidPincode(PinCode, PincodeName);
                    if (isValid)
                    {
                        boxView.BackgroundColor = (Color)App.Current.Resources["appColor8"];
                    }
                    else
                    {
                        boxView.BackgroundColor = (Color)App.Current.Resources["appColor3"];
                    }
                }
                else
                {
                    Common.DisplayErrorMessage(Constraints.Required_PinCode);
                    boxView.BackgroundColor = (Color)App.Current.Resources["appColor8"];
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("PostNewRequiremntPage/PinCodeValidation: " + ex.Message);
            }
            return isValid;
        }

        private void FieldsTrim()
        {
            try
            {
                txtTitle.Text = txtTitle.Text.Trim();
                txtDescription.Text = txtDescription.Text.Trim();
                txtQuantity.Text = txtQuantity.Text.Trim();
                txtEstimation.Text = txtEstimation.Text.Trim();

                txtName.Text = txtName.Text.Trim();
                txtBuilding.Text = txtBuilding.Text.Trim();
                txtStreet.Text = txtStreet.Text.Trim();
                txtCity.Text = txtCity.Text.Trim();
                txtState.Text = txtState.Text.Trim();
                txtPinCode.Text = txtPinCode.Text.Trim();

                if (!Common.EmptyFiels(txtSAName.Text))
                    txtSAName.Text = txtSAName.Text.Trim();
                if (!Common.EmptyFiels(txtSABuilding.Text))
                    txtSABuilding.Text = txtSABuilding.Text.Trim();
                if (!Common.EmptyFiels(txtSAStreet.Text))
                    txtSAStreet.Text = txtSAStreet.Text.Trim();
                if (!Common.EmptyFiels(txtSACity.Text))
                    txtSACity.Text = txtSACity.Text.Trim();
                if (!Common.EmptyFiels(txtSAState.Text))
                    txtSAState.Text = txtSAState.Text.Trim();
                if (!Common.EmptyFiels(txtSAPinCode.Text))
                    txtSAPinCode.Text = txtSAPinCode.Text.Trim();
                if (!Common.EmptyFiels(txtSALandmark.Text))
                    txtSALandmark.Text = txtSALandmark.Text.Trim();

                if (!Common.EmptyFiels(txtSourceSupply.Text))
                    txtSourceSupply.Text = txtSourceSupply.Text.Trim();
                if (!Common.EmptyFiels(txtLocationPinCode.Text))
                    txtLocationPinCode.Text = txtLocationPinCode.Text.Trim();
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("PostNewRequiremntPage/FieldsTrim: " + ex.Message);
            }
        }
        #endregion

        #region [ Submit Requirement ]
        private Model.Request.Requirement FillRequirement()
        {
            Requirement mRequirement = new Requirement();
            try
            {
                mRequirement.UserId = Settings.UserId;
                mRequirement.Title = txtTitle.Text;
                mRequirement.Category = mCategories.Where(x => x.Name == pkCategory.SelectedItem.ToString()).FirstOrDefault()?.Name;
                mRequirement.SubCategories = selectedSubCategory;
                mRequirement.ProductDescription = txtDescription.Text;
                mRequirement.Quantity = Convert.ToInt32(txtQuantity.Text);
                mRequirement.Unit = pkQuantityUnits.SelectedItem.ToString();
                mRequirement.TotalPriceEstimation = Convert.ToDecimal(txtEstimation.Text);

                mRequirement.BillingAddressName = txtName.Text;
                mRequirement.BillingAddressBuilding = txtBuilding.Text;
                mRequirement.BillingAddressStreet = txtStreet.Text;
                mRequirement.BillingAddressCity = txtCity.Text;
                mRequirement.BillingAddressState = txtState.Text;
                mRequirement.BillingAddressPinCode = txtPinCode.Text;

                mRequirement.ShippingAddressName = txtSAName.Text;
                mRequirement.ShippingAddressBuilding = txtSABuilding.Text;
                mRequirement.ShippingAddressStreet = txtSAStreet.Text;
                mRequirement.ShippingAddressCity = txtSACity.Text;
                mRequirement.ShippingAddressState = txtSAState.Text;
                mRequirement.ShippingAddressPinCode = txtSAPinCode.Text;
                if (!Common.EmptyFiels(txtSALandmark.Text))
                {
                    mRequirement.ShippingAddressLandmark = txtSALandmark.Text;
                }

                mRequirement.PreferInIndiaProducts = isIndiaProducts;
                mRequirement.PickupProductDirectly = isPickupProductFromSeller;
                mRequirement.NeedInsuranceCoverage = isNeedInsuranceCoverage;

                if (!Common.EmptyFiels(relativePath))
                {
                    string baseURL = (string)App.Current.Resources["BaseURL"];
                    mRequirement.ProductImage = relativePath.Replace(baseURL, "");
                }
                if (!Common.EmptyFiels(txtSourceSupply.Text))
                {
                    mRequirement.PreferredSourceOfSupply = txtSourceSupply.Text;
                }
                if (dpExpectedDeliveryDate.NullableDate != null && dpExpectedDeliveryDate.NullableDate != DateTime.MinValue)
                {
                    mRequirement.ExpectedDeliveryDate = dpExpectedDeliveryDate.NullableDate.Value;
                }
                if (!Common.EmptyFiels(txtLocationPinCode.Text))
                {
                    mRequirement.DeliveryLocationPinCode = txtLocationPinCode.Text;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return null;
            }

            return mRequirement;
        }

        private async Task SubmitRequirement()
        {
            try
            {
                if (Validations())
                {
                    UserDialogs.Instance.ShowLoading(Constraints.Loading);
                    if (!await PinCodeValidation(txtPinCode.Text, BoxPinCode, "Billing"))
                        return;

                    if (!isPickupProductFromSeller)
                    {
                        if (!await PinCodeValidation(txtLocationPinCode.Text, BoxLocationPinCode, "Delivery"))
                            return;
                        else if (!await PinCodeValidation(txtSAPinCode.Text, BoxSAPinCode, "Shipping"))
                            return;
                        else if (txtLocationPinCode.Text != txtSAPinCode.Text)
                        {
                            BoxLocationPinCode.BackgroundColor = (Color)App.Current.Resources["appColor3"];
                            BoxSAPinCode.BackgroundColor = (Color)App.Current.Resources["appColor3"];
                            Common.DisplayErrorMessage(Constraints.Same_Delivery_Shipping_PinCode);
                            return;
                        }
                    }
                    else if (!Common.EmptyFiels(txtSAPinCode.Text))
                    {
                        if (!await PinCodeValidation(txtSAPinCode.Text, BoxSAPinCode, "Shipping"))
                            return;
                    }

                    FieldsTrim();
                    RequirementAPI requirementAPI = new RequirementAPI();
                    var mRequirement = FillRequirement();

                    if (mRequirement != null)
                    {
                        var mResponse = await requirementAPI.CreateRequirement(mRequirement);
                        if (mResponse != null && mResponse.Succeeded)
                        {
                            await SuccessfullRequirementAsync(mResponse.Message);
                            ClearPropeties();
                        }
                        else
                        {
                            if (mResponse != null)
                                Common.DisplayErrorMessage(mResponse.Message);
                            else
                                Common.DisplayErrorMessage(Constraints.Something_Wrong);
                        }
                    }
                    else
                    {
                        if (!Common.EmptyFiels(ErrorMessage))
                        {
                            Common.DisplayErrorMessage(ErrorMessage);
                        }
                        else
                        {
                            Common.DisplayErrorMessage(Constraints.Something_Wrong);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("PostNewRequiremntPage/SubmitRequirement: " + ex.Message);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private async Task SuccessfullRequirementAsync(string MessageString)
        {
            try
            {
                var successPopup = new PopupPages.SuccessPopup(MessageString);
                successPopup.isRefresh += async (s1, e1) =>
                {
                    bool res = (bool)s1;
                    if (res)
                    {
                        await Navigation.PopAsync();
                    }
                };

                await PopupNavigation.Instance.PushAsync(successPopup);
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("PostNewRequiremntPage/SuccessfullRequirement: " + ex.Message);
            }
        }

        private void ClearPropeties()
        {
            try
            {
                txtTitle.Text = string.Empty;
                pkCategory.SelectedIndex = -1;
                pkSubCategory.SelectedIndex = -1;
                txtDescription.Text = string.Empty;
                txtQuantity.Text = string.Empty;
                txtLocationPinCode.Text = string.Empty;
                txtEstimation.Text = string.Empty;
                txtSourceSupply.Text = string.Empty;

                txtName.Text = string.Empty;
                txtBuilding.Text = string.Empty;
                txtStreet.Text = string.Empty;
                txtCity.Text = string.Empty;
                txtState.Text = string.Empty;
                txtPinCode.Text = string.Empty;

                txtSAName.Text = string.Empty;
                txtSABuilding.Text = string.Empty;
                txtSAStreet.Text = string.Empty;
                txtSACity.Text = string.Empty;
                txtSAState.Text = string.Empty;
                txtSAPinCode.Text = string.Empty;
                txtSALandmark.Text = string.Empty;

                ImgProductImage.Source = Constraints.Img_UploadImage;
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("PostNewRequiremntPage/ClearPropeties: " + ex.Message);
            }
        }
        #endregion

        private void PickupDirectlyFromSeller()
        {
            try
            {
                if (imgPickup.Source.ToString().Replace("File: ", "") == Constraints.Img_CheckBoxChecked)
                {
                    imgPickup.Source = Constraints.Img_CheckBoxUnChecked;
                    lblLocationPINCode.Text = Constraints.Str_DeliveryLocationPINCode;
                    lblDeliveryDate.Text = Constraints.Str_ExpectedDeliveryDate;
                    StkDeliveryLocationPINCode.IsVisible = true;
                    isPickupProductFromSeller = false;
                    lblSAName.IsVisible = true;
                    lblSABuilding.IsVisible = true;
                    lblSAStreet.IsVisible = true;
                    lblSACity.IsVisible = true;
                    lblSAState.IsVisible = true;
                    lblSAPINCode.IsVisible = true;
                }
                else
                {
                    imgPickup.Source = Constraints.Img_CheckBoxChecked;
                    lblLocationPINCode.Text = Constraints.Str_PickupLocationPINCode;
                    lblDeliveryDate.Text = Constraints.Str_ExpectedPickupDate;
                    StkDeliveryLocationPINCode.IsVisible = false;
                    EmptyGrayShippingAddress();
                    isPickupProductFromSeller = true;
                    lblSAName.IsVisible = false;
                    lblSABuilding.IsVisible = false;
                    lblSAStreet.IsVisible = false;
                    lblSACity.IsVisible = false;
                    lblSAState.IsVisible = false;
                    lblSAPINCode.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("PostNewRequiremntPage/PickupDirectlyFromSeller: " + ex.Message);
            }
        }

        private void SameAsBillingAddress()
        {
            try
            {
                if (imgSameAddress.Source.ToString().Replace("File: ", "") == Constraints.Img_CheckBoxChecked)
                {
                    imgSameAddress.Source = Constraints.Img_CheckBoxUnChecked;
                    txtSAName.Text = string.Empty;
                    txtSABuilding.Text = string.Empty;
                    txtSAStreet.Text = string.Empty;
                    txtSACity.Text = string.Empty;
                    txtSAState.Text = string.Empty;
                    txtSAPinCode.Text = string.Empty;
                    txtSALandmark.Text = string.Empty;
                }
                else
                {
                    imgSameAddress.Source = Constraints.Img_CheckBoxChecked;
                    UnfocussedFields(entry: txtSAName);
                    UnfocussedFields(entry: txtSABuilding);
                    UnfocussedFields(entry: txtSAStreet);
                    UnfocussedFields(entry: txtSACity);
                    UnfocussedFields(entry: txtSAState);
                    UnfocussedFields(entry: txtSAPinCode);
                    //UnfocussedFields(entry: txtSALandmark);
                    txtSAName.Text = txtName.Text;
                    txtSABuilding.Text = txtBuilding.Text;
                    txtSAStreet.Text = txtStreet.Text;
                    txtSACity.Text = txtCity.Text;
                    txtSAState.Text = txtState.Text;
                    txtSAPinCode.Text = txtPinCode.Text;
                    txtSALandmark.Text = mBuyerDetail.Landmark;
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("PostNewRequiremntPage/StkSameAddress_Tapped: " + ex.Message);
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
                    await Navigation.PushAsync(new NotificationPage());
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("PostNewRequiremntPage/ImgNotification_Tapped: " + ex.Message);
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
        #endregion

        private void BtnUnits_Clicked(object sender, EventArgs e)
        {
            pkQuantityUnits.Focus();
        }

        #region [ Checkboxes ]
        private void StkPrefer_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (imgPrefer.Source.ToString().Replace("File: ", "") == Constraints.Img_CheckBoxChecked)
                {
                    isIndiaProducts = false;
                    imgPrefer.Source = Constraints.Img_CheckBoxUnChecked;
                }
                else
                {
                    isIndiaProducts = true;
                    imgPrefer.Source = Constraints.Img_CheckBoxChecked;
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("PostNewRequiremntPage/StkPrefer_Tapped: " + ex.Message);
            }
        }

        private void StkPickup_Tapped(object sender, EventArgs e)
        {
            PickupDirectlyFromSeller();
        }

        private void StkShipping_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (imgShippingDown.Source.ToString().Replace("File: ", "") == Constraints.Img_ArrowDown)
                {
                    imgShippingDown.Source = Constraints.Img_ArrowUp;
                    grdShippingAddress.IsVisible = true;
                    ScrPrimary.ScrollToAsync(StkShipping, ScrollToPosition.Start, true);
                }
                else
                {
                    imgShippingDown.Source = Constraints.Img_ArrowDown;
                    grdShippingAddress.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("PostNewRequiremntPage/StkShipping_Tapped: " + ex.Message);
            }
        }

        private void StkInsCoverage_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (imgInsCoverage.Source.ToString().Replace("File: ", "") == Constraints.Img_CheckBoxChecked)
                {
                    isNeedInsuranceCoverage = false;
                    imgInsCoverage.Source = Constraints.Img_CheckBoxUnChecked;
                }
                else
                {
                    isNeedInsuranceCoverage = true;
                    imgInsCoverage.Source = Constraints.Img_CheckBoxChecked;
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("PostNewRequiremntPage/StkInsCoverage_Tapped: " + ex.Message);
            }
        }
        #endregion

        #region [ Category / SubCategory ]
        private void PkCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (pkCategory.SelectedIndex > -1 && pkCategory.SelectedItem != null)
                {
                    var catId = mCategories.Where(x => x.Name == pkCategory.SelectedItem.ToString()).Select(x => x.CategoryId).FirstOrDefault();
                    if (catId != null)
                    {
                        GetSubCategoryByCategoryId(catId);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("PostNewRequiremntPage/Category_SelectedIndex: " + ex.Message);
            }
        }

        private void PkSubCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (pkSubCategory.SelectedIndex > -1 && pkSubCategory.SelectedItem != null)
                {
                    selectedSubCategory.Clear();
                    selectedSubCategory.Add(pkSubCategory.SelectedItem.ToString());
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("PostNewRequiremntPage/pkSubCategory_SelectedIndexChanged: " + ex.Message);
            }
        }

        private void BtnCategory_Clicked(object sender, EventArgs e)
        {
            pkCategory.Focus();
        }

        private void BtnSubCategory_Clicked(object sender, EventArgs e)
        {
            pkSubCategory.Focus();
        }
        #endregion

        private void ExpectedDeliveryDate_Tapped(object sender, EventArgs e)
        {
            dpExpectedDeliveryDate.Focus();
        }

        private async void UploadProductImage_Tapped(object sender, EventArgs e)
        {
            try
            {
                UserDialogs.Instance.ShowLoading(Constraints.Loading);
                ImageConvertion.SelectedImagePath = ImgProductImage;
                ImageConvertion.SetNullSource((int)FileUploadCategory.RequirementImages);
                await ImageConvertion.SelectImage();

                if (ImageConvertion.SelectedImageByte != null)
                {
                    relativePath = await DependencyService.Get<IFileUploadRepository>().UploadFile((int)FileUploadCategory.RequirementImages);
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("PostNewRequiremntPage/UploadProductImage: " + ex.Message);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private void StkSameAddress_Tapped(object sender, EventArgs e)
        {
            SameAsBillingAddress();
        }

        #region [ Unfocused ]
        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = (ExtEntry)sender;
            if (!Common.EmptyFiels(entry.Text))
            {
                UnfocussedFields(entry: entry);
            }
        }

        private void Editor_Unfocused(object sender, FocusEventArgs e)
        {
            var editor = (Editor)sender;
            if (!Common.EmptyFiels(editor.Text))
            {
                UnfocussedFields(editor: editor);
            }
        }

        private void Picker_Unfocused(object sender, FocusEventArgs e)
        {
            var picker = (Picker)sender;
            if (picker.SelectedIndex != -1)
            {
                UnfocussedFields(picker: picker);
            }
        }
        #endregion

        private async void BtnSubmitRequirement_Clicked(object sender, EventArgs e)
        {
            var Tab = (Button)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    Common.BindAnimation(button: BtnSubmitRequirement);
                    await SubmitRequirement();
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("PostNewRequiremntPage/BtnSubmitRequirement: " + ex.Message);
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
        #endregion
    }
}